namespace Helios.Compiler.Core
{
    internal sealed class HMATNode<TValue>
    {
        public uint Bitmap { get; set; } = 0u;
        public HMATNode<TValue>?[] Children { get; set; } = [];

        public TensorId Key { get; set; } = TensorId.Invalid;
        public TValue? Value { get; set; } = default;
        public bool IsLeaf { get; set; } = false;
        public List<ChainEntry<TValue>> Chain { get; set; } = [];

        // Internal node
        public HMATNode() { }

        // Leaf node
        public HMATNode(TensorId key, TValue value)
        {
            Key = key;
            Value = value;
            IsLeaf = true;
        }

        // Check if a logical slot is populated
        public bool HasSlot(int slot)
            => (Bitmap & (1u << slot)) != 0u;

        // Get the dense array index for a logical slot
        // popcount of all bits below the slot position
        public int SlotToIndex(int slot)
            => int.PopCount((int)(Bitmap & ((1u << slot) - 1u)));

        // Insert a child at a logical slot — expands Children array
        public void InsertChild(int slot, HMATNode<TValue> child)
        {
            int index = SlotToIndex(slot);
            Bitmap |= (1u << slot);

            var newChildren = new HMATNode<TValue>?[Children.Length + 1];
            Array.Copy(Children, 0, newChildren, 0, index);
            Array.Copy(Children, index, newChildren, index + 1, Children.Length - index);
            newChildren[index] = child;
            Children = newChildren;
        }

        // Get child at a logical slot — returns null if not populated
        public HMATNode<TValue>? GetChild(int slot)
        {
            if (!HasSlot(slot)) return null;
            return Children[SlotToIndex(slot)];
        }
    }
}