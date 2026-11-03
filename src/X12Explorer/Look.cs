using Avalonia.Media;

namespace X12Explorer;

/// <summary>
/// The colours and fonts the window is drawn in: dark, with the header and trailer boxes in rose, after the old
/// program's RosyBrown and MistyRose boxes.
/// </summary>
internal static class Look
{
    public static readonly IBrush Window = Brush("#14161B");
    public static readonly IBrush Bar = Brush("#1B1E25");
    public static readonly IBrush Border = Brush("#2D323D");
    public static readonly IBrush Heading = Brush("#8C93A1");
    public static readonly IBrush Text = Brush("#DDE1E8");
    public static readonly IBrush Accent = Brush("#E8A3AE");
    public static readonly IBrush Error = Brush("#FF7A7A");

    /// <summary>The header and trailer boxes.</summary>
    public static readonly IBrush Envelope = Brush("#2A1D22");
    public static readonly IBrush EnvelopeText = Brush("#F3D9DD");

    /// <summary>The loops box.</summary>
    public static readonly IBrush Sheet = Brush("#101217");

    /// <summary>A fixed-width face, with fallbacks on every platform.</summary>
    public static readonly FontFamily Mono = new("Cascadia Mono, Consolas, Menlo, DejaVu Sans Mono, Liberation Mono, Courier New, monospace");

    /// <summary>The text size the boxes start at, and the range the font buttons keep to.</summary>
    public const double FontSize = 13;
    public const double SmallestFont = 7;
    public const double BiggestFont = 32;

    private static SolidColorBrush Brush(string colour) => new(Color.Parse(colour));
}
