using System.Runtime.InteropServices;

namespace Helios.Compiler.Core
{
    public sealed unsafe class FrozenHashMappedArrayTrie<TValue>
           : IDisposable where TValue : unmanaged
    {
        // ── Constants ─────────────────────────────────────────────────
        private const int BitsPerLevel = 5;
        private const int SlotMask = 0x1F;
        private const int MaxDepth = 7;

        // ── Block Layout ──────────────────────────────────────────────
        // [ FrozenHMATNode[] | TValue[] | ChainEntry[] ]
        // All offsets are byte offsets from start of _block

        // ── Fields ────────────────────────────────────────────────────
        private void* _block;
        private int _count;
        private int _nodeCount;
        private int _valueBufferOffset;     // byte offset to start of TValue[]
        private int _chainBufferOffset;     // byte offset to start of ChainEntry[]
        private bool _disposed;

        // ── Properties ────────────────────────────────────────────────
        public int Count => _count;

        // ── Private constructor ───────────────────────────────────────
        private FrozenHashMappedArrayTrie() { }

        // ── Factory ───────────────────────────────────────────────────
        internal static FrozenHashMappedArrayTrie<TValue> Build(
            HMATNode<TValue> root, int count){
            // ── Pass 1 — count nodes, values, chain entries ───────────
            int nodeCount = 0;
            int valueCount = 0;
            int chainCount = 0;
            CountNodes(root, ref nodeCount, ref valueCount, ref chainCount);

            // ── Compute block layout ──────────────────────────────────
            int nodeArraySize = nodeCount * sizeof(FrozenHMATNode);
            int valueArraySize = valueCount * sizeof(TValue);
            int chainEntrySize = sizeof(int) + sizeof(TValue);           // TensorId.Value + TValue
            int chainArraySize = chainCount * chainEntrySize;
            int totalSize = nodeArraySize + valueArraySize + chainArraySize;

            // ── Allocate native block ─────────────────────────────────
            void* block = NativeMemory.AllocZeroed((nuint)totalSize);

            var instance = new FrozenHashMappedArrayTrie<TValue>
            {
                _block = block,
                _count = count,
                _nodeCount = nodeCount,
                _valueBufferOffset = nodeArraySize,
                _chainBufferOffset = nodeArraySize + valueArraySize,
                _disposed = false,
            };

            // ── Pass 2 — BFS walk, write nodes into block ─────────────
            var nodeQueue = new Queue<(HMATNode<TValue> Node, int AssignedIndex)>();
            var indexMap = new Dictionary<HMATNode<TValue>, int>(nodeCount);
            int nodeIndex = 0;
            int valueIndex = 0;
            int chainIndex = 0;

            nodeQueue.Enqueue((root, nodeIndex));
            indexMap[root] = nodeIndex++;

            // assign indices to all nodes first (BFS order)
            var bfsOrder = new List<HMATNode<TValue>>();
            var visited = new HashSet<HMATNode<TValue>>();
            var queue2 = new Queue<HMATNode<TValue>>();
            queue2.Enqueue(root);
            visited.Add(root);

            while (queue2.Count > 0)
            {
                var node = queue2.Dequeue();
                bfsOrder.Add(node);
                if (!node.IsLeaf)
                {
                    foreach (var child in node.Children)
                    {
                        if (child is not null && !visited.Contains(child))
                        {
                            visited.Add(child);
                            indexMap[child] = nodeIndex++;
                            queue2.Enqueue(child);
                        }
                    }
                }
            }

            // write nodes in BFS order
            FrozenHMATNode* nodeBuffer = (FrozenHMATNode*)block;
            TValue* valueBuffer = (TValue*)((byte*)block + instance._valueBufferOffset);
            byte* chainBuffer = (byte*)block + instance._chainBufferOffset;

            foreach (var node in bfsOrder)
            {
                int idx = indexMap[node];
                ref FrozenHMATNode frozen = ref nodeBuffer[idx];

                frozen.Bitmap = node.Bitmap;
                frozen.IsLeaf = node.IsLeaf;

                if (node.IsLeaf)
                {
                    // write value into value buffer
                    frozen.KeyValue = node.Key.Value;
                    frozen.ValueIndex = valueIndex;
                    valueBuffer[valueIndex++] = node.Value!;

                    // write chain entries if any
                    if (node.Chain.Count > 0)
                    {
                        frozen.ChainOffset = instance._chainBufferOffset
                                            + chainIndex * (sizeof(int) + sizeof(TValue));
                        frozen.ChainCount = node.Chain.Count;

                        foreach (var entry in node.Chain)
                        {
                            byte* chainSlot = chainBuffer + chainIndex * (sizeof(int) + sizeof(TValue));
                            *(int*)chainSlot = entry.Key.Value;
                            *(TValue*)(chainSlot + sizeof(int)) = entry.Value;
                            chainIndex++;
                        }
                    }
                    else
                    {
                        frozen.ChainOffset = -1;
                        frozen.ChainCount = 0;
                    }

                    frozen.ChildrenOffset = -1;
                    frozen.ChildCount = 0;
                }
                else
                {
                    // internal node — write children offsets
                    frozen.KeyValue = -1;
                    frozen.ValueIndex = -1;
                    frozen.ChainOffset = -1;
                    frozen.ChainCount = 0;
                    frozen.ChildCount = node.Children.Length;

                    // children array starts at next available slot after all nodes
                    // store offset as index into node buffer — resolve on read
                    frozen.ChildrenOffset = node.Children.Length > 0
                        ? indexMap[node.Children[0]!]
                        : -1;
                }
            }

            return instance;
        }

        // ── TryGetValue ───────────────────────────────────────────────
        public bool TryGetValue(TensorId key, out TValue value)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(FrozenHashMappedArrayTrie<TValue>));

            FrozenHMATNode* nodeBuffer = (FrozenHMATNode*)_block;
            TValue* valueBuffer = (TValue*)((byte*)_block + _valueBufferOffset);
            byte* chainBuffer = (byte*)_block + _chainBufferOffset;

            int hash = key.GetHashCode();
            int current = 0;    // root is always at index 0

            for (int depth = 0; depth < MaxDepth; depth++)
            {
                ref FrozenHMATNode node = ref nodeBuffer[current];
                int slot = (hash >> (depth * BitsPerLevel)) & SlotMask;

                if (!node.HasSlot(slot))
                {
                    value = default;
                    return false;
                }

                if (node.IsLeaf)
                {
                    // check primary slot
                    if (node.KeyValue == key.Value)
                    {
                        value = valueBuffer[node.ValueIndex];
                        return true;
                    }

                    // check collision chain
                    if (node.ChainCount > 0)
                    {
                        byte* chainSlot = chainBuffer + (node.ChainOffset - _chainBufferOffset);
                        for (int i = 0; i < node.ChainCount; i++)
                        {
                            int chainKey = *(int*)chainSlot;
                            if (chainKey == key.Value)
                            {
                                value = *(TValue*)(chainSlot + sizeof(int));
                                return true;
                            }
                            chainSlot += sizeof(int) + sizeof(TValue);
                        }
                    }

                    value = default;
                    return false;
                }

                // internal node — find child index
                int childSlotIndex = node.SlotToIndex(slot);
                current = node.ChildrenOffset + childSlotIndex;
            }

            value = default;
            return false;
        }

        // ── Dispose ───────────────────────────────────────────────────
        public void Dispose()
        {
            if (_disposed) return;
            if (_block != null)
            {
                NativeMemory.Free(_block);
                _block = null;
            }
            _disposed = true;
        }

        // ── Pass 1 — Count ────────────────────────────────────────────
        private static void CountNodes(
            HMATNode<TValue> node,
            ref int nodeCount,
            ref int valueCount,
            ref int chainCount)
        {
            nodeCount++;

            if (node.IsLeaf)
            {
                valueCount++;
                chainCount += node.Chain.Count;
                return;
            }

            foreach (var child in node.Children)
                if (child is not null)
                    CountNodes(child, ref nodeCount, ref valueCount, ref chainCount);
        }
    }
}