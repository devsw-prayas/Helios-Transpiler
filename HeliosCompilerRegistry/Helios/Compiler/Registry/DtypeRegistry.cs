using System.Collections.Frozen;

namespace Helios.Compiler.Registry
{
    public static class DtypeRegistry
    {
        public static readonly FrozenDictionary<string, byte> DTypeRegistry;

        static DtypeRegistry()
        {
            DTypeRegistry = new Dictionary<string, byte>
            {
                ["float32"] = 0x00,
                ["float6"] = 0x01,
                ["bfloat16"] = 0x02,
                ["float64"] = 0x03,
                ["int8"] = 0x04,
                ["int16"] = 0x05,
                ["int32"] = 0x06,
                ["int64"] = 0x07,
                ["tf32"] = 0x08

            }.ToFrozenDictionary(StringComparer.Ordinal);
        }
    }
}