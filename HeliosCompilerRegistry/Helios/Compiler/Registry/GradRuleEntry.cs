namespace Helios.Compiler.Registry
{
    public readonly record struct GradRuleEntry(string ForwardOp, GradExpr[][] InputGradients)
    {
        public bool Equals(GradRuleEntry other)
        {
            return string.Equals(ForwardOp, other.ForwardOp, StringComparison.Ordinal)
                   && InputGradients.Length == other.InputGradients.Length
                   && InputGradients.Zip(other.InputGradients)
                       .All(pair => pair.First.SequenceEqual(pair.Second));
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(ForwardOp, StringComparer.Ordinal);

            foreach (var inputGrad in InputGradients)
                foreach (var expr in inputGrad)
                    hash.Add(expr);

            return hash.ToHashCode();
        }
    }
}