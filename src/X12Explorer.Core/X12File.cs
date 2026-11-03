namespace X12Explorer.Core;

/// <summary>
/// An X12 file read into its transactions. Each ST to SE transaction remembers the ISA and GS that enclose it and
/// the GE and IEA that close them, so its header and trailer can show the envelope too.
/// </summary>
public sealed class X12File
{
    private X12File(Delimiters delimiters, List<Segment> segments, List<Transaction> transactions, List<Segment> outside)
    {
        Delimiters = delimiters;
        Segments = segments;
        Transactions = transactions;
        Outside = outside;
    }

    public Delimiters Delimiters { get; }

    /// <summary>Every segment, in file order.</summary>
    public IReadOnlyList<Segment> Segments { get; }

    public IReadOnlyList<Transaction> Transactions { get; }

    /// <summary>Segments after the last transaction that aren't part of its envelope (GE, IEA).</summary>
    public IReadOnlyList<Segment> Outside { get; }

    /// <summary>
    /// The ST01 code most of the file's transactions share (the first one's when they tie), or null when the file
    /// has no ST segment.
    /// </summary>
    public string? Code => Transactions.Count == 0
        ? null
        : Transactions.GroupBy(t => t.Code).OrderByDescending(g => g.Count()).First().Key;

    public static X12File Load(string path) => Parse(File.ReadAllText(path));

    public static X12File Parse(string text)
    {
        var delimiters = Delimiters.Detect(text);
        var segments = Split(text, delimiters).Select((s, i) => new Segment(s, delimiters, i)).ToList();

        var transactions = new List<Transaction>();
        var loose = new List<Segment>();
        Segment? isa = null, gs = null;
        Transaction? open = null;

        foreach (var segment in segments)
        {
            switch (segment.Id)
            {
                case "ISA" when open == null:
                    isa = segment;
                    gs = null;
                    loose.Clear();
                    break;
                case "GS" when open == null:
                    gs = segment;
                    break;
                case "ST":
                    open?.Close(null);
                    open = new Transaction(segment, isa, gs, [.. loose]);
                    transactions.Add(open);
                    loose.Clear();
                    break;
                case "SE" when open != null:
                    open.Close(segment);
                    open = null;
                    break;
                case "GE" when open == null:
                    foreach (var t in transactions.Where(t => t.Group == gs && t.GroupEnd == null)) t.GroupEnd = segment;
                    gs = null;
                    break;
                case "IEA" when open == null:
                    foreach (var t in transactions.Where(t => t.Interchange == isa && t.InterchangeEnd == null)) t.InterchangeEnd = segment;
                    isa = null;
                    break;
                default:
                    if (open != null) open.Add(segment);
                    else loose.Add(segment);
                    break;
            }
        }
        open?.Close(null);

        return new X12File(delimiters, segments, transactions, loose);
    }

    /// <summary>
    /// Splits the text into segment texts. Line breaks around segments are dropped; a line break inside what should
    /// be one segment ends it too, which reads the files that end some segments with a bare line feed instead of the
    /// terminator (the old program's 837 tab allowed for these).
    /// </summary>
    public static IEnumerable<string> Split(string text, Delimiters delimiters)
    {
        var pieces = delimiters.LineTerminated
            ? text.Split(['\r', '\n'])
            : text.Split(delimiters.Segment).SelectMany(p => p.Split(['\r', '\n']));
        return pieces.Select(p => p.Trim('﻿', ' ', '\t')).Where(p => p.Length > 0);
    }
}

/// <summary>
/// One ST to SE transaction, with its header (ST up to the first loop), its loops, and its trailer (SE). A
/// transaction cut off before its SE has no <see cref="Trailer"/>.
/// </summary>
public sealed class Transaction
{
    private readonly List<Segment> _body = [];

    internal Transaction(Segment st, Segment? interchange, Segment? group, List<Segment> before)
    {
        Start = st;
        Interchange = interchange;
        Group = group;
        Before = before;
    }

    /// <summary>The ST segment.</summary>
    public Segment Start { get; }

    /// <summary>ST01: 820, 834, 835, 270, 271, 837, 276, 277, 999 and so on.</summary>
    public string Code => Start[1].Trim();

    /// <summary>ST02, the transaction set control number.</summary>
    public string Control => Start[2].Trim();

    public TransactionKind? Kind => TransactionKind.ForCode(Code);

    public Segment? Interchange { get; }
    public Segment? Group { get; }
    public Segment? GroupEnd { get; internal set; }
    public Segment? InterchangeEnd { get; internal set; }

    /// <summary>Segments between the envelope and ST that belong to neither (some senders put their own lines there).</summary>
    public IReadOnlyList<Segment> Before { get; }

    /// <summary>Everything between ST and SE.</summary>
    public IReadOnlyList<Segment> Body => _body;

    /// <summary>The SE segment, or null when the file ends first.</summary>
    public Segment? Trailer { get; private set; }

    internal void Add(Segment segment) => _body.Add(segment);

    internal void Close(Segment? se) => Trailer = se;

    /// <summary>
    /// The body split at each <paramref name="loopStart"/> segment: the segments before the first one are the
    /// header's, and each loop runs from its start segment up to the next. A <paramref name="closing"/> segment
    /// ends the loops; it and everything after it are returned as the closing segments.
    /// </summary>
    public (IReadOnlyList<Segment> Header, IReadOnlyList<IReadOnlyList<Segment>> Loops, IReadOnlyList<Segment> Closing)
        SplitAt(string loopStart, string? closing = null)
    {
        var header = new List<Segment>();
        var loops = new List<IReadOnlyList<Segment>>();
        var end = new List<Segment>();
        List<Segment>? loop = null;
        foreach (var segment in _body)
        {
            if (end.Count > 0 || (closing != null && segment.Id == closing)) end.Add(segment);
            else if (segment.Id == loopStart)
            {
                loop = [segment];
                loops.Add(loop);
            }
            else if (loop != null) loop.Add(segment);
            else header.Add(segment);
        }
        return (header, loops, end);
    }
}
