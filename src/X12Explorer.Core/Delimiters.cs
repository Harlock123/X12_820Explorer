namespace X12Explorer.Core;

/// <summary>
/// The characters that split an X12 file: between segments, between elements, and between the parts of a composite
/// element. The ISA segment names all three; files without one get the usual <c>~</c>, <c>*</c> and <c>:</c>.
/// </summary>
public sealed record Delimiters(char Segment, char Element, char Component)
{
    public static readonly Delimiters Standard = new('~', '*', ':');

    /// <summary>True when each segment ends at a line break instead of a terminator character.</summary>
    public bool LineTerminated => Segment is '\r' or '\n';

    /// <summary>
    /// Reads the delimiters from the ISA segment at the start of <paramref name="text"/>. The element separator is
    /// the character after "ISA"; ISA16, after the sixteenth separator, is one character, the component separator,
    /// and the character after it ends the segment. Counting separators instead of using the fixed 106-character
    /// width also reads files whose ISA fields were not padded.
    /// </summary>
    public static Delimiters Detect(string text)
    {
        var start = text.IndexOf("ISA", StringComparison.Ordinal);
        if (start < 0 || start + 4 > text.Length || text[..start].Any(c => !char.IsWhiteSpace(c) && c != '﻿'))
            return Standard;

        var element = text[start + 3];
        if (char.IsLetterOrDigit(element) || char.IsWhiteSpace(element)) return Standard;

        // The first separator is right after "ISA"; walk on to the sixteenth, which ISA16 follows.
        var at = start + 3;
        for (var n = 2; n <= 16; n++)
        {
            at = text.IndexOf(element, at + 1);
            if (at < 0) return Standard;
        }

        if (at + 2 >= text.Length) return Standard;
        var component = text[at + 1];
        var segment = text[at + 2];
        return new Delimiters(segment, element, component);
    }

    /// <summary>How the terminator is written after a segment in the views: nothing when it is a line break.</summary>
    public string Shown => LineTerminated ? string.Empty : Segment.ToString();
}
