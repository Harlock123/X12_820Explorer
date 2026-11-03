# X12 Explorer

A viewer for HIPAA X12 EDI files. X12 files usually arrive as one long line of `~`-terminated segments; X12
Explorer pulls a file apart into its envelope and header, its loops (one claim, member or payment per line, or a
segment per line), and its trailer, and lists the elements of whichever segment you click.

It runs on Windows, macOS and Linux. It started as a Windows Forms program in VB.NET (kept in [`legacy/`](legacy))
and is now written in C# with [Avalonia](https://avaloniaui.net/).

![The 835 tab: the header with the payment details, a line per claim, and the BPR segment's elements](Screenshots/remittance-835.png)

## Download

Builds for Windows, macOS and Linux (Intel/AMD and ARM) are on the
[releases page](https://github.com/Harlock123/X12_820Explorer/releases). Each is a single self-contained program:
unzip or untar it and run `X12Explorer` (`X12Explorer.exe` on Windows). Nothing else needs installing.

## What it reads

| Tab | Transaction | Loops start at |
|---|---|---|
| Capitation (820) | Payment order / remittance advice, as used for capitation | `ENT` |
| Membership (834) | Benefit enrolment and maintenance | `INS` |
| Remittance (835) | Health care claim payment / advice | `CLP` |
| Benefit Info (271) | Eligibility, coverage or benefit information | `HL` |
| Claim (837) | Health care claim | `HL` |
| Claim Status Response (277) | Health care claim status | `HL` |

- **Open a file from any tab.** It goes to the tab its `ST` segment names. You can also drop a file on the window,
  or name one on the command line (`X12Explorer claims.edi`).
- **Header, loops and trailer.** The header box shows the `ISA` and `GS` envelope, `ST`, and everything before the
  first loop (`BPR`, `TRN`, the `N1` parties and so on). The trailer shows `SE`, `GE` and `IEA`. Drag the splitters
  to give a box more room.
- **One line per loop, or one segment per line.** Each loop starts out on one line on the 820, 834, 835 and 271
  tabs. On the 837 and 277 tabs, each segment starts out on its own line, indented under its `HL`. A tick box
  switches between the two.
- **Every transaction in the file.** When a file holds several `ST`…`SE` transactions, a list picks one, or shows
  them all together.
- **Elements.** Click a segment in any box and the strip below lists its elements by name: `CLP01 CLAIM0001 ·
  CLP02 1 · CLP03 200.00 …`.
- **Any delimiters.** The segment terminator and the element and component separators come from the `ISA`
  segment, so files using `|`, `^`, `'` or line breaks read the same as `~` and `*`. Line breaks between segments
  are ignored, and so are segments that end with a bare line feed. A file without an `ISA` is read with `~` and
  `*`.
- **Text size.** The A− and A+ buttons (Ctrl+− and Ctrl+=) change the text size in every box.

![The 837 tab showing the second of two claims, a segment per line](Screenshots/claim-837.png)

## Trying it

[`tests/X12Explorer.Tests/Samples`](tests/X12Explorer.Tests/Samples) has a small made-up file for each
transaction. The names, IDs and amounts in them are invented. Don't add real files to the repository: they carry
personal health information.

## Building

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet build X12Explorer.slnx
dotnet run --project src/X12Explorer [file]
dotnet test X12Explorer.slnx
```

## Project structure

- `src/X12Explorer.Core`: reading files, with no user interface code:
  - `Delimiters`: reads the delimiters from the `ISA` segment.
  - `X12File`: splits a file into segments and `ST`…`SE` transactions, each with its envelope.
  - `Views`: the text of the three boxes.
  - `Elements`: finds the segment under the caret and lists its elements.
  - `TransactionKind`: the six tabs.
- `src/X12Explorer`: the window (`MainWindow`) and one `TransactionPage` per tab, built in code.
- `tests/X12Explorer.Tests`: xUnit tests for the reader, plus headless window tests. Set `X12_SHOTS` to a folder to
  save pictures of the window; the screenshots here come from them.
- `legacy/`: the original Windows Forms program (VB.NET, .NET Framework 4.8), for reference.

## Licence

MIT. See [LICENSE](LICENSE).
