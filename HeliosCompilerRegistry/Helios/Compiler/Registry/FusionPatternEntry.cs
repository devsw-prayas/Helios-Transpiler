namespace Helios.Compiler.Registry
{
    public readonly record struct FusionPatternEntry(OpCode Code, FusionTier Tier, ExpansionStep[] ExpSteps)
    {
        public byte Priority => (byte)(ExpSteps.Length * 10 + (byte)Tier);

        public bool Equals(FusionPatternEntry other)
            => Code == other.Code
               && Tier == other.Tier
               && ExpSteps.SequenceEqual(other.ExpSteps);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Code);
            hash.Add(Tier);
            foreach (var step in ExpSteps)
                hash.Add(step);
            return hash.ToHashCode();
        }
    }
}