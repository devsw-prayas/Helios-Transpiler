namespace Helios.Compiler.Registry
{
    public readonly record struct ExpansionStep(OpCode Code, string[] Operands)
    {
        public bool Equals(ExpansionStep other)
            => Code == other.Code
               && Operands.SequenceEqual(other.Operands, StringComparer.Ordinal);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Code);
            foreach (var op in Operands)
                hash.Add(op, StringComparer.Ordinal);
            return hash.ToHashCode();
        }
    }
}