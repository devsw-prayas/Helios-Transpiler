namespace Helios.Compiler.Core
{
    public readonly struct ChainEntry<TValue>(TensorId key, TValue value) : IEquatable<ChainEntry<TValue>>
    {
        public TensorId Key { get; } = key;
        public TValue Value { get; } = value;

        public bool Equals(ChainEntry<TValue> other)
            => Key == other.Key
            && EqualityComparer<TValue>.Default.Equals(Value, other.Value);

        public override bool Equals(object? obj)
            => obj is ChainEntry<TValue> other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(Key, EqualityComparer<TValue>.Default.GetHashCode(Value!));

        public static bool operator ==(ChainEntry<TValue> left, ChainEntry<TValue> right) => left.Equals(right);
        public static bool operator !=(ChainEntry<TValue> left, ChainEntry<TValue> right) => !left.Equals(right);
    }
}