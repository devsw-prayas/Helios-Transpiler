using System.Collections.Frozen;

namespace Helios.Compiler.Registry
{
    public static class DiagnosticRegistry
    {
        public static readonly FrozenDictionary<string, string> Diagnostics;

        public static readonly FrozenDictionary<string, string> Notes;

        static DiagnosticRegistry()
        {
            Notes = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                // ── H-0200 — Parser ───────────────────────────────────────────
                ["H-0200-0023"] = "precision hierarchy is f16 < bf16 < f32 < f64",
                ["H-0200-0026"] = "remove either mix_prec or quantize — only one precision strategy is permitted per file",

                // ── H-0300 — Resolver ─────────────────────────────────────────
                ["H-0300-0024"] = "add #pragma loop_depth(N) immediately before this while_begin",
                ["H-0300-0026"] = "add #pragma loop_depth(N) before #pragma grow in memory(static) context",
                ["H-0300-0027"] = "reduce loop_depth(N) to 256 or below in memory(static) context",
                ["H-0300-0028"] = "reduce switch case count to 512 or below",
                ["H-0300-0033"] = "add loss_scale as the first op in backward() before gradient computation",
                ["H-0300-0034"] = "add loss_unscale as the last op in backward() after check_overflow",
                ["H-0300-0035"] = "add check_overflow after gradient computation in backward()",
                ["H-0300-0036"] = "add update_scale after check_overflow in backward()",
                ["H-0300-0037"] = "remove grad_clip directive or switch to backward() = default / optimize_default(N)",
                ["H-0300-0038"] = "loss_scale must be the first op in backward() — move it before gradient computation",
                ["H-0300-0039"] = "reorder: check_overflow must appear before update_scale in backward()",
                ["H-0300-0040"] = "reorder: loss_unscale must appear after check_overflow in backward()",

                // ── H-0600 — Type Checker ─────────────────────────────────────
                ["H-0600-0013"] = "declare #pragma grow(tensor, delta:axis) before the enclosing loop if growth is intended",
                ["H-0600-0016"] = "only axes declared in #pragma grow may change shape at loop_out",
                ["H-0600-0017"] = "axis index must be less than the tensor rank",

            }.ToFrozenDictionary(StringComparer.Ordinal);

            Diagnostics = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                // ── H-0100 — Lexer ────────────────────────────────────────────
                ["H-0100-0001"] = "unexpected character",
                ["H-0100-0002"] = "unterminated string literal",
                ["H-0100-0003"] = "invalid escape sequence",
                ["H-0100-0004"] = "invalid number literal",
                ["H-0100-0005"] = "indentation error",
                ["H-0100-0006"] = "inconsistent indentation",
                ["H-0100-0007"] = "unexpected end of file",

                // ── H-0200 — Parser ───────────────────────────────────────────
                ["H-0200-0001"] = "expected model keyword",
                ["H-0200-0002"] = "expected init operation",
                ["H-0200-0003"] = "expected forward operation",
                ["H-0200-0004"] = "expected backward operation",
                ["H-0200-0005"] = "missing colon after block header",
                ["H-0200-0006"] = "expected NAME got TOKEN",
                ["H-0200-0007"] = "malformed param declaration",
                ["H-0200-0008"] = "malformed submodel declaration",
                ["H-0200-0009"] = "malformed scalar declaration",
                ["H-0200-0010"] = "malformed tensor expression",
                ["H-0200-0011"] = "malformed return statement",
                ["H-0200-0012"] = "malformed tuple expression",
                ["H-0200-0013"] = "malformed type hint",
                ["H-0200-0014"] = "malformed run statement",
                ["H-0200-0015"] = "malformed include statement",
                ["H-0200-0016"] = "unexpected token in block",
                ["H-0200-0017"] = "unexpected end of block",
                ["H-0200-0018"] = "missing #compiler optimize as first statement",
                ["H-0200-0019"] = "#compiler optimize must precede all other directives",
                ["H-0200-0020"] = "duplicate #compiler directive",
                ["H-0200-0021"] = "#compiler directive after include or model",
                ["H-0200-0022"] = "invalid mix_prec argument — expected PREC_DTYPE",
                ["H-0200-0023"] = "mix_prec lower precision must be strictly less than higher precision",
                ["H-0200-0024"] = "invalid quantize argument — expected int8 or int4",
                ["H-0200-0025"] = "invalid optimize level — expected 0..3",
                ["H-0200-0026"] = "mix_prec and quantize cannot be used together",
                ["H-0200-0027"] = "carry tensor in loop_begin/while_begin missing type hint",
                ["H-0200-0028"] = "#pragma grow requires at least one delta:axis pair",
                ["H-0200-0029"] = "#pragma grow duplicate axis declaration",
                ["H-0200-0030"] = "malformed #pragma grow — expected NAME, INT:INT pairs",

                // ── H-0300 — Resolver ─────────────────────────────────────────
                ["H-0300-0001"] = "undefined name",
                ["H-0300-0002"] = "undefined model",
                ["H-0300-0003"] = "undefined submodel field",
                ["H-0300-0004"] = "undefined param",
                ["H-0300-0005"] = "name already declared in scope",
                ["H-0300-0006"] = "model name collision across includes",
                ["H-0300-0007"] = "circular include detected",
                ["H-0300-0008"] = "include file not found",
                ["H-0300-0009"] = "param declared outside init",
                ["H-0300-0010"] = "tensor op in init block",
                ["H-0300-0011"] = "HADX op in forward block",
                ["H-0300-0012"] = "H-ISA op in backward block",
                ["H-0300-0013"] = "submodel call on non-submodel",
                ["H-0300-0014"] = "self access outside model scope",
                ["H-0300-0015"] = "run references undefined model",
                ["H-0300-0016"] = "submodel depth exceeds resolution",
                ["H-0300-0017"] = "backward default on non-differentiable op",
                ["H-0300-0018"] = "optimize level below #compiler floor",
                ["H-0300-0019"] = "optimize_default level below #compiler floor",
                ["H-0300-0020"] = "loop_out carry_out count does not match carry_in count",
                ["H-0300-0021"] = "switch_case count does not match declared N in switch_begin",
                ["H-0300-0022"] = "capture used as carry_in — declare as carry_in only",
                ["H-0300-0023"] = "tensor from outer scope referenced without capture",
                ["H-0300-0024"] = "while_begin in memory(static) context requires #pragma loop_depth",
                ["H-0300-0025"] = "dynamic shape in memory(static) context without #pragma grow",
                ["H-0300-0026"] = "#pragma grow without #pragma loop_depth in memory(static) context",
                ["H-0300-0027"] = "loop_depth exceeds maximum of 256 in memory(static) context",
                ["H-0300-0028"] = "switch_begin N exceeds maximum of 512",
                ["H-0300-0029"] = "grad_barrier inside loop_begin body — place at loop boundary",
                ["H-0300-0030"] = "grad_barrier inside while_begin body — place at loop boundary",
                ["H-0300-0031"] = "grad_barrier inside if_begin body — place outside branch",
                ["H-0300-0032"] = "grad_barrier inside switch_begin body — place outside switch",
                ["H-0300-0033"] = "mix_prec active — loss_scale missing in custom backward()",
                ["H-0300-0034"] = "mix_prec active — loss_unscale missing in custom backward()",
                ["H-0300-0035"] = "mix_prec active — check_overflow missing in custom backward()",
                ["H-0300-0036"] = "mix_prec active — update_scale missing in custom backward()",
                ["H-0300-0037"] = "grad_clip active — custom backward() is forbidden",
                ["H-0300-0038"] = "loss_scale must precede gradient computation in backward()",
                ["H-0300-0039"] = "check_overflow must precede update_scale in backward()",
                ["H-0300-0040"] = "loss_unscale must follow check_overflow in backward()",
                ["H-0300-0041"] = "HCX op in backward block",

                // ── H-0400 — Scalar Folder ────────────────────────────────────
                ["H-0400-0001"] = "division by zero in scalar expression",
                ["H-0400-0002"] = "scalar overflow",
                ["H-0400-0003"] = "scalar underflow",
                ["H-0400-0004"] = "invalid scalar operand type",
                ["H-0400-0005"] = "unresolved scalar reference",

                // ── H-0500 — Emitter ──────────────────────────────────────────
                ["H-0500-0001"] = "unknown opcode mnemonic",
                ["H-0500-0002"] = "opcode operand count mismatch",
                ["H-0500-0003"] = "const pool overflow",
                ["H-0500-0004"] = "param table overflow",
                ["H-0500-0005"] = "opcode stream overflow",
                ["H-0500-0006"] = "invalid opcode for block type",
                ["H-0500-0007"] = ".hlx write failure",

                // ── H-0600 — Type Checker ─────────────────────────────────────
                ["H-0600-0001"] = "dtype mismatch",
                ["H-0600-0002"] = "shape mismatch",
                ["H-0600-0003"] = "rank mismatch",
                ["H-0600-0004"] = "incompatible dtypes in binop",
                ["H-0600-0005"] = "incompatible shapes in binop",
                ["H-0600-0006"] = "invalid dtype for op",
                ["H-0600-0007"] = "invalid shape for op",
                ["H-0600-0008"] = "type hint on non-tensor",
                ["H-0600-0009"] = "return type mismatch",
                ["H-0600-0010"] = "tuple element type mismatch",
                ["H-0600-0011"] = "submodel input type mismatch",
                ["H-0600-0012"] = "unsupported dtype",
                ["H-0600-0013"] = "carry tensor shape mismatch at loop_out — declare #pragma grow if growth is intended",
                ["H-0600-0014"] = "carry tensor dtype mismatch at loop_out",
                ["H-0600-0015"] = "carry tensor rank mismatch at loop_out",
                ["H-0600-0016"] = "#pragma grow axis mismatch — non-grow axis shape changed at loop_out",
                ["H-0600-0017"] = "#pragma grow axis out of bounds for tensor rank",
                ["H-0600-0018"] = "tuple carry element type mismatch at loop_out",

                // ── W-0200 — Parser Warnings ──────────────────────────────────
                ["W-0200-0001"] = "unrecognized target hint, ignoring",
                ["W-0200-0002"] = "mix_prec on CPU target may not be supported",

                // ── W-0300 — Resolver Warnings ────────────────────────────────
                ["W-0300-0001"] = "unused param declared in init",
                ["W-0300-0002"] = "unused submodel declared in init",
                ["W-0300-0003"] = "forward() contains #pragma optimize but backward() = default — consider optimize_default(N)",
                ["W-0300-0004"] = "seed(replicate) in training context produces correlated dropout/random samples",
                ["W-0300-0005"] = "subset 12 ops present but no #compiler seed declared — results are non-deterministic",

                // ── W-0400 — Scalar Folder Warnings ──────────────────────────
                ["W-0400-0001"] = "scalar expression result truncated",

            }.ToFrozenDictionary(StringComparer.Ordinal);
        }
    }
}