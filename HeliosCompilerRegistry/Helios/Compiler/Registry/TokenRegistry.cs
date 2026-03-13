using System.Collections.Frozen;

namespace Helios.Compiler.Registry
{
    /// <summary>
    /// Flat keyword lookup table — built once at startup into a FrozenDictionary.
    /// All lookups use StringComparer.Ordinal (case-sensitive) per spec.
    /// '#' disambiguation (compiler / pragma) is handled by the lexer before this table
    /// is consulted, so "compiler" and "pragma" are included here for post-'#' lookup only.
    /// </summary>
    public static class KeywordRegistry
    {
        public static readonly FrozenDictionary<string, TokenKind> TokenRegistry;

        static KeywordRegistry()
        {
            TokenRegistry = new Dictionary<string, TokenKind>(StringComparer.Ordinal)
            {
                ["model"] = TokenKind.KdModel,
                ["init"] = TokenKind.KdInit,
                ["forward"] = TokenKind.KdForward,
                ["backward"] = TokenKind.KdBackward,
                ["return"] = TokenKind.KdReturn,
                ["include"] = TokenKind.KdInclude,
                ["run"] = TokenKind.KdRun,
                ["tuple"] = TokenKind.KdTuple,
                ["self"] = TokenKind.KdSelf,
                ["default"] = TokenKind.KdDefault,

                ["loop_begin"] = TokenKind.KdLoopBegin,
                ["loop_out"] = TokenKind.KdLoopOut,
                ["while_begin"] = TokenKind.KdWhileBegin,
                ["while_out"] = TokenKind.KdWhileOut,
                ["if_begin"] = TokenKind.KdIfBegin,
                ["if_else"] = TokenKind.KdIfElse,
                ["if_end"] = TokenKind.KdIfEnd,
                ["switch_begin"] = TokenKind.KdSwitchBegin,
                ["switch_case"] = TokenKind.KdSwitchCase,
                ["switch_end"] = TokenKind.KdSwitchEnd,

                ["compiler"] = TokenKind.KdCompiler,
                ["pragma"] = TokenKind.KdPragma,

                ["optimize"] = TokenKind.KdOptimize,
                ["target"] = TokenKind.KdTarget,
                ["mix_prec"] = TokenKind.KdMixPrec,
                ["memory"] = TokenKind.KdMemory,
                ["enforce"] = TokenKind.KdEnforce,
                ["quantize"] = TokenKind.KdQuantize,
                ["grad_clip"] = TokenKind.KdGradClip,
                ["seed"] = TokenKind.KdSeed,
                ["verify"] = TokenKind.KdVerify,
                ["debug"] = TokenKind.KdDebug,
                ["profile"] = TokenKind.KdProfile,

                ["loop_depth"] = TokenKind.KdLoopDepth,
                ["grow"] = TokenKind.KdGrow,
                ["end"] = TokenKind.KdEnd,

                ["cpu"] = TokenKind.KdCpu,
                ["gpu"] = TokenKind.KdGpu,

                ["static"] = TokenKind.KdStatic,
                ["dynamic"] = TokenKind.KdDynamic,

                ["on"] = TokenKind.KdOn,
                ["off"] = TokenKind.KdOff,

                ["reuse"] = TokenKind.KdReuse,

                ["norm"] = TokenKind.KdNorm,
                ["value"] = TokenKind.KdValue,

                ["f16"] = TokenKind.KdF16,
                ["bf16"] = TokenKind.KdBf16,
                ["f32"] = TokenKind.KdF32,
                ["f64"] = TokenKind.KdF64,
                ["int8"] = TokenKind.KdInt8Arg,
                ["int4"] = TokenKind.KdInt4Arg,

                ["replicate"] = TokenKind.KdReplicate,
                ["jitter"] = TokenKind.KdJitter,
                ["hash"] = TokenKind.KdHash,

                ["float16"] = TokenKind.KdFloat16,
                ["bfloat16"] = TokenKind.KdBFloat16,
                ["float32"] = TokenKind.KdFloat32,
                ["float64"] = TokenKind.KdFloat64,
                ["tf32"] = TokenKind.KdTf32,

                ["int16"] = TokenKind.KdInt16,
                ["int32"] = TokenKind.KdInt32,
                ["int64"] = TokenKind.KdInt64,
                ["tensor"] = TokenKind.KdTensor,

            }.ToFrozenDictionary(StringComparer.Ordinal);
        }
    }
}