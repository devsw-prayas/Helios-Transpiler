# HDS — Helios DSL Grammar Specification

---

## Overview

HDS is the frontend language for Helios-DLX. Every construct in HDS maps
directly to one or more H-ISA, HFX, or HADX opcodes. If the ISA cannot
express it, the DSL cannot do it.

### Value Kinds

```
tensor      runtime value       param, input, op output     → opcodes
scalar      compile time        init args, op attributes    → folded at compile time
submodel    compile time        nested model instances      → resolved at compile time
```

### Dtype System

```
float32     float16     bfloat16    float64
int8        int16       int32       int64
tf32
```

Type hints are mandatory when declared and enforced strictly.
A dtype mismatch is a hard compile error.

### Shape System

All shapes are static — fully known at compile time.
Shapes are expressed as integer tuples: `(batch, seq, d_model)`.
A shape mismatch is a hard compile error.

---

## Grammar

```
; ─────────────────────────────────────────────────────────
; Notation: := definition
;            |  alternation
;            *  zero or more
;            +  one or more
;            ?  optional
;            () grouping
; ─────────────────────────────────────────────────────────


; ── Top Level ─────────────────────────────────────────────

program         := include_stmt*
                   model_def+
                   run_stmt?

; ── Includes ──────────────────────────────────────────────

include_stmt    := 'include' STRING NEWLINE
STRING          := '"' [a-zA-Z0-9_./]+ '.hds' '"'

; e.g.
;   include "attention.hds"
;   include "layers/ffn.hds"


; ── Model Definition ──────────────────────────────────────

model_def       := 'model' NAME ':' NEWLINE INDENT
                       init_op
                       forward_op
                       backward_op
                   DEDENT

; all three ops are mandatory
; omitting any one is a hard compile error


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
submodel_arg    := NAME '=' scalar_expr
                 | scalar_expr

scalar_decl     := 'self' '.' NAME '=' scalar_expr NEWLINE

; e.g.
;   model Attention:
;       init(heads: int, d_model: int):
;           self.w_q    = param()
;           self.w_k    = param()
;           self.scale  = 1.0 / sqrt(d_model)
;           self.norm   = LayerNorm()


; ── Forward Operation ─────────────────────────────────────

forward_op      := 'forward' '(' forward_params? ')' ':' NEWLINE INDENT
                       forward_stmt+
                   DEDENT

forward_params  := forward_param (',' forward_param)*
forward_param   := NAME (':' type_hint)?

forward_stmt    := tensor_assign
                 | return_stmt

tensor_assign   := NAME '=' tensor_expr NEWLINE

return_stmt     := 'return' return_expr NEWLINE
return_expr     := tuple_expr
                 | tensor_expr

tuple_expr      := 'tuple' '(' tensor_expr (',' tensor_expr)+ ')'

; e.g.
;   forward(x: tensor[float32, (batch, seq, d_model)]):
;       q = matmul(x, self.w_q)
;       return tuple(q, scores)


; ── Backward Operation ────────────────────────────────────

backward_op     := 'backward' '(' ')' '=' 'default' NEWLINE
                 | 'backward' '(' ')' ':' NEWLINE INDENT
                       backward_stmt+
                   DEDENT

backward_stmt   := tensor_assign
                 | return_stmt

; backward() = default
;     HADX pass auto-differentiates from forward()
;
; backward():
;     only HADX opcodes permitted
;     H-ISA ops in backward() are a hard compile error


; ── Run Entry Point ───────────────────────────────────────

run_stmt        := 'run' NAME '(' submodel_args? ')' ':' NEWLINE INDENT
                       run_forward
                       run_backward?
                   DEDENT

run_forward     := 'forward' '(' tensor_expr (',' tensor_expr)* ')' NEWLINE
run_backward    := 'backward' '(' ')' '=' 'default' NEWLINE
                 | 'backward' '(' ')' NEWLINE

; e.g.
;   run Transformer(layers=12, d_model=512):
;       forward(tokens)
;       backward() = default


; ── Type Hints ────────────────────────────────────────────

type_hint       := 'tensor' '[' DTYPE ',' shape ']'
shape           := '(' dim (',' dim)* ')'
dim             := NAME        ; named dimension e.g. batch, seq
                 | INT         ; fixed dimension e.g. 512

DTYPE           := 'float32' | 'float16' | 'bfloat16' | 'float64'
                 | 'int8'    | 'int16'   | 'int32'    | 'int64'
                 | 'tf32'

; e.g.
;   x: tensor[float32, (batch, seq, 512)]
;   w: tensor[float16, (512, 2048)]


; ── Tensor Expressions ────────────────────────────────────

tensor_expr     := tensor_call
                 | tensor_binop
                 | tensor_unop
                 | submodel_call
                 | self_access
                 | NAME

tensor_call     := NAME '(' tensor_args? ')'
tensor_args     := tensor_arg (',' tensor_arg)*
tensor_arg      := tensor_expr
                 | kwarg

submodel_call   := self_path '.' NAME '(' tensor_args? ')'
self_path       := 'self' '.' NAME ('.' NAME)*

self_access     := 'self' '.' NAME ('.' NAME)*

tensor_binop    := tensor_expr TENSOR_OP tensor_expr
tensor_unop     := '-' tensor_expr

TENSOR_OP       := '+' | '-' | '*' | '/' | '@'

kwarg           := NAME '=' scalar_expr
                 | NAME '=' tensor_expr

; tensor binop desugaring
;   a + b   →  add(a, b)
;   a - b   →  sub(a, b)
;   a * b   →  mul(a, b)
;   a / b   →  div(a, b)
;   a @ b   →  matmul(a, b)
;   -a      →  neg(a)


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
FLOAT           := [0-9]+ '.' [0-9]*
               |  '.' [0-9]+
literal         := INT | FLOAT
NEWLINE         := '\n'
INDENT          := increased indentation level
DEDENT          := decreased indentation level
COMMENT         := '#' .* NEWLINE   ; ignored by lexer
```

---

## Validation Rules

### Block Restrictions

```
init()
    ✓   param()
    ✓   submodel instantiation
    ✓   scalar declarations
    ✗   tensor ops          hard compile error
    ✗   HADX ops            hard compile error

forward()
    ✓   H-ISA ops
    ✓   HFX ops
    ✓   submodel calls
    ✗   param()             hard compile error
    ✗   HADX ops            hard compile error
    ✗   scalar declarations hard compile error

backward()
    ✓   HADX ops
    ✓   submodel calls
    ✗   H-ISA ops           hard compile error
    ✗   HFX ops             hard compile error
    ✗   param()             hard compile error
```

### Name Resolution Rules

```
self.x in forward() or backward()
    x is param      →  emit param opcode reference
    x is scalar     →  fold into const opcode
    x is submodel   →  resolve submodel call

NAME in forward()
    declared in forward params      →  emit input opcode reference
    declared in tensor_assign       →  emit result reference
    not found                       →  hard compile error

NAME in backward()
    declared in forward params      →  emit input opcode reference
    declared in backward_assign     →  emit result reference
    not found                       →  hard compile error
```

### Type Rules

```
type hint declared      →  enforced strictly
dtype mismatch          →  hard compile error
shape mismatch          →  hard compile error
type hint omitted       →  compiler infers, no error
```

### Include Rules

```
include "file.hds"      →  all models in file.hds available in scope
circular includes       →  hard compile error
model name collision    →  hard compile error
```

---

## Compiler Pipeline

```
.hds source
    ↓
lexer               tokens (NAME, INDENT, DEDENT, OP, LITERAL, DTYPE, KEYWORD)
    ↓
parser              AST
    ↓
resolver            name binding, type checking, block validation
    ↓
scalar folder       compile time scalar math → const values
    ↓
emitter             AST → H-ISA opcode stream
    ↓
HFX pass            pattern match → insert fuse_begin / fuse_wrap
    ↓
HADX pass           auto-diff forward graph → emit backward opcodes
    (skipped if backward() = user defined)
    ↓
.hlx binary         sections: const pool | metadata | opcode stream
                              | fusion table | grad table
```

---

## Full Example

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
