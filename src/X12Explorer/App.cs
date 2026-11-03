using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;

namespace X12Explorer;

public sealed class App : Application
{
    public override void Initialize()
    {
        Styles.Add(new FluentTheme());
        RequestedThemeVariant = ThemeVariant.Dark;
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // X12Explorer [file]: opens the file on its transaction's tab.
            var file = desktop.Args is { Length: > 0 } args ? args[0] : null;
            desktop.MainWindow = new MainWindow(file);
        }
        base.OnFrameworkInitializationCompleted();
    }
}
