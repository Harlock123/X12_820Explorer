using System.IO.Pipes;
using System.Text.Json;
using System.Text.Json.Nodes;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using X12Explorer.Mcp;

namespace X12Explorer.Tests;

/// <summary>
/// The MCP server, driven as an agent drives it: an MCP client talking to the server over in-memory pipes, on a
/// temporary folder holding copies of the samples. One test also starts the real program with --mcp.
/// </summary>
public sealed class McpTests : IAsyncLifetime
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "x12-mcp-" + Guid.NewGuid().ToString("N"));
    private readonly Wire _up = new(), _down = new();
    private readonly CancellationTokenSource _stop = new();
    private Task _server = Task.CompletedTask;
    private McpClient _client = null!;

    public async ValueTask InitializeAsync()
    {
        Directory.CreateDirectory(Path.Combine(_folder, "in"));
        foreach (var sample in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Samples"), "*.edi"))
            File.Copy(sample, Path.Combine(_folder, "in", Path.GetFileName(sample)));
        await File.WriteAllTextAsync(Path.Combine(_folder, "notes.txt"), "Not an X12 file.");

        // Through the command line, as the program starts it.
        _server = Task.Run(() => McpCommandLine.RunAsync(["--mcp", "--root", _folder], _up.Reader, _down.Writer, TextWriter.Null, _stop.Token));
        _client = await McpClient.CreateAsync(new StreamClientTransport(_up.Writer, _down.Reader));
    }

    public async ValueTask DisposeAsync()
    {
        await _client.DisposeAsync();
        await _stop.CancelAsync();
        try { await _server.WaitAsync(TimeSpan.FromSeconds(5)); }
        catch (Exception e) when (e is OperationCanceledException or TimeoutException or IOException) { }
        _up.Dispose();
        _down.Dispose();
        try { Directory.Delete(_folder, true); }
        catch (IOException) { }
    }

    [Fact]
    public async Task Offers_seven_tools_that_only_read()
    {
        var tools = await _client.ListToolsAsync();
        Assert.Equal(
            ["check_file", "find_segments", "get_segment", "get_transaction", "list_files", "open_file", "summarize"],
            tools.Select(t => t.Name).Order());
        Assert.All(tools, t =>
        {
            Assert.True(t.ProtocolTool.Annotations?.ReadOnlyHint);
            Assert.False(t.ProtocolTool.Annotations?.DestructiveHint);
            Assert.False(string.IsNullOrWhiteSpace(t.Description));
        });
        Assert.Contains("protected health information", _client.ServerInstructions);
    }

    [Fact]
    public async Task Lists_only_X12_files()
    {
        var result = await Json("list_files");
        Assert.Equal(9, (int)result["count"]!);
        Assert.Contains("in/remittance-835.edi", Strings(result["files"]));
        Assert.DoesNotContain("notes.txt", Strings(result["files"]));
    }

    [Fact]
    public async Task Opens_a_file_with_two_transactions()
    {
        var result = await Json("open_file", new { file = "in/claim-837.edi" });
        var transactions = result["transactions"]!.AsArray();
        Assert.Equal(2, transactions.Count);
        Assert.Equal("Claim (837)", (string)transactions[0]!["name"]!);
        Assert.Equal("0002", (string)transactions[1]!["control_number"]!);
        Assert.Equal("~", (string)result["delimiters"]!["segment"]!);
        Assert.Equal("Test", (string)result["interchanges"]![0]!["usage"]!);
        Assert.Equal(0, (int)result["problems"]!);
    }

    [Fact]
    public async Task Summarizes_a_remittance_and_checks_it_balances()
    {
        var result = await Json("summarize", new { file = "in/remittance-835.edi" });
        Assert.Equal("245.00", (string)result["payment"]!["amount"]!);
        Assert.True((bool)result["balanced"]!);
        Assert.Equal(2, (int)result["totals"]!["claims"]!);
        Assert.Equal(245.00m, (decimal)result["totals"]!["paid"]!);

        var first = result["claims"]![0]!;
        Assert.Equal("CLAIM0001", (string)first["claim_id"]!);
        Assert.Equal("Processed as Primary", (string)first["status"]!["meaning"]!);
        Assert.Equal("DOE, JANE", (string)first["patient"]!);
        var adjustments = first["services"]![0]!["adjustments"]!.AsArray();
        Assert.Equal(["Contractual Obligations", "Patient Responsibility"], adjustments.Select(a => (string)a!["group"]!["meaning"]!));
    }

    [Fact]
    public async Task Summarizes_an_acknowledgment_down_to_the_bad_element()
    {
        var result = await Json("summarize", new { file = "in/acknowledgment-999.edi" });
        Assert.Equal("Partially Accepted, At Least One Transaction Set Was Rejected", (string)result["group_result"]!["result"]!["meaning"]!);

        var sets = result["transactions"]!.AsArray();
        Assert.Equal("Accepted", (string)sets[0]!["result"]!["meaning"]!);
        Assert.Equal("Rejected", (string)sets[1]!["result"]!["meaning"]!);
        Assert.Equal("One or More Segments in Error", (string)sets[1]!["reasons"]![0]!["meaning"]!);

        var error = sets[1]!["errors"]![0]!;
        Assert.Equal("DTP", (string)error["segment_id"]!);
        Assert.Equal("CLM01:PATACCT0002", (string)error["context"]![0]!);
        var element = error["elements"]![0]!;
        Assert.Equal("Invalid Date", (string)element["error"]!["meaning"]!);
        Assert.Equal("20261321", (string)element["bad_value"]!);
    }

    [Fact]
    public async Task Summarizes_members_payments_statuses_and_benefits()
    {
        var members = await Json("summarize", new { file = "in/membership-834.edi" });
        Assert.Equal(2, (int)members["totals"]!["members"]!);
        Assert.Equal("Addition", (string)members["members"]![0]!["maintenance"]!["meaning"]!);
        Assert.Equal("Spouse", (string)members["members"]![1]!["relationship"]!["meaning"]!);
        Assert.Equal("2026-11-01", (string)members["members"]![0]!["coverages"]![0]!["begins"]!);

        var capitation = await Json("summarize", new { file = "in/capitation-820.edi" });
        Assert.True((bool)capitation["balanced"]!);
        Assert.Equal(3, (int)capitation["totals"]!["entities"]!);

        var status = await Json("summarize", new { file = "in/claim-status-277.edi" });
        Assert.Equal("Acknowledgement/Acceptance into adjudication system", (string)status["statuses"]![0]!["category"]!["meaning"]!);
        Assert.Equal("PAYERCLM01", (string)status["statuses"]![0]!["payer_claim_number"]!);

        var benefits = await Json("summarize", new { file = "in/benefit-info-271.edi" });
        Assert.Equal("Active Coverage", (string)benefits["people"]![0]!["benefits"]![0]!["information"]!["meaning"]!);

        var claims = await Json("summarize", new { file = "in/claim-837.edi", transaction = 2 });
        Assert.Equal("0002", (string)claims["control_number"]!);
        Assert.NotEmpty(claims["claims"]!.AsArray());
    }

    [Fact]
    public async Task Finds_segments_by_an_element_value()
    {
        var result = await Json("find_segments", new { file = "in/remittance-835.edi", segment_id = "cas", element = 1, value = "PR" });
        Assert.Equal(1, (int)result["count"]!);
        var match = result["matches"]![0]!;
        Assert.Equal("CAS*PR*2*25.00", (string)match["text"]!);
        Assert.Equal(1, (int)match["transaction"]!);
        Assert.Equal(1, (int)match["loop"]!);

        var none = await Json("find_segments", new { file = "in/remittance-835.edi", segment_id = "CLP", element = 2, value = "4" });
        Assert.Equal(0, (int)none["count"]!);
    }

    [Fact]
    public async Task Lists_a_segments_elements_and_components()
    {
        var found = await Json("find_segments", new { file = "in/remittance-835.edi", segment_id = "SVC" });
        var number = (int)found["matches"]![0]!["segment"]!;

        var result = await Json("get_segment", new { file = "in/remittance-835.edi", segment = number });
        Assert.Equal("SVC", (string)result["id"]!);
        var first = result["elements"]![0]!;
        Assert.Equal("SVC01", (string)first["name"]!);
        Assert.Equal(["HC", "99213"], first["components"]!.AsArray().Select(c => (string)c!["value"]!));
        Assert.Equal("SVC01-2", (string)first["components"]![1]!["name"]!);
    }

    [Fact]
    public async Task Pages_through_a_transactions_loops()
    {
        var result = await Json("get_transaction", new { file = "in/capitation-820.edi", loops = 2 });
        Assert.Equal(3, (int)result["loop_count"]!);
        Assert.Equal(2, result["loops"]!.AsArray().Count);
        Assert.Equal(3, (int)result["next_loop"]!);
        Assert.StartsWith("ISA", (string)result["header"]![0]!["text"]!);
        Assert.StartsWith("IEA", (string)result["trailer"]!.AsArray()[^1]!["text"]!);

        var rest = await Json("get_transaction", new { file = "in/capitation-820.edi", first_loop = 3 });
        Assert.StartsWith("ENT*3", (string)rest["loops"]![0]!["segments"]![0]!["text"]!);
        Assert.Null(rest["next_loop"]);
    }

    [Fact]
    public async Task Every_sample_passes_the_envelope_checks()
    {
        foreach (var file in Strings((await Json("list_files"))["files"]))
            Assert.True((bool)(await Json("check_file", new { file }))["ok"]!, file);
    }

    [Fact]
    public async Task Check_file_finds_a_wrong_segment_count_and_control_number()
    {
        var text = await File.ReadAllTextAsync(Path.Combine(_folder, "in", "remittance-835.edi"));
        await File.WriteAllTextAsync(Path.Combine(_folder, "broken.edi"), text.Replace("SE*19*0001", "SE*18*0009"));

        var result = await Json("check_file", new { file = "broken.edi" });
        Assert.False((bool)result["ok"]!);
        var problems = result["problems"]!.AsArray().Select(p => (string)p!["problem"]!).ToList();
        Assert.Contains(problems, p => p.Contains("SE01 says 18 segments") && p.Contains("has 19"));
        Assert.Contains(problems, p => p.Contains("SE02 \"0009\" doesn't match ST02 \"0001\""));
    }

    [Fact]
    public async Task Refuses_files_outside_its_folder()
    {
        var outside = Path.Combine(Path.GetTempPath(), "x12-outside-" + Guid.NewGuid().ToString("N") + ".edi");
        File.Copy(Path.Combine(_folder, "in", "remittance-835.edi"), outside);
        try
        {
            Assert.Contains("outside the folder", await Fails("open_file", new { file = "../" + Path.GetFileName(outside) }));
            Assert.Contains("outside the folder", await Fails("summarize", new { file = outside }));
            Assert.Contains("There's no file", await Fails("open_file", new { file = "missing.edi" }));
            Assert.Contains("transactions 1 to 2", await Fails("summarize", new { file = "in/claim-837.edi", transaction = 3 }));
        }
        finally
        {
            File.Delete(outside);
        }
    }

    [Fact]
    public async Task With_deidentify_no_tool_shows_the_real_names_or_IDs()
    {
        var up = new Wire();
        var down = new Wire();
        using var stop = new CancellationTokenSource();
        var server = Task.Run(() => McpCommandLine.RunAsync(["--mcp", "--root", _folder, "--deidentify"], up.Reader, down.Writer, TextWriter.Null, stop.Token));
        try
        {
            await using var client = await McpClient.CreateAsync(new StreamClientTransport(up.Writer, down.Reader));
            Assert.Contains("de-identifies every file", client.ServerInstructions);

            var opened = await Json(client, "open_file", new { file = "in/remittance-835.edi" });
            Assert.True((bool)opened["deidentified"]!);

            var summary = await Json(client, "summarize", new { file = "in/remittance-835.edi" });
            var patient = (string)summary["claims"]![0]!["patient"]!;
            Assert.DoesNotContain("DOE", patient);
            Assert.True((bool)summary["balanced"]!);
            Assert.Equal("245.00", (string)summary["payment"]!["amount"]!);

            var found = await Json(client, "find_segments", new { file = "in/membership-834.edi", segment_id = "NM1" });
            Assert.DoesNotContain(found["matches"]!.AsArray(), m => ((string)m!["text"]!).Contains("DOE"));
        }
        finally
        {
            await stop.CancelAsync();
            try { await server.WaitAsync(TimeSpan.FromSeconds(5)); }
            catch (Exception e) when (e is OperationCanceledException or TimeoutException or IOException) { }
            up.Dispose();
            down.Dispose();
        }
    }

    [Fact]
    public async Task Rejects_unknown_arguments_on_the_command_line()
    {
        var errors = new StringWriter();
        Assert.Equal(2, await McpCommandLine.RunAsync(["--mcp", "--bogus"], Stream.Null, Stream.Null, errors));
        Assert.Contains("unknown argument", errors.ToString());
        Assert.Equal(2, await McpCommandLine.RunAsync(["--mcp", "--root", Path.Combine(_folder, "nope")], Stream.Null, Stream.Null, errors));
    }

    [Fact]
    public async Task The_program_serves_MCP_on_stdio()
    {
        var program = Path.Combine(AppContext.BaseDirectory, "X12Explorer.dll");
        Assert.True(File.Exists(program), program);
        var transport = new StdioClientTransport(new StdioClientTransportOptions
        {
            Name = "X12Explorer",
            Command = "dotnet",
            Arguments = [program, "--mcp", "--root", _folder],
        });
        await using var client = await McpClient.CreateAsync(transport);
        Assert.Equal("X12Explorer", client.ServerInfo?.Name);
        var result = await client.CallToolAsync("list_files");
        Assert.NotEqual(true, result.IsError);
        Assert.Contains("in/membership-834.edi", Text(result));
    }

    // ───────────────────────────── helpers ─────────────────────────────

    private Task<JsonObject> Json(string tool, object? args = null) => Json(_client, tool, args);

    private static async Task<JsonObject> Json(McpClient client, string tool, object? args = null)
    {
        var result = await Call(client, tool, args);
        Assert.True(result.IsError != true, $"{tool} failed: {Text(result)}");
        return (JsonObject)JsonNode.Parse(Text(result))!;
    }

    private async Task<string> Fails(string tool, object? args = null)
    {
        var result = await Call(tool, args);
        Assert.True(result.IsError == true, $"{tool} should have failed but said: {Text(result)}");
        return Text(result);
    }

    private Task<CallToolResult> Call(string tool, object? args) => Call(_client, tool, args);

    private static Task<CallToolResult> Call(McpClient client, string tool, object? args)
    {
        var dict = args == null ? null : JsonSerializer.SerializeToElement(args).EnumerateObject().ToDictionary(p => p.Name, p => (object?)p.Value);
        return client.CallToolAsync(tool, dict).AsTask();
    }

    private static string Text(CallToolResult r) => string.Join("\n", r.Content.OfType<TextContentBlock>().Select(t => t.Text));

    private static List<string> Strings(JsonNode? node) => node?.AsArray().Select(n => n?.ToString() ?? "").ToList() ?? [];

    /// <summary>One direction of an in-process byte stream: what one side writes, the other reads.</summary>
    private sealed class Wire : IDisposable
    {
        private readonly AnonymousPipeServerStream _write = new(PipeDirection.Out);
        private readonly AnonymousPipeClientStream _read;

        public Wire() => _read = new AnonymousPipeClientStream(PipeDirection.In, _write.ClientSafePipeHandle);

        public Stream Writer => _write;
        public Stream Reader => _read;

        public void Dispose()
        {
            _write.Dispose();
            _read.Dispose();
        }
    }
}
