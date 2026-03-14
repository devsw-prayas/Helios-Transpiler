using System.Runtime.InteropServices;

namespace Helios.Compiler.Core
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct FrozenHMATNode
    {
        public uint Bitmap;             // 32-bit — which slots are populated
        public int ChildrenOffset;     // byte offset into native block where children array starts
        public int ChildCount;         // number of populated children — popcount(Bitmap)
        public int ValueIndex;         // index into value buffer — -1 if internal node
        public int KeyValue;           // TensorId.Value — -1 if internal node
        public int ChainOffset;        // byte offset into chain buffer — -1 if no chain
        public int ChainCount;         // number of chain entries — 0 if no chain
        public bool IsLeaf;             // true if this is a leaf node

        public static readonly FrozenHMATNode Invalid = new()
        {
            Bitmap = 0u,
            ChildrenOffset = -1,
            ChildCount = 0,
            ValueIndex = -1,
            KeyValue = -1,
            ChainOffset = -1,
            ChainCount = 0,
            IsLeaf = false,
        };

        // Check if a logical slot is populated
        public readonly bool HasSlot(int slot)
            => (Bitmap & (1u << slot)) != 0u;

        // Get dense array index for a logical slot
        public readonly int SlotToIndex(int slot)
            => int.PopCount((int)(Bitmap & ((1u << slot) - 1u)));
    }
}