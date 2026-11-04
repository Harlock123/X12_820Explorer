using System.Reflection;
using X12Explorer.Core;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace X12Explorer.Mcp;

/// <summary>
/// The MCP server: <see cref="X12Tools"/>' methods as MCP tools (the SDK's attribute model, read by reflection), with
/// instructions that tell an agent the explorer's conventions. It runs headless over stdio, reading the files in one
/// folder and writing nothing.
/// </summary>
public static class X12Server
{
    public const string Name = "X12Explorer";

    /// <summary>What every agent is told on connecting: the conventions the tools' descriptions build on.</summary>
    public const string Instructions = """
        X12 Explorer reads HIPAA X12 EDI files: 820 capitation payments, 834 enrollments, 835 remittances, 270/271 eligibility requests and responses, 837 claims, 276/277 claim status requests and responses, and 999 acknowledgments. It only reads: it never changes, writes or sends a file, and it reads only inside one folder.

        Conventions:
        - Files are named relative to that folder (list_files shows them).
        - A file is segments; a segment is elements. Segments are numbered from 1 in file order, and every tool reports segment numbers, so you can go from a summary or a search to get_segment. Elements are named by segment ID and position, as implementation guides name them: CLP01 is the first element after the ID; a composite element's parts are CLP01-1, CLP01-2.
        - The envelope: ISA opens an interchange, GS a functional group, ST a transaction; SE, GE and IEA close them. A file may hold several of each. Transactions are numbered from 1 in file order.
        - Each transaction splits into loops at one segment, as the explorer's tab does: ENT for an 820, INS for an 834, CLP for an 835, AK2 for a 999, HL for the rest.
        - Amounts come back as the file wrote them; dates in CCYYMMDD come back as ISO dates.

        A good way to work: list_files; open_file for what a file holds; check_file for envelope problems; summarize a transaction for what it says (claims and their statuses, members, payments, acknowledgment errors); find_segments to pick out particular segments (every CLP with CLP02 = 4, every NM1 with NM101 = IL); get_segment for one segment's elements; get_transaction to read the segments themselves a page of loops at a time.

        These files usually carry protected health information: names, member IDs, birth dates, diagnoses. Quote only what the user asked about, and don't copy it anywhere else.
        """;

    /// <summary>The MCP tools, reading from <paramref name="files"/>.</summary>
    public static IEnumerable<McpServerTool> Tools(Workspace files)
    {
        var target = new X12Tools(files);
        foreach (var method in typeof(X12Tools).GetMethods(BindingFlags.Public | BindingFlags.Instance))
            if (method.GetCustomAttribute<McpServerToolAttribute>() != null)
                yield return McpServerTool.Create(method, target);
    }

    public static Implementation Info => new()
    {
        Name = Name,
        Title = "X12 Explorer",
        Version = typeof(X12Server).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+')[0] ?? "1.0",
    };

    /// <summary>Added to the instructions when the server de-identifies what it reads.</summary>
    public const string DeidentifiedNote = """

        This server de-identifies every file as it reads it: people's names, member IDs, account and claim numbers, street addresses, phone numbers and free-text notes are replaced with consistent stand-ins (the same member ID always gets the same stand-in, so files still match up), ZIP codes keep three digits, birth dates keep only their year, and all other dates are moved back by the same number of days. Amounts, codes, diagnoses, providers and payers are as in the file. Say so when you quote names, IDs or dates.
        """;

    public static McpServerOptions Options(Workspace files)
    {
        var instructions = files.Deidentifier == null ? Instructions : Instructions + DeidentifiedNote;
        var options = new McpServerOptions { ServerInfo = Info, ServerInstructions = instructions, ToolCollection = [] };
        foreach (var tool in Tools(files)) options.ToolCollection.Add(tool);
        return options;
    }

    /// <summary>Serves one MCP session over a transport until the client goes away or <paramref name="cancel"/> fires.</summary>
    public static async Task RunAsync(ITransport transport, Workspace files, CancellationToken cancel = default)
    {
        await using var server = McpServer.Create(transport, Options(files));
        await server.RunAsync(cancel).ConfigureAwait(false);
    }
}

/// <summary>
/// The command line that starts the server instead of the window: <c>X12Explorer --mcp [--root FOLDER] [--deidentify]</c>.
/// The folder is the one files are read from: the current folder unless <c>--root</c> names another. With
/// <c>--deidentify</c> every file is de-identified as it is read, with one random key for the session.
/// </summary>
public static class McpCommandLine
{
    public static bool Wanted(string[] args) => args.Contains("--mcp", StringComparer.OrdinalIgnoreCase);

    /// <summary>Serves MCP on <paramref name="input"/> and <paramref name="output"/>; returns the exit code.</summary>
    public static async Task<int> RunAsync(string[] args, Stream input, Stream output, TextWriter errors, CancellationToken cancel = default)
    {
        string? root = null;
        var deidentify = false;
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i].Equals("--mcp", StringComparison.OrdinalIgnoreCase)) continue;
            if (args[i].Equals("--root", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                root = args[++i];
                continue;
            }
            if (args[i].Equals("--deidentify", StringComparison.OrdinalIgnoreCase))
            {
                deidentify = true;
                continue;
            }
            await errors.WriteLineAsync($"X12Explorer --mcp: unknown argument \"{args[i]}\". Usage: X12Explorer --mcp [--root FOLDER] [--deidentify]");
            return 2;
        }

        Workspace files;
        try
        {
            files = new Workspace(root ?? Environment.CurrentDirectory, deidentify ? new Deidentifier() : null);
        }
        catch (Exception e) when (e is IOException or ArgumentException or UnauthorizedAccessException)
        {
            await errors.WriteLineAsync($"X12Explorer --mcp: {e.Message}");
            return 2;
        }

        // Only MCP messages go to stdout; anything for a person goes to stderr.
        await errors.WriteLineAsync($"X12 Explorer MCP server, reading {files.Root}{(deidentify ? ", de-identified" : "")}");
        await X12Server.RunAsync(new StreamServerTransport(input, output, X12Server.Name), files, cancel);
        return 0;
    }
}
