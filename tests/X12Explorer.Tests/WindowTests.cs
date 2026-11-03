using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using X12Explorer.Core;

namespace X12Explorer.Tests;

/// <summary>
/// The window on Avalonia's headless platform: each sample lands on its own tab, whichever Open button opened it,
/// the transaction choice, the segment-per-line switch, the element strip and the font buttons. Set X12_SHOTS to a
/// folder to save pictures of it (the README's screenshots come from here).
/// </summary>
public class WindowTests
{
    private static string? Shots => Environment.GetEnvironmentVariable("X12_SHOTS") is { Length: > 0 } d ? d : null;

    private static MainWindow Show(string? file = null)
    {
        var w = new MainWindow(file) { Width = 1180, Height = 780 };
        w.PickFile = () => throw new Xunit.Sdk.XunitException("Nothing should have asked for a file.");
        w.Show();
        Pump();
        return w;
    }

    private static void Pump() => Dispatcher.UIThread.RunJobs();

    /// <summary>A copy of a sample in a temp folder, so the pictures show a short path rather than the build's.</summary>
    private static string Sample(string name)
    {
        var dir = Path.Combine(Path.GetTempPath(), "x12explorer-samples");
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, name);
        File.Copy(ParserTests.Sample(name), path, overwrite: true);
        return path;
    }

    private static void Shot(MainWindow w, string name)
    {
        Pump();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        var frame = w.CaptureRenderedFrame();
        Assert.NotNull(frame);
        if (Shots is not { } dir) return;
        Directory.CreateDirectory(dir);
        frame.Save(Path.Combine(dir, $"X12Explorer-{name}.png"), Avalonia.Media.Imaging.PngBitmapEncoderOptions.Default);
    }

    [AvaloniaTheory]
    [InlineData("capitation-820.edi", "820")]
    [InlineData("membership-834.edi", "834")]
    [InlineData("remittance-835.edi", "835")]
    [InlineData("eligibility-270.edi", "270")]
    [InlineData("benefit-info-271.edi", "271")]
    [InlineData("claim-837.edi", "837")]
    [InlineData("status-request-276.edi", "276")]
    [InlineData("claim-status-277.edi", "277")]
    [InlineData("acknowledgment-999.edi", "999")]
    public void Each_sample_opens_on_its_own_tab(string name, string code)
    {
        var w = Show();
        // Open from the first tab's button, whatever the file is.
        w.PickFile = () => Task.FromResult<string?>(Sample(name));
        w.Pages["820"].OpenButton.RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(Avalonia.Controls.Button.ClickEvent));
        Pump();

        var page = w.Current;
        Assert.Equal(code, page.Kind.Code);
        Assert.Equal(Sample(name), page.FileName.Text);
        Assert.StartsWith("ISA*", page.HeaderBox.Text);
        Assert.StartsWith(page.Kind.LoopStart + "*", page.LoopsBox.Text);
        Assert.Contains(page.TrailerBox.Text!.Split(Environment.NewLine), l => l.StartsWith("SE*"));
        Assert.Contains(page.Kind.Title, w.Message.Text);
        Shot(w, code);
        w.Close();
    }

    [AvaloniaFact]
    public void A_file_named_on_the_command_line_opens()
    {
        var w = Show(Sample("remittance-835.edi"));

        Assert.Equal("835", w.Current.Kind.Code);
        Assert.Equal("Loops: 2 CLP", w.Current.LoopsLabel.Text);
        Assert.Equal("remittance-835.edi: Remittance (835), 1 transaction, 23 segments (segments end ~, elements *).", w.Message.Text);
        w.Close();
    }

    [AvaloniaFact]
    public void Several_transactions_can_be_shown_together_or_one_at_a_time()
    {
        var w = Show(Sample("claim-837.edi"));
        var page = w.Current;

        Assert.True(page.TransactionChoice.IsVisible);
        Assert.Equal(["All 2 transactions", "1: ST 837 #0001", "2: ST 837 #0002"], page.TransactionChoice.ItemsSource!.Cast<string>());
        Assert.Contains("DOE*JANE", page.LoopsBox.Text);
        Assert.Contains("ROE*RICHARD", page.LoopsBox.Text);
        Assert.Equal("Loops: 4 HL", page.LoopsLabel.Text);

        page.TransactionChoice.SelectedIndex = 2;
        Pump();
        Assert.DoesNotContain("DOE*JANE", page.LoopsBox.Text);
        Assert.Contains("ROE*RICHARD", page.LoopsBox.Text);
        Assert.Equal("Loops: 2 HL", page.LoopsLabel.Text);
        Assert.StartsWith("SE*21*0002~", page.TrailerBox.Text);
        Shot(w, "837-second");
        w.Close();
    }

    [AvaloniaFact]
    public void Loops_switch_between_one_line_and_a_segment_per_line()
    {
        var w = Show(Sample("remittance-835.edi"));
        var page = w.Current;

        Assert.False(page.Expanded.IsChecked);
        Assert.Equal(2, page.LoopsBox.Text!.Split(Environment.NewLine).Length);

        page.Expanded.IsChecked = true;
        Pump();
        Assert.Equal(11, page.LoopsBox.Text!.Split(Environment.NewLine).Length);
        Assert.Contains(Environment.NewLine + Views.Indent + "SVC*HC:99213*", page.LoopsBox.Text);
        w.Close();
    }

    [AvaloniaFact]
    public void The_element_strip_lists_the_segment_under_the_caret()
    {
        var w = Show(Sample("remittance-835.edi"));
        var page = w.Current;

        page.LoopsBox.CaretIndex = page.LoopsBox.Text!.IndexOf("NM1*QC", StringComparison.Ordinal) + 2;
        Pump();
        Assert.StartsWith("NM101 QC   ·   NM102 1   ·   NM103 DOE", page.ElementStrip.Text);

        page.HeaderBox.CaretIndex = page.HeaderBox.Text!.IndexOf("BPR", StringComparison.Ordinal);
        Pump();
        Assert.StartsWith("BPR01 I   ·   BPR02 245.00", page.ElementStrip.Text);
        Shot(w, "elements");
        w.Close();
    }

    [AvaloniaFact]
    public void Files_it_cant_show_say_why_and_leave_the_tabs_alone()
    {
        var dir = Directory.CreateTempSubdirectory("x12explorer-").FullName;
        var notX12 = Path.Combine(dir, "notes.txt");
        File.WriteAllText(notX12, "just some notes");
        var other = Path.Combine(dir, "order.edi");
        File.WriteAllText(other, "ST*850*0001~BEG*00*SA*PO1~SE*3*0001~");

        var w = Show(Sample("membership-834.edi"));

        Assert.False(w.Open(notX12));
        Assert.Contains("no ST segment", w.Message.Text);
        Assert.False(w.Open(other));
        Assert.Equal("order.edi is an X12 850; there are tabs for 820, 834, 835, 270, 271, 837, 276, 277 and 999.", w.Message.Text);
        Assert.False(w.Open(Path.Combine(dir, "missing.edi")));
        Assert.Contains("Couldn't read missing.edi", w.Message.Text);

        Assert.Equal("834", w.Current.Kind.Code);
        Assert.Contains("INS*Y*18", w.Current.LoopsBox.Text);
        w.Close();
        Directory.Delete(dir, true);
    }

    [AvaloniaFact]
    public void Font_buttons_and_keys_resize_every_box_within_limits()
    {
        var w = Show(Sample("claim-837.edi"));

        w.BiggerButton.RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(Avalonia.Controls.Button.ClickEvent));
        Assert.Equal(Look.FontSize + 1, w.BoxFontSize);
        Assert.All(w.Pages.Values.SelectMany(p => p.Boxes), b => Assert.Equal(Look.FontSize + 1, b.FontSize));

        w.KeyPress(Key.OemMinus, RawInputModifiers.Control, PhysicalKey.Minus, "-");
        w.KeyPress(Key.OemMinus, RawInputModifiers.Control, PhysicalKey.Minus, "-");
        Pump();
        Assert.Equal(Look.FontSize - 1, w.BoxFontSize);

        for (var i = 0; i < 50; i++) w.SetFontSize(w.BoxFontSize + 1);
        Assert.Equal(Look.BiggestFont, w.BoxFontSize);
        Assert.False(w.BiggerButton.IsEnabled);
        for (var i = 0; i < 50; i++) w.SetFontSize(w.BoxFontSize - 1);
        Assert.Equal(Look.SmallestFont, w.BoxFontSize);
        Assert.False(w.SmallerButton.IsEnabled);
        w.Close();
    }
}
