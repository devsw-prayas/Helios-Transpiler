namespace Helios.Compiler.Registry
{
    public readonly struct OpCode(byte majorMinor, byte subset, byte opType, byte opId)
    {
        public const uint Magic = 0x494f5441u;

        private readonly ulong _opcode = ((ulong)Magic << 32)
                    | ((ulong)majorMinor << 24)
                    | ((ulong)subset << 16)
                    | ((ulong)opType << 8)
                    | ((ulong)opId);

        public ulong Packed => _opcode;
        public uint High => (uint)(_opcode >> 32);          
        public uint Low => (uint)(_opcode & 0xFFFFFFFFu); 
        public byte Subset => (byte)((_opcode >> 16) & 0xFF);
        public byte OpType => (byte)((_opcode >> 8) & 0xFF);
        public byte OpId => (byte)(_opcode & 0xFF);

        public bool Equals(OpCode other) => _opcode == other._opcode;
        public override bool Equals(object? obj) => obj is OpCode other && Equals(other);
        public override int GetHashCode() => _opcode.GetHashCode();

        public static bool operator ==(OpCode left, OpCode right) => left._opcode == right._opcode;
        public static bool operator !=(OpCode left, OpCode right) => left._opcode != right._opcode;

        public override string ToString() => $"0x{_opcode:X16}";
    }
}