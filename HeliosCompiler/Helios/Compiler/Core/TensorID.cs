namespace Helios.Compiler.Core
{
    public readonly struct TensorId(int value) : IEquatable<TensorId>
    {
        public int Value { get; } = value;

        public static TensorId From(int val) => new(val);
        public static readonly TensorId Invalid = From(-1);

        public bool Equals(TensorId other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is TensorId other && Equals(other);
        public override int GetHashCode() => Value;
        public override string ToString() => $"t{Value}";

        public static bool operator ==(TensorId left, TensorId right) => left.Value == right.Value;
        public static bool operator !=(TensorId left, TensorId right) => left.Value != right.Value;
    }
}