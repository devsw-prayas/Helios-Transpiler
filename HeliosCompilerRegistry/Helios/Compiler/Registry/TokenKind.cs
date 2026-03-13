namespace Helios.Compiler.Registry
{
    public enum TokenKind : byte
    {
        // ── Structural ────────────────────────────────────────────────
        KdNewLine,
        KdIndent,
        KdDedent,
        KdEoF,

        // ── Identifiers & Literals ────────────────────────────────────
        KdName,
        KdIntegerLiteral,
        KdFloatingLiteral,
        KdStringLiteral,

        // ── Model Structure Keywords ──────────────────────────────────
        KdModel,
        KdInit,
        KdForward,
        KdBackward,
        KdReturn,
        KdInclude,
        KdRun,
        KdTuple,
        KdSelf,
        KdDefault,

        // ── HCX Control Flow ──────────────────────────────────────────
        KdLoopBegin,
        KdLoopOut,
        KdWhileBegin,
        KdWhileOut,
        KdIfBegin,
        KdIfElse,           // if_else  (not "else")
        KdIfEnd,
        KdSwitchBegin,
        KdSwitchCase,       // switch_case  (not "case")
        KdSwitchEnd,

        // ── Compiler Directive Triggers ───────────────────────────────
        // Emitted by lexer after '#' disambiguation — '#' itself is never emitted
        KdCompiler,
        KdPragma,

        // ── Compiler Directive Names ──────────────────────────────────
        KdOptimize,
        KdTarget,
        KdMixPrec,
        KdMemory,
        KdEnforce,
        KdQuantize,
        KdGradClip,
        KdSeed,
        KdVerify,
        KdDebug,
        KdProfile,

        // ── Pragma Directive Names ────────────────────────────────────
        KdLoopDepth,
        KdGrow,
        KdEnd,              // #pragma end

        // ── Directive Argument Values — Target ────────────────────────
        KdCpu,              // cpu
        KdGpu,              // gpu

        // ── Directive Argument Values — Memory ───────────────────────
        KdStatic,           // static
        KdDynamic,          // dynamic

        // ── Directive Argument Values — Switch ────────────────────────
        KdOn,               // on
        KdOff,              // off

        // ── Directive Argument Values — Enforce ───────────────────────
        KdReuse,            // reuse

        // ── Directive Argument Values — grad_clip Mode ────────────────
        KdNorm,             // norm
        KdValue,            // value

        // ── Directive Argument Values — Precision (mix_prec / quantize)
        KdF16,              // f16
        KdBf16,             // bf16
        KdF32,              // f32
        KdF64,              // f64
        KdInt8Arg,          // int8  (quantize argument)
        KdInt4Arg,          // int4  (quantize argument)

        // ── Directive Argument Values — Seed Strategy ─────────────────
        KdReplicate,        // replicate
        KdJitter,           // jitter
        KdHash,             // hash

        // ── Dtype Keywords (type hint positions) ──────────────────────
        KdFloat16,          // float16
        KdBFloat16,         // bfloat16
        KdFloat32,          // float32
        KdFloat64,          // float64
        KdTf32,             // tf32
        KdInt8,             // int8
        KdInt16,            // int16
        KdInt32,            // int32
        KdInt64,            // int64
        KdTensor,           // tensor  (type hint wrapper)

        // ── Operators ─────────────────────────────────────────────────
        KdEquals,           // =
        KdPlus,             // +
        KdMinus,            // -
        KdStar,             // *
        KdSlash,            // /
        KdMatmul,           // @
        KdDot,              // .
        KdComma,            // ,
        KdColon,            // :
        KdPipe,             // |
        KdLParen,           // (
        KdRParen,           // )
        KdLBracket,         // [
        KdRBracket,         // ]

        // ── Meta ──────────────────────────────────────────────────────
        KdError             // carries offending character — never blocks tokenization
    }
}