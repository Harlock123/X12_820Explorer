# AGENTS.md

Notes for anyone, human or AI agent, changing this code. `README.md` covers what the program does; this file covers
how the code is put together and the rules that keep it working.

## What this is

A viewer for HIPAA X12 EDI files in C# on .NET 10 and Avalonia 12. It has one tab per transaction: 820, 834, 835,
271, 837 and 277. Each tab shows the file's header (envelope, ST, and everything before the first loop), its loops,
and its trailer (SE, GE, IEA), plus the elements of the segment under the caret. It runs on Windows, macOS and
Linux. The original Windows Forms program is in `legacy/` and is no longer built.

## Commands

```bash
dotnet build X12Explorer.slnx
dotnet test X12Explorer.slnx
dotnet run --project src/X12Explorer [file]
X12_SHOTS=/tmp/shots dotnet test X12Explorer.slnx    # also saves pictures of the window, headless
```

## Layout

- `Directory.Build.props`: net10.0, nullable, warnings as errors, for everything under `src/` and `tests/`.
  `legacy/` has its own empty one so the old project is left alone.
- `src/X12Explorer.Core/`: reading files, with no Avalonia code.
  - `Delimiters.cs`: the segment terminator and the element and component separators. `Detect` reads them from
    the ISA segment by counting separators, so ISA fields that weren't padded still read. Without an ISA it
    falls back to `~ * :`.
  - `Segment.cs`: a segment's text, its elements (`[1]` is the first after the ID) and its position in the file.
  - `X12File.cs`:
    - `X12File.Parse` splits the text into segments and groups them into `Transaction`s.
    - Each transaction keeps its ISA and GS, the GE and IEA that close them, and any stray segments before its
      ST (`Before`).
    - `Transaction.SplitAt(loopStart)` gives the header segments and the loops.
  - `Views.cs`: the text of the three boxes for one transaction or several, a loop per line or a segment per line.
  - `Elements.cs`: the segment under a caret in a box's text, and its elements listed as `CLP01 value`.
  - `TransactionKind.cs`: the six tabs. Each has its ST01 code, its name, its loop-start segment, and whether it
    starts with a segment per line.
- `src/X12Explorer/`: the window, built in code with no XAML.
  - `MainWindow.cs`:
    - The tool bar (Open, text size), the tabs and the status line.
    - `Open(path)` sends a file to the tab its ST01 names.
    - Drag and drop, and the Ctrl+O, Ctrl+= and Ctrl+− keys.
  - `TransactionPage.cs`: one tab. It holds the file name, the transaction choice, the "One segment per line"
    tick box, the three boxes with splitters, and the element strip.
  - `Look.cs`: colours, the monospace font, and the text-size limits.
- `tests/X12Explorer.Tests/`: xUnit v3.
  - `ParserTests` covers delimiters, splitting, transactions, views and elements.
  - `WindowTests` drives the window on Avalonia.Headless.
  - `Samples/*.edi`: made-up files, one per transaction.
- `Screenshots/`: the README's pictures, from `X12_SHOTS`.

## Rules that keep it working

- **Core stays free of Avalonia.** Parsing and the box text belong in `X12Explorer.Core`, where `ParserTests` can
  check them without a window. The window only puts text in boxes.
- **Never assume `~` and `*`.** Take delimiters from `X12File.Delimiters`. Line breaks around segments are not
  data. A bare line feed ends a segment too, because some senders write files that way.
- **Show every transaction.** The old program showed only the first `ST` of an 820. Here every transaction is
  read, and the tab's list chooses between them.
- **Adding a transaction type** means:
  - a `TransactionKind` in `All`, which gives it a tab automatically;
  - a made-up sample in `tests/X12Explorer.Tests/Samples`;
  - a row in `ParserTests.Reads_each_sample` and in `WindowTests.Each_sample_opens_on_its_own_tab`;
  - a row in the README's table.
- **No real files in the repository.** Real X12 files carry personal health information. Samples must use made-up
  names, member IDs and amounts.
- **`legacy/` is for reference.** Don't change it; port the behaviour into `src/` instead.

## Making a change

- Run `dotnet test` after any change.
- After a layout change, run with `X12_SHOTS` and look at the pictures. Refresh `Screenshots/` when they change.
- Pushes to the default branch (`master`) publish builds to the rolling `latest` release; a `v*` tag makes a proper
  release.
