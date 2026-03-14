using System.Collections.Frozen;

namespace Helios.Compiler.Registry
{
    public static class FusionPatternRegistry
    {
        public static readonly FrozenDictionary<string, FusionPatternEntry> Patterns;
        public static readonly FrozenDictionary<string, ExpansionStep[]> FullExpansions;

        private static OpCode Op(string key) => OpCodeRegistry.OpcodeRegistry[key];

        static FusionPatternRegistry()
        {
            Patterns = new Dictionary<string, FusionPatternEntry>(StringComparer.Ordinal)
            {
                // ── HFX-I — Local Fusion (Subset 14) ─────────────────────────
                ["hfx1.add_relu"] = new(Op("hfx1.add_relu"), FusionTier.Hfx1,
                [
                    new(Op("add"),  ["dst", "a", "b"]),
                    new(Op("relu"), ["dst", "dst"]),
                ]),

                ["hfx1.add_gelu"] = new(Op("hfx1.add_gelu"), FusionTier.Hfx1,
                [
                    new(Op("add"),  ["dst", "a", "b"]),
                    new(Op("gelu"), ["dst", "dst"]),
                ]),

                ["hfx1.add_silu"] = new(Op("hfx1.add_silu"), FusionTier.Hfx1,
                [
                    new(Op("add"),   ["dst", "a", "b"]),
                    new(Op("swish"), ["dst", "dst"]),
                ]),

                ["hfx1.add_sigmoid"] = new(Op("hfx1.add_sigmoid"), FusionTier.Hfx1,
                [
                    new(Op("add"),     ["dst", "a", "b"]),
                    new(Op("sigmoid"), ["dst", "dst"]),
                ]),

                ["hfx1.add_tanh"] = new(Op("hfx1.add_tanh"), FusionTier.Hfx1,
                [
                    new(Op("add"),  ["dst", "a", "b"]),
                    new(Op("tanh"), ["dst", "dst"]),
                ]),

                ["hfx1.mul_add"] = new(Op("hfx1.mul_add"), FusionTier.Hfx1,
                [
                    new(Op("mul"), ["dst", "a", "b"]),
                    new(Op("add"), ["dst", "dst", "c"]),
                ]),

                ["hfx1.mul_relu"] = new(Op("hfx1.mul_relu"), FusionTier.Hfx1,
                [
                    new(Op("mul"),  ["dst", "a", "b"]),
                    new(Op("relu"), ["dst", "dst"]),
                ]),

                ["hfx1.mul_sigmoid"] = new(Op("hfx1.mul_sigmoid"), FusionTier.Hfx1,
                [
                    new(Op("mul"),     ["dst", "a", "b"]),
                    new(Op("sigmoid"), ["dst", "dst"]),
                ]),

                ["hfx1.mul_tanh"] = new(Op("hfx1.mul_tanh"), FusionTier.Hfx1,
                [
                    new(Op("mul"),  ["dst", "a", "b"]),
                    new(Op("tanh"), ["dst", "dst"]),
                ]),

                ["hfx1.mul_gelu"] = new(Op("hfx1.mul_gelu"), FusionTier.Hfx1,
                [
                    new(Op("mul"),  ["dst", "a", "b"]),
                    new(Op("gelu"), ["dst", "dst"]),
                ]),

                ["hfx1.scale_add"] = new(Op("hfx1.scale_add"), FusionTier.Hfx1,
                [
                    new(Op("mul"), ["dst", "input", "scale"]),
                    new(Op("add"), ["dst", "dst",   "bias" ]),
                ]),

                ["hfx1.scale_relu"] = new(Op("hfx1.scale_relu"), FusionTier.Hfx1,
                [
                    new(Op("mul"),  ["dst", "input", "scale"]),
                    new(Op("relu"), ["dst", "dst"          ]),
                ]),

                ["hfx1.scale_add_relu"] = new(Op("hfx1.scale_add_relu"), FusionTier.Hfx1,
                [
                    new(Op("mul"),  ["dst", "input", "scale"]),
                    new(Op("add"),  ["dst", "dst",   "bias" ]),
                    new(Op("relu"), ["dst", "dst"          ]),
                ]),

                ["hfx1.scale_add_gelu"] = new(Op("hfx1.scale_add_gelu"), FusionTier.Hfx1,
                [
                    new(Op("mul"),  ["dst", "input", "scale"]),
                    new(Op("add"),  ["dst", "dst",   "bias" ]),
                    new(Op("gelu"), ["dst", "dst"          ]),
                ]),

                ["hfx1.scale_add_silu"] = new(Op("hfx1.scale_add_silu"), FusionTier.Hfx1,
                [
                    new(Op("mul"),   ["dst", "input", "scale"]),
                    new(Op("add"),   ["dst", "dst",   "bias" ]),
                    new(Op("swish"), ["dst", "dst"          ]),
                ]),

                ["hfx1.sigmoid_mul"] = new(Op("hfx1.sigmoid_mul"), FusionTier.Hfx1,
                [
                    new(Op("sigmoid"), ["dst", "a"      ]),
                    new(Op("mul"),     ["dst", "dst", "b"]),
                ]),

                ["hfx1.relu_mul"] = new(Op("hfx1.relu_mul"), FusionTier.Hfx1,
                [
                    new(Op("relu"), ["dst", "a"      ]),
                    new(Op("mul"),  ["dst", "dst", "b"]),
                ]),

                ["hfx1.silu_mul"] = new(Op("hfx1.silu_mul"), FusionTier.Hfx1,
                [
                    new(Op("swish"), ["dst", "a"      ]),
                    new(Op("mul"),   ["dst", "dst", "b"]),
                ]),

                ["hfx1.tanh_mul"] = new(Op("hfx1.tanh_mul"), FusionTier.Hfx1,
                [
                    new(Op("tanh"), ["dst", "a"      ]),
                    new(Op("mul"),  ["dst", "dst", "b"]),
                ]),

                ["hfx1.gelu_mul"] = new(Op("hfx1.gelu_mul"), FusionTier.Hfx1,
                [
                    new(Op("gelu"), ["dst", "a"      ]),
                    new(Op("mul"),  ["dst", "dst", "b"]),
                ]),

                ["hfx1.exp_mul"] = new(Op("hfx1.exp_mul"), FusionTier.Hfx1,
                [
                    new(Op("exp"), ["dst", "a"      ]),
                    new(Op("mul"), ["dst", "dst", "b"]),
                ]),

                ["hfx1.neg_add"] = new(Op("hfx1.neg_add"), FusionTier.Hfx1,
                [
                    new(Op("neg"), ["dst", "a"      ]),
                    new(Op("add"), ["dst", "dst", "b"]),
                ]),

                ["hfx1.abs_add"] = new(Op("hfx1.abs_add"), FusionTier.Hfx1,
                [
                    new(Op("abs"), ["dst", "a"      ]),
                    new(Op("add"), ["dst", "dst", "b"]),
                ]),

                ["hfx1.clamp_add"] = new(Op("hfx1.clamp_add"), FusionTier.Hfx1,
                [
                    new(Op("clamp"), ["dst", "a",   "min", "max"]),
                    new(Op("add"),   ["dst", "dst", "b"         ]),
                ]),

                ["hfx1.add_add"] = new(Op("hfx1.add_add"), FusionTier.Hfx1,
                [
                    new(Op("add"), ["dst", "a",   "b"]),
                    new(Op("add"), ["dst", "dst", "c"]),
                ]),

                ["hfx1.mul_mul"] = new(Op("hfx1.mul_mul"), FusionTier.Hfx1,
                [
                    new(Op("mul"), ["dst", "a",   "b"]),
                    new(Op("mul"), ["dst", "dst", "c"]),
                ]),

                ["hfx1.sub_relu"] = new(Op("hfx1.sub_relu"), FusionTier.Hfx1,
                [
                    new(Op("sub"),  ["dst", "a", "b"]),
                    new(Op("relu"), ["dst", "dst"   ]),
                ]),

                ["hfx1.sub_abs"] = new(Op("hfx1.sub_abs"), FusionTier.Hfx1,
                [
                    new(Op("sub"), ["dst", "a", "b"]),
                    new(Op("abs"), ["dst", "dst"   ]),
                ]),

                ["hfx1.rsqrt_mul"] = new(Op("hfx1.rsqrt_mul"), FusionTier.Hfx1,
                [
                    new(Op("rsqrt"), ["dst", "a"      ]),
                    new(Op("mul"),   ["dst", "dst", "b"]),
                ]),

                ["hfx1.log_sum"] = new(Op("hfx1.log_sum"), FusionTier.Hfx1,
                [
                    new(Op("log"), ["dst", "a"      ]),
                    new(Op("add"), ["dst", "dst", "b"]),
                ]),

                ["hfx1.exp_add"] = new(Op("hfx1.exp_add"), FusionTier.Hfx1,
                [
                    new(Op("exp"), ["dst", "a"      ]),
                    new(Op("add"), ["dst", "dst", "b"]),
                ]),

                // ── HFX-II — Kernel Fusion (Subset 15) ───────────────────────
                // Linear
                ["hfx2.linear"] = new(Op("hfx2.linear"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["dst", "input", "weight"]),
                    new(Op("add"),    ["dst", "dst",   "bias"  ]),
                ]),

                ["hfx2.linear_relu"] = new(Op("hfx2.linear_relu"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["dst", "input", "weight"]),
                    new(Op("add"),    ["dst", "dst",   "bias"  ]),
                    new(Op("relu"),   ["dst", "dst"            ]),
                ]),

                ["hfx2.linear_gelu"] = new(Op("hfx2.linear_gelu"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["dst", "input", "weight"]),
                    new(Op("add"),    ["dst", "dst",   "bias"  ]),
                    new(Op("gelu"),   ["dst", "dst"            ]),
                ]),

                ["hfx2.linear_silu"] = new(Op("hfx2.linear_silu"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["dst", "input", "weight"]),
                    new(Op("add"),    ["dst", "dst",   "bias"  ]),
                    new(Op("swish"),  ["dst", "dst"            ]),
                ]),

                ["hfx2.linear_tanh"] = new(Op("hfx2.linear_tanh"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["dst", "input", "weight"]),
                    new(Op("add"),    ["dst", "dst",   "bias"  ]),
                    new(Op("tanh"),   ["dst", "dst"            ]),
                ]),

                ["hfx2.linear_sigmoid"] = new(Op("hfx2.linear_sigmoid"), FusionTier.Hfx2,
                [
                    new(Op("matmul"),  ["dst", "input", "weight"]),
                    new(Op("add"),     ["dst", "dst",   "bias"  ]),
                    new(Op("sigmoid"), ["dst", "dst"            ]),
                ]),

                ["hfx2.linear_dropout"] = new(Op("hfx2.linear_dropout"), FusionTier.Hfx2,
                [
                    new(Op("matmul"),  ["dst", "input", "weight"]),
                    new(Op("add"),     ["dst", "dst",   "bias"  ]),
                    new(Op("dropout"), ["dst", "dst"            ]),
                ]),

                ["hfx2.linear_layernorm"] = new(Op("hfx2.linear_layernorm"), FusionTier.Hfx2,
                [
                    new(Op("matmul"),    ["dst", "input", "weight"]),
                    new(Op("add"),       ["dst", "dst",   "bias"  ]),
                    new(Op("layernorm"), ["dst", "dst"            ]),
                ]),

                ["hfx2.linear_rmsnorm"] = new(Op("hfx2.linear_rmsnorm"), FusionTier.Hfx2,
                [
                    new(Op("matmul"),  ["dst", "input", "weight"]),
                    new(Op("add"),     ["dst", "dst",   "bias"  ]),
                    new(Op("rmsnorm"), ["dst", "dst"            ]),
                ]),

                ["hfx2.bilinear"] = new(Op("hfx2.bilinear"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["t0",  "input1", "weight"]),
                    new(Op("matmul"), ["t1",  "input2", "weight"]),
                    new(Op("add"),    ["dst", "t0",     "t1"    ]),
                ]),

                // Matmul
                ["hfx2.matmul_add"] = new(Op("hfx2.matmul_add"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["dst", "a", "b"   ]),
                    new(Op("add"),    ["dst", "dst", "bias"]),
                ]),

                ["hfx2.matmul_add_relu"] = new(Op("hfx2.matmul_add_relu"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["dst", "a",   "b"   ]),
                    new(Op("add"),    ["dst", "dst", "bias" ]),
                    new(Op("relu"),   ["dst", "dst"        ]),
                ]),

                ["hfx2.matmul_add_gelu"] = new(Op("hfx2.matmul_add_gelu"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["dst", "a",   "b"   ]),
                    new(Op("add"),    ["dst", "dst", "bias" ]),
                    new(Op("gelu"),   ["dst", "dst"        ]),
                ]),

                ["hfx2.matmul_add_silu"] = new(Op("hfx2.matmul_add_silu"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["dst", "a",   "b"   ]),
                    new(Op("add"),    ["dst", "dst", "bias" ]),
                    new(Op("swish"),  ["dst", "dst"        ]),
                ]),

                ["hfx2.matmul_add_tanh"] = new(Op("hfx2.matmul_add_tanh"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["dst", "a",   "b"   ]),
                    new(Op("add"),    ["dst", "dst", "bias" ]),
                    new(Op("tanh"),   ["dst", "dst"        ]),
                ]),

                ["hfx2.matmul_scale"] = new(Op("hfx2.matmul_scale"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["dst", "a",   "b"    ]),
                    new(Op("mul"),    ["dst", "dst", "scale" ]),
                ]),

                ["hfx2.matmul_scale_add"] = new(Op("hfx2.matmul_scale_add"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["dst", "a",   "b"    ]),
                    new(Op("mul"),    ["dst", "dst", "scale" ]),
                    new(Op("add"),    ["dst", "dst", "bias"  ]),
                ]),

                ["hfx2.matmul_softmax"] = new(Op("hfx2.matmul_softmax"), FusionTier.Hfx2,
                [
                    new(Op("matmul"),  ["dst", "a", "b"]),
                    new(Op("softmax"), ["dst", "dst"   ]),
                ]),

                ["hfx2.matmul_scale_softmax"] = new(Op("hfx2.matmul_scale_softmax"), FusionTier.Hfx2,
                [
                    new(Op("matmul"),  ["dst", "a",   "b"    ]),
                    new(Op("mul"),     ["dst", "dst", "scale" ]),
                    new(Op("softmax"), ["dst", "dst"          ]),
                ]),

                ["hfx2.batch_matmul_add"] = new(Op("hfx2.batch_matmul_add"), FusionTier.Hfx2,
                [
                    new(Op("batch_matmul"), ["dst", "a",   "b"   ]),
                    new(Op("add"),          ["dst", "dst", "bias" ]),
                ]),

                ["hfx2.batch_matmul_softmax"] = new(Op("hfx2.batch_matmul_softmax"), FusionTier.Hfx2,
                [
                    new(Op("batch_matmul"), ["dst", "a", "b"]),
                    new(Op("softmax"),      ["dst", "dst"   ]),
                ]),

                // Conv
                ["hfx2.conv2d_bias"] = new(Op("hfx2.conv2d_bias"), FusionTier.Hfx2,
                [
                    new(Op("conv2d"), ["dst", "input", "weight"]),
                    new(Op("add"),    ["dst", "dst",   "bias"  ]),
                ]),

                ["hfx2.conv2d_relu"] = new(Op("hfx2.conv2d_relu"), FusionTier.Hfx2,
                [
                    new(Op("conv2d"), ["dst", "input", "weight"]),
                    new(Op("add"),    ["dst", "dst",   "bias"  ]),
                    new(Op("relu"),   ["dst", "dst"            ]),
                ]),

                ["hfx2.conv2d_gelu"] = new(Op("hfx2.conv2d_gelu"), FusionTier.Hfx2,
                [
                    new(Op("conv2d"), ["dst", "input", "weight"]),
                    new(Op("add"),    ["dst", "dst",   "bias"  ]),
                    new(Op("gelu"),   ["dst", "dst"            ]),
                ]),

                ["hfx2.conv2d_silu"] = new(Op("hfx2.conv2d_silu"), FusionTier.Hfx2,
                [
                    new(Op("conv2d"), ["dst", "input", "weight"]),
                    new(Op("add"),    ["dst", "dst",   "bias"  ]),
                    new(Op("swish"),  ["dst", "dst"            ]),
                ]),

                ["hfx2.conv2d_bias_relu"] = new(Op("hfx2.conv2d_bias_relu"), FusionTier.Hfx2,
                [
                    new(Op("conv2d"), ["dst", "input", "weight"]),
                    new(Op("add"),    ["dst", "dst",   "bias"  ]),
                    new(Op("relu"),   ["dst", "dst"            ]),
                ]),

                ["hfx2.conv2d_bn"] = new(Op("hfx2.conv2d_bn"), FusionTier.Hfx2,
                [
                    new(Op("conv2d"),     ["dst", "input", "weight"]),
                    new(Op("batchnorm"),  ["dst", "dst",   "bn_w",  "bn_b"]),
                ]),

                ["hfx2.conv2d_bn_relu"] = new(Op("hfx2.conv2d_bn_relu"), FusionTier.Hfx2,
                [
                    new(Op("conv2d"),    ["dst", "input", "weight"        ]),
                    new(Op("batchnorm"), ["dst", "dst",   "bn_w",  "bn_b" ]),
                    new(Op("relu"),      ["dst", "dst"                    ]),
                ]),

                ["hfx2.conv2d_bn_leaky_relu"] = new(Op("hfx2.conv2d_bn_leaky_relu"), FusionTier.Hfx2,
                [
                    new(Op("conv2d"),    ["dst", "input", "weight"        ]),
                    new(Op("batchnorm"), ["dst", "dst",   "bn_w",  "bn_b" ]),
                    new(Op("leaky_relu"),["dst", "dst"                    ]),
                ]),

                ["hfx2.conv2d_bn_silu"] = new(Op("hfx2.conv2d_bn_silu"), FusionTier.Hfx2,
                [
                    new(Op("conv2d"),    ["dst", "input", "weight"        ]),
                    new(Op("batchnorm"), ["dst", "dst",   "bn_w",  "bn_b" ]),
                    new(Op("swish"),     ["dst", "dst"                    ]),
                ]),

                ["hfx2.conv2d_add"] = new(Op("hfx2.conv2d_add"), FusionTier.Hfx2,
                [
                    new(Op("conv2d"), ["dst", "input", "weight"  ]),
                    new(Op("add"),    ["dst", "dst",   "residual" ]),
                ]),

                ["hfx2.conv2d_add_relu"] = new(Op("hfx2.conv2d_add_relu"), FusionTier.Hfx2,
                [
                    new(Op("conv2d"), ["dst", "input", "weight"  ]),
                    new(Op("add"),    ["dst", "dst",   "residual" ]),
                    new(Op("relu"),   ["dst", "dst"              ]),
                ]),

                ["hfx2.conv1d_relu"] = new(Op("hfx2.conv1d_relu"), FusionTier.Hfx2,
                [
                    new(Op("conv1d"), ["dst", "input", "weight"]),
                    new(Op("add"),    ["dst", "dst",   "bias"  ]),
                    new(Op("relu"),   ["dst", "dst"            ]),
                ]),

                ["hfx2.conv1d_gelu"] = new(Op("hfx2.conv1d_gelu"), FusionTier.Hfx2,
                [
                    new(Op("conv1d"), ["dst", "input", "weight"]),
                    new(Op("add"),    ["dst", "dst",   "bias"  ]),
                    new(Op("gelu"),   ["dst", "dst"            ]),
                ]),

                ["hfx2.conv1d_bn"] = new(Op("hfx2.conv1d_bn"), FusionTier.Hfx2,
                [
                    new(Op("conv1d"),    ["dst", "input", "weight"       ]),
                    new(Op("batchnorm"), ["dst", "dst",   "bn_w", "bn_b" ]),
                ]),

                ["hfx2.conv1d_bn_relu"] = new(Op("hfx2.conv1d_bn_relu"), FusionTier.Hfx2,
                [
                    new(Op("conv1d"),    ["dst", "input", "weight"       ]),
                    new(Op("batchnorm"), ["dst", "dst",   "bn_w", "bn_b" ]),
                    new(Op("relu"),      ["dst", "dst"                   ]),
                ]),

                ["hfx2.depthwise_conv2d_bn"] = new(Op("hfx2.depthwise_conv2d_bn"), FusionTier.Hfx2,
                [
                    new(Op("conv2d"),    ["dst", "input", "weight"       ]),
                    new(Op("batchnorm"), ["dst", "dst",   "bn_w", "bn_b" ]),
                ]),

                ["hfx2.depthwise_conv2d_bn_relu"] = new(Op("hfx2.depthwise_conv2d_bn_relu"), FusionTier.Hfx2,
                [
                    new(Op("conv2d"),    ["dst", "input", "weight"       ]),
                    new(Op("batchnorm"), ["dst", "dst",   "bn_w", "bn_b" ]),
                    new(Op("relu"),      ["dst", "dst"                   ]),
                ]),

                ["hfx2.separable_conv2d"] = new(Op("hfx2.separable_conv2d"), FusionTier.Hfx2,
                [
                    new(Op("conv2d"), ["t0",  "input", "dw_weight"]),
                    new(Op("conv2d"), ["dst", "t0",    "pw_weight" ]),
                ]),

                // Norm + Linear
                ["hfx2.layernorm_linear"] = new(Op("hfx2.layernorm_linear"), FusionTier.Hfx2,
                [
                    new(Op("layernorm"), ["t0",  "input", "norm_w"  ]),
                    new(Op("matmul"),    ["dst", "t0",    "linear_w"]),
                ]),

                ["hfx2.layernorm_linear_relu"] = new(Op("hfx2.layernorm_linear_relu"), FusionTier.Hfx2,
                [
                    new(Op("layernorm"), ["t0",  "input", "norm_w"  ]),
                    new(Op("matmul"),    ["t1",  "t0",    "linear_w"]),
                    new(Op("relu"),      ["dst", "t1"               ]),
                ]),

                ["hfx2.rmsnorm_linear"] = new(Op("hfx2.rmsnorm_linear"), FusionTier.Hfx2,
                [
                    new(Op("rmsnorm"), ["t0",  "input", "norm_w"  ]),
                    new(Op("matmul"),  ["dst", "t0",    "linear_w"]),
                ]),

                ["hfx2.rmsnorm_linear_relu"] = new(Op("hfx2.rmsnorm_linear_relu"), FusionTier.Hfx2,
                [
                    new(Op("rmsnorm"), ["t0",  "input", "norm_w"  ]),
                    new(Op("matmul"),  ["t1",  "t0",    "linear_w"]),
                    new(Op("relu"),    ["dst", "t1"               ]),
                ]),

                ["hfx2.batchnorm_relu"] = new(Op("hfx2.batchnorm_relu"), FusionTier.Hfx2,
                [
                    new(Op("batchnorm"), ["dst", "input", "bn_w", "bn_b"]),
                    new(Op("relu"),      ["dst", "dst"                  ]),
                ]),

                ["hfx2.groupnorm_relu"] = new(Op("hfx2.groupnorm_relu"), FusionTier.Hfx2,
                [
                    new(Op("groupnorm"), ["dst", "input", "gn_w", "gn_b"]),
                    new(Op("relu"),      ["dst", "dst"                  ]),
                ]),

                // Residual
                ["hfx2.add_layernorm"] = new(Op("hfx2.add_layernorm"), FusionTier.Hfx2,
                [
                    new(Op("add"),       ["t0",  "a",  "b"    ]),
                    new(Op("layernorm"), ["dst", "t0", "norm_w"]),
                ]),

                ["hfx2.add_rmsnorm"] = new(Op("hfx2.add_rmsnorm"), FusionTier.Hfx2,
                [
                    new(Op("add"),     ["t0",  "a",  "b"    ]),
                    new(Op("rmsnorm"), ["dst", "t0", "norm_w"]),
                ]),

                ["hfx2.add_relu"] = new(Op("hfx2.add_relu"), FusionTier.Hfx2,
                [
                    new(Op("add"),  ["t0",  "a", "b"]),
                    new(Op("relu"), ["dst", "t0"    ]),
                ]),

                ["hfx2.add_dropout"] = new(Op("hfx2.add_dropout"), FusionTier.Hfx2,
                [
                    new(Op("add"),     ["t0",  "a", "b"]),
                    new(Op("dropout"), ["dst", "t0"    ]),
                ]),

                ["hfx2.norm_add"] = new(Op("hfx2.norm_add"), FusionTier.Hfx2,
                [
                    new(Op("layernorm"), ["t0",  "input",    "norm_w"  ]),
                    new(Op("add"),       ["dst", "t0",       "residual"]),
                ]),

                ["hfx2.norm_add_linear"] = new(Op("hfx2.norm_add_linear"), FusionTier.Hfx2,
                [
                    new(Op("layernorm"), ["t0",  "input", "norm_w"  ]),
                    new(Op("add"),       ["t1",  "t0",    "residual" ]),
                    new(Op("matmul"),    ["dst", "t1",    "linear_w" ]),
                ]),

                // Pooling
                ["hfx2.conv2d_maxpool"] = new(Op("hfx2.conv2d_maxpool"), FusionTier.Hfx2,
                [
                    new(Op("conv2d"),    ["t0",  "input", "weight"]),
                    new(Op("maxpool2d"), ["dst", "t0"             ]),
                ]),

                ["hfx2.conv2d_avgpool"] = new(Op("hfx2.conv2d_avgpool"), FusionTier.Hfx2,
                [
                    new(Op("conv2d"),    ["t0",  "input", "weight"]),
                    new(Op("avgpool2d"), ["dst", "t0"             ]),
                ]),

                ["hfx2.conv2d_bn_relu_maxpool"] = new(Op("hfx2.conv2d_bn_relu_maxpool"), FusionTier.Hfx2,
                [
                    new(Op("conv2d"),    ["t0",  "input", "weight"       ]),
                    new(Op("batchnorm"), ["t1",  "t0",    "bn_w", "bn_b" ]),
                    new(Op("relu"),      ["t2",  "t1"                    ]),
                    new(Op("maxpool2d"), ["dst", "t2"                    ]),
                ]),

                ["hfx2.conv2d_bn_relu_avgpool"] = new(Op("hfx2.conv2d_bn_relu_avgpool"), FusionTier.Hfx2,
                [
                    new(Op("conv2d"),    ["t0",  "input", "weight"       ]),
                    new(Op("batchnorm"), ["t1",  "t0",    "bn_w", "bn_b" ]),
                    new(Op("relu"),      ["t2",  "t1"                    ]),
                    new(Op("avgpool2d"), ["dst", "t2"                    ]),
                ]),

                // Elementwise Chains
                ["hfx2.exp_sum"] = new(Op("hfx2.exp_sum"), FusionTier.Hfx2,
                [
                    new(Op("exp"),        ["t0",  "input"]),
                    new(Op("reduce_sum"), ["dst", "t0"   ]),
                ]),

                ["hfx2.log_sum_exp"] = new(Op("hfx2.log_sum_exp"), FusionTier.Hfx2,
                [
                    new(Op("exp"),        ["t0",  "input"]),
                    new(Op("reduce_sum"), ["t1",  "t0"   ]),
                    new(Op("log"),        ["dst", "t1"   ]),
                ]),

                ["hfx2.sum_square"] = new(Op("hfx2.sum_square"), FusionTier.Hfx2,
                [
                    new(Op("mul"),        ["t0",  "input", "input"]),
                    new(Op("reduce_sum"), ["dst", "t0"            ]),
                ]),

                ["hfx2.mean_square"] = new(Op("hfx2.mean_square"), FusionTier.Hfx2,
                [
                    new(Op("mul"),         ["t0",  "input", "input"]),
                    new(Op("reduce_mean"), ["dst", "t0"            ]),
                ]),

                ["hfx2.rms"] = new(Op("hfx2.rms"), FusionTier.Hfx2,
                [
                    new(Op("mul"),         ["t0",  "input", "input"]),
                    new(Op("reduce_mean"), ["t1",  "t0"            ]),
                    new(Op("sqrt"),        ["dst", "t1"            ]),
                ]),

                ["hfx2.l2_norm"] = new(Op("hfx2.l2_norm"), FusionTier.Hfx2,
                [
                    new(Op("reduce_l2"), ["t0",  "input"    ]),
                    new(Op("div"),       ["dst", "input", "t0"]),
                ]),

                ["hfx2.l1_norm"] = new(Op("hfx2.l1_norm"), FusionTier.Hfx2,
                [
                    new(Op("reduce_l1"), ["t0",  "input"    ]),
                    new(Op("div"),       ["dst", "input", "t0"]),
                ]),

                ["hfx2.mul_add_relu"] = new(Op("hfx2.mul_add_relu"), FusionTier.Hfx2,
                [
                    new(Op("mul"),  ["t0",  "a",  "b"]),
                    new(Op("add"),  ["t1",  "t0", "c"]),
                    new(Op("relu"), ["dst", "t1"     ]),
                ]),

                // Gated Activations
                ["hfx2.geglu"] = new(Op("hfx2.geglu"), FusionTier.Hfx2,
                [
                    new(Op("matmul"),  ["t0",  "input",  "weight"]),
                    new(Op("chunk"),   ["t1",  "t2",     "t0"    ]),  // split into gate + value
                    new(Op("gelu"),    ["t3",  "t1"              ]),
                    new(Op("mul"),     ["dst", "t3",     "t2"    ]),
                ]),

                ["hfx2.swiglu"] = new(Op("hfx2.swiglu"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["t0",  "input", "weight"]),
                    new(Op("chunk"),  ["t1",  "t2",    "t0"    ]),
                    new(Op("swish"),  ["t3",  "t1"             ]),
                    new(Op("mul"),    ["dst", "t3",    "t2"    ]),
                ]),

                ["hfx2.reglu"] = new(Op("hfx2.reglu"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["t0",  "input", "weight"]),
                    new(Op("chunk"),  ["t1",  "t2",    "t0"    ]),
                    new(Op("relu"),   ["t3",  "t1"             ]),
                    new(Op("mul"),    ["dst", "t3",    "t2"    ]),
                ]),

                ["hfx2.geglu_linear"] = new(Op("hfx2.geglu_linear"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["t0",  "input", "weight"]),
                    new(Op("add"),    ["t1",  "t0",    "bias"  ]),
                    new(Op("chunk"),  ["t2",  "t3",    "t1"    ]),
                    new(Op("gelu"),   ["t4",  "t2"             ]),
                    new(Op("mul"),    ["dst", "t4",    "t3"    ]),
                ]),

                ["hfx2.swiglu_linear"] = new(Op("hfx2.swiglu_linear"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["t0",  "input", "weight"]),
                    new(Op("add"),    ["t1",  "t0",    "bias"  ]),
                    new(Op("chunk"),  ["t2",  "t3",    "t1"    ]),
                    new(Op("swish"),  ["t4",  "t2"             ]),
                    new(Op("mul"),    ["dst", "t4",    "t3"    ]),
                ]),

                ["hfx2.reglu_linear"] = new(Op("hfx2.reglu_linear"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["t0",  "input", "weight"]),
                    new(Op("add"),    ["t1",  "t0",    "bias"  ]),
                    new(Op("chunk"),  ["t2",  "t3",    "t1"    ]),
                    new(Op("relu"),   ["t4",  "t2"             ]),
                    new(Op("mul"),    ["dst", "t4",    "t3"    ]),
                ]),

                // Loss
                ["hfx2.softmax_cross_entropy"] = new(Op("hfx2.softmax_cross_entropy"), FusionTier.Hfx2,
                [
                    new(Op("softmax"),     ["t0",  "logits"     ]),
                    new(Op("log"),         ["t1",  "t0"         ]),
                    new(Op("mul"),         ["t2",  "t1", "targets"]),
                    new(Op("reduce_mean"), ["dst", "t2"         ]),
                ]),

                ["hfx2.log_softmax_nll"] = new(Op("hfx2.log_softmax_nll"), FusionTier.Hfx2,
                [
                    new(Op("log_softmax"), ["t0",  "logits"       ]),
                    new(Op("mul"),         ["t1",  "t0", "targets" ]),
                    new(Op("reduce_mean"), ["dst", "t1"           ]),
                ]),

                ["hfx2.cross_entropy"] = new(Op("hfx2.cross_entropy"), FusionTier.Hfx2,
                [
                    new(Op("softmax"),     ["t0",  "logits"         ]),
                    new(Op("log"),         ["t1",  "t0"             ]),
                    new(Op("mul"),         ["t2",  "t1",  "targets" ]),
                    new(Op("neg"),         ["t3",  "t2"             ]),
                    new(Op("reduce_mean"), ["dst", "t3"             ]),
                ]),

                ["hfx2.binary_cross_entropy"] = new(Op("hfx2.binary_cross_entropy"), FusionTier.Hfx2,
                [
                    new(Op("log"),         ["t0",  "pred"              ]),
                    new(Op("mul"),         ["t1",  "targets",  "t0"   ]),
                    new(Op("sub"),         ["t2",  "pred",     "pred"  ]),  // 1 - pred
                    new(Op("log"),         ["t3",  "t2"                ]),
                    new(Op("sub"),         ["t4",  "pred",     "targets"]),  // 1 - targets
                    new(Op("mul"),         ["t5",  "t4",       "t3"   ]),
                    new(Op("add"),         ["t6",  "t1",       "t5"   ]),
                    new(Op("neg"),         ["t7",  "t6"                ]),
                    new(Op("reduce_mean"), ["dst", "t7"                ]),
                ]),

                ["hfx2.mse_loss"] = new(Op("hfx2.mse_loss"), FusionTier.Hfx2,
                [
                    new(Op("sub"),         ["t0",  "pred", "targets"]),
                    new(Op("mul"),         ["t1",  "t0",   "t0"     ]),
                    new(Op("reduce_mean"), ["dst", "t1"             ]),
                ]),

                ["hfx2.mae_loss"] = new(Op("hfx2.mae_loss"), FusionTier.Hfx2,
                [
                    new(Op("sub"),         ["t0",  "pred", "targets"]),
                    new(Op("abs"),         ["t1",  "t0"             ]),
                    new(Op("reduce_mean"), ["dst", "t1"             ]),
                ]),

                ["hfx2.huber_loss"] = new(Op("hfx2.huber_loss"), FusionTier.Hfx2,
                [
                    new(Op("sub"),         ["t0",  "pred",  "targets"]),
                    new(Op("abs"),         ["t1",  "t0"              ]),
                    new(Op("clamp"),       ["t2",  "t1",    "t1",  "t1"]),
                    new(Op("reduce_mean"), ["dst", "t2"              ]),
                ]),

                ["hfx2.kl_divergence"] = new(Op("hfx2.kl_divergence"), FusionTier.Hfx2,
                [
                    new(Op("div"),         ["t0",  "p",  "q"  ]),
                    new(Op("log"),         ["t1",  "t0"        ]),
                    new(Op("mul"),         ["t2",  "p",  "t1" ]),
                    new(Op("reduce_sum"),  ["dst", "t2"        ]),
                ]),

                ["hfx2.focal_loss"] = new(Op("hfx2.focal_loss"), FusionTier.Hfx2,
                [
                    new(Op("softmax"),     ["t0",  "logits"           ]),
                    new(Op("log"),         ["t1",  "t0"               ]),
                    new(Op("mul"),         ["t2",  "targets",   "t1" ]),
                    new(Op("sub"),         ["t3",  "t0",        "t0" ]),  // 1 - pt
                    new(Op("mul"),         ["t4",  "t3",        "t2" ]),
                    new(Op("neg"),         ["t5",  "t4"               ]),
                    new(Op("reduce_mean"), ["dst", "t5"               ]),
                ]),

                // Quantization
                ["hfx2.quantize_linear"] = new(Op("hfx2.quantize_linear"), FusionTier.Hfx2,
                [
                    new(Op("div"),   ["t0",  "input", "scale"     ]),
                    new(Op("round"), ["t1",  "t0"                 ]),
                    new(Op("add"),   ["t2",  "t1",    "zero_point"]),
                    new(Op("clamp"), ["dst", "t2",    "t2", "t2"  ]),
                ]),

                ["hfx2.dequantize_linear"] = new(Op("hfx2.dequantize_linear"), FusionTier.Hfx2,
                [
                    new(Op("sub"), ["t0",  "input", "zero_point"]),
                    new(Op("mul"), ["dst", "t0",    "scale"     ]),
                ]),

                ["hfx2.quantized_matmul"] = new(Op("hfx2.quantized_matmul"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["t0",  "a",  "b"               ]),
                    new(Op("mul"),    ["t1",  "t0", "scale_a"         ]),
                    new(Op("mul"),    ["dst", "t1", "scale_b"         ]),
                ]),

                ["hfx2.quantized_conv2d"] = new(Op("hfx2.quantized_conv2d"), FusionTier.Hfx2,
                [
                    new(Op("conv2d"), ["t0",  "input", "weight"     ]),
                    new(Op("mul"),    ["t1",  "t0",    "scale"      ]),
                    new(Op("add"),    ["dst", "t1",    "zero_point" ]),
                ]),

                ["hfx2.int8_linear"] = new(Op("hfx2.int8_linear"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["t0",  "input", "weight"]),
                    new(Op("mul"),    ["dst", "t0",    "scale" ]),
                ]),

                ["hfx2.int4_linear"] = new(Op("hfx2.int4_linear"), FusionTier.Hfx2,
                [
                    new(Op("matmul"), ["t0",  "input", "weight"]),
                    new(Op("mul"),    ["dst", "t0",    "scale" ]),
                ]),

                // ── HFX-III — Block Fusion (Subset 16) ───────────────────────
                // Attention — compact form, references HFX-II sub-patterns
                ["hfx3.scaled_dot_product_attn"] = new(Op("hfx3.scaled_dot_product_attn"), FusionTier.Hfx3,
                [
                    new(Op("hfx2.matmul_scale_softmax"), ["s",   "q",  "k",  "scale"]),
                    new(Op("matmul"),                    ["dst", "s",  "v"          ]),
                ]),

                ["hfx3.masked_scaled_dot_attn"] = new(Op("hfx3.masked_scaled_dot_attn"), FusionTier.Hfx3,
                [
                    new(Op("hfx2.matmul_scale"),  ["s",   "q",   "k",     "scale"]),
                    new(Op("masked_fill"),         ["s",   "s",   "mask",  "s"    ]),
                    new(Op("softmax"),             ["s",   "s"                    ]),
                    new(Op("matmul"),              ["dst", "s",   "v"            ]),
                ]),

                ["hfx3.causal_self_attn"] = new(Op("hfx3.causal_self_attn"), FusionTier.Hfx3,
                [
                    new(Op("hfx2.matmul_scale_softmax"), ["s",   "q", "k", "scale"]),
                    new(Op("matmul"),                    ["dst", "s", "v"         ]),
                ]),

                ["hfx3.flash_attn"] = new(Op("hfx3.flash_attn"), FusionTier.Hfx3,
                [
                    new(Op("matmul"),  ["q",   "input", "w_q"          ]),
                    new(Op("matmul"),  ["k",   "input", "w_k"          ]),
                    new(Op("matmul"),  ["v",   "input", "w_v"          ]),
                    new(Op("hfx3.scaled_dot_product_attn"), ["dst", "q", "k", "v"]),
                ]),

                ["hfx3.flash_attn_causal"] = new(Op("hfx3.flash_attn_causal"), FusionTier.Hfx3,
                [
                    new(Op("matmul"),              ["q",   "input", "w_q"        ]),
                    new(Op("matmul"),              ["k",   "input", "w_k"        ]),
                    new(Op("matmul"),              ["v",   "input", "w_v"        ]),
                    new(Op("hfx3.causal_self_attn"), ["dst", "q", "k", "v"      ]),
                ]),

                ["hfx3.flash_attn_masked"] = new(Op("hfx3.flash_attn_masked"), FusionTier.Hfx3,
                [
                    new(Op("matmul"),                    ["q",   "input", "w_q"         ]),
                    new(Op("matmul"),                    ["k",   "input", "w_k"         ]),
                    new(Op("matmul"),                    ["v",   "input", "w_v"         ]),
                    new(Op("hfx3.masked_scaled_dot_attn"), ["dst", "q", "k", "v", "mask"]),
                ]),

                ["hfx3.cross_attn"] = new(Op("hfx3.cross_attn"), FusionTier.Hfx3,
                [
                    new(Op("matmul"),                        ["q",   "q",  "w_q"         ]),
                    new(Op("matmul"),                        ["k",   "kv", "w_k"         ]),
                    new(Op("matmul"),                        ["v",   "kv", "w_v"         ]),
                    new(Op("hfx3.scaled_dot_product_attn"), ["dst", "q",  "k",   "v"    ]),
                ]),

                ["hfx3.multi_head_attn"] = new(Op("hfx3.multi_head_attn"), FusionTier.Hfx3,
                [
                    new(Op("matmul"),              ["q",   "q",  "w_q"             ]),
                    new(Op("matmul"),              ["k",   "k",  "w_k"             ]),
                    new(Op("matmul"),              ["v",   "v",  "w_v"             ]),
                    new(Op("hfx3.flash_attn"),     ["o",   "q",  "k",    "v"       ]),
                    new(Op("matmul"),              ["dst", "o",  "w_o"             ]),
                ]),

                ["hfx3.grouped_query_attn"] = new(Op("hfx3.grouped_query_attn"), FusionTier.Hfx3,
                [
                    new(Op("matmul"),                        ["q",   "q", "w_q"       ]),
                    new(Op("matmul"),                        ["k",   "k", "w_k"       ]),
                    new(Op("matmul"),                        ["v",   "v", "w_v"       ]),
                    new(Op("hfx3.scaled_dot_product_attn"), ["dst", "q", "k",  "v"   ]),
                ]),

                ["hfx3.multi_query_attn"] = new(Op("hfx3.multi_query_attn"), FusionTier.Hfx3,
                [
                    new(Op("matmul"),                        ["q",   "q", "w_q"      ]),
                    new(Op("matmul"),                        ["k",   "k", "w_k"      ]),
                    new(Op("matmul"),                        ["v",   "v", "w_v"      ]),
                    new(Op("hfx3.scaled_dot_product_attn"), ["dst", "q", "k", "v"   ]),
                ]),

                ["hfx3.rotary_attn"] = new(Op("hfx3.rotary_attn"), FusionTier.Hfx3,
                [
                    new(Op("hfx3.rope_apply"),               ["q",   "q", "k",  "cos", "sin"]),
                    new(Op("hfx3.scaled_dot_product_attn"), ["dst", "q", "k",  "v"         ]),
                ]),

                ["hfx3.alibi_attn"] = new(Op("hfx3.alibi_attn"), FusionTier.Hfx3,
                [
                    new(Op("hfx3.alibi_bias"),               ["s",   "q",  "k",  "slopes"]),
                    new(Op("softmax"),                        ["s",   "s"                 ]),
                    new(Op("matmul"),                         ["dst", "s",  "v"           ]),
                ]),

                ["hfx3.sliding_window_attn"] = new(Op("hfx3.sliding_window_attn"), FusionTier.Hfx3,
                [
                    new(Op("hfx3.scaled_dot_product_attn"), ["dst", "q", "k", "v"]),
                ]),

                // Positional Encoding
                ["hfx3.rotary_embed"] = new(Op("hfx3.rotary_embed"), FusionTier.Hfx3,
                [
                    new(Op("mul"), ["t0",  "input", "cos"]),
                    new(Op("mul"), ["t1",  "input", "sin"]),
                    new(Op("add"), ["dst", "t0",    "t1" ]),
                ]),

                ["hfx3.alibi_bias"] = new(Op("hfx3.alibi_bias"), FusionTier.Hfx3,
                [
                    new(Op("matmul"), ["t0",  "q",  "k"     ]),
                    new(Op("add"),    ["dst", "t0", "slopes" ]),
                ]),

                ["hfx3.sinusoidal_embed"] = new(Op("hfx3.sinusoidal_embed"), FusionTier.Hfx3,
                [
                    new(Op("sin"), ["t0",  "input"]),
                    new(Op("cos"), ["t1",  "input"]),
                    new(Op("add"), ["dst", "t0",  "t1"]),
                ]),

                ["hfx3.learned_embed"] = new(Op("hfx3.learned_embed"), FusionTier.Hfx3,
                [
                    new(Op("gather"), ["dst", "input", "weight"]),
                ]),

                ["hfx3.rope_apply"] = new(Op("hfx3.rope_apply"), FusionTier.Hfx3,
                [
                    new(Op("hfx3.rotary_embed"), ["q_rot", "q", "cos", "sin"]),
                    new(Op("hfx3.rotary_embed"), ["k_rot", "k", "cos", "sin"]),
                    new(Op("stack"),             ["dst", "q_rot", "k_rot"   ]),
                ]),

                // Embedding
                ["hfx3.tok_pos_embed"] = new(Op("hfx3.tok_pos_embed"), FusionTier.Hfx3,
                [
                    new(Op("gather"), ["t0",  "tokens",    "w_tok"]),
                    new(Op("gather"), ["t1",  "positions", "w_pos"]),
                    new(Op("add"),    ["dst", "t0",        "t1"   ]),
                ]),

                ["hfx3.embed_add_layernorm"] = new(Op("hfx3.embed_add_layernorm"), FusionTier.Hfx3,
                [
                    new(Op("hfx3.tok_pos_embed"), ["t0",  "tokens", "positions", "w_tok", "w_pos"]),
                    new(Op("layernorm"),           ["dst", "t0",     "norm_w"                    ]),
                ]),

                ["hfx3.embed_dropout"] = new(Op("hfx3.embed_dropout"), FusionTier.Hfx3,
                [
                    new(Op("gather"),  ["t0",  "input", "weight"]),
                    new(Op("dropout"), ["dst", "t0"             ]),
                ]),

                ["hfx3.embed_layernorm"] = new(Op("hfx3.embed_layernorm"), FusionTier.Hfx3,
                [
                    new(Op("gather"),    ["t0",  "input", "weight"]),
                    new(Op("layernorm"), ["dst", "t0",    "norm_w"]),
                ]),

                // FFN Blocks
                ["hfx3.ffn_relu"] = new(Op("hfx3.ffn_relu"), FusionTier.Hfx3,
                [
                    new(Op("hfx2.linear_relu"), ["t0",  "input", "w1", "bias1"]),
                    new(Op("hfx2.linear"),      ["dst", "t0",    "w2", "bias2"]),
                ]),

                ["hfx3.ffn_gelu"] = new(Op("hfx3.ffn_gelu"), FusionTier.Hfx3,
                [
                    new(Op("hfx2.linear_gelu"), ["t0",  "input", "w1", "bias1"]),
                    new(Op("hfx2.linear"),      ["dst", "t0",    "w2", "bias2"]),
                ]),

                ["hfx3.ffn_swiglu"] = new(Op("hfx3.ffn_swiglu"), FusionTier.Hfx3,
                [
                    new(Op("hfx2.swiglu_linear"), ["t0",  "input", "w1", "w2"]),
                    new(Op("matmul"),              ["dst", "t0",    "w3"     ]),
                ]),

                ["hfx3.ffn_geglu"] = new(Op("hfx3.ffn_geglu"), FusionTier.Hfx3,
                [
                    new(Op("hfx2.geglu_linear"), ["t0",  "input", "w1", "w2"]),
                    new(Op("matmul"),             ["dst", "t0",    "w3"     ]),
                ]),

                ["hfx3.pre_norm_ffn"] = new(Op("hfx3.pre_norm_ffn"), FusionTier.Hfx3,
                [
                    new(Op("layernorm"),   ["t0",  "input", "norm_w"       ]),
                    new(Op("hfx3.ffn_gelu"), ["dst", "t0",  "w1", "w2"    ]),
                ]),

                ["hfx3.post_norm_ffn"] = new(Op("hfx3.post_norm_ffn"), FusionTier.Hfx3,
                [
                    new(Op("hfx3.ffn_gelu"), ["t0",  "input", "w1",    "w2"]),
                    new(Op("layernorm"),      ["dst", "t0",    "norm_w"    ]),
                ]),

                ["hfx3.ffn_dropout"] = new(Op("hfx3.ffn_dropout"), FusionTier.Hfx3,
                [
                    new(Op("hfx2.linear_relu"), ["t0",  "input", "w1", "w1"]),
                    new(Op("dropout"),           ["t1",  "t0"               ]),
                    new(Op("matmul"),            ["dst", "t1",    "w2"      ]),
                ]),

                // Transformer Blocks
                ["hfx3.transformer_block"] = new(Op("hfx3.transformer_block"), FusionTier.Hfx3,
                [
                    new(Op("hfx2.add_layernorm"),  ["t0",  "input", "input",  "norm_w1"           ]),
                    new(Op("hfx3.flash_attn"),     ["t1",  "t0",    "attn_w", "attn_w", "attn_w"  ]),
                    new(Op("add"),                 ["t2",  "input", "t1"                           ]),
                    new(Op("hfx2.add_layernorm"),  ["t3",  "t2",    "t2",     "norm_w2"            ]),
                    new(Op("hfx3.ffn_gelu"),       ["t4",  "t3",    "ffn_w1", "ffn_w2"            ]),
                    new(Op("add"),                 ["dst", "t2",    "t4"                           ]),
                ]),

                ["hfx3.pre_norm_attn_block"] = new(Op("hfx3.pre_norm_attn_block"), FusionTier.Hfx3,
                [
                    new(Op("layernorm"),       ["t0",  "input", "norm_w"          ]),
                    new(Op("hfx3.flash_attn"), ["t1",  "t0",    "q",    "k",  "v" ]),
                    new(Op("add"),             ["dst", "input", "t1"              ]),
                ]),

                ["hfx3.post_norm_attn_block"] = new(Op("hfx3.post_norm_attn_block"), FusionTier.Hfx3,
                [
                    new(Op("hfx3.flash_attn"), ["t0",  "input", "q",     "k",  "v"]),
                    new(Op("add"),             ["t1",  "input", "t0"            ]),
                    new(Op("layernorm"),        ["dst", "t1",    "norm_w"       ]),
                ]),

                ["hfx3.encoder_block"] = new(Op("hfx3.encoder_block"), FusionTier.Hfx3,
                [
                    new(Op("hfx3.pre_norm_attn_block"), ["t0",  "input", "q",    "k",     "v",     "norm_w1"]),
                    new(Op("hfx3.pre_norm_ffn"),        ["dst", "t0",    "ffn_w","norm_w2"                  ]),
                ]),

                ["hfx3.decoder_block"] = new(Op("hfx3.decoder_block"), FusionTier.Hfx3,
                [
                    new(Op("hfx3.pre_norm_attn_block"), ["t0",  "input", "q",     "k",     "v",     "norm_w1"]),
                    new(Op("hfx3.cross_attn"),          ["t1",  "t0",    "kv",                               ]),
                    new(Op("add"),                      ["t2",  "t0",    "t1"                                ]),
                    new(Op("hfx3.pre_norm_ffn"),        ["dst", "t2",    "ffn_w", "norm_w2"                  ]),
                ]),

                // Residual Blocks
                ["hfx3.residual_block"] = new(Op("hfx3.residual_block"), FusionTier.Hfx3,
                [
                    new(Op("matmul"),    ["t0",  "input", "weight"  ]),
                    new(Op("layernorm"), ["t1",  "t0",    "norm_w"  ]),
                    new(Op("add"),       ["dst", "input", "t1"      ]),
                ]),

                ["hfx3.pre_norm_residual"] = new(Op("hfx3.pre_norm_residual"), FusionTier.Hfx3,
                [
                    new(Op("layernorm"), ["t0",  "input",    "norm_w"  ]),
                    new(Op("matmul"),    ["t1",  "t0",       "weight"  ]),
                    new(Op("add"),       ["dst", "input",    "t1"      ]),
                ]),

                ["hfx3.post_norm_residual"] = new(Op("hfx3.post_norm_residual"), FusionTier.Hfx3,
                [
                    new(Op("matmul"),    ["t0",  "input",    "weight"  ]),
                    new(Op("add"),       ["t1",  "input",    "t0"      ]),
                    new(Op("layernorm"), ["dst", "t1",       "norm_w"  ]),
                ]),

            }.ToFrozenDictionary(StringComparer.Ordinal);

            // ── Compute full H-ISA expansions ─────────────────────────────────
            FullExpansions = Patterns
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => ResolveFullExpansion(kvp.Key, Patterns),
                    StringComparer.Ordinal)
                .ToFrozenDictionary(StringComparer.Ordinal);
        }

        private static ExpansionStep[] ResolveFullExpansion(
            string key,
            FrozenDictionary<string, FusionPatternEntry> registry)
        {
            var visited = new HashSet<string>(StringComparer.Ordinal);
            return Resolve(key, registry, visited);
        }

        private static ExpansionStep[] Resolve(
            string key,
            FrozenDictionary<string, FusionPatternEntry> registry,
            HashSet<string> visited)
        {
            if (!visited.Add(key))
                throw new InvalidOperationException(
                    $"Circular fusion expansion detected at '{key}'");

            var result = new List<ExpansionStep>();

            foreach (var step in registry[key].ExpSteps)
            {
                // check if this step's op is itself a fusion pattern
                var stepKey = OpCodeRegistry.OpcodeRegistry
                    .FirstOrDefault(kvp => kvp.Value == step.Code).Key;

                if (stepKey is not null && registry.ContainsKey(stepKey))
                {
                    // recurse — expand the sub-fusion
                    result.AddRange(Resolve(stepKey, registry, visited));
                }
                else
                {
                    // pure H-ISA op — emit as-is
                    result.Add(step);
                }
            }

            visited.Remove(key); // backtrack — allows diamond patterns
            return [.. result];
        }
    }
}