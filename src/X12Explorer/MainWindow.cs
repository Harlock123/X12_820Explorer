using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using X12Explorer.Core;

namespace X12Explorer;

/// <summary>
/// The window: a tab for each transaction (<see cref="TransactionKind.All"/>), a tool bar with Open, De-identify and
/// the font buttons, and a status line. A file goes to the tab its ST01 names, whichever Open button opened it, and files can
/// be dropped on the window too. Built in code; its controls are public so tests can drive them.
/// </summary>
public sealed class MainWindow : Window
{
    private double _fontSize = Look.FontSize;

    public TabControl Tabs { get; } = new() { Margin = new Thickness(10, 4, 10, 0) };

    /// <summary>The pages, by ST01 code.</summary>
    public IReadOnlyDictionary<string, TransactionPage> Pages { get; }

    public Button OpenButton { get; } = new() { Content = "Open…" };
    public Button DeidentifyButton { get; } = new() { Content = "De-identify…", IsEnabled = false };
    public Button SmallerButton { get; } = new() { Content = "A−", MinWidth = 40 };
    public Button BiggerButton { get; } = new() { Content = "A+", MinWidth = 40 };
    public TextBlock Message { get; } = new() { Foreground = Look.Heading, FontSize = 12, TextTrimming = TextTrimming.CharacterEllipsis };

    /// <summary>Asks for a file to open; tests answer it themselves.</summary>
    public Func<Task<string?>> PickFile { get; set; }

    /// <summary>Asks where to save a de-identified copy, given the name to suggest; tests answer it themselves.</summary>
    public Func<string, string, Task<string?>> PickSaveFile { get; set; }

    /// <summary>One key for the window's life, so copies saved in one session use the same stand-ins.</summary>
    private readonly Deidentifier _deidentifier = new();

    public MainWindow(string? open = null)
    {
        Title = "X12 Explorer";
        Width = 1180;
        Height = 780;
        MinWidth = 640;
        MinHeight = 480;
        Background = Look.Window;
        PickFile = AskForFile;
        PickSaveFile = AskWhereToSave;

        var pages = new Dictionary<string, TransactionPage>();
        var items = new List<TabItem>();
        foreach (var kind in TransactionKind.All)
        {
            var page = new TransactionPage(kind) { Margin = new Thickness(0, 10, 0, 0) };
            page.OpenButton.Click += async (_, _) => await OpenPicked();
            pages[kind.Code] = page;
            items.Add(new TabItem { Header = kind.Title, Content = page, FontSize = 13, Padding = new Thickness(10, 0) });
        }
        Pages = pages;
        Tabs.ItemsSource = items;
        Tabs.SelectionChanged += (_, _) => DeidentifyButton.IsEnabled = Current.File != null;

        var root = new DockPanel();
        var tools = BuildToolBar();
        DockPanel.SetDock(tools, Dock.Top);
        root.Children.Add(tools);
        var status = new Border
        {
            Child = Message,
            Background = Look.Bar,
            BorderBrush = Look.Border,
            BorderThickness = new Thickness(0, 1, 0, 0),
            Padding = new Thickness(12, 5)
        };
        DockPanel.SetDock(status, Dock.Bottom);
        root.Children.Add(status);
        root.Children.Add(new Border { Child = Tabs, Padding = new Thickness(0, 0, 0, 10) });
        Content = root;

        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DragOverEvent, (_, e) => e.DragEffects = e.DataTransfer.TryGetFiles() is { Length: > 0 } ? DragDropEffects.Copy : DragDropEffects.None);
        AddHandler(DragDrop.DropEvent, (_, e) =>
        {
            if (e.DataTransfer.TryGetFiles()?.FirstOrDefault()?.TryGetLocalPath() is { } path) Open(path);
        });
        AddHandler(KeyDownEvent, OnKey, RoutingStrategies.Tunnel);

        Note($"Open an X12 file ({TransactionKind.Codes}), or drop one here. It opens on its own tab.");
        if (open != null) Open(open);
    }

    /// <summary>The page on the tab that is showing.</summary>
    public TransactionPage Current => (TransactionPage)((TabItem)Tabs.SelectedItem!).Content!;

    public double BoxFontSize => _fontSize;

    /// <summary>
    /// Reads <paramref name="path"/> and shows it on the tab for its transaction, switching to that tab. Returns
    /// false, with the reason on the status line, when the file can't be read or has no tab.
    /// </summary>
    public bool Open(string path)
    {
        X12File file;
        try
        {
            file = X12File.Load(path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Note($"Couldn't read {Path.GetFileName(path)}: {ex.Message}", error: true);
            return false;
        }

        if (file.Code is not { } code)
        {
            Note($"{Path.GetFileName(path)} has no ST segment, so it isn't an X12 transaction file.", error: true);
            return false;
        }
        if (!Pages.TryGetValue(code, out var page))
        {
            Note($"{Path.GetFileName(path)} is an X12 {code}; there are tabs for {TransactionKind.Codes}.", error: true);
            return false;
        }

        page.Load(path, file);
        Tabs.SelectedIndex = TransactionKind.All.ToList().IndexOf(page.Kind);
        DeidentifyButton.IsEnabled = true;

        var d = file.Delimiters;
        var transactions = file.Transactions.Count == 1 ? "1 transaction" : $"{file.Transactions.Count} transactions";
        var delimiters = d.LineTerminated ? $"segments end at line breaks, elements {d.Element}" : $"segments end {d.Segment}, elements {d.Element}";
        var mixed = file.Transactions.Count(t => t.Code != code) is > 0 and var others ? $"; {others} of another type" : "";
        Note($"{Path.GetFileName(path)}: {page.Kind.Title}, {transactions}{mixed}, {file.Segments.Count} segments ({delimiters}).");
        return true;
    }

    /// <summary>
    /// Saves a de-identified copy of the file on the showing tab where <see cref="PickSaveFile"/> says, then opens the
    /// copy. Returns false, with the reason on the status line, when there's nothing to save or it can't be written.
    /// </summary>
    public async Task<bool> DeidentifyCurrent()
    {
        if (Current.File == null || Current.FileName.Text is not { Length: > 0 } source)
        {
            Note("Open a file first: De-identify saves a copy of the file on the showing tab.", error: true);
            return false;
        }
        var suggested = Path.GetFileNameWithoutExtension(source) + "-deidentified" + Path.GetExtension(source);
        if (await PickSaveFile(source, suggested) is not { } target) return false;
        if (string.Equals(Path.GetFullPath(target), Path.GetFullPath(source), StringComparison.Ordinal))
        {
            Note("Choose another name: the de-identified copy can't replace the original.", error: true);
            return false;
        }

        Deidentified result;
        try
        {
            result = _deidentifier.Load(source);
            File.WriteAllText(target, result.Text);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Note($"Couldn't save {Path.GetFileName(target)}: {ex.Message}", error: true);
            return false;
        }

        Open(target);
        Note($"Saved and opened a de-identified copy, {Path.GetFileName(target)}: replaced {result.Summary} (dates moved back " +
             $"{-_deidentifier.ShiftDays} days). Check it before you share it.");
        return true;
    }

    public void SetFontSize(double size)
    {
        _fontSize = Math.Clamp(size, Look.SmallestFont, Look.BiggestFont);
        foreach (var page in Pages.Values) page.SetFontSize(_fontSize);
        SmallerButton.IsEnabled = _fontSize > Look.SmallestFont;
        BiggerButton.IsEnabled = _fontSize < Look.BiggestFont;
    }

    private Control BuildToolBar()
    {
        var title = new TextBlock
        {
            Text = "X12 Explorer",
            FontSize = 15,
            FontWeight = FontWeight.SemiBold,
            Foreground = Look.Accent,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 14, 0)
        };
        var fonts = new TextBlock { Text = "Text size", Foreground = Look.Heading, FontSize = 12, VerticalAlignment = VerticalAlignment.Center };

        ToolTip.SetTip(OpenButton, "Open a file (Ctrl+O). It goes to the tab its ST segment names.");
        ToolTip.SetTip(DeidentifyButton, "Save a copy of this tab's file with people's names, IDs, addresses and dates replaced, for sharing or test data.");
        ToolTip.SetTip(SmallerButton, "Smaller text (Ctrl+−)");
        ToolTip.SetTip(BiggerButton, "Bigger text (Ctrl+=)");
        OpenButton.Click += async (_, _) => await OpenPicked();
        DeidentifyButton.Click += async (_, _) => await DeidentifyCurrent();
        SmallerButton.Click += (_, _) => SetFontSize(_fontSize - 1);
        BiggerButton.Click += (_, _) => SetFontSize(_fontSize + 1);

        var bar = new DockPanel();
        var left = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { title, OpenButton, DeidentifyButton } };
        var right = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6, Children = { fonts, SmallerButton, BiggerButton } };
        DockPanel.SetDock(right, Dock.Right);
        bar.Children.Add(right);
        bar.Children.Add(left);

        return new Border
        {
            Child = bar,
            Background = Look.Bar,
            BorderBrush = Look.Border,
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(12, 8)
        };
    }

    private async Task OpenPicked()
    {
        if (await PickFile() is { } path) Open(path);
    }

    private async Task<string?> AskForFile()
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open an X12 file",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("X12 files") { Patterns = ["*.edi", "*.x12", "*.txt", "*.dat", "*.820", "*.834", "*.835", "*.837", "*.271", "*.277"] },
                FilePickerFileTypes.All
            ]
        });
        return files.Count > 0 ? files[0].TryGetLocalPath() : null;
    }

    private async Task<string?> AskWhereToSave(string source, string suggested)
    {
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save a de-identified copy",
            SuggestedFileName = suggested,
            SuggestedStartLocation = Path.GetDirectoryName(source) is { } folder ? await StorageProvider.TryGetFolderFromPathAsync(folder) : null,
            ShowOverwritePrompt = true,
        });
        return file?.TryGetLocalPath();
    }

    private async void OnKey(object? sender, KeyEventArgs e)
    {
        if (!e.KeyModifiers.HasFlag(KeyModifiers.Control) && !e.KeyModifiers.HasFlag(KeyModifiers.Meta)) return;
        switch (e.Key)
        {
            case Key.O:
                e.Handled = true;
                await OpenPicked();
                break;
            case Key.OemPlus or Key.Add:
                e.Handled = true;
                SetFontSize(_fontSize + 1);
                break;
            case Key.OemMinus or Key.Subtract:
                e.Handled = true;
                SetFontSize(_fontSize - 1);
                break;
        }
    }

    private void Note(string text, bool error = false)
    {
        Message.Text = text;
        Message.Foreground = error ? Look.Error : Look.Heading;
    }
}
