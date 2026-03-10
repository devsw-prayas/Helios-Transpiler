# Helios-DLX Transpiler
## Internal Tool Design Document

---

# 1. Overview

The Helios-DLX Transpiler is a standalone internal desktop application for
authoring, compiling, and inspecting Helios-DLX IR. It provides a full
project-based development environment for HDS source files and produces
compiled .hlx binaries.

It is a proprietary companion tool to the Helios-DLX C++ engine. It is not
the production compiler — that lives inside Helios-DLX itself. The transpiler
is a development and authoring tool.

```
Helios-DLX (C++)                Helios-DLX Transpiler (C# / WPF)
────────────────────            ────────────────────────────────
production compiler             internal development tool
embedded in runtime             standalone desktop application
optimized, battle-hardened      full pipeline, developer focused
consumed by end users           used by the Helios-DLX team
```

---

# 2. Technology Stack

```
Language        C# (.NET 8)
UI Framework    WPF
Editor          AvalonEdit
XML             System.Xml.Linq (XDocument)
Project files   .hdxproj (XML)
Source files    .hds
Output files    .hlx
```

No external NuGet dependencies beyond AvalonEdit.

---

# 3. Project System

## Project File Format (.hdxproj)

```xml
<?xml version="1.0" encoding="utf-8"?>
<HeliosProject version="1.0">

    <Metadata>
        <Name>Transformer</Name>
        <Author>StormWeaver</Author>
        <Created>2026-03-10</Created>
        <Modified>2026-03-10</Modified>
    </Metadata>

    <Compiler>
        <EntryPoint>src/transformer.hds</EntryPoint>
        <OutputPath>out/transformer.hlx</OutputPath>
        <OptimizationLevel>2</OptimizationLevel>
        <EmitDebug>true</EmitDebug>
    </Compiler>

    <Sources>
        <File path="src/attention.hds" />
        <File path="src/ffn.hds" />
        <File path="src/norm.hds" />
        <File path="src/transformer.hds" />
    </Sources>

</HeliosProject>
```

## Optimization Levels

```
0       base H-ISA only             no passes
1       base H-ISA + HADX           autodiff only
2       HFX-I + HADX                local fusion
3       HFX-I + HFX-II + HADX       kernel fusion
4       HFX-I + II + III + HADX     full fusion
```

## Project Compilation

- Whole project compiles as a unit — all .hds files together
- Includes resolved globally across the project
- Single .hlx output per project
- Entry point is whichever .hds contains the run statement
- Compiler resolves dependency order automatically from include graph

---

# 4. Application Architecture

## Solution Structure

```
HeliosDLX.Transpiler.sln
├── HeliosDLX.Transpiler          main WPF application
│   ├── App.xaml
│   ├── MainWindow.xaml
│   ├── Views/
│   │   ├── EditorView.xaml
│   │   ├── ProjectView.xaml
│   │   ├── IRView.xaml
│   │   └── OutputView.xaml
│   ├── ViewModels/
│   │   ├── MainViewModel.cs
│   │   ├── EditorViewModel.cs
│   │   ├── ProjectViewModel.cs
│   │   ├── IRViewModel.cs
│   │   └── OutputViewModel.cs
│   ├── Models/
│   │   ├── HdxProject.cs
│   │   ├── HdsFile.cs
│   │   └── CompilerResult.cs
│   ├── Themes/
│   │   └── *.xshd
│   └── Resources/
│       └── HDS.xshd              base HDS language definition
│
├── HeliosDLX.Compiler            C# compiler pipeline
│   ├── Lexer/
│   │   ├── Lexer.cs
│   │   └── Token.cs
│   ├── Parser/
│   │   ├── Parser.cs
│   │   └── AST/
│   │       ├── AstNode.cs
│   │       ├── ModelNode.cs
│   │       ├── InitNode.cs
│   │       ├── ForwardNode.cs
│   │       ├── BackwardNode.cs
│   │       └── ExprNode.cs
│   ├── Resolver/
│   │   └── Resolver.cs
│   ├── TypeChecker/
│   │   └── TypeChecker.cs
│   ├── ScalarFolder/
│   │   └── ScalarFolder.cs
│   ├── Emitter/
│   │   ├── Emitter.cs
│   │   └── HlxWriter.cs
│   ├── Passes/
│   │   ├── HfxPass.cs
│   │   └── HadxPass.cs
│   └── Diagnostics/
│       ├── CompilerError.cs
│       └── ErrorCodes.cs
│
└── HeliosDLX.IR                  IR model and opcode definitions
    ├── Opcodes/
    │   ├── Opcode.cs
    │   ├── OpcodeTable.cs
    │   └── HlxBinary.cs
    └── Model/
        ├── IrInstruction.cs
        ├── IrOperand.cs
        └── IrModule.cs
```

## MVVM Pattern

The application follows strict MVVM. Views contain no logic — all state
and commands live in ViewModels. The compiler pipeline lives entirely in
HeliosDLX.Compiler, with no WPF dependencies.

---

# 5. UI Layout

## Main Window

```
┌──────────────────────────────────────────────────────────────────┐
│  File   Project   View   Build   Themes                [─][□][×] │
├───────────────┬──────────────────────────┬───────────────────────┤
│               │  [tab] attn.hds  [tab]   │                       │
│  PROJECT      │  ffn.hds ×               │  IR OUTPUT            │
│  ───────      ├──────────────────────────┤  ─────────            │
│  ▼ src        │                          │                       │
│    attn.hds   │   AvalonEdit             │  mnemonic / hex       │
│    ffn.hds    │   code editor            │  view of compiled     │
│    norm.hds   │                          │  .hlx opcodes         │
│    trans.hds  │                          │                       │
│               │                          │                       │
│  ▼ out        │                          │                       │
│    trans.hlx  │                          │                       │
│               │                          │                       │
├───────────────┴──────────────────────────┴───────────────────────┤
│  OUTPUT / ERRORS                                          [^][v] │
│  > Build succeeded — transformer.hlx  291 ops  1.2 KB           │
│  > warning W-0600-0001  ffn.hds:8:12  type hint omitted         │
└──────────────────────────────────────────────────────────────────┘
```

All three panels are resizable via WPF GridSplitter.
The output panel is collapsible.
The window has a fixed minimum size but is otherwise freely resizable.

## Panels

### Project Panel (left)
- TreeView of project structure
- Source files (.hds) and output files (.hlx)
- Right-click context menu: New File, Rename, Remove, Open in Explorer
- Double-click opens file in editor

### Editor Panel (center)
- AvalonEdit instance per open file
- Tab strip across the top
- Tabs show filename, dirty indicator (•) when unsaved
- Middle-click or × button closes tab
- Ctrl+Tab cycles tabs

### IR Output Panel (right)
- Read-only AvalonEdit view
- Shows compiled opcode stream for the active .hds file
- Toggle between views: Mnemonic | Hex | Both
- Updates on successful build
- Scrolls to opcode corresponding to cursor position in editor (if EmitDebug=true)

### Output Panel (bottom)
- Build output, warnings, errors
- Errors are clickable — jumps to file:line:col in editor
- Color coded: errors red, warnings yellow, info white
- Collapsible with keyboard shortcut

---

# 6. Editor Features

## AvalonEdit Configuration

```
Line numbers            enabled
Code folding            model blocks, init/forward/backward ops
Syntax highlighting     HDS .xshd definition
Word wrap               off by default, toggleable
Tab size                4 spaces
Show whitespace         off by default, toggleable
Bracket matching        enabled
Current line highlight  enabled
```

## HDS Syntax Highlighting (.xshd)

Token categories for the HDS language definition:

```
Keywords        model init forward backward return run include self default
Types           tensor float32 float16 bfloat16 float64 int8 int16 int32 int64 tf32
Builtins        param input const tuple
Ops             matmul add sub mul div relu gelu sigmoid softmax layernorm
                rmsnorm batchnorm conv2d ... (all H-ISA mnemonics)
ScalarFuncs     sqrt log exp abs floor ceil
Operators       + - * / @ ** = : . , ( ) [ ]
Literals        integers, floats
Comments        # line comments
Strings         "file.hds" in include statements
```

## Code Folding

Foldable regions:

```
model ModelName:
    [foldable]

    init(...):
        [foldable]

    forward(...):
        [foldable]

    backward():
        [foldable]
```

## Error Markers

Compiler errors displayed as:
- Red squiggle underline at error location in editor
- Red marker in the scroll bar gutter
- Entry in the output panel (clickable)
- Tooltip on hover showing error code + message

## Autocomplete

CompletionWindow triggered on:
- Any letter key — suggests op names, keywords, known model names
- `self.` — suggests declared params and submodels
- `tensor[` — suggests dtypes

---

# 7. Themes

Themes are embedded .xshd files compiled as assembly resources.
Each theme is a complete AvalonEdit syntax highlighting definition
covering the HDS token set.

Theme selection is stored in user settings and persists across sessions.

## Theme List

Sourced from the StormWeaver portfolio. Embedded at build time.
Available via View → Themes submenu.

## Applying a Theme

```csharp
var stream = Assembly.GetExecutingAssembly()
    .GetManifestResourceStream($"HeliosDLX.Transpiler.Themes.{themeName}.xshd");
using var reader = new XmlTextReader(stream);
editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
```

---

# 8. Keyboard Shortcuts

```
File
  Ctrl+N              new file
  Ctrl+O              open project
  Ctrl+S              save current file
  Ctrl+Shift+S        save all
  Ctrl+W              close current tab

Build
  F5                  build project
  Ctrl+B              build project
  Escape              cancel build

Editor
  Ctrl+Z              undo
  Ctrl+Y              redo
  Ctrl+F              find
  Ctrl+H              find and replace
  Ctrl+G              go to line
  Ctrl+/              toggle line comment
  Ctrl+D              duplicate line
  Alt+Up/Down         move line up/down
  Ctrl+Tab            next tab
  Ctrl+Shift+Tab      previous tab

View
  Ctrl+`              toggle output panel
  Ctrl+\              toggle project panel
  Ctrl+Shift+I        toggle IR panel
  Ctrl+=              zoom in
  Ctrl+-              zoom out
  Ctrl+0              reset zoom
```

---

# 9. Compiler Pipeline (C# Implementation)

The C# compiler pipeline mirrors the spec defined in the Helios-DLX
specification. It is an independent implementation — not a wrapper around
the C++ compiler.

```
ProjectCompiler.Compile(HdxProject project)
    │
    ├── 1. load all .hds source files
    ├── 2. resolve include graph, determine compile order
    ├── 3. per file: Lexer → token stream
    ├── 4. per file: Parser → AST
    ├── 5. global: Resolver → name binding, block validation
    ├── 6. global: TypeChecker → dtype + shape enforcement
    ├── 7. global: ScalarFolder → compile time scalar evaluation
    ├── 8. global: Emitter → H-ISA opcode stream
    ├── 9. if OptimizationLevel >= 2: HfxPass
    ├── 10. if backward present: HadxPass
    └── 11. HlxWriter → .hlx binary
```

## Error Collection

Errors are collected across all stages and reported together.
The compiler does not stop at the first error — it collects as many
errors as possible before aborting, so the user sees all issues at once.

Fatal errors (e.g. circular includes, .hlx write failure) abort immediately.

## CompilerResult

```csharp
public class CompilerResult
{
    public bool Success { get; }
    public List<CompilerError> Errors { get; }
    public List<CompilerWarning> Warnings { get; }
    public IrModule Module { get; }       // null on failure
    public byte[] HlxBytes { get; }       // null on failure
    public TimeSpan CompileTime { get; }
    public int OpCount { get; }
}

public class CompilerError
{
    public string Code { get; }           // e.g. "H-0300-0009"
    public string Stage { get; }          // e.g. "resolver"
    public string File { get; }
    public int Line { get; }
    public int Column { get; }
    public string Message { get; }
    public string Note { get; }           // optional hint
}
```

---

# 10. IR Output View

The IR panel shows the compiled opcode stream for the currently active file.

## Display Modes

### Mnemonic View
```
00  input           494f544110000000    dst
01  param           494f544110000001    dst
02  param           494f544110000001    dst
03  matmul          494f5441100a0201    dst, a, b
04  add             494f544110040200    dst, a, b
05  gelu            494f544110060106    dst, a
```

### Hex View
```
00  494f544110000000
01  494f544110000001
02  494f544110000001
03  494f5441100a0201
04  494f544110040200
05  494f544110060106
```

### Both
```
00  input     494f544110000000    dst
01  param     494f544110000001    dst
...
```

## Source Correlation

When EmitDebug=true, clicking an opcode in the IR view highlights the
corresponding source line in the editor. Clicking a source line highlights
the corresponding opcode(s) in the IR view.

## Fusion Regions

Fused regions are visually grouped with a colored bracket and labeled
with the HFX tier and fusion op name:

```
     ╔══ HFX-II: matmul_add_gelu ══════════════╗
03   ║  matmul   494f5441100a0201   dst, a, b   ║
04   ║  add      494f544110040200   dst, a, b   ║
05   ║  gelu     494f544110060106   dst, a      ║
     ╚══════════════════════════════════════════╝
```

---

# 11. Build Output Format

```
────────────────────────────────────────────
  Building Transformer  -O2  [debug]
────────────────────────────────────────────
  > lexing        4 files
  > parsing       4 files
  > resolving     ok
  > type check    ok
  > scalar fold   ok
  > emitting      139 ops
  > HFX pass      12 fusions applied (HFX-I: 8, HFX-II: 4)
  > writing       out/transformer.hlx

  Build succeeded
  291 ops · 12 fusions · 2.3 KB · 48ms

────────────────────────────────────────────
```

On failure:

```
────────────────────────────────────────────
  Building Transformer  -O2  [debug]
────────────────────────────────────────────
  > lexing        4 files
  > parsing       4 files
  > resolving     2 errors

  error H-0300-0009  [resolver]
    → src/attention.hds:12:9
    │
    │       w_q = param()
    │       ^^^
    │
    param() declared outside init() block
    note: param declarations are only valid inside init()

  error H-0300-0001  [resolver]
    → src/ffn.hds:24:13
    │
    │       y = matmul(x, self.w3)
    │                      ^^^^
    │
    undefined name: w3
    note: did you mean w2?

  Build failed — 2 errors
────────────────────────────────────────────
```

---

# 12. Settings

Persisted in user settings (per-machine):

```
Theme               last selected theme name
FontSize            editor font size (default 13)
FontFamily          editor font (default Consolas)
WordWrap            bool (default false)
ShowWhitespace      bool (default false)
TabSize             int (default 4)
OutputPanelHeight   last panel height
ProjectPanelWidth   last panel width
IRPanelWidth        last panel width
RecentProjects      list of last 10 opened .hdxproj paths
IRDisplayMode       Mnemonic | Hex | Both
```

---

# 13. Phased Implementation Plan

## Phase 1 — Shell
```
MainWindow layout       three panels + output bar
Project panel           TreeView, open/close project
Tab system              open/close tabs, dirty state
AvalonEdit integration  basic editor, no highlighting yet
.hdxproj loading        XDocument parse, file tree population
```

## Phase 2 — Editor
```
HDS .xshd definition    syntax highlighting
Code folding            model / init / forward / backward
Themes                  embed .xshd files, theme switcher
Keyboard shortcuts      full shortcut map
Autocomplete            keyword + op name suggestions
```

## Phase 3 — Compiler
```
Lexer                   full HDS token set
Parser                  AST construction
Resolver                name binding, block validation
Type checker            dtype + shape enforcement
Scalar folder           compile time evaluation
Emitter                 H-ISA opcode stream → .hlx
Build command           F5 triggers full pipeline
Error display           squiggles + output panel entries
```

## Phase 4 — IR View
```
IR output panel         mnemonic / hex / both modes
Source correlation      click opcode → highlight source
Build output            formatted build log
```

## Phase 5 — Passes
```
HFX pass                fusion pattern matching
HADX pass               autodiff graph emission
Fusion visualization    colored brackets in IR view
Optimization levels     -O0 through -O4
```
