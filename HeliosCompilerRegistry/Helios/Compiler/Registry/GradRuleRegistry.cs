using System.Collections.Frozen;

namespace Helios.Compiler.Registry
{
    public static class GradRuleRegistry
    {
        public static readonly FrozenDictionary<string, GradRuleEntry> GradRules;

        static GradRuleRegistry()
        {
            GradRules = new Dictionary<string, GradRuleEntry>(StringComparer.Ordinal)
            {
                // ── Subset 0a — Linear Algebra ────────────────────────────────
                // matmul(A, B) → C
                // grad_A = matmul(dC, Bᵀ)
                // grad_B = matmul(Aᵀ, dC)
                ["matmul"] = new("matmul",
                [
                    [   // grad w.r.t A
                        new("transpose", ["B"],       "Bt"),
                        new("matmul",    ["dC", "Bt"], "dA"),
                    ],
                    [   // grad w.r.t B
                        new("transpose", ["A"],       "At"),
                        new("matmul",    ["At", "dC"], "dB"),
                    ],
                ]),

                // batch_matmul(A, B) → C
                // grad_A = batch_matmul(dC, Bᵀ)
                // grad_B = batch_matmul(Aᵀ, dC)
                ["batch_matmul"] = new("batch_matmul",
                [
                    [
                        new("transpose",    ["B"],       "Bt"),
                        new("batch_matmul", ["dC", "Bt"], "dA"),
                    ],
                    [
                        new("transpose",    ["A"],       "At"),
                        new("batch_matmul", ["At", "dC"], "dB"),
                    ],
                ]),

                // dot(A, B) → C
                // grad_A = mul(dC, B)
                // grad_B = mul(dC, A)
                ["dot"] = new("dot",
                [
                    [new("mul", ["dC", "B"], "dA")],
                    [new("mul", ["dC", "A"], "dB")],
                ]),

                // ── Subset 04 — Arithmetic ────────────────────────────────────
                // add(A, B) → C
                // grad_A = identity(dC)
                // grad_B = identity(dC)
                ["add"] = new("add",
                [
                    [new("identity", ["dC"], "dA")],
                    [new("identity", ["dC"], "dB")],
                ]),

                // sub(A, B) → C
                // grad_A = identity(dC)
                // grad_B = neg(dC)
                ["sub"] = new("sub",
                [
                    [new("identity", ["dC"],  "dA")],
                    [new("neg",      ["dC"],  "dB")],
                ]),

                // mul(A, B) → C
                // grad_A = mul(dC, B)
                // grad_B = mul(dC, A)
                ["mul"] = new("mul",
                [
                    [new("mul", ["dC", "B"], "dA")],
                    [new("mul", ["dC", "A"], "dB")],
                ]),

                // div(A, B) → C
                // grad_A = div(dC, B)
                // grad_B = neg(div(mul(dC, A), mul(B, B)))
                ["div"] = new("div",
                [
                    [
                        new("div", ["dC", "B"], "dA"),
                    ],
                    [
                        new("mul", ["dC", "A"],  "t0"),
                        new("mul", ["B",  "B"],  "t1"),
                        new("div", ["t0", "t1"], "t2"),
                        new("neg", ["t2"],        "dB"),
                    ],
                ]),

                // neg(A) → C
                // grad_A = neg(dC)
                ["neg"] = new("neg",
                [
                    [new("neg", ["dC"], "dA")],
                ]),

                // abs(A) → C
                // grad_A = mul(dC, sign(A))
                ["abs"] = new("abs",
                [
                    [
                        new("sign", ["A"],       "t0"),
                        new("mul",  ["dC", "t0"], "dA"),
                    ],
                ]),

                // ── Subset 05 — Math ──────────────────────────────────────────
                // exp(A) → C
                // grad_A = mul(dC, C)
                ["exp"] = new("exp",
                [
                    [new("mul", ["dC", "C"], "dA")],
                ]),

                // log(A) → C
                // grad_A = div(dC, A)
                ["log"] = new("log",
                [
                    [new("div", ["dC", "A"], "dA")],
                ]),

                // pow(A, B) → C
                // grad_A = mul(dC, mul(B, pow(A, sub(B, 1))))
                ["pow"] = new("pow",
                [
                    [
                        new("sub", ["B",  "B"],       "t0"),   // B - 1 (placeholder — emitter resolves scalar 1)
                        new("pow", ["A",  "t0"],       "t1"),
                        new("mul", ["B",  "t1"],       "t2"),
                        new("mul", ["dC", "t2"],       "dA"),
                    ],
                    [
                        new("log", ["A"],              "t0"),
                        new("mul", ["C",  "t0"],       "t1"),
                        new("mul", ["dC", "t1"],       "dB"),
                    ],
                ]),

                // sqrt(A) → C
                // grad_A = div(dC, mul(2, C))
                ["sqrt"] = new("sqrt",
                [
                    [
                        new("add", ["C",  "C"],  "t0"),   // 2 * C
                        new("div", ["dC", "t0"], "dA"),
                    ],
                ]),

                // sin(A) → C
                // grad_A = mul(dC, cos(A))
                ["sin"] = new("sin",
                [
                    [
                        new("cos", ["A"],       "t0"),
                        new("mul", ["dC", "t0"], "dA"),
                    ],
                ]),

                // cos(A) → C
                // grad_A = mul(dC, neg(sin(A)))
                ["cos"] = new("cos",
                [
                    [
                        new("sin", ["A"],       "t0"),
                        new("neg", ["t0"],       "t1"),
                        new("mul", ["dC", "t1"], "dA"),
                    ],
                ]),

                // tanh(A) → C
                // grad_A = mul(dC, sub(1, mul(C, C)))
                ["tanh"] = new("tanh",
                [
                    [
                        new("mul", ["C",  "C"],  "t0"),
                        new("sub", ["C",  "t0"], "t1"),   // 1 - tanh²  (emitter substitutes scalar 1)
                        new("mul", ["dC", "t1"], "dA"),
                    ],
                ]),

                // erf(A) → C
                // grad_A = mul(dC, mul(2/sqrt(π), exp(neg(mul(A,A)))))
                ["erf"] = new("erf",
                [
                    [
                        new("mul", ["A",  "A"],  "t0"),
                        new("neg", ["t0"],        "t1"),
                        new("exp", ["t1"],        "t2"),
                        new("mul", ["dC", "t2"], "dA"),   // scale by 2/sqrt(π) handled by emitter
                    ],
                ]),

                // ── Subset 06 — Activation ────────────────────────────────────
                // relu(A) → C
                // grad_A = mul(dC, gt(A, 0))
                ["relu"] = new("relu",
                [
                    [
                        new("gt",  ["A",  "A"],  "t0"),   // gt(A, 0) — emitter substitutes scalar 0
                        new("mul", ["dC", "t0"], "dA"),
                    ],
                ]),

                // gelu(A) → C
                // grad_A = mul(dC, gelu_grad(A))   — gelu_grad is an HADX primitive
                ["gelu"] = new("gelu",
                [
                    [
                        new("hadx.vjp", ["C", "A", "dC"], "dA"),
                    ],
                ]),

                // sigmoid(A) → C
                // grad_A = mul(dC, mul(C, sub(1, C)))
                ["sigmoid"] = new("sigmoid",
                [
                    [
                        new("sub", ["C",  "C"],  "t0"),   // 1 - C  (emitter substitutes scalar 1)
                        new("mul", ["C",  "t0"], "t1"),
                        new("mul", ["dC", "t1"], "dA"),
                    ],
                ]),

                // swish(A) → C  (swish = A * sigmoid(A))
                // grad_A = mul(dC, add(C, mul(sigmoid(A), sub(1, C))))
                ["swish"] = new("swish",
                [
                    [
                        new("sigmoid", ["A"],        "s"),
                        new("sub",     ["s",  "C"],  "t0"),   // sigmoid - C
                        new("mul",     ["s",  "t0"], "t1"),
                        new("add",     ["C",  "t1"], "t2"),
                        new("mul",     ["dC", "t2"], "dA"),
                    ],
                ]),

                // softplus(A) → C
                // grad_A = mul(dC, sigmoid(A))
                ["softplus"] = new("softplus",
                [
                    [
                        new("sigmoid", ["A"],       "t0"),
                        new("mul",     ["dC", "t0"], "dA"),
                    ],
                ]),

                // ── Subset 07 — Comparison (no gradients — discrete outputs) ──
                // gt, lt, ge, le, eq, ne produce boolean tensors — not differentiable
                // emitter skips grad emission for these — no entries needed

                // ── Subset 09 — Reduction ─────────────────────────────────────
                // reduce_sum(A) → C
                // grad_A = expand_as(dC, A)
                ["reduce_sum"] = new("reduce_sum",
                [
                    [new("expand_as", ["dC", "A"], "dA")],
                ]),

                // reduce_mean(A) → C
                // grad_A = div(expand_as(dC, A), numel(A))
                ["reduce_mean"] = new("reduce_mean",
                [
                    [
                        new("expand_as", ["dC", "A"],  "t0"),
                        new("numel",     ["A"],          "t1"),
                        new("div",       ["t0", "t1"],  "dA"),
                    ],
                ]),

                // reduce_max(A) → C
                // grad_A = mul(dC, eq(A, C))
                ["reduce_max"] = new("reduce_max",
                [
                    [
                        new("expand_as", ["C",  "A"],  "t0"),
                        new("eq",        ["A",  "t0"], "t1"),
                        new("expand_as", ["dC", "A"],  "t2"),
                        new("mul",       ["t2", "t1"], "dA"),
                    ],
                ]),

                // reduce_min(A) → C  — symmetric to reduce_max
                ["reduce_min"] = new("reduce_min",
                [
                    [
                        new("expand_as", ["C",  "A"],  "t0"),
                        new("eq",        ["A",  "t0"], "t1"),
                        new("expand_as", ["dC", "A"],  "t2"),
                        new("mul",       ["t2", "t1"], "dA"),
                    ],
                ]),

                // ── Subset 0d — Normalization ─────────────────────────────────
                // layernorm(A) → C
                // grad_A = layernorm_vjp(dC, A, C)  — HADX vjp primitive
                ["layernorm"] = new("layernorm",
                [
                    [new("hadx.vjp", ["C", "A", "dC"], "dA")],
                ]),

                // rmsnorm(A) → C
                // grad_A = hadx.vjp(C, A, dC)
                ["rmsnorm"] = new("rmsnorm",
                [
                    [new("hadx.vjp", ["C", "A", "dC"], "dA")],
                ]),

                // batchnorm(A) → C
                // grad_A = hadx.vjp(C, A, dC)
                ["batchnorm"] = new("batchnorm",
                [
                    [new("hadx.vjp", ["C", "A", "dC"], "dA")],
                ]),

                // ── Subset 0b — Convolution ───────────────────────────────────
                // conv2d(X, W) → C
                // grad_X = conv_t2d(dC, W)
                // grad_W = conv2d_grad_w(X, dC)   — HADX primitive
                ["conv2d"] = new("conv2d",
                [
                    [new("conv_t2d",    ["dC", "W"],  "dX")],
                    [new("hadx.vjp",    ["C",  "W",   "dC"], "dW")],
                ]),

                // conv1d(X, W) → C
                // grad_X = conv_t1d(dC, W)
                // grad_W = hadx.vjp
                ["conv1d"] = new("conv1d",
                [
                    [new("conv_t1d", ["dC", "W"],        "dX")],
                    [new("hadx.vjp", ["C",  "W",  "dC"], "dW")],
                ]),

                // ── Subset 02 — Shape / Transform ─────────────────────────────
                // reshape(A) → C
                // grad_A = reshape(dC, shape(A))
                ["reshape"] = new("reshape",
                [
                    [
                        new("shape",   ["A"],          "s"),
                        new("reshape", ["dC", "s"],    "dA"),
                    ],
                ]),

                // transpose(A) → C
                // grad_A = transpose(dC)
                ["transpose"] = new("transpose",
                [
                    [new("transpose", ["dC"], "dA")],
                ]),

                // flatten(A) → C
                // grad_A = reshape(dC, shape(A))
                ["flatten"] = new("flatten",
                [
                    [
                        new("shape",   ["A"],       "s"),
                        new("reshape", ["dC", "s"], "dA"),
                    ],
                ]),

                // concat(A, B) → C
                // grad_A = slice(dC, 0, size(A))
                // grad_B = slice(dC, size(A), size(C))
                ["concat"] = new("concat",
                [
                    [new("slice", ["dC", "A"], "dA")],
                    [new("slice", ["dC", "B"], "dB")],
                ]),

                // ── Subset 10 — Probability ───────────────────────────────────
                // softmax(A) → C
                // grad_A = softmax_vjp(dC, C)  — HADX vjp primitive
                ["softmax"] = new("softmax",
                [
                    [new("hadx.vjp", ["C", "A", "dC"], "dA")],
                ]),

                // log_softmax(A) → C
                // grad_A = hadx.vjp(C, A, dC)
                ["log_softmax"] = new("log_softmax",
                [
                    [new("hadx.vjp", ["C", "A", "dC"], "dA")],
                ]),

                // ── Subset 12 — Random ────────────────────────────────────────
                // dropout(A) → C
                // grad_A = mul(dC, mask)   — mask stored from forward pass
                ["dropout"] = new("dropout",
                [
                    [new("mul", ["dC", "mask"], "dA")],
                ]),

                // ── Subset 0e — Indexing ──────────────────────────────────────
                // gather(input, index) → C
                // grad_input = scatter(dC, index)
                ["gather"] = new("gather",
                [
                    [new("scatter", ["dC", "index"], "d_input")],
                    // index has no gradient — discrete
                ]),

            }.ToFrozenDictionary(StringComparer.Ordinal);
        }
    }
}