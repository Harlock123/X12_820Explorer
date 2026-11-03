namespace X12Explorer.Core;

/// <summary>One segment: its text as it was in the file (without the terminator) and its elements.</summary>
public sealed class Segment
{
    public Segment(string text, Delimiters delimiters, int index = 0)
    {
        Text = text;
        Index = index;
        Elements = text.Split(delimiters.Element);
        Id = Elements[0].Trim();
    }

    /// <summary>The segment's text, element separators and all, without the terminator.</summary>
    public string Text { get; }

    /// <summary>The elements, with the segment ID at [0], so <c>Elements[1]</c> is the first data element (ST01).</summary>
    public string[] Elements { get; }

    /// <summary>Where the segment is in the file: 0 for the first.</summary>
    public int Index { get; }

    /// <summary>The segment ID: ST, ISA, CLP, HL and so on.</summary>
    public string Id { get; }

    /// <summary>The element at <paramref name="position"/> (1 for the first after the ID), or "" when there isn't one.</summary>
    public string this[int position] => position < Elements.Length ? Elements[position] : string.Empty;

    public override string ToString() => Text;
}
