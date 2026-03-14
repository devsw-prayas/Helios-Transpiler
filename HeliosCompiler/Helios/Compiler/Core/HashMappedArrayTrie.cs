
namespace Helios.Compiler.Core
{
    public class HashMappedArrayTrie<TKey, TValue> where TKey : notnull
    { 
        private const int BitsPerLevel = 5;
        private const int SlotMask = 0x1F;    // 0b11111 — 5 bits
        private const int MaxDepth = 7;       // ceil(32 / 5) — covers full 32-bit hash

        private HMATNode<TValue> _root;
        private int _count;
        private EqualityComparer<TKey> _comparer;

        public int Count => _count;

        public HashMappedArrayTrie()
        {
            _root = new HMATNode<TValue>();
            _count = 0;
            _comparer = EqualityComparer<TKey>.Default;
        }

        public void Add(TensorId key, TValue value)
        {
            int hash = key.GetHashCode();
            AddToNode(ref _root, key, value, hash, 0);
            _count++;
        }

        public bool TryGetValue(TensorId key, out TValue? value)
        {
            int hash = key.GetHashCode();
            var node = FindLeaf(_root, key, hash);

            if (node is null)
            {
                value = default;
                return false;
            }

            // check primary slot first
            if (node.Key == key)
            {
                value = node.Value;
                return true;
            }

            // check collision chain
            foreach (var entry in node.Chain)
            {
                if (entry.Key == key)
                {
                    value = entry.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public bool ContainsKey(TensorId key)
            => TryGetValue(key, out _);

        public bool Remove(TensorId key)
        {
            int hash = key.GetHashCode();
            bool removed = RemoveFromNode(_root, key, hash, 0);
            if (removed) _count--;
            return removed;
        }

        public FrozenHashMappedArrayTrie<TValue> Freeze()
            => FrozenHashMappedArrayTrie<TValue>.Build(_root, _count);

        private void AddToNode(
            ref HMATNode<TValue> node,
            TensorId key,
            TValue value,
            int hash,
            int depth)
        {
            int slot = (hash >> (depth * BitsPerLevel)) & SlotMask;

            if (!node.HasSlot(slot))
            {
                // empty slot — insert leaf directly
                node.InsertChild(slot, new HMATNode<TValue>(key, value));
                return;
            }

            var child = node.GetChild(slot)!;

            if (child.IsLeaf)
            {
                if (child.Key == key)
                {
                    // exact key match — update value
                    child.Value = value;
                    _count--;   // counteract the ++ in Add
                    return;
                }

                if (depth >= MaxDepth - 1)
                {
                    // max depth reached — chain the collision
                    child.Chain.Add(new ChainEntry<TValue>(key, value));
                    return;
                }

                // hash collision at this level — expand leaf into internal node
                var existing = child;
                var internal_ = new HMATNode<TValue>();
                int existHash = existing.Key.GetHashCode();
                int existSlot = (existHash >> ((depth + 1) * BitsPerLevel)) & SlotMask;
                internal_.InsertChild(existSlot, existing);

                // replace child with new internal node
                ReplaceChild(node, slot, internal_);

                // re-insert new key into expanded internal node
                AddToNode(ref internal_, key, value, hash, depth + 1);
                return;
            }

            // internal node — descend
            AddToNode(ref child, key, value, hash, depth + 1);
        }

        private HMATNode<TValue>? FindLeaf(
            HMATNode<TValue> node,
            TensorId key,
            int hash,
            int depth = 0)
        {
            int slot = (hash >> (depth * BitsPerLevel)) & SlotMask;
            var child = node.GetChild(slot);

            if (child is null) return null;
            if (child.IsLeaf) return child;
            if (depth >= MaxDepth) return null;

            return FindLeaf(child, key, hash, depth + 1);
        }

        private bool RemoveFromNode(
            HMATNode<TValue> node,
            TensorId key,
            int hash,
            int depth)
        {
            int slot = (hash >> (depth * BitsPerLevel)) & SlotMask;
            var child = node.GetChild(slot);

            if (child is null) return false;

            if (child.IsLeaf)
            {
                if (child.Key == key)
                {
                    // promote first chain entry if any
                    if (child.Chain.Count > 0)
                    {
                        var promoted = child.Chain[0];
                        child.Key = promoted.Key;
                        child.Value = promoted.Value;
                        child.Chain.RemoveAt(0);
                    }
                    else
                    {
                        RemoveChild(node, slot);
                    }
                    return true;
                }

                // check chain
                for (int i = 0; i < child.Chain.Count; i++)
                {
                    if (child.Chain[i].Key == key)
                    {
                        child.Chain.RemoveAt(i);
                        return true;
                    }
                }

                return false;
            }

            return RemoveFromNode(child, key, hash, depth + 1);
        }

        private static void ReplaceChild(
            HMATNode<TValue> node,
            int slot,
            HMATNode<TValue> newChild)
        {
            int index = node.SlotToIndex(slot);
            node.Children[index] = newChild;
        }

        private static void RemoveChild(HMATNode<TValue> node, int slot)
        {
            int index = node.SlotToIndex(slot);
            var newChildren = new HMATNode<TValue>?[node.Children.Length - 1];
            Array.Copy(node.Children, 0, newChildren, 0, index);
            Array.Copy(node.Children, index + 1, newChildren, index, node.Children.Length - index - 1);
            node.Children = newChildren;
            node.Bitmap &= ~(1u << slot);
        }
    }
}