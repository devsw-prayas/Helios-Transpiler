# Helios-DLX — Unified Specification
## H-ISA · HFX · HADX · HDS · .hlx Binary Format

---

## Table of Contents

```
1.  Overview
2.  Opcode Encoding
3.  Optype Map
4.  Subset Map
5.  H-ISA — Base Instruction Set
6.  HFX-I — Local Fusion
7.  HFX-II — Kernel Fusion
8.  HFX-III — Block Fusion
9.  HADX — Helios AutoDiff eXtensions
10. Fusion Region Markers
11. .hlx Binary Format
12. HDS — Helios DSL Grammar
13. Error System
```

---

## 1. Overview

Helios-DLX is a deep learning compiler infrastructure. It defines a tiered
instruction set architecture (ISA), a binary IR format (.hlx), and a
domain-specific frontend language (HDS).

### Stack

```
HDS source (.hds)
      ↓
HDS compiler
      ↓
.hlx binary  ←─── H-ISA base opcodes
      ↓
HFX pass     ←─── fusion annotation
      ↓
HADX pass    ←─── autodiff + optimizer opcodes
      ↓
.hlx binary  ←─── final IR
      ↓
executor graph (out of scope)
```

### Extension Namespaces

```
H-ISA       base instruction set         139 ops       subset 00–13
HFX-I       local fusion                 31 ops        subset 14
HFX-II      kernel fusion                84 ops        subset 15
HFX-III     block fusion                 37 ops        subset 16
            reserved for future HFX               subset 17–1F
HADX        autodiff extensions                   subset 20–25
            reserved for future HADX              subset 26–2F
            reserved                              subset 30–FF
```

### Design Rules

```
1.  Fusion never changes semantics. Constituent ops inside a fused region
    must always produce the correct result if executed independently.

2.  Fusion is additive. Stripping all fuse_begin/fuse_wrap pairs from any
    .hlx binary produces valid H-ISA that executes correctly.

3.  HFX-III patterns are pre-verified. The fusion pass only recognizes
    known-safe block patterns. It never dynamically constructs HFX-III fusions.

4.  op_id is append-only. New ops within a subset are always added at the
    end. Existing op_ids are never renumbered.

5.  Subset 30–FF is reserved. Do not emit opcodes in this range.

6.  Every DSL construct maps to one or more opcodes. If the ISA cannot
    express it, HDS cannot do it.
```

---

## 2. Opcode Encoding

```
[4B magic] [4b major] [4b minor] [8b subset] [8b optype] [8b op_id]

magic   : 494f5441  ("IOTA" — retained from Iota IR)
major   : 1
minor   : 0
subset  : semantic group
optype  : operand class
op_id   : index within subset
```

Total opcode size: 8 bytes.

---

## 3. Optype Map

```
00      creation        produces tensor with no input tensors
01      unary           1 input tensor
02      binary          2 input tensors
03      ternary         3 input tensors
04      reduction       reduces dimensions
05      transform       reshapes or reorders layout
06      indexing        reads/writes via index tensors
07      normalization   statistical normalization
08      convolution     convolutional operators
09      pooling         spatial pooling
0a      logical         boolean logic
0b      comparison      produces boolean tensor
0c      meta            control / metadata / debug
0d      random          stochastic ops
0e      fusion          fusion region marker
0f      reserved
```

For HFX and HADX ops, optype encodes input tensor count (excluding dst).

---

## 4. Subset Map

```
00      memory          input, param, const, load, store
01      tensor          identity, cast, clone
02      shape           reshape, flatten, expand, ...
03      info            shape, size, rank, stride, numel
04      arithmetic      add, sub, mul, div, ...
05      math            exp, log, sin, cos, ...
06      activation      relu, gelu, sigmoid, ...
07      comparison      eq, ne, lt, le, gt, ge
08      logical         and, or, xor, not
09      reduction       reduce_sum, reduce_mean, argmax, ...
0a      linalg          dot, matmul, diag, trace, ...
0b      convolution     conv1d, conv2d, conv_t2d, ...
0c      pooling         maxpool2d, avgpool2d, ...
0d      normalization   batchnorm, layernorm, rmsnorm, ...
0e      indexing        gather, scatter, where, ...
0f      sorting         topk, sort, argsort, select
10      probability     softmax, log_softmax
11      masking         masked_fill, masked_select
12      random          random_uniform, random_normal, dropout
13      control         noop, assert, debug, fuse markers
14      HFX-I           local fusion
15      HFX-II          kernel fusion
16      HFX-III         block fusion
17–1F   reserved        future HFX tiers
20      HADX            grad flow
21      HADX            first order derivatives
22      HADX            second order derivatives
23      HADX            grad transforms
24      HADX            loss scaling
25      HADX            optimizers
26–2F   reserved        future HADX
30–FF   reserved
```

---

## 5. H-ISA — Base Instruction Set

### Subset 00 — Memory / IO

```
opcode    hexcode            operands
input     494f544110000000   dst
param     494f544110000001   dst
const     494f544110000002   dst,const_id
load      494f544110000003   dst,src
store     494f544110000004   dst,src
```

### Subset 01 — Tensor Ops

```
opcode    hexcode            operands
identity  494f544110010100   dst,src
cast      494f544110010101   dst,src
clone     494f544110010102   dst,src
```

### Subset 02 — Shape / Transform

```
opcode    hexcode            operands
reshape   494f544110020500   dst,src,shape
flatten   494f544110020501   dst,src
expand    494f544110020502   dst,src,shape
expand_as 494f544110020503   dst,src,ref
squeeze   494f544110020504   dst,src
unsqueeze 494f544110020505   dst,src,axis
transpose 494f544110020506   dst,src
permute   494f544110020507   dst,src
concat    494f544110020508   dst,a,b
split     494f544110020509   dst,src
chunk     494f54411002050a   dst,src
pad       494f54411002050b   dst,src
slice     494f54411002050c   dst,src
narrow    494f54411002050d   dst,src
roll      494f54411002050e   dst,src
repeat    494f54411002050f   dst,src
tile      494f544110020510   dst,src
stack     494f544110020511   dst,a,b
unstack   494f544110020512   dst,src
```

### Subset 03 — Tensor Info

```
opcode    hexcode            operands
shape     494f544110030c00   dst,src
size      494f544110030c01   dst,src
rank      494f544110030c02   dst,src
stride    494f544110030c03   dst,src
numel     494f544110030c04   dst,src
```

### Subset 04 — Arithmetic

```
opcode    hexcode            operands
add       494f544110040200   dst,a,b
sub       494f544110040201   dst,a,b
mul       494f544110040202   dst,a,b
div       494f544110040203   dst,a,b
mod       494f544110040204   dst,a,b
neg       494f544110040105   dst,a
abs       494f544110040106   dst,a
sign      494f544110040107   dst,a
floor     494f544110040108   dst,a
ceil      494f544110040109   dst,a
round     494f54411004010a   dst,a
clamp     494f54411004030b   dst,a,min,max
```

### Subset 05 — Math

```
opcode    hexcode            operands
exp       494f544110050100   dst,a
log       494f544110050101   dst,a
log2      494f544110050102   dst,a
log10     494f544110050103   dst,a
pow       494f544110050204   dst,a,b
sqrt      494f544110050105   dst,a
rsqrt     494f544110050106   dst,a
sin       494f544110050107   dst,a
cos       494f544110050108   dst,a
tan       494f544110050109   dst,a
asin      494f54411005010a   dst,a
acos      494f54411005010b   dst,a
atan      494f54411005010c   dst,a
sinh      494f54411005010d   dst,a
cosh      494f54411005010e   dst,a
tanh      494f54411005010f   dst,a
erf       494f544110050110   dst,a
```

### Subset 06 — Activation

```
opcode        hexcode            operands
relu          494f544110060100   dst,a
relu6         494f544110060101   dst,a
leaky_relu    494f544110060102   dst,a
elu           494f544110060103   dst,a
selu          494f544110060104   dst,a
sigmoid       494f544110060105   dst,a
gelu          494f544110060106   dst,a
swish         494f544110060107   dst,a
softplus      494f544110060108   dst,a
softsign      494f544110060109   dst,a
hardsigmoid   494f54411006010a   dst,a
hardswish     494f54411006010b   dst,a
```

### Subset 07 — Comparison

```
opcode    hexcode            operands
eq        494f544110070b00   dst,a,b
ne        494f544110070b01   dst,a,b
lt        494f544110070b02   dst,a,b
le        494f544110070b03   dst,a,b
gt        494f544110070b04   dst,a,b
ge        494f544110070b05   dst,a,b
```

### Subset 08 — Logical

```
opcode    hexcode            operands
and       494f544110080a00   dst,a,b
or        494f544110080a01   dst,a,b
xor       494f544110080a02   dst,a,b
not       494f544110080100   dst,a
```

### Subset 09 — Reduction

```
opcode        hexcode            operands
reduce_sum    494f544110090400   dst,a
reduce_mean   494f544110090401   dst,a
reduce_max    494f544110090402   dst,a
reduce_min    494f544110090403   dst,a
reduce_prod   494f544110090404   dst,a
reduce_l1     494f544110090405   dst,a
reduce_l2     494f544110090406   dst,a
argmax        494f544110090407   dst,a
argmin        494f544110090408   dst,a
```

### Subset 0a — Linear Algebra

```
opcode          hexcode            operands
dot             494f5441100a0200   dst,a,b
matmul          494f5441100a0201   dst,a,b
batch_matmul    494f5441100a0202   dst,a,b
outer           494f5441100a0203   dst,a,b
diag            494f5441100a0104   dst,a
trace           494f5441100a0105   dst,a
```

### Subset 0b — Convolution

```
opcode      hexcode            operands
conv1d      494f5441100b0800   dst,input,weight
conv2d      494f5441100b0801   dst,input,weight
conv3d      494f5441100b0802   dst,input,weight
conv_t1d    494f5441100b0803   dst,input,weight
conv_t2d    494f5441100b0804   dst,input,weight
conv_t3d    494f5441100b0805   dst,input,weight
```

### Subset 0c — Pooling

```
opcode      hexcode            operands
maxpool1d   494f5441100c0900   dst,input
maxpool2d   494f5441100c0901   dst,input
maxpool3d   494f5441100c0902   dst,input
avgpool1d   494f5441100c0903   dst,input
avgpool2d   494f5441100c0904   dst,input
avgpool3d   494f5441100c0905   dst,input
gmaxpool    494f5441100c0906   dst,input
gavgpool    494f5441100c0907   dst,input
```

### Subset 0d — Normalization

```
opcode          hexcode            operands
batchnorm       494f5441100d0700   dst,input
layernorm       494f5441100d0701   dst,input
groupnorm       494f5441100d0702   dst,input
instancenorm    494f5441100d0703   dst,input
rmsnorm         494f5441100d0704   dst,input
```

### Subset 0e — Indexing

```
opcode          hexcode            operands
gather          494f5441100e0600   dst,input,index
scatter         494f5441100e0601   dst,input,index
index_select    494f5441100e0602   dst,input,index
take            494f5441100e0603   dst,input,index
put             494f5441100e0604   dst,input,index
where           494f5441100e0305   dst,cond,a,b
```

### Subset 0f — Sorting

```
opcode      hexcode            operands
topk        494f5441100f0600   dst,input
sort        494f5441100f0601   dst,input
argsort     494f5441100f0602   dst,input
select      494f5441100f0603   dst,input,index
```

### Subset 10 — Probability

```
opcode          hexcode            operands
softmax         494f544110100100   dst,input
log_softmax     494f544110100101   dst,input
```

### Subset 11 — Masking

```
opcode            hexcode            operands
masked_fill       494f544110110300   dst,input,mask,value
masked_select     494f544110110601   dst,input,mask
```

### Subset 12 — Random

```
opcode            hexcode            operands
random_uniform    494f544110120d00   dst
random_normal     494f544110120d01   dst
dropout           494f544110120d02   dst,input
```

### Subset 13 — Control

```
opcode        hexcode            operands
noop          494f544110130c00   none
assert        494f544110130c01   cond
debug         494f544110130c02   value
fuse_begin    494f544110130e00   pattern_id
fuse_wrap     494f544110130e01   none
```

---

## 6. HFX-I — Local Fusion (Subset 14)

```
Scope       2–4 pointwise primitive ops
Rule        all ops index-aligned: y[i] = f(a[i], b[i], ...)
            no reductions, no layout changes
Kernel      vectorized SIMD loop
Optype      input tensor count (excluding dst)
```

```
opcode            hexcode            operands
add_relu          494f544110140100   dst,a,b
add_gelu          494f544110140201   dst,a,b
add_silu          494f544110140202   dst,a,b
add_sigmoid       494f544110140203   dst,a,b
add_tanh          494f544110140204   dst,a,b
mul_add           494f544110140305   dst,a,b,c
mul_relu          494f544110140206   dst,a,b
mul_sigmoid       494f544110140207   dst,a,b
mul_tanh          494f544110140208   dst,a,b
mul_gelu          494f544110140209   dst,a,b
scale_add         494f54411014030a   dst,input,scale,bias
scale_relu        494f54411014020b   dst,input,scale
scale_add_relu    494f54411014030c   dst,input,scale,bias
scale_add_gelu    494f54411014030d   dst,input,scale,bias
scale_add_silu    494f54411014030e   dst,input,scale,bias
sigmoid_mul       494f54411014020f   dst,a,b
relu_mul          494f544110140210   dst,a,b
silu_mul          494f544110140211   dst,a,b
tanh_mul          494f544110140212   dst,a,b
gelu_mul          494f544110140213   dst,a,b
exp_mul           494f544110140214   dst,a,b
neg_add           494f544110140215   dst,a,b
abs_add           494f544110140216   dst,a,b
clamp_add         494f544110140317   dst,a,min,max
add_add           494f544110140318   dst,a,b,c
mul_mul           494f544110140319   dst,a,b,c
sub_relu          494f54411014021a   dst,a,b
sub_abs           494f54411014021b   dst,a,b
rsqrt_mul         494f54411014021c   dst,a,b
log_sum           494f54411014021d   dst,a,b
exp_add           494f54411014021e   dst,a,b
```

---

## 7. HFX-II — Kernel Fusion (Subset 15)

```
Scope       one dominant compute op + elementwise epilogue
Rule        single matmul/conv/linear anchor + bias/activation/norm tail
Kernel      tiled GPU/CPU kernel — avoids intermediate global memory writes
Optype      input tensor count (excluding dst)
```

### Linear

```
opcode                hexcode            operands
linear                494f544110150300   dst,input,weight,bias
linear_relu           494f544110150301   dst,input,weight,bias
linear_gelu           494f544110150302   dst,input,weight,bias
linear_silu           494f544110150303   dst,input,weight,bias
linear_tanh           494f544110150304   dst,input,weight,bias
linear_sigmoid        494f544110150305   dst,input,weight,bias
linear_dropout        494f544110150306   dst,input,weight,bias
linear_layernorm      494f544110150307   dst,input,weight,bias
linear_rmsnorm        494f544110150308   dst,input,weight,bias
bilinear              494f544110150409   dst,input1,input2,weight
```

### Matmul

```
opcode                hexcode            operands
matmul_add            494f54411015030a   dst,a,b,bias
matmul_add_relu       494f54411015030b   dst,a,b,bias
matmul_add_gelu       494f54411015030c   dst,a,b,bias
matmul_add_silu       494f54411015030d   dst,a,b,bias
matmul_add_tanh       494f54411015030e   dst,a,b,bias
matmul_scale          494f54411015030f   dst,a,b,scale
matmul_scale_add      494f544110150410   dst,a,b,scale,bias
matmul_softmax        494f544110150211   dst,a,b
matmul_scale_softmax  494f544110150312   dst,a,b,scale
batch_matmul_add      494f544110150313   dst,a,b,bias
batch_matmul_softmax  494f544110150214   dst,a,b
```

### Conv

```
opcode                    hexcode            operands
conv2d_bias               494f544110150315   dst,input,weight,bias
conv2d_relu               494f544110150316   dst,input,weight,bias
conv2d_gelu               494f544110150317   dst,input,weight,bias
conv2d_silu               494f544110150318   dst,input,weight,bias
conv2d_bias_relu          494f544110150319   dst,input,weight,bias
conv2d_bn                 494f54411015041a   dst,input,weight,bn_w,bn_b
conv2d_bn_relu            494f54411015041b   dst,input,weight,bn_w,bn_b
conv2d_bn_leaky_relu      494f54411015041c   dst,input,weight,bn_w,bn_b
conv2d_bn_silu            494f54411015041d   dst,input,weight,bn_w,bn_b
conv2d_add                494f54411015031e   dst,input,weight,residual
conv2d_add_relu           494f54411015031f   dst,input,weight,residual
conv1d_relu               494f544110150320   dst,input,weight,bias
conv1d_gelu               494f544110150321   dst,input,weight,bias
conv1d_bn                 494f544110150422   dst,input,weight,bn_w,bn_b
conv1d_bn_relu            494f544110150423   dst,input,weight,bn_w,bn_b
depthwise_conv2d_bn       494f544110150424   dst,input,weight,bn_w,bn_b
depthwise_conv2d_bn_relu  494f544110150425   dst,input,weight,bn_w,bn_b
separable_conv2d          494f544110150326   dst,input,dw_weight,pw_weight
```

### Norm + Linear

```
opcode                    hexcode            operands
layernorm_linear          494f544110150327   dst,input,norm_w,linear_w
layernorm_linear_relu     494f544110150328   dst,input,norm_w,linear_w
rmsnorm_linear            494f544110150329   dst,input,norm_w,linear_w
rmsnorm_linear_relu       494f54411015032a   dst,input,norm_w,linear_w
batchnorm_relu            494f54411015032b   dst,input,bn_w,bn_b
groupnorm_relu            494f54411015032c   dst,input,gn_w,gn_b
```

### Residual

```
opcode                hexcode            operands
add_layernorm         494f54411015032d   dst,a,b,norm_w
add_rmsnorm           494f54411015032e   dst,a,b,norm_w
add_relu              494f54411015022f   dst,a,b
add_dropout           494f544110150230   dst,a,b
norm_add              494f544110150331   dst,input,residual,norm_w
norm_add_linear       494f544110150432   dst,input,residual,norm_w,linear_w
```

### Pooling

```
opcode                    hexcode            operands
conv2d_maxpool            494f544110150233   dst,input,weight
conv2d_avgpool            494f544110150234   dst,input,weight
conv2d_bn_relu_maxpool    494f544110150435   dst,input,weight,bn_w,bn_b
conv2d_bn_relu_avgpool    494f544110150436   dst,input,weight,bn_w,bn_b
```

### Elementwise Chains

```
opcode            hexcode            operands
exp_sum           494f544110150137   dst,input
log_sum_exp       494f544110150138   dst,input
sum_square        494f544110150139   dst,input
mean_square       494f54411015013a   dst,input
rms               494f54411015013b   dst,input
l2_norm           494f54411015013c   dst,input
l1_norm           494f54411015013d   dst,input
mul_add_relu      494f54411015033e   dst,a,b,c
```

### Gated Activations

```
opcode        hexcode            operands
geglu         494f54411015023f   dst,input,weight
swiglu        494f544110150240   dst,input,weight
reglu         494f544110150241   dst,input,weight
geglu_linear  494f544110150342   dst,input,weight,bias
swiglu_linear 494f544110150343   dst,input,weight,bias
reglu_linear  494f544110150344   dst,input,weight,bias
```

### Loss

```
opcode                    hexcode            operands
softmax_cross_entropy     494f544110150245   dst,logits,targets
log_softmax_nll           494f544110150246   dst,logits,targets
cross_entropy             494f544110150247   dst,logits,targets
binary_cross_entropy      494f544110150248   dst,pred,targets
mse_loss                  494f544110150249   dst,pred,targets
mae_loss                  494f54411015024a   dst,pred,targets
huber_loss                494f54411015024b   dst,pred,targets
kl_divergence             494f54411015024c   dst,p,q
focal_loss                494f54411015024d   dst,logits,targets
```

### Quantization

```
opcode                hexcode            operands
quantize_linear       494f54411015034e   dst,input,scale,zero_point
dequantize_linear     494f54411015034f   dst,input,scale,zero_point
quantized_matmul      494f544110150450   dst,a,b,scale_a,scale_b
quantized_conv2d      494f544110150451   dst,input,weight,scale,zero_point
int8_linear           494f544110150352   dst,input,weight,scale
int4_linear           494f544110150353   dst,input,weight,scale
```

---

## 8. HFX-III — Block Fusion (Subset 16)

```
Scope       full subgraph / neural network block, 5–40 primitive ops
Rule        multiple tensor flows, may include reductions, layout ops,
            normalization
Kernel      specialized algorithm — not just fused ops, may use a
            different compute strategy (e.g. flash attention)
Correctness pre-verified only — pass never dynamically constructs HFX-III
Optype      input tensor count (excluding dst)
```

### Attention

```
opcode                    hexcode            operands
scaled_dot_product_attn   494f544110160300   dst,q,k,v
masked_scaled_dot_attn    494f544110160401   dst,q,k,v,mask
causal_self_attn          494f544110160302   dst,q,k,v
flash_attn                494f544110160303   dst,q,k,v
flash_attn_causal         494f544110160304   dst,q,k,v
flash_attn_masked         494f544110160405   dst,q,k,v,mask
cross_attn                494f544110160206   dst,q,kv
multi_head_attn           494f544110160707   dst,q,k,v,w_q,w_k,w_v,w_o
grouped_query_attn        494f544110160308   dst,q,k,v
multi_query_attn          494f544110160309   dst,q,k,v
rotary_attn               494f54411016050a   dst,q,k,v,cos,sin
alibi_attn                494f54411016040b   dst,q,k,v,slopes
sliding_window_attn       494f54411016030c   dst,q,k,v
```

### Positional Encoding

```
opcode            hexcode            operands
rotary_embed      494f54411016030d   dst,input,cos,sin
alibi_bias        494f54411016020e   dst,input,slopes
sinusoidal_embed  494f54411016010f   dst,input
learned_embed     494f544110160210   dst,input,weight
rope_apply        494f544110160411   dst,q,k,cos,sin
```

### Embedding

```
opcode                hexcode            operands
tok_pos_embed         494f544110160412   dst,tokens,positions,w_tok,w_pos
embed_add_layernorm   494f544110160513   dst,tokens,positions,w_tok,w_pos,norm_w
embed_dropout         494f544110160214   dst,input,weight
embed_layernorm       494f544110160315   dst,input,weight,norm_w
```

### FFN Blocks

```
opcode            hexcode            operands
ffn_relu          494f544110160516   dst,input,w1,w2,bias1,bias2
ffn_gelu          494f544110160517   dst,input,w1,w2,bias1,bias2
ffn_swiglu        494f544110160418   dst,input,w1,w2,w3
ffn_geglu         494f544110160419   dst,input,w1,w2,w3
pre_norm_ffn      494f54411016041a   dst,input,w1,w2,norm_w
post_norm_ffn     494f54411016041b   dst,input,w1,w2,norm_w
ffn_dropout       494f54411016031c   dst,input,w1,w2
```

### Transformer Blocks

```
opcode                hexcode            operands
transformer_block     494f54411016071d   dst,input,attn_w,ffn_w1,ffn_w2,norm_w1,norm_w2
pre_norm_attn_block   494f54411016041e   dst,input,q,k,v,norm_w
post_norm_attn_block  494f54411016041f   dst,input,q,k,v,norm_w
encoder_block         494f544110160620   dst,input,attn_w,ffn_w,norm_w1,norm_w2
decoder_block         494f544110160921   dst,input,self_attn_w,cross_attn_w,ffn_w,norm_w1,norm_w2,bias1,bias2,bias3
```

### Residual Blocks

```
opcode                hexcode            operands
residual_block        494f544110160322   dst,input,weight,norm_w
pre_norm_residual     494f544110160323   dst,input,residual,norm_w
post_norm_residual    494f544110160324   dst,input,residual,norm_w
```

---

## 9. HADX — Helios AutoDiff eXtensions (Subsets 20–25)

```
All optimizer buffer tensors are declared as param ops in H-ISA.
Optype field: input tensor count (excluding dst).
```

### Param Type Encoding (for optimizer buffers)

```
00      weight      model parameter
01      gradient    gradient buffer
02      momentum    first moment (Adam m, SGD velocity)
03      variance    second moment (Adam v)
04      auxiliary   any other optimizer state
```

### Subset 20 — Gradient Flow Control

```
opcode          hexcode            operands
detach          494f544110200100   dst,input
checkpoint      494f544110200101   dst,input
zero_grad       494f544110200102   dst,param
accumulate      494f544110200203   dst,grad,delta
stop_grad       494f544110200104   dst,input
enable_grad     494f544110200105   dst,input
grad_barrier    494f544110200c06   none
```

### Subset 21 — First Order Derivatives

```
opcode          hexcode            operands
grad            494f544110210200   dst,output,input
vjp             494f544110210301   dst,output,input,vector
jvp             494f544110210302   dst,output,input,vector
grad_accum      494f544110210203   dst,output,input
partial         494f544110210204   dst,output,input
```

### Subset 22 — Second Order Derivatives

```
opcode          hexcode            operands
hessian         494f544110220200   dst,output,input
jacobian        494f544110220201   dst,output,input
hvp             494f544110220302   dst,output,input,vector
hessian_diag    494f544110220203   dst,output,input
fisher          494f544110220204   dst,output,input
```

### Subset 23 — Gradient Transforms

```
opcode                  hexcode            operands
clip_by_value           494f544110230300   dst,grad,min,max
clip_by_norm            494f544110230201   dst,grad,max_norm
clip_by_global_norm     494f544110230202   dst,grad,global_norm
grad_normalize          494f544110230103   dst,grad
grad_scale              494f544110230204   dst,grad,scale
grad_reverse            494f544110230105   dst,grad
```

### Subset 24 — Loss Scaling

```
opcode              hexcode            operands
loss_scale          494f544110240200   dst,loss,scale
loss_unscale        494f544110240201   dst,grad,scale
check_overflow      494f544110240102   dst,grad
update_scale        494f544110240203   dst,scale,overflow
dynamic_scale       494f544110240104   dst,loss
```

### Subset 25 — Optimizers

```
opcode          hexcode            operands

; SGD
sgd_step        494f544110250300   dst,param,grad,lr
sgd_momentum    494f544110250401   dst,param,grad,velocity,lr
sgd_nesterov    494f544110250402   dst,param,grad,velocity,lr

; Adam
adam_step       494f544110250603   dst,param,grad,m,v,lr,beta1
adamw_step      494f544110250704   dst,param,grad,m,v,lr,beta1,beta2
adam_amsgrad    494f544110250605   dst,param,grad,m,v,v_max,lr

; RMSProp
rmsprop_step    494f544110250406   dst,param,grad,v,lr
rmsprop_mom     494f544110250507   dst,param,grad,v,velocity,lr

; Adagrad
adagrad_step    494f544110250308   dst,param,grad,acc,lr
adagrad_clip    494f544110250409   dst,param,grad,acc,lr
```

---

## 10. Fusion Region Markers

```
opcode        hexcode            operands
fuse_begin    494f544110130e00   pattern_id
fuse_wrap     494f544110130e01   none
```

`pattern_id` is the full opcode of the corresponding HFX op.
Constituent base ops between `fuse_begin` and `fuse_wrap` remain valid
H-ISA and serve as the fallback expansion path if the target device does
not implement the fusion kernel.

---

## 11. .hlx Binary Format

### File Header

```
offset  size    field
00      4B      magic           494f5441
04      1B      major           0x01
05      1B      minor           0x00
06      2B      flags           bitfield (see below)
08      4B      op_count        total opcodes in stream
0C      4B      section_count   number of sections
10      4B      checksum        CRC32 over everything after header
14      4B      reserved        0x00000000
```

Total header size: 24 bytes.

### Flags Bitfield

```
bit 0   has_fusion          HFX section present
bit 1   has_gradients       HADX section present
bit 2   has_debug           metadata section present
bit 3   has_params          param table section present
bit 4–15  reserved          must be zero
```

### Section Table Entry

Immediately follows file header. Variable count — `section_count` entries.

```
offset  size    field
00      1B      section_type    (see below)
01      1B      reserved        0x00
02      2B      flags           reserved, must be zero
04      4B      offset          byte offset from start of file
08      4B      size            byte size of section data
```

Total per entry: 12 bytes.

### Section Types

```
00      const pool
01      param table
02      opcode stream       required — all others optional
03      fusion table
04      grad table
05      metadata
06–FF   reserved
```

Loaders iterate the section table, check section_type, and skip unknown
entries. A minimal valid .hlx contains only section type 02.

### Const Pool Section (type 00)

```
offset  size    field
00      4B      entry_count

; then entry_count × const_entry
```

**Const Entry:**

```
offset  size    field
00      4B      const_id        matches const_id in opcode operand
04      1B      dtype           (see dtype encoding below)
05      1B      rank            number of dimensions (0 = scalar)
06      2B      reserved        0x0000
08      4B×rank shape           one uint32 per dimension
??      ??      data            raw tensor bytes, tightly packed
```

**Dtype Encoding:**

```
00      float32
01      float16
02      bfloat16
03      float64
04      int8
05      int16
06      int32
07      int64
08      tf32
09–FF   reserved
```

`numel` = product of all shape dims. For a scalar, rank=0 and numel=1.
`const_id` assigned sequentially from 00000001. 0 is null/invalid.

### Param Table Section (type 01)

```
offset  size    field
00      4B      entry_count

; then entry_count × param_entry
```

**Param Entry:**

```
offset  size    field
00      4B      param_id        index of param opcode in opcode stream
04      1B      dtype           same encoding as const pool
05      1B      rank            number of dimensions
06      1B      param_type      (see below)
07      1B      reserved        0x00
08      4B×rank shape           one uint32 per dimension
??      ??      name            null terminated UTF-8 string
                                fully qualified dot path from model root
                                e.g. "TransformerBlock.attn.w_q\0"
```

**Param Type Encoding:**

```
00      weight
01      gradient
02      momentum
03      variance
04      auxiliary
```

`param_id` assigned sequentially from 00000001 in declaration order,
depth-first through init(). 0 is null/invalid. Entries are variable
length — loader reads sequentially scanning for null terminator.

### Opcode Stream Section (type 02)

```
offset  size    field
00      ??      op_count × 8B opcodes
                each opcode is the full 8-byte encoding
                [4B magic][4b major][4b minor][8b subset][8b optype][8b op_id]
                followed immediately by operand references
```

### Tier Summary

```
Tier      Subset   Scope              Op count   Kernel type
H-ISA     00–13    primitive ops      1          scalar/vector
HFX-I     14       pointwise chain    2–4        SIMD loop
HFX-II    15       kernel + epilogue  3–6        GPU/CPU kernel
HFX-III   16       full subgraph      5–40       custom algorithm
HADX      20–25    autodiff/optim     varies     grad engine
```

---

## 12. HDS — Helios DSL Grammar

### Overview

HDS is the frontend language for Helios-DLX. Every construct maps directly
to one or more H-ISA, HFX, or HADX opcodes. If the ISA cannot express it,
the DSL cannot do it.

### Value Kinds

```
tensor      runtime value       param, input, op output     → opcodes
scalar      compile time        init args, op attributes    → folded to const
submodel    compile time        nested model instances      → resolved at compile time
```

### Dtype System

```
float32   float16   bfloat16   float64
int8      int16     int32      int64
tf32
```

Type hints are strictly enforced. Dtype or shape mismatch is a hard
compile error.

### Grammar

```
; ─────────────────────────────────────────────────────────
; Notation: := definition  |  alternation
;            *  zero or more  +  one or more
;            ?  optional       () grouping
; ─────────────────────────────────────────────────────────

; ── Top Level ─────────────────────────────────────────────

program         := include_stmt*
                   model_def+
                   run_stmt?

; ── Includes ──────────────────────────────────────────────

include_stmt    := 'include' STRING NEWLINE
STRING          := '"' [a-zA-Z0-9_./]+ '.hds' '"'

; ── Model Definition ──────────────────────────────────────

model_def       := 'model' NAME ':' NEWLINE INDENT
                       init_op
                       forward_op
                       backward_op
                   DEDENT

; all three ops are mandatory — omitting any is a hard compile error

; ── Init Operation ────────────────────────────────────────

init_op         := 'init' '(' init_params? ')' ':' NEWLINE INDENT
                       init_stmt+
                   DEDENT

init_params     := scalar_param (',' scalar_param)*
scalar_param    := NAME (':' SCALAR_TYPE)?
SCALAR_TYPE     := 'int' | 'float'

init_stmt       := param_decl
                 | submodel_decl
                 | scalar_decl

param_decl      := 'self' '.' NAME '=' 'param' '(' ')' NEWLINE
submodel_decl   := 'self' '.' NAME '=' NAME '(' submodel_args? ')' NEWLINE
submodel_args   := submodel_arg (',' submodel_arg)*
submodel_arg    := NAME '=' scalar_expr | scalar_expr
scalar_decl     := 'self' '.' NAME '=' scalar_expr NEWLINE

; ── Forward Operation ─────────────────────────────────────

forward_op      := 'forward' '(' forward_params? ')' ':' NEWLINE INDENT
                       forward_stmt+
                   DEDENT

forward_params  := forward_param (',' forward_param)*
forward_param   := NAME (':' type_hint)?

forward_stmt    := tensor_assign | return_stmt
tensor_assign   := NAME '=' tensor_expr NEWLINE
return_stmt     := 'return' return_expr NEWLINE
return_expr     := tuple_expr | tensor_expr
tuple_expr      := 'tuple' '(' tensor_expr (',' tensor_expr)+ ')'

; ── Backward Operation ────────────────────────────────────

backward_op     := 'backward' '(' ')' '=' 'default' NEWLINE
                 | 'backward' '(' ')' ':' NEWLINE INDENT
                       backward_stmt+
                   DEDENT

backward_stmt   := tensor_assign | return_stmt

; backward() = default  →  HADX pass auto-differentiates from forward()
; backward(): <code>    →  only HADX opcodes permitted

; ── Run Entry Point ───────────────────────────────────────

run_stmt        := 'run' NAME '(' submodel_args? ')' ':' NEWLINE INDENT
                       run_forward
                       run_backward?
                   DEDENT

run_forward     := 'forward' '(' tensor_expr (',' tensor_expr)* ')' NEWLINE
run_backward    := 'backward' '(' ')' '=' 'default' NEWLINE
                 | 'backward' '(' ')' NEWLINE

; ── Type Hints ────────────────────────────────────────────

type_hint       := 'tensor' '[' DTYPE ',' shape ']'
shape           := '(' dim (',' dim)* ')'
dim             := NAME | INT

DTYPE           := 'float32' | 'float16' | 'bfloat16' | 'float64'
                 | 'int8'    | 'int16'   | 'int32'    | 'int64'
                 | 'tf32'

; ── Tensor Expressions ────────────────────────────────────

tensor_expr     := tensor_call
                 | tensor_binop
                 | tensor_unop
                 | submodel_call
                 | self_access
                 | NAME

tensor_call     := NAME '(' tensor_args? ')'
tensor_args     := tensor_arg (',' tensor_arg)*
tensor_arg      := tensor_expr | kwarg

submodel_call   := self_path '.' NAME '(' tensor_args? ')'
self_path       := 'self' '.' NAME ('.' NAME)*
self_access     := 'self' '.' NAME ('.' NAME)*

tensor_binop    := tensor_expr TENSOR_OP tensor_expr
tensor_unop     := '-' tensor_expr
TENSOR_OP       := '+' | '-' | '*' | '/' | '@'

kwarg           := NAME '=' scalar_expr | NAME '=' tensor_expr

; tensor binop desugaring
;   a + b  →  add(a, b)       a - b  →  sub(a, b)
;   a * b  →  mul(a, b)       a / b  →  div(a, b)
;   a @ b  →  matmul(a, b)    -a     →  neg(a)

; ── Scalar Expressions ────────────────────────────────────

scalar_expr     := scalar_call
                 | scalar_binop
                 | scalar_unop
                 | self_access
                 | NAME
                 | literal

scalar_call     := SCALAR_FUNC '(' scalar_expr ')'
SCALAR_FUNC     := 'sqrt' | 'log' | 'exp' | 'abs' | 'floor' | 'ceil'

scalar_binop    := scalar_expr SCALAR_OP scalar_expr
scalar_unop     := '-' scalar_expr
SCALAR_OP       := '+' | '-' | '*' | '/' | '**'

; all scalar_expr resolved at compile time
; result folded into const opcode in emitted .hlx

; ── Terminals ─────────────────────────────────────────────

NAME            := [a-zA-Z_][a-zA-Z0-9_]*
INT             := [0-9]+
FLOAT           := [0-9]+ '.' [0-9]* | '.' [0-9]+
literal         := INT | FLOAT
NEWLINE         := '\n'
INDENT          := increased indentation level
DEDENT          := decreased indentation level
COMMENT         := '#' .* NEWLINE      ; ignored by lexer
```

### Block Validation Rules

```
init()
    ✓   param()
    ✓   submodel instantiation
    ✓   scalar declarations
    ✗   tensor ops              hard compile error
    ✗   HADX ops                hard compile error

forward()
    ✓   H-ISA ops
    ✓   HFX ops
    ✓   submodel calls
    ✗   param()                 hard compile error
    ✗   HADX ops                hard compile error
    ✗   scalar declarations     hard compile error

backward()
    ✓   HADX ops
    ✓   submodel calls
    ✗   H-ISA ops               hard compile error
    ✗   HFX ops                 hard compile error
    ✗   param()                 hard compile error
```

### Name Resolution Rules

```
self.x in forward() or backward()
    x is param      →  emit param opcode reference
    x is scalar     →  fold into const opcode
    x is submodel   →  resolve submodel call

NAME in forward()
    in forward params           →  emit input opcode reference
    in tensor_assign            →  emit result reference
    not found                   →  hard compile error

NAME in backward()
    in forward params           →  emit input opcode reference
    in backward_assign          →  emit result reference
    not found                   →  hard compile error
```

### Type Rules

```
type hint declared      →  enforced strictly
dtype mismatch          →  hard compile error
shape mismatch          →  hard compile error
type hint omitted       →  compiler infers, emits warning
```

### Include Rules

```
include "file.hds"      →  all models in scope
circular includes       →  hard compile error
model name collision    →  hard compile error
file not found          →  hard compile error
```

### Compiler Pipeline

```
.hds source
    ↓
lexer               tokens
    ↓
parser              AST
    ↓
resolver            name binding, block validation
    ↓
type checker        dtype + shape enforcement
    ↓
scalar folder       compile time scalar math → const values
    ↓
emitter             AST → H-ISA opcode stream
    ↓
HFX pass            pattern match → fuse_begin / fuse_wrap
    ↓
HADX pass           auto-diff forward graph → backward opcodes
                    (skipped if backward() user defined)
    ↓
.hlx binary
```

### Full Example

```
include "layers/norm.hds"

model Attention:
    init(heads: int, d_model: int):
        self.w_q    = param()
        self.w_k    = param()
        self.w_v    = param()
        self.w_o    = param()
        self.scale  = 1.0 / sqrt(d_model)
        self.norm   = LayerNorm()

    forward(x: tensor[float32, (batch, seq, d_model)]):
        q       = matmul(x, self.w_q)
        k       = matmul(x, self.w_k)
        v       = matmul(x, self.w_v)
        scores  = matmul(q, transpose(k))
        scores  = mul(scores, self.scale)
        scores  = softmax(scores)
        out     = matmul(scores, v)
        out     = matmul(out, self.w_o)
        out     = self.norm.forward(out)
        return out

    backward() = default


model FFN:
    init(d_model: int, d_ff: int):
        self.w1 = param()
        self.w2 = param()
        self.b1 = param()
        self.b2 = param()

    forward(x: tensor[float32, (batch, seq, d_model)]):
        x = matmul(x, self.w1) + self.b1
        x = gelu(x)
        x = matmul(x, self.w2) + self.b2
        return x

    backward() = default


model TransformerBlock:
    init(d_model: int, heads: int, d_ff: int):
        self.attn   = Attention(heads=heads, d_model=d_model)
        self.ffn    = FFN(d_model=d_model, d_ff=d_ff)
        self.norm1  = LayerNorm()
        self.norm2  = LayerNorm()

    forward(x: tensor[float32, (batch, seq, d_model)]):
        residual    = x
        x           = self.attn.forward(x)
        x           = x + residual
        x           = self.norm1.forward(x)
        residual    = x
        x           = self.ffn.forward(x)
        x           = x + residual
        x           = self.norm2.forward(x)
        return x

    backward() = default


run TransformerBlock(d_model=512, heads=8, d_ff=2048):
    forward(tokens)
    backward() = default
```

---

## 13. Error System

### Error Code Format

```
H - SSSS - EEEE

H       Helios prefix
SSSS    stage code
EEEE    error index within stage
```

### Stage Codes

```
0100    lexer
0200    parser
0300    resolver
0400    scalar folder
0500    emitter
0600    type checker
```

### Error Table

**Lexer — H-0100-xxxx**
```
H-0100-0001     unexpected character
H-0100-0002     unterminated string literal
H-0100-0003     invalid escape sequence
H-0100-0004     invalid number literal
H-0100-0005     indentation error
H-0100-0006     inconsistent indentation
H-0100-0007     unexpected end of file
```

**Parser — H-0200-xxxx**
```
H-0200-0001     expected model keyword
H-0200-0002     expected init operation
H-0200-0003     expected forward operation
H-0200-0004     expected backward operation
H-0200-0005     missing colon after block header
H-0200-0006     expected NAME got TOKEN
H-0200-0007     malformed param declaration
H-0200-0008     malformed submodel declaration
H-0200-0009     malformed scalar declaration
H-0200-0010     malformed tensor expression
H-0200-0011     malformed return statement
H-0200-0012     malformed tuple expression
H-0200-0013     malformed type hint
H-0200-0014     malformed run statement
H-0200-0015     malformed include statement
H-0200-0016     unexpected token in block
H-0200-0017     unexpected end of block
```

**Resolver — H-0300-xxxx**
```
H-0300-0001     undefined name
H-0300-0002     undefined model
H-0300-0003     undefined submodel field
H-0300-0004     undefined param
H-0300-0005     name already declared in scope
H-0300-0006     model name collision across includes
H-0300-0007     circular include detected
H-0300-0008     include file not found
H-0300-0009     param declared outside init
H-0300-0010     tensor op in init block
H-0300-0011     HADX op in forward block
H-0300-0012     H-ISA op in backward block
H-0300-0013     submodel call on non-submodel
H-0300-0014     self access outside model scope
H-0300-0015     run references undefined model
H-0300-0016     submodel depth exceeds resolution
H-0300-0017     backward default on non-differentiable op
```

**Scalar Folder — H-0400-xxxx**
```
H-0400-0001     division by zero in scalar expression
H-0400-0002     scalar overflow
H-0400-0003     scalar underflow
H-0400-0004     invalid scalar operand type
H-0400-0005     unresolved scalar reference
```

**Emitter — H-0500-xxxx**
```
H-0500-0001     unknown opcode mnemonic
H-0500-0002     opcode operand count mismatch
H-0500-0003     const pool overflow
H-0500-0004     param table overflow
H-0500-0005     opcode stream overflow
H-0500-0006     invalid opcode for block type
H-0500-0007     .hlx write failure
```

**Type Checker — H-0600-xxxx**
```
H-0600-0001     dtype mismatch
H-0600-0002     shape mismatch
H-0600-0003     rank mismatch
H-0600-0004     incompatible dtypes in binop
H-0600-0005     incompatible shapes in binop
H-0600-0006     invalid dtype for op
H-0600-0007     invalid shape for op
H-0600-0008     type hint on non-tensor
H-0600-0009     return type mismatch
H-0600-0010     tuple element type mismatch
H-0600-0011     submodel input type mismatch
H-0600-0012     unsupported dtype
```

### Warning Table

```
W-0600-0001     type hint omitted, inferring dtype
W-0600-0002     type hint omitted, inferring shape
W-0300-0001     unused param declared in init
W-0300-0002     unused submodel declared in init
W-0400-0001     scalar expression result truncated
```

Warnings never block compilation. Errors always do.

### Error Report Format

```
error H-SSSS-EEEE  [stage]
  → file.hds:line:col
  │
  │   source line text
  │   ^^^^^^^^^^^^
  │
  message
  note: optional hint
```

### Example

```
error H-0300-0009  [resolver]
  → attention.hds:12:9
  │
  │       w_q = param()
  │       ^^^
  │
  param() declared outside init() block
  note: param declarations are only valid inside init()
```
