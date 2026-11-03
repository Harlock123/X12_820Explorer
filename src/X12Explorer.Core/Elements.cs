namespace X12Explorer.Core;

/// <summary>Finds the segment under the caret in one of the views, and lists its elements by position.</summary>
public static class Elements
{
    /// <summary>
    /// The segment at <paramref name="caret"/> in a view's <paramref name="text"/>, or null when the caret is on a
    /// blank line. A line holding a whole loop is split at the terminators to find the one segment.
    /// </summary>
    public static Segment? At(string text, int caret, Delimiters delimiters)
    {
        if (text.Length == 0) return null;
        caret = Math.Clamp(caret, 0, text.Length);

        var lineStart = caret == 0 ? 0 : text.LastIndexOf('\n', caret - 1) + 1;
        var lineEnd = text.IndexOf('\n', caret);
        if (lineEnd < 0) lineEnd = text.Length;
        var line = text[lineStart..lineEnd].TrimEnd('\r');
        var column = caret - lineStart;

        if (!delimiters.LineTerminated && line.Length > 0)
        {
            // Take the character after the caret (the last one at the end of a line); a terminator belongs to the
            // segment it ends.
            var at = Math.Min(column, line.Length - 1);
            var from = at == 0 ? 0 : line.LastIndexOf(delimiters.Segment, at - 1) + 1;
            var to = line.IndexOf(delimiters.Segment, at);
            line = line[from..(to < 0 ? line.Length : to)];
        }

        line = line.Trim();
        return line.Length == 0 || line.StartsWith('(') ? null : new Segment(line, delimiters);
    }

    /// <summary>
    /// The segment's elements, one per entry, named by segment ID and position: "CLP01 PATIENT1". Empty elements
    /// are left out, since they say nothing; the numbers still count them.
    /// </summary>
    public static IReadOnlyList<(string Name, string Value)> List(Segment segment) =>
        segment.Elements.Skip(1)
            .Select((value, i) => (Name: $"{segment.Id}{i + 1:00}", Value: value))
            .Where(e => e.Value.Length > 0)
            .ToList();

    /// <summary>The elements on one line, for the window's element strip: "CLP01 PATIENT1 · CLP02 1".</summary>
    public static string Describe(Segment segment)
    {
        var list = List(segment);
        return list.Count == 0
            ? $"{segment.Id}: no elements"
            : string.Join("   ·   ", list.Select(e => $"{e.Name} {e.Value}"));
    }
}
