using System.Text;

namespace X12Explorer.Core;

/// <summary>
/// The three boxes a tab shows for a transaction, as text: the header (envelope, ST and everything before the first
/// loop), the loops, and the trailer (SE and the envelope's closing segments). Each segment keeps its terminator, as
/// in the file, so a box can be copied back out as X12.
/// </summary>
public sealed record Views(string Header, string Loops, string Trailer, int LoopCount)
{
    /// <summary>The indent for a loop's segments after the first, when loops are shown a segment per line.</summary>
    public const string Indent = "    ";

    /// <summary>
    /// The views of <paramref name="transactions"/> (one, or all of a file's), split into loops at
    /// <paramref name="loopStart"/>. Expanded puts each segment of a loop on its own line, indented under the loop's
    /// first; otherwise each loop is one line. Several transactions are shown one after another, a blank line apart.
    /// </summary>
    public static Views Of(IEnumerable<Transaction> transactions, Delimiters delimiters, string loopStart, bool expanded)
    {
        var end = delimiters.Shown;
        string Line(Segment s) => s.Text + end;

        var header = new StringBuilder();
        var loops = new StringBuilder();
        var trailer = new StringBuilder();
        var count = 0;
        Segment? lastInterchange = null, lastGroup = null;

        foreach (var t in transactions)
        {
            Gap(header);
            Gap(loops);
            Gap(trailer);

            // The envelope once per interchange and group, not again for every transaction inside it.
            foreach (var s in t.Before) header.AppendLine(Line(s));
            if (t.Interchange is { } isa && isa != lastInterchange) header.AppendLine(Line(isa));
            if (t.Group is { } gs && gs != lastGroup) header.AppendLine(Line(gs));
            lastInterchange = t.Interchange;
            lastGroup = t.Group;

            var (head, body) = t.SplitAt(loopStart);
            header.AppendLine(Line(t.Start));
            foreach (var s in head) header.AppendLine(Line(s));

            foreach (var loop in body)
            {
                count++;
                if (expanded)
                {
                    loops.AppendLine(Line(loop[0]));
                    foreach (var s in loop.Skip(1)) loops.Append(Indent).AppendLine(Line(s));
                }
                else
                {
                    loops.AppendLine(string.Concat(loop.Select(Line)));
                }
            }

            if (t.Trailer is { } se) trailer.AppendLine(Line(se));
            else trailer.AppendLine("(no SE: the file ends inside this transaction)");
        }

        // The closing GE and IEA segments, once each, in file order.
        var closers = transactions.SelectMany(t => new[] { t.GroupEnd, t.InterchangeEnd })
            .OfType<Segment>().Distinct().OrderBy(s => s.Index);
        foreach (var s in closers) trailer.AppendLine(Line(s));

        return new Views(Tidy(header), Tidy(loops), Tidy(trailer), count);
    }

    private static void Gap(StringBuilder sb)
    {
        if (sb.Length > 0) sb.AppendLine();
    }

    private static string Tidy(StringBuilder sb) => sb.ToString().TrimEnd('\r', '\n');
}
