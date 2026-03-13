using System.Collections.Frozen;

namespace Helios.Compiler.Registry
{
    public static class OpCodeRegistry
    {
        public static readonly FrozenDictionary<string, OpCode> OpcodeRegistry;
        private const byte MajorMinor = 0x10;

        static OpCodeRegistry()
        {
            OpcodeRegistry = new Dictionary<string, OpCode>(StringComparer.Ordinal)
            {
                // ── Subset 00 — Memory / IO ───────────────────────────────────
                ["input"] = new(MajorMinor, 0x00, 0x00, 0x00),
                ["param"] = new(MajorMinor, 0x00, 0x00, 0x01),
                ["const"] = new(MajorMinor, 0x00, 0x00, 0x02),
                ["load"] = new(MajorMinor, 0x00, 0x00, 0x03),
                ["store"] = new(MajorMinor, 0x00, 0x00, 0x04),

                // ── Subset 01 — Tensor Ops ────────────────────────────────────
                ["identity"] = new(MajorMinor, 0x01, 0x01, 0x00),
                ["cast"] = new(MajorMinor, 0x01, 0x01, 0x01),
                ["clone"] = new(MajorMinor, 0x01, 0x01, 0x02),

                // ── Subset 02 — Shape / Transform ─────────────────────────────
                ["reshape"] = new(MajorMinor, 0x02, 0x05, 0x00),
                ["flatten"] = new(MajorMinor, 0x02, 0x05, 0x01),
                ["expand"] = new(MajorMinor, 0x02, 0x05, 0x02),
                ["expand_as"] = new(MajorMinor, 0x02, 0x05, 0x03),
                ["squeeze"] = new(MajorMinor, 0x02, 0x05, 0x04),
                ["unsqueeze"] = new(MajorMinor, 0x02, 0x05, 0x05),
                ["transpose"] = new(MajorMinor, 0x02, 0x05, 0x06),
                ["permute"] = new(MajorMinor, 0x02, 0x05, 0x07),
                ["concat"] = new(MajorMinor, 0x02, 0x05, 0x08),
                ["split"] = new(MajorMinor, 0x02, 0x05, 0x09),
                ["chunk"] = new(MajorMinor, 0x02, 0x05, 0x0a),
                ["pad"] = new(MajorMinor, 0x02, 0x05, 0x0b),
                ["slice"] = new(MajorMinor, 0x02, 0x05, 0x0c),
                ["narrow"] = new(MajorMinor, 0x02, 0x05, 0x0d),
                ["roll"] = new(MajorMinor, 0x02, 0x05, 0x0e),
                ["repeat"] = new(MajorMinor, 0x02, 0x05, 0x0f),
                ["tile"] = new(MajorMinor, 0x02, 0x05, 0x10),
                ["stack"] = new(MajorMinor, 0x02, 0x05, 0x11),
                ["unstack"] = new(MajorMinor, 0x02, 0x05, 0x12),

                // ── Subset 03 — Tensor Info ───────────────────────────────────
                ["shape"] = new(MajorMinor, 0x03, 0x0c, 0x00),
                ["size"] = new(MajorMinor, 0x03, 0x0c, 0x01),
                ["rank"] = new(MajorMinor, 0x03, 0x0c, 0x02),
                ["stride"] = new(MajorMinor, 0x03, 0x0c, 0x03),
                ["numel"] = new(MajorMinor, 0x03, 0x0c, 0x04),

                // ── Subset 04 — Arithmetic ────────────────────────────────────
                ["add"] = new(MajorMinor, 0x04, 0x02, 0x00),
                ["sub"] = new(MajorMinor, 0x04, 0x02, 0x01),
                ["mul"] = new(MajorMinor, 0x04, 0x02, 0x02),
                ["div"] = new(MajorMinor, 0x04, 0x02, 0x03),
                ["mod"] = new(MajorMinor, 0x04, 0x02, 0x04),
                ["neg"] = new(MajorMinor, 0x04, 0x01, 0x05),
                ["abs"] = new(MajorMinor, 0x04, 0x01, 0x06),
                ["sign"] = new(MajorMinor, 0x04, 0x01, 0x07),
                ["floor"] = new(MajorMinor, 0x04, 0x01, 0x08),
                ["ceil"] = new(MajorMinor, 0x04, 0x01, 0x09),
                ["round"] = new(MajorMinor, 0x04, 0x01, 0x0a),
                ["clamp"] = new(MajorMinor, 0x04, 0x03, 0x0b),

                // ── Subset 05 — Math ──────────────────────────────────────────
                ["exp"] = new(MajorMinor, 0x05, 0x01, 0x00),
                ["log"] = new(MajorMinor, 0x05, 0x01, 0x01),
                ["log2"] = new(MajorMinor, 0x05, 0x01, 0x02),
                ["log10"] = new(MajorMinor, 0x05, 0x01, 0x03),
                ["pow"] = new(MajorMinor, 0x05, 0x02, 0x04),
                ["sqrt"] = new(MajorMinor, 0x05, 0x01, 0x05),
                ["rsqrt"] = new(MajorMinor, 0x05, 0x01, 0x06),
                ["sin"] = new(MajorMinor, 0x05, 0x01, 0x07),
                ["cos"] = new(MajorMinor, 0x05, 0x01, 0x08),
                ["tan"] = new(MajorMinor, 0x05, 0x01, 0x09),
                ["asin"] = new(MajorMinor, 0x05, 0x01, 0x0a),
                ["acos"] = new(MajorMinor, 0x05, 0x01, 0x0b),
                ["atan"] = new(MajorMinor, 0x05, 0x01, 0x0c),
                ["sinh"] = new(MajorMinor, 0x05, 0x01, 0x0d),
                ["cosh"] = new(MajorMinor, 0x05, 0x01, 0x0e),
                ["tanh"] = new(MajorMinor, 0x05, 0x01, 0x0f),
                ["erf"] = new(MajorMinor, 0x05, 0x01, 0x10),

                // ── Subset 06 — Activation ────────────────────────────────────
                ["relu"] = new(MajorMinor, 0x06, 0x01, 0x00),
                ["relu6"] = new(MajorMinor, 0x06, 0x01, 0x01),
                ["leaky_relu"] = new(MajorMinor, 0x06, 0x01, 0x02),
                ["elu"] = new(MajorMinor, 0x06, 0x01, 0x03),
                ["selu"] = new(MajorMinor, 0x06, 0x01, 0x04),
                ["sigmoid"] = new(MajorMinor, 0x06, 0x01, 0x05),
                ["gelu"] = new(MajorMinor, 0x06, 0x01, 0x06),
                ["swish"] = new(MajorMinor, 0x06, 0x01, 0x07),
                ["softplus"] = new(MajorMinor, 0x06, 0x01, 0x08),
                ["softsign"] = new(MajorMinor, 0x06, 0x01, 0x09),
                ["hardsigmoid"] = new(MajorMinor, 0x06, 0x01, 0x0a),
                ["hardswish"] = new(MajorMinor, 0x06, 0x01, 0x0b),

                // ── Subset 07 — Comparison ────────────────────────────────────
                ["eq"] = new(MajorMinor, 0x07, 0x0b, 0x00),
                ["ne"] = new(MajorMinor, 0x07, 0x0b, 0x01),
                ["lt"] = new(MajorMinor, 0x07, 0x0b, 0x02),
                ["le"] = new(MajorMinor, 0x07, 0x0b, 0x03),
                ["gt"] = new(MajorMinor, 0x07, 0x0b, 0x04),
                ["ge"] = new(MajorMinor, 0x07, 0x0b, 0x05),

                // ── Subset 08 — Logical ───────────────────────────────────────
                ["and"] = new(MajorMinor, 0x08, 0x0a, 0x00),
                ["or"] = new(MajorMinor, 0x08, 0x0a, 0x01),
                ["xor"] = new(MajorMinor, 0x08, 0x0a, 0x02),
                ["not"] = new(MajorMinor, 0x08, 0x01, 0x00),

                // ── Subset 09 — Reduction ─────────────────────────────────────
                ["reduce_sum"] = new(MajorMinor, 0x09, 0x04, 0x00),
                ["reduce_mean"] = new(MajorMinor, 0x09, 0x04, 0x01),
                ["reduce_max"] = new(MajorMinor, 0x09, 0x04, 0x02),
                ["reduce_min"] = new(MajorMinor, 0x09, 0x04, 0x03),
                ["reduce_prod"] = new(MajorMinor, 0x09, 0x04, 0x04),
                ["reduce_l1"] = new(MajorMinor, 0x09, 0x04, 0x05),
                ["reduce_l2"] = new(MajorMinor, 0x09, 0x04, 0x06),
                ["argmax"] = new(MajorMinor, 0x09, 0x04, 0x07),
                ["argmin"] = new(MajorMinor, 0x09, 0x04, 0x08),

                // ── Subset 0a — Linear Algebra ────────────────────────────────
                ["dot"] = new(MajorMinor, 0x0a, 0x02, 0x00),
                ["matmul"] = new(MajorMinor, 0x0a, 0x02, 0x01),
                ["batch_matmul"] = new(MajorMinor, 0x0a, 0x02, 0x02),
                ["outer"] = new(MajorMinor, 0x0a, 0x02, 0x03),
                ["diag"] = new(MajorMinor, 0x0a, 0x01, 0x04),
                ["trace"] = new(MajorMinor, 0x0a, 0x01, 0x05),

                // ── Subset 0b — Convolution ───────────────────────────────────
                ["conv1d"] = new(MajorMinor, 0x0b, 0x08, 0x00),
                ["conv2d"] = new(MajorMinor, 0x0b, 0x08, 0x01),
                ["conv3d"] = new(MajorMinor, 0x0b, 0x08, 0x02),
                ["conv_t1d"] = new(MajorMinor, 0x0b, 0x08, 0x03),
                ["conv_t2d"] = new(MajorMinor, 0x0b, 0x08, 0x04),
                ["conv_t3d"] = new(MajorMinor, 0x0b, 0x08, 0x05),

                // ── Subset 0c — Pooling ───────────────────────────────────────
                ["maxpool1d"] = new(MajorMinor, 0x0c, 0x09, 0x00),
                ["maxpool2d"] = new(MajorMinor, 0x0c, 0x09, 0x01),
                ["maxpool3d"] = new(MajorMinor, 0x0c, 0x09, 0x02),
                ["avgpool1d"] = new(MajorMinor, 0x0c, 0x09, 0x03),
                ["avgpool2d"] = new(MajorMinor, 0x0c, 0x09, 0x04),
                ["avgpool3d"] = new(MajorMinor, 0x0c, 0x09, 0x05),
                ["gmaxpool"] = new(MajorMinor, 0x0c, 0x09, 0x06),
                ["gavgpool"] = new(MajorMinor, 0x0c, 0x09, 0x07),

                // ── Subset 0d — Normalization ─────────────────────────────────
                ["batchnorm"] = new(MajorMinor, 0x0d, 0x07, 0x00),
                ["layernorm"] = new(MajorMinor, 0x0d, 0x07, 0x01),
                ["groupnorm"] = new(MajorMinor, 0x0d, 0x07, 0x02),
                ["instancenorm"] = new(MajorMinor, 0x0d, 0x07, 0x03),
                ["rmsnorm"] = new(MajorMinor, 0x0d, 0x07, 0x04),

                // ── Subset 0e — Indexing ──────────────────────────────────────
                ["gather"] = new(MajorMinor, 0x0e, 0x06, 0x00),
                ["scatter"] = new(MajorMinor, 0x0e, 0x06, 0x01),
                ["index_select"] = new(MajorMinor, 0x0e, 0x06, 0x02),
                ["take"] = new(MajorMinor, 0x0e, 0x06, 0x03),
                ["put"] = new(MajorMinor, 0x0e, 0x06, 0x04),
                ["where"] = new(MajorMinor, 0x0e, 0x03, 0x05),

                // ── Subset 0f — Sorting ───────────────────────────────────────
                ["topk"] = new(MajorMinor, 0x0f, 0x06, 0x00),
                ["sort"] = new(MajorMinor, 0x0f, 0x06, 0x01),
                ["argsort"] = new(MajorMinor, 0x0f, 0x06, 0x02),
                ["select"] = new(MajorMinor, 0x0f, 0x06, 0x03),

                // ── Subset 10 — Probability ───────────────────────────────────
                ["softmax"] = new(MajorMinor, 0x10, 0x01, 0x00),
                ["log_softmax"] = new(MajorMinor, 0x10, 0x01, 0x01),

                // ── Subset 11 — Masking ───────────────────────────────────────
                ["masked_fill"] = new(MajorMinor, 0x11, 0x03, 0x00),
                ["masked_select"] = new(MajorMinor, 0x11, 0x06, 0x01),

                // ── Subset 12 — Random ────────────────────────────────────────
                ["random_uniform"] = new(MajorMinor, 0x12, 0x0d, 0x00),
                ["random_normal"] = new(MajorMinor, 0x12, 0x0d, 0x01),
                ["dropout"] = new(MajorMinor, 0x12, 0x0d, 0x02),

                // ── Subset 13 — Control ───────────────────────────────────────
                ["noop"] = new(MajorMinor, 0x13, 0x0c, 0x00),
                ["assert"] = new(MajorMinor, 0x13, 0x0c, 0x01),
                ["debug_op"] = new(MajorMinor, 0x13, 0x0c, 0x02),  // "debug" is a reserved directive keyword
                ["fuse_begin"] = new(MajorMinor, 0x13, 0x0e, 0x00),
                ["fuse_wrap"] = new(MajorMinor, 0x13, 0x0e, 0x01),

                // ── Subset 14 — HFX-I Local Fusion ───────────────────────────
                ["hfx1.add_relu"] = new(MajorMinor, 0x14, 0x01, 0x00),
                ["hfx1.add_gelu"] = new(MajorMinor, 0x14, 0x02, 0x01),
                ["hfx1.add_silu"] = new(MajorMinor, 0x14, 0x02, 0x02),
                ["hfx1.add_sigmoid"] = new(MajorMinor, 0x14, 0x02, 0x03),
                ["hfx1.add_tanh"] = new(MajorMinor, 0x14, 0x02, 0x04),
                ["hfx1.mul_add"] = new(MajorMinor, 0x14, 0x03, 0x05),
                ["hfx1.mul_relu"] = new(MajorMinor, 0x14, 0x02, 0x06),
                ["hfx1.mul_sigmoid"] = new(MajorMinor, 0x14, 0x02, 0x07),
                ["hfx1.mul_tanh"] = new(MajorMinor, 0x14, 0x02, 0x08),
                ["hfx1.mul_gelu"] = new(MajorMinor, 0x14, 0x02, 0x09),
                ["hfx1.scale_add"] = new(MajorMinor, 0x14, 0x03, 0x0a),
                ["hfx1.scale_relu"] = new(MajorMinor, 0x14, 0x02, 0x0b),
                ["hfx1.scale_add_relu"] = new(MajorMinor, 0x14, 0x03, 0x0c),
                ["hfx1.scale_add_gelu"] = new(MajorMinor, 0x14, 0x03, 0x0d),
                ["hfx1.scale_add_silu"] = new(MajorMinor, 0x14, 0x03, 0x0e),
                ["hfx1.sigmoid_mul"] = new(MajorMinor, 0x14, 0x02, 0x0f),
                ["hfx1.relu_mul"] = new(MajorMinor, 0x14, 0x02, 0x10),
                ["hfx1.silu_mul"] = new(MajorMinor, 0x14, 0x02, 0x11),
                ["hfx1.tanh_mul"] = new(MajorMinor, 0x14, 0x02, 0x12),
                ["hfx1.gelu_mul"] = new(MajorMinor, 0x14, 0x02, 0x13),
                ["hfx1.exp_mul"] = new(MajorMinor, 0x14, 0x02, 0x14),
                ["hfx1.neg_add"] = new(MajorMinor, 0x14, 0x02, 0x15),
                ["hfx1.abs_add"] = new(MajorMinor, 0x14, 0x02, 0x16),
                ["hfx1.clamp_add"] = new(MajorMinor, 0x14, 0x03, 0x17),
                ["hfx1.add_add"] = new(MajorMinor, 0x14, 0x03, 0x18),
                ["hfx1.mul_mul"] = new(MajorMinor, 0x14, 0x03, 0x19),
                ["hfx1.sub_relu"] = new(MajorMinor, 0x14, 0x02, 0x1a),
                ["hfx1.sub_abs"] = new(MajorMinor, 0x14, 0x02, 0x1b),
                ["hfx1.rsqrt_mul"] = new(MajorMinor, 0x14, 0x02, 0x1c),
                ["hfx1.log_sum"] = new(MajorMinor, 0x14, 0x02, 0x1d),
                ["hfx1.exp_add"] = new(MajorMinor, 0x14, 0x02, 0x1e),

                // ── Subset 15 — HFX-II Kernel Fusion ─────────────────────────
                // Linear
                ["hfx2.linear"] = new(MajorMinor, 0x15, 0x03, 0x00),
                ["hfx2.linear_relu"] = new(MajorMinor, 0x15, 0x03, 0x01),
                ["hfx2.linear_gelu"] = new(MajorMinor, 0x15, 0x03, 0x02),
                ["hfx2.linear_silu"] = new(MajorMinor, 0x15, 0x03, 0x03),
                ["hfx2.linear_tanh"] = new(MajorMinor, 0x15, 0x03, 0x04),
                ["hfx2.linear_sigmoid"] = new(MajorMinor, 0x15, 0x03, 0x05),
                ["hfx2.linear_dropout"] = new(MajorMinor, 0x15, 0x03, 0x06),
                ["hfx2.linear_layernorm"] = new(MajorMinor, 0x15, 0x03, 0x07),
                ["hfx2.linear_rmsnorm"] = new(MajorMinor, 0x15, 0x03, 0x08),
                ["hfx2.bilinear"] = new(MajorMinor, 0x15, 0x04, 0x09),
                // Matmul
                ["hfx2.matmul_add"] = new(MajorMinor, 0x15, 0x03, 0x0a),
                ["hfx2.matmul_add_relu"] = new(MajorMinor, 0x15, 0x03, 0x0b),
                ["hfx2.matmul_add_gelu"] = new(MajorMinor, 0x15, 0x03, 0x0c),
                ["hfx2.matmul_add_silu"] = new(MajorMinor, 0x15, 0x03, 0x0d),
                ["hfx2.matmul_add_tanh"] = new(MajorMinor, 0x15, 0x03, 0x0e),
                ["hfx2.matmul_scale"] = new(MajorMinor, 0x15, 0x03, 0x0f),
                ["hfx2.matmul_scale_add"] = new(MajorMinor, 0x15, 0x04, 0x10),
                ["hfx2.matmul_softmax"] = new(MajorMinor, 0x15, 0x02, 0x11),
                ["hfx2.matmul_scale_softmax"] = new(MajorMinor, 0x15, 0x03, 0x12),
                ["hfx2.batch_matmul_add"] = new(MajorMinor, 0x15, 0x03, 0x13),
                ["hfx2.batch_matmul_softmax"] = new(MajorMinor, 0x15, 0x02, 0x14),
                // Conv
                ["hfx2.conv2d_bias"] = new(MajorMinor, 0x15, 0x03, 0x15),
                ["hfx2.conv2d_relu"] = new(MajorMinor, 0x15, 0x03, 0x16),
                ["hfx2.conv2d_gelu"] = new(MajorMinor, 0x15, 0x03, 0x17),
                ["hfx2.conv2d_silu"] = new(MajorMinor, 0x15, 0x03, 0x18),
                ["hfx2.conv2d_bias_relu"] = new(MajorMinor, 0x15, 0x03, 0x19),
                ["hfx2.conv2d_bn"] = new(MajorMinor, 0x15, 0x04, 0x1a),
                ["hfx2.conv2d_bn_relu"] = new(MajorMinor, 0x15, 0x04, 0x1b),
                ["hfx2.conv2d_bn_leaky_relu"] = new(MajorMinor, 0x15, 0x04, 0x1c),
                ["hfx2.conv2d_bn_silu"] = new(MajorMinor, 0x15, 0x04, 0x1d),
                ["hfx2.conv2d_add"] = new(MajorMinor, 0x15, 0x03, 0x1e),
                ["hfx2.conv2d_add_relu"] = new(MajorMinor, 0x15, 0x03, 0x1f),
                ["hfx2.conv1d_relu"] = new(MajorMinor, 0x15, 0x03, 0x20),
                ["hfx2.conv1d_gelu"] = new(MajorMinor, 0x15, 0x03, 0x21),
                ["hfx2.conv1d_bn"] = new(MajorMinor, 0x15, 0x04, 0x22),
                ["hfx2.conv1d_bn_relu"] = new(MajorMinor, 0x15, 0x04, 0x23),
                ["hfx2.depthwise_conv2d_bn"] = new(MajorMinor, 0x15, 0x04, 0x24),
                ["hfx2.depthwise_conv2d_bn_relu"] = new(MajorMinor, 0x15, 0x04, 0x25),
                ["hfx2.separable_conv2d"] = new(MajorMinor, 0x15, 0x03, 0x26),
                // Norm + Linear
                ["hfx2.layernorm_linear"] = new(MajorMinor, 0x15, 0x03, 0x27),
                ["hfx2.layernorm_linear_relu"] = new(MajorMinor, 0x15, 0x03, 0x28),
                ["hfx2.rmsnorm_linear"] = new(MajorMinor, 0x15, 0x03, 0x29),
                ["hfx2.rmsnorm_linear_relu"] = new(MajorMinor, 0x15, 0x03, 0x2a),
                ["hfx2.batchnorm_relu"] = new(MajorMinor, 0x15, 0x03, 0x2b),
                ["hfx2.groupnorm_relu"] = new(MajorMinor, 0x15, 0x03, 0x2c),
                // Residual
                ["hfx2.add_layernorm"] = new(MajorMinor, 0x15, 0x03, 0x2d),
                ["hfx2.add_rmsnorm"] = new(MajorMinor, 0x15, 0x03, 0x2e),
                ["hfx2.add_relu"] = new(MajorMinor, 0x15, 0x02, 0x2f),
                ["hfx2.add_dropout"] = new(MajorMinor, 0x15, 0x02, 0x30),
                ["hfx2.norm_add"] = new(MajorMinor, 0x15, 0x03, 0x31),
                ["hfx2.norm_add_linear"] = new(MajorMinor, 0x15, 0x04, 0x32),
                // Pooling
                ["hfx2.conv2d_maxpool"] = new(MajorMinor, 0x15, 0x02, 0x33),
                ["hfx2.conv2d_avgpool"] = new(MajorMinor, 0x15, 0x02, 0x34),
                ["hfx2.conv2d_bn_relu_maxpool"] = new(MajorMinor, 0x15, 0x04, 0x35),
                ["hfx2.conv2d_bn_relu_avgpool"] = new(MajorMinor, 0x15, 0x04, 0x36),
                // Elementwise Chains
                ["hfx2.exp_sum"] = new(MajorMinor, 0x15, 0x01, 0x37),
                ["hfx2.log_sum_exp"] = new(MajorMinor, 0x15, 0x01, 0x38),
                ["hfx2.sum_square"] = new(MajorMinor, 0x15, 0x01, 0x39),
                ["hfx2.mean_square"] = new(MajorMinor, 0x15, 0x01, 0x3a),
                ["hfx2.rms"] = new(MajorMinor, 0x15, 0x01, 0x3b),
                ["hfx2.l2_norm"] = new(MajorMinor, 0x15, 0x01, 0x3c),
                ["hfx2.l1_norm"] = new(MajorMinor, 0x15, 0x01, 0x3d),
                ["hfx2.mul_add_relu"] = new(MajorMinor, 0x15, 0x03, 0x3e),
                // Gated Activations
                ["hfx2.geglu"] = new(MajorMinor, 0x15, 0x02, 0x3f),
                ["hfx2.swiglu"] = new(MajorMinor, 0x15, 0x02, 0x40),
                ["hfx2.reglu"] = new(MajorMinor, 0x15, 0x02, 0x41),
                ["hfx2.geglu_linear"] = new(MajorMinor, 0x15, 0x03, 0x42),
                ["hfx2.swiglu_linear"] = new(MajorMinor, 0x15, 0x03, 0x43),
                ["hfx2.reglu_linear"] = new(MajorMinor, 0x15, 0x03, 0x44),
                // Loss
                ["hfx2.softmax_cross_entropy"] = new(MajorMinor, 0x15, 0x02, 0x45),
                ["hfx2.log_softmax_nll"] = new(MajorMinor, 0x15, 0x02, 0x46),
                ["hfx2.cross_entropy"] = new(MajorMinor, 0x15, 0x02, 0x47),
                ["hfx2.binary_cross_entropy"] = new(MajorMinor, 0x15, 0x02, 0x48),
                ["hfx2.mse_loss"] = new(MajorMinor, 0x15, 0x02, 0x49),
                ["hfx2.mae_loss"] = new(MajorMinor, 0x15, 0x02, 0x4a),
                ["hfx2.huber_loss"] = new(MajorMinor, 0x15, 0x02, 0x4b),
                ["hfx2.kl_divergence"] = new(MajorMinor, 0x15, 0x02, 0x4c),
                ["hfx2.focal_loss"] = new(MajorMinor, 0x15, 0x02, 0x4d),
                // Quantization
                ["hfx2.quantize_linear"] = new(MajorMinor, 0x15, 0x03, 0x4e),
                ["hfx2.dequantize_linear"] = new(MajorMinor, 0x15, 0x03, 0x4f),
                ["hfx2.quantized_matmul"] = new(MajorMinor, 0x15, 0x04, 0x50),
                ["hfx2.quantized_conv2d"] = new(MajorMinor, 0x15, 0x04, 0x51),
                ["hfx2.int8_linear"] = new(MajorMinor, 0x15, 0x03, 0x52),
                ["hfx2.int4_linear"] = new(MajorMinor, 0x15, 0x03, 0x53),

                // ── Subset 16 — HFX-III Block Fusion ─────────────────────────
                // Attention
                ["hfx3.scaled_dot_product_attn"] = new(MajorMinor, 0x16, 0x03, 0x00),
                ["hfx3.masked_scaled_dot_attn"] = new(MajorMinor, 0x16, 0x04, 0x01),
                ["hfx3.causal_self_attn"] = new(MajorMinor, 0x16, 0x03, 0x02),
                ["hfx3.flash_attn"] = new(MajorMinor, 0x16, 0x03, 0x03),
                ["hfx3.flash_attn_causal"] = new(MajorMinor, 0x16, 0x03, 0x04),
                ["hfx3.flash_attn_masked"] = new(MajorMinor, 0x16, 0x04, 0x05),
                ["hfx3.cross_attn"] = new(MajorMinor, 0x16, 0x02, 0x06),
                ["hfx3.multi_head_attn"] = new(MajorMinor, 0x16, 0x07, 0x07),
                ["hfx3.grouped_query_attn"] = new(MajorMinor, 0x16, 0x03, 0x08),
                ["hfx3.multi_query_attn"] = new(MajorMinor, 0x16, 0x03, 0x09),
                ["hfx3.rotary_attn"] = new(MajorMinor, 0x16, 0x05, 0x0a),
                ["hfx3.alibi_attn"] = new(MajorMinor, 0x16, 0x04, 0x0b),
                ["hfx3.sliding_window_attn"] = new(MajorMinor, 0x16, 0x03, 0x0c),
                // Positional Encoding
                ["hfx3.rotary_embed"] = new(MajorMinor, 0x16, 0x03, 0x0d),
                ["hfx3.alibi_bias"] = new(MajorMinor, 0x16, 0x02, 0x0e),
                ["hfx3.sinusoidal_embed"] = new(MajorMinor, 0x16, 0x01, 0x0f),
                ["hfx3.learned_embed"] = new(MajorMinor, 0x16, 0x02, 0x10),
                ["hfx3.rope_apply"] = new(MajorMinor, 0x16, 0x04, 0x11),
                // Embedding
                ["hfx3.tok_pos_embed"] = new(MajorMinor, 0x16, 0x04, 0x12),
                ["hfx3.embed_add_layernorm"] = new(MajorMinor, 0x16, 0x05, 0x13),
                ["hfx3.embed_dropout"] = new(MajorMinor, 0x16, 0x02, 0x14),
                ["hfx3.embed_layernorm"] = new(MajorMinor, 0x16, 0x03, 0x15),
                // FFN Blocks
                ["hfx3.ffn_relu"] = new(MajorMinor, 0x16, 0x05, 0x16),
                ["hfx3.ffn_gelu"] = new(MajorMinor, 0x16, 0x05, 0x17),
                ["hfx3.ffn_swiglu"] = new(MajorMinor, 0x16, 0x04, 0x18),
                ["hfx3.ffn_geglu"] = new(MajorMinor, 0x16, 0x04, 0x19),
                ["hfx3.pre_norm_ffn"] = new(MajorMinor, 0x16, 0x04, 0x1a),
                ["hfx3.post_norm_ffn"] = new(MajorMinor, 0x16, 0x04, 0x1b),
                ["hfx3.ffn_dropout"] = new(MajorMinor, 0x16, 0x03, 0x1c),
                // Transformer Blocks
                ["hfx3.transformer_block"] = new(MajorMinor, 0x16, 0x07, 0x1d),
                ["hfx3.pre_norm_attn_block"] = new(MajorMinor, 0x16, 0x04, 0x1e),
                ["hfx3.post_norm_attn_block"] = new(MajorMinor, 0x16, 0x04, 0x1f),
                ["hfx3.encoder_block"] = new(MajorMinor, 0x16, 0x06, 0x20),
                ["hfx3.decoder_block"] = new(MajorMinor, 0x16, 0x09, 0x21),
                // Residual Blocks
                ["hfx3.residual_block"] = new(MajorMinor, 0x16, 0x03, 0x22),
                ["hfx3.pre_norm_residual"] = new(MajorMinor, 0x16, 0x03, 0x23),
                ["hfx3.post_norm_residual"] = new(MajorMinor, 0x16, 0x03, 0x24),

                // ── Subset 17 — HCX Control Flow ──────────────────────────────
                ["hcx.loop_begin"] = new(MajorMinor, 0x17, 0x0c, 0x00),
                ["hcx.loop_out"] = new(MajorMinor, 0x17, 0x0c, 0x01),
                ["hcx.while_begin"] = new(MajorMinor, 0x17, 0x0c, 0x02),
                ["hcx.while_out"] = new(MajorMinor, 0x17, 0x0c, 0x03),
                ["hcx.if_begin"] = new(MajorMinor, 0x17, 0x0c, 0x04),
                ["hcx.if_else"] = new(MajorMinor, 0x17, 0x0c, 0x05),
                ["hcx.if_end"] = new(MajorMinor, 0x17, 0x0c, 0x06),
                ["hcx.switch_begin"] = new(MajorMinor, 0x17, 0x0c, 0x07),
                ["hcx.switch_case"] = new(MajorMinor, 0x17, 0x0c, 0x08),
                ["hcx.switch_end"] = new(MajorMinor, 0x17, 0x0c, 0x09),

                // ── Subset 20 — HADX Gradient Flow ───────────────────────────
                ["hadx.detach"] = new(MajorMinor, 0x20, 0x01, 0x00),
                ["hadx.checkpoint"] = new(MajorMinor, 0x20, 0x01, 0x01),
                ["hadx.zero_grad"] = new(MajorMinor, 0x20, 0x01, 0x02),
                ["hadx.accumulate"] = new(MajorMinor, 0x20, 0x02, 0x03),
                ["hadx.stop_grad"] = new(MajorMinor, 0x20, 0x01, 0x04),
                ["hadx.enable_grad"] = new(MajorMinor, 0x20, 0x01, 0x05),
                ["hadx.grad_barrier"] = new(MajorMinor, 0x20, 0x0c, 0x06),

                // ── Subset 21 — HADX First Order Derivatives ─────────────────
                ["hadx.grad"] = new(MajorMinor, 0x21, 0x02, 0x00),
                ["hadx.vjp"] = new(MajorMinor, 0x21, 0x03, 0x01),
                ["hadx.jvp"] = new(MajorMinor, 0x21, 0x03, 0x02),
                ["hadx.grad_accum"] = new(MajorMinor, 0x21, 0x02, 0x03),
                ["hadx.partial"] = new(MajorMinor, 0x21, 0x02, 0x04),

                // ── Subset 22 — HADX Second Order Derivatives ────────────────
                ["hadx.hessian"] = new(MajorMinor, 0x22, 0x02, 0x00),
                ["hadx.jacobian"] = new(MajorMinor, 0x22, 0x02, 0x01),
                ["hadx.hvp"] = new(MajorMinor, 0x22, 0x03, 0x02),
                ["hadx.hessian_diag"] = new(MajorMinor, 0x22, 0x02, 0x03),
                ["hadx.fisher"] = new(MajorMinor, 0x22, 0x02, 0x04),

                // ── Subset 23 — HADX Gradient Transforms ─────────────────────
                ["hadx.clip_by_value"] = new(MajorMinor, 0x23, 0x03, 0x00),
                ["hadx.clip_by_norm"] = new(MajorMinor, 0x23, 0x02, 0x01),
                ["hadx.clip_by_global_norm"] = new(MajorMinor, 0x23, 0x02, 0x02),
                ["hadx.grad_normalize"] = new(MajorMinor, 0x23, 0x01, 0x03),
                ["hadx.grad_scale"] = new(MajorMinor, 0x23, 0x02, 0x04),
                ["hadx.grad_reverse"] = new(MajorMinor, 0x23, 0x01, 0x05),

                // ── Subset 24 — HADX Loss Scaling ─────────────────────────────
                ["hadx.loss_scale"] = new(MajorMinor, 0x24, 0x02, 0x00),
                ["hadx.loss_unscale"] = new(MajorMinor, 0x24, 0x02, 0x01),
                ["hadx.check_overflow"] = new(MajorMinor, 0x24, 0x01, 0x02),
                ["hadx.update_scale"] = new(MajorMinor, 0x24, 0x02, 0x03),
                ["hadx.dynamic_scale"] = new(MajorMinor, 0x24, 0x01, 0x04),

                // ── Subset 25 — HADX Optimizers ───────────────────────────────
                ["hadx.sgd_step"] = new(MajorMinor, 0x25, 0x03, 0x00),
                ["hadx.sgd_momentum"] = new(MajorMinor, 0x25, 0x04, 0x01),
                ["hadx.sgd_nesterov"] = new(MajorMinor, 0x25, 0x04, 0x02),
                ["hadx.adam_step"] = new(MajorMinor, 0x25, 0x06, 0x03),
                ["hadx.adamw_step"] = new(MajorMinor, 0x25, 0x07, 0x04),
                ["hadx.adam_amsgrad"] = new(MajorMinor, 0x25, 0x06, 0x05),
                ["hadx.rmsprop_step"] = new(MajorMinor, 0x25, 0x04, 0x06),
                ["hadx.rmsprop_mom"] = new(MajorMinor, 0x25, 0x05, 0x07),
                ["hadx.adagrad_step"] = new(MajorMinor, 0x25, 0x03, 0x08),
                ["hadx.adagrad_clip"] = new(MajorMinor, 0x25, 0x04, 0x09),

            }.ToFrozenDictionary(StringComparer.Ordinal);
        }
    }
}