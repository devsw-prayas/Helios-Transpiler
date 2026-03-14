namespace Helios.Compiler.Registry
{
    public readonly record struct GradExpr(string Op, string[] Operands, string Output)
    {
        public bool Equals(GradExpr exp)
        {
            return string.Equals(exp.Op, Op, StringComparison.Ordinal) && 
                   string.Equals(exp.Output, Output, StringComparison.Ordinal) &&
            exp.Operands.SequenceEqual(Operands, StringComparer.Ordinal);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Op);
            hash.Add(Output);
            foreach (var step in Operands)
                hash.Add(step);
            return hash.ToHashCode();
        }
    }
}