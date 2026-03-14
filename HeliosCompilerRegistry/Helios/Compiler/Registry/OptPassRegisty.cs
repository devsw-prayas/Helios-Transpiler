using System.Collections.Frozen;

namespace Helios.Compiler.Registry
{
    public static class OptPassRegistry
    {
        public static readonly FrozenDictionary<string, OptPassEntry> Passes;

        static OptPassRegistry()
        {
            Passes = new Dictionary<string, OptPassEntry>(StringComparer.Ordinal)
            {
                ["DeadCodeElimination"] = new("DeadCodeElimination", 1, 1),
                ["CommonSubexpression"] = new("CommonSubexpression", 1, 2),
                ["GradCheckpoint"] = new("GradCheckpoint", 2, 3),
                ["BufferReuse"] = new("BufferReuse", 2, 4),
                ["CheckpointAccumulation"] = new("CheckpointAccumulation", 3, 5),
            }.ToFrozenDictionary(StringComparer.Ordinal);
        }
    }
}