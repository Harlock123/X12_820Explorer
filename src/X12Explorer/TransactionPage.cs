using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using X12Explorer.Core;

namespace X12Explorer;

/// <summary>
/// One tab: the file it shows, a choice of transaction when the file holds several, and three boxes, as the old
/// program had them: the header (envelope, ST and what comes before the first loop), the loops, and the trailer
/// (SE and the envelope's end). Under them, the element strip lists the elements of the segment under the caret.
/// </summary>
public sealed class TransactionPage : DockPanel
{
    private X12File? _file;
    private bool _showing;

    public TransactionPage(TransactionKind kind)
    {
        Kind = kind;
        Expanded.IsChecked = kind.Expanded;

        var top = new DockPanel { Margin = new Thickness(0, 0, 0, 8) };
        DockPanel.SetDock(OpenButton, Dock.Left);
        top.Children.Add(OpenButton);
        DockPanel.SetDock(Expanded, Dock.Right);
        top.Children.Add(Expanded);
        DockPanel.SetDock(TransactionChoice, Dock.Right);
        top.Children.Add(TransactionChoice);
        top.Children.Add(FileName);
        DockPanel.SetDock(top, Dock.Top);
        Children.Add(top);

        var strip = new Border
        {
            Child = ElementStrip,
            Background = Look.Bar,
            BorderBrush = Look.Border,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(8, 6),
            Margin = new Thickness(0, 8, 0, 0)
        };
        DockPanel.SetDock(strip, Dock.Bottom);
        Children.Add(strip);

        // Header, loops and trailer, with splitters between to give one more room.
        var boxes = new Grid { RowDefinitions = new RowDefinitions("150,6,*,6,100") };
        Place(boxes, Section(HeaderLabel, HeaderBox), 0);
        Place(boxes, Splitter(), 1);
        Place(boxes, Section(LoopsLabel, LoopsBox), 2);
        Place(boxes, Splitter(), 3);
        Place(boxes, Section(TrailerLabel, TrailerBox), 4);
        Children.Add(boxes);

        TransactionChoice.SelectionChanged += (_, _) => Show();
        Expanded.IsCheckedChanged += (_, _) => Show();
        foreach (var box in new[] { HeaderBox, LoopsBox, TrailerBox })
        {
            box.PropertyChanged += (_, e) =>
            {
                if (e.Property == TextBox.CaretIndexProperty) DescribeCaret(box);
            };
        }
        Clear();
    }

    public TransactionKind Kind { get; }

    public Button OpenButton { get; } = new() { Content = "Open…", Margin = new Thickness(0, 0, 8, 0) };

    public TextBox FileName { get; } = new()
    {
        IsReadOnly = true,
        PlaceholderText = "No file open",
        FontFamily = Look.Mono,
        FontSize = 12,
        VerticalContentAlignment = VerticalAlignment.Center
    };

    /// <summary>"All transactions" first when the file holds more than one, then each by its ST02.</summary>
    public ComboBox TransactionChoice { get; } = new() { MinWidth = 210, Margin = new Thickness(8, 0, 0, 0), IsVisible = false };

    public CheckBox Expanded { get; } = new() { Content = "One segment per line", Margin = new Thickness(12, 0, 0, 0) };

    public TextBlock HeaderLabel { get; } = Label("Header");
    public TextBlock LoopsLabel { get; } = Label("Loops");
    public TextBlock TrailerLabel { get; } = Label("Trailer");

    public TextBox HeaderBox { get; } = Box(Look.Envelope, Look.EnvelopeText);
    public TextBox LoopsBox { get; } = Box(Look.Sheet, Look.Text);
    public TextBox TrailerBox { get; } = Box(Look.Envelope, Look.EnvelopeText);

    public TextBlock ElementStrip { get; } = new()
    {
        FontFamily = Look.Mono,
        FontSize = 12,
        Foreground = Look.Heading,
        TextWrapping = TextWrapping.Wrap
    };

    /// <summary>The file on this tab, or null.</summary>
    public X12File? File => _file;

    public IEnumerable<TextBox> Boxes => [HeaderBox, LoopsBox, TrailerBox];

    /// <summary>The transactions shown: all of them, or the one chosen.</summary>
    public IReadOnlyList<Transaction> Shown => _file == null
        ? []
        : TransactionChoice.SelectedIndex <= 0 || _file.Transactions.Count == 1
            ? _file.Transactions
            : [_file.Transactions[TransactionChoice.SelectedIndex - 1]];

    /// <summary>Shows <paramref name="file"/>, read from <paramref name="path"/>.</summary>
    public void Load(string path, X12File file)
    {
        _file = file;
        FileName.Text = path;

        _showing = true;
        var items = new List<string>();
        if (file.Transactions.Count > 1) items.Add($"All {file.Transactions.Count} transactions");
        items.AddRange(file.Transactions.Select((t, i) =>
            $"{i + 1}: ST {t.Code} #{t.Control}{(t.Kind == Kind ? "" : $" ({t.Kind?.Name ?? "other"})")}"));
        TransactionChoice.ItemsSource = items;
        TransactionChoice.SelectedIndex = 0;
        TransactionChoice.IsVisible = file.Transactions.Count > 1;
        _showing = false;
        Show();
    }

    public void Clear()
    {
        _file = null;
        FileName.Text = string.Empty;
        TransactionChoice.IsVisible = false;
        foreach (var box in Boxes) box.Text = string.Empty;
        LoopsLabel.Text = $"Loops: one per {Kind.LoopStart}";
        ElementStrip.Text = "Open a file, then click a segment to list its elements.";
    }

    public void SetFontSize(double size)
    {
        foreach (var box in Boxes) box.FontSize = size;
    }

    private void Show()
    {
        if (_showing || _file == null) return;
        var views = Views.Of(Shown, _file.Delimiters, Kind, Expanded.IsChecked == true);
        HeaderBox.Text = views.Header;
        LoopsBox.Text = views.Loops;
        TrailerBox.Text = views.Trailer;
        LoopsLabel.Text = views.LoopCount == 1 ? $"Loops: 1 {Kind.LoopStart}" : $"Loops: {views.LoopCount} {Kind.LoopStart}";
        ElementStrip.Text = "Click a segment to list its elements.";
    }

    private void DescribeCaret(TextBox box)
    {
        if (_file == null) return;
        ElementStrip.Text = Core.Elements.At(box.Text ?? string.Empty, box.CaretIndex, _file.Delimiters) is { } segment
            ? Core.Elements.Describe(segment)
            : string.Empty;
    }

    private static void Place(Grid grid, Control control, int row)
    {
        Grid.SetRow(control, row);
        grid.Children.Add(control);
    }

    private static DockPanel Section(TextBlock label, TextBox box)
    {
        var panel = new DockPanel();
        DockPanel.SetDock(label, Dock.Top);
        panel.Children.Add(label);
        panel.Children.Add(box);
        return panel;
    }

    private static GridSplitter Splitter() => new()
    {
        ResizeDirection = GridResizeDirection.Rows,
        Background = Brushes.Transparent,
        HorizontalAlignment = HorizontalAlignment.Stretch
    };

    private static TextBlock Label(string text) => new()
    {
        Text = text,
        Foreground = Look.Heading,
        FontSize = 12,
        FontWeight = FontWeight.SemiBold,
        Margin = new Thickness(2, 4, 0, 3)
    };

    private static TextBox Box(IBrush background, IBrush foreground)
    {
        var box = new TextBox
        {
            IsReadOnly = true,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.NoWrap,
            FontFamily = Look.Mono,
            FontSize = Look.FontSize,
            Background = background,
            Foreground = foreground,
            BorderBrush = Look.Border,
            VerticalContentAlignment = VerticalAlignment.Top
        };
        // Fluent's TextBox swaps in its own brushes on hover and focus; keep the box's colours.
        box.Resources["TextControlBackgroundPointerOver"] = background;
        box.Resources["TextControlBackgroundFocused"] = background;
        box.Resources["TextControlForegroundPointerOver"] = foreground;
        box.Resources["TextControlForegroundFocused"] = foreground;
        ScrollViewer.SetHorizontalScrollBarVisibility(box, ScrollBarVisibility.Auto);
        ScrollViewer.SetVerticalScrollBarVisibility(box, ScrollBarVisibility.Auto);
        return box;
    }
}
