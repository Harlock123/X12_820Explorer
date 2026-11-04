using Avalonia;
using X12Explorer.Mcp;

namespace X12Explorer;

internal static class Program
{
    /// <summary>
    /// Starts the explorer. With <c>--mcp</c> it opens no window: it serves the Model Context Protocol on stdio for an
    /// AI agent instead, reading the X12 files in one folder (see <see cref="McpCommandLine"/>).
    /// </summary>
    [STAThread]
    public static int Main(string[] args)
    {
        if (McpCommandLine.Wanted(args))
            return McpCommandLine.RunAsync(args, Console.OpenStandardInput(), Console.OpenStandardOutput(), Console.Error)
                .GetAwaiter().GetResult();

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        return 0;
    }

    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .WithInterFont()
        .LogToTrace();
}
