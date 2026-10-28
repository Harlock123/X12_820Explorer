# AGENTS.md

Notes for anyone, human or AI agent, changing this code. There is no README; this file covers what the program
is, how it is built and what to watch when changing it.

## What this is

A small Windows Forms viewer for HIPAA X12 EDI files, written in VB.NET for .NET Framework 4.8. Each tab opens one
transaction type (820, 834, 835, 271, 837 and 277), splits the file into its `ST` header, its body segments and
its `SE` trailer, and shows them in text boxes, one segment per line. It builds and runs on Windows only.

## Commands

Open `X12_820Explorer.sln` in Visual Studio and build, or from a Developer Command Prompt:

```bat
msbuild X12_820Explorer.sln /p:Configuration=Release
```

Output goes to `bin\` (both configurations). There are no tests yet.

## Layout

- `X12_820Explorer.sln`: the solution. It contains only `X12_820Explorer.vbproj`.
- `X12_820Explorer.vbproj`: the real project. `WinExe`, .NET Framework 4.8, `Option Strict Off`, startup object
  `frmX12820main`.
- `frmX12820main.vb`: the whole program.
  - `InitializeComponent` lays out the tab control, the open buttons and the text boxes.
  - `DecodeFile` (820), `DecodeFile834`, `DecodeFile835`, `DecodeFile271`, `Decode837File` and `Decode277File`
    each read a file, strip CR/LF, and use `Regex.Split` on `~ST`, `~SE` and the type's key segment (`ENT`,
    `INS`, `CLP`, `HL`) to fill the text boxes.
  - The 837 and 277 decoders loop over every `ST` transaction in the file; 837 does it through `DecodeENT`.
  - The font buttons set every text box to bold Courier New between about 6 and 31 points.
- `frmX12820main.resx`, `AssemblyInfo.vb`, `app.config`: the form resources, assembly attributes and runtime
  config.
- `X12_820Explorer.csproj`, `frmX12820main.cs`, `AssemblyInfo.cs`, `My Project/*.cs`: a C# conversion of the
  form, targeting .NET Framework 2.0. It is not in the solution, and the last change to the screen layout was made
  only in the VB files, so it is out of date.
- `X12_820Explorer.sln.bak`: an older copy of the solution file.

## Rules that keep it working

- **The VB project is the one that matters.** Make changes in `frmX12820main.vb`; do not assume the `.cs` files
  match it.
- **The parsers assume one layout.** Each decoder expects `~` as the segment terminator. The 820 decoder
  shows only the first `ST` to `SE` transaction; the others treat several transactions in different ways, so try
  a multi-transaction file after changing one.
- **Designer code lives in the form file.** The form's controls are declared and laid out in `frmX12820main.vb` itself.
  If you add a text box, add it to both font-size handlers too.

## Making a change

- Build in Visual Studio on Windows and open a sample file of the affected type to check the tab still fills.
- Keep sample EDI files out of the repository; real ones carry personal health information.
