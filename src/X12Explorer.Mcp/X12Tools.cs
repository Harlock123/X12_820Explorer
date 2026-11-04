using System.ComponentModel;
using System.Text.Json.Nodes;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using X12Explorer.Core;

namespace X12Explorer.Mcp;

/// <summary>
/// The MCP tools: what an agent can do with X12 files. All of them only read, and only inside the
/// <see cref="Workspace"/>'s folder. Failures come back as MCP tool errors (<see cref="McpException"/>) with a message
/// that says what to do instead.
/// </summary>
/// <remarks>The parameter names are the JSON names agents see, hence snake_case. Segment and transaction numbers count
/// from 1, as a person reading the file would.</remarks>
public sealed class X12Tools(Workspace files)
{
    /// <summary>The most loops <c>get_transaction</c> returns at once.</summary>
    public const int MaxLoops = 100;

    /// <summary>The most segments <c>find_segments</c> returns.</summary>
    public const int MaxFound = 200;

    [McpServerTool(Name = "list_files", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
    [Description("Lists the X12 files in the folder this server reads from (and its subfolders): any file whose text starts with ISA or ST, whatever its extension. Paths are relative to the folder, as the other tools take them.")]
    public string ListFiles(
        [Description("A subfolder to list instead of the whole folder. Optional.")] string? folder = null) =>
        Run(() =>
        {
            var (list, more) = files.List(folder);
            var result = new JsonObject { ["folder"] = files.Root, ["count"] = list.Count, ["files"] = new JsonArray(list.Select(f => (JsonNode)f!).ToArray()) };
            if (more) result["note"] = $"Only the first {Workspace.MaxListed} are listed. Name a subfolder to see the rest.";
            if (list.Count == 0) result["note"] = "No X12 files here. Files must start with an ISA (or ST) segment.";
            return result;
        });

    [McpServerTool(Name = "open_file", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
    [Description("Reads an X12 file and describes it: its delimiters, each interchange (ISA: sender, receiver, date, control number, test or production), each functional group (GS), and each transaction (ST01 code and name, control number, segment and loop counts), plus how many structural problems check_file would report. Start here.")]
    public string OpenFile(
        [Description("The file, relative to the folder (list_files shows them).")] string file) =>
        Run(() =>
        {
            var x = files.Load(file);
            var d = x.Delimiters;
            var interchanges = x.Segments.Where(s => s.Id == "ISA").Select(s => (JsonNode)new JsonObject
            {
                ["segment"] = s.Index + 1,
                ["sender"] = $"{s[5].Trim()}:{s[6].Trim()}",
                ["receiver"] = $"{s[7].Trim()}:{s[8].Trim()}",
                ["date"] = s[9].Trim(),
                ["time"] = s[10].Trim(),
                ["version"] = s[12].Trim(),
                ["control_number"] = s[13].Trim(),
                ["usage"] = s[15].Trim() switch { "P" => "Production", "T" => "Test", var u => u },
            }).ToArray();
            var groups = x.Segments.Where(s => s.Id == "GS").Select(s => (JsonNode)new JsonObject
            {
                ["segment"] = s.Index + 1,
                ["functional_id"] = s[1].Trim(),
                ["sender"] = s[2].Trim(),
                ["receiver"] = s[3].Trim(),
                ["date"] = s[4].Trim(),
                ["control_number"] = s[6].Trim(),
                ["version"] = s[8].Trim(),
            }).ToArray();
            var transactions = x.Transactions.Select((t, i) =>
            {
                var o = new JsonObject
                {
                    ["transaction"] = i + 1,
                    ["segment"] = t.Start.Index + 1,
                    ["code"] = t.Code,
                    ["name"] = t.Kind?.Title ?? "Not one the explorer has a tab for",
                    ["control_number"] = t.Control,
                    ["segments"] = t.Body.Count + (t.Trailer != null ? 2 : 1),
                };
                if (t.Kind is { } kind) o["loops"] = new JsonObject { ["count"] = t.SplitAt(kind.LoopStart, kind.Closing).Loops.Count, ["start_segment"] = kind.LoopStart };
                if (t.Trailer == null) o["note"] = "The file ends before this transaction's SE.";
                return (JsonNode)o;
            }).ToArray();

            return new JsonObject
            {
                ["file"] = file,
                ["segments"] = x.Segments.Count,
                ["delimiters"] = new JsonObject
                {
                    ["segment"] = d.LineTerminated ? "line break" : d.Segment.ToString(),
                    ["element"] = d.Element.ToString(),
                    ["component"] = d.Component.ToString(),
                },
                ["interchanges"] = new JsonArray(interchanges),
                ["groups"] = new JsonArray(groups),
                ["transactions"] = new JsonArray(transactions),
                ["problems"] = Checks.Of(x).Count,
            };
        });

    [McpServerTool(Name = "get_transaction", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
    [Description("Shows one transaction's segments as the explorer's tab does: the header (envelope, ST and everything before the first loop), the loops (each starting at its transaction's loop segment: ENT for an 820, INS for an 834, CLP for an 835, AK2 for a 999, HL for the others) and the trailer (SE, GE, IEA). Each segment comes with its number in the file, for get_segment. Long transactions come a page of loops at a time.")]
    public string GetTransaction(
        [Description("The file, relative to the folder.")] string file,
        [Description("Which transaction, counting from 1 (open_file lists them). Default 1.")] int transaction = 1,
        [Description("The first loop to show, counting from 1. Default 1.")] int first_loop = 1,
        [Description("How many loops to show, at most 100. Default 25.")] int loops = 25,
        [Description("The segment a loop starts at, to split differently from the tab, e.g. \"NM1\". Optional.")] string? loop_start = null) =>
        Run(() =>
        {
            var x = files.Load(file);
            var t = Transaction(x, transaction);
            var kind = t.Kind;
            var start = string.IsNullOrWhiteSpace(loop_start) ? kind?.LoopStart ?? "HL" : loop_start.Trim().ToUpperInvariant();
            var closing = string.IsNullOrWhiteSpace(loop_start) ? kind?.Closing : null;
            var (header, body, close) = t.SplitAt(start, closing);
            if (first_loop < 1) throw new ArgumentException("first_loop counts from 1.");
            if (loops < 1) throw new ArgumentException("loops must be at least 1.");
            loops = Math.Min(loops, MaxLoops);

            var head = new JsonArray();
            foreach (var s in t.Before) head.Add(Line(s));
            if (t.Interchange is { } isa) head.Add(Line(isa));
            if (t.Group is { } gs) head.Add(Line(gs));
            head.Add(Line(t.Start));
            foreach (var s in header) head.Add(Line(s));

            var page = body.Skip(first_loop - 1).Take(loops).Select((loop, i) => (JsonNode)new JsonObject
            {
                ["loop"] = first_loop + i,
                ["segments"] = new JsonArray(loop.Select(Line).ToArray()),
            }).ToArray();

            var tail = new JsonArray();
            foreach (var s in close) tail.Add(Line(s));
            if (t.Trailer is { } se) tail.Add(Line(se));
            if (t.GroupEnd is { } ge) tail.Add(Line(ge));
            if (t.InterchangeEnd is { } iea) tail.Add(Line(iea));

            var result = new JsonObject
            {
                ["transaction"] = transaction,
                ["code"] = t.Code,
                ["name"] = kind?.Title ?? "Not one the explorer has a tab for",
                ["loop_start"] = start,
                ["loop_count"] = body.Count,
                ["header"] = head,
                ["loops"] = new JsonArray(page),
                ["trailer"] = tail,
            };
            var next = first_loop + page.Length;
            if (next <= body.Count) result["next_loop"] = next;
            if (body.Count == 0) result["note"] = $"No {start} segments here, so there are no loops. Try another loop_start.";
            return result;
        });

    [McpServerTool(Name = "get_segment", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
    [Description("Lists one segment's elements by position, as the explorer's element strip does (CLP01, CLP02…), with each composite element split into its components (SVC01-1, SVC01-2…). Also says which transaction and loop the segment is in.")]
    public string GetSegment(
        [Description("The file, relative to the folder.")] string file,
        [Description("The segment's number in the file, counting from 1 (the other tools give it).")] int segment) =>
        Run(() =>
        {
            var x = files.Load(file);
            if (segment < 1 || segment > x.Segments.Count) throw new ArgumentException($"The file has segments 1 to {x.Segments.Count}; there's no segment {segment}.");
            var s = x.Segments[segment - 1];
            var elements = new JsonArray();
            for (var i = 1; i < s.Elements.Length; i++)
            {
                var name = $"{s.Id}{i:00}";
                var value = s.Elements[i];
                var e = new JsonObject { ["name"] = name, ["value"] = value };
                var parts = value.Split(x.Delimiters.Component);
                if (s.Id != "ISA" && parts.Length > 1)
                    e["components"] = new JsonArray(parts.Select((p, j) => (JsonNode)new JsonObject { ["name"] = $"{name}-{j + 1}", ["value"] = p }).ToArray());
                elements.Add(e);
            }
            var result = new JsonObject { ["segment"] = segment, ["id"] = s.Id, ["text"] = s.Text, ["elements"] = elements };
            Place(x, s, result);
            return result;
        });

    [McpServerTool(Name = "find_segments", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
    [Description("Finds segments by ID, optionally where one element has a value: every CLP whose CLP02 is 4 (denied claims), every NM1 whose NM101 is IL, every CAS whose CAS01 is PR. Each match comes with its number, its transaction and loop, and its elements.")]
    public string FindSegments(
        [Description("The file, relative to the folder.")] string file,
        [Description("The segment ID, e.g. \"CLP\", \"NM1\", \"REF\".")] string segment_id,
        [Description("An element position to test, counting from 1 (2 for CLP02). Optional; needs value.")] int? element = null,
        [Description("The value that element must have (compared ignoring case and surrounding spaces). With no element, any element may have it.")] string? value = null,
        [Description("Only this transaction, counting from 1. Optional.")] int? transaction = null) =>
        Run(() =>
        {
            var x = files.Load(file);
            if (string.IsNullOrWhiteSpace(segment_id)) throw new ArgumentException("Name the segment ID to look for, e.g. \"CLP\".");
            if (element is < 1) throw new ArgumentException("element counts from 1 (CLP02 is 2).");
            if (element != null && value == null) throw new ArgumentException("Give the value element must have.");
            var id = segment_id.Trim().ToUpperInvariant();
            IEnumerable<Segment> pool = x.Segments;
            if (transaction != null)
            {
                var t = Transaction(x, transaction.Value);
                pool = [t.Start, .. t.Body, .. t.Trailer is { } se ? [se] : Array.Empty<Segment>()];
            }

            bool Same(string a) => string.Equals(a.Trim(), value!.Trim(), StringComparison.OrdinalIgnoreCase);
            var found = pool.Where(s => s.Id == id && (value == null || (element is { } e ? Same(s[e]) : s.Elements.Skip(1).Any(Same)))).ToList();

            var matches = found.Take(MaxFound).Select(s =>
            {
                var o = new JsonObject { ["segment"] = s.Index + 1, ["text"] = s.Text };
                Place(x, s, o);
                return (JsonNode)o;
            }).ToArray();
            var result = new JsonObject { ["count"] = found.Count, ["matches"] = new JsonArray(matches) };
            if (found.Count > MaxFound) result["note"] = $"Only the first {MaxFound} are listed. Narrow it with element and value, or transaction.";
            if (found.Count == 0 && !x.Segments.Any(s => s.Id == id)) result["note"] = $"The file has no {id} segments at all.";
            return result;
        });

    [McpServerTool(Name = "summarize", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
    [Description("Reads a transaction the way its implementation guide lays it out, with code meanings beside the codes. 835: the payment, each claim's status, charge, payment and adjustments, the totals, and whether the payment balances. 837: each claim's amount, patient, provider, diagnoses and lines. 834: each member's maintenance, relationship and coverages. 820: the payment and each entity's remittances, and whether they balance. 999: each acknowledged transaction's result and its errors, down to the element. 277: each claim status. 271: each person's benefits. 270 and 276: the HL hierarchy.")]
    public string Summarize(
        [Description("The file, relative to the folder.")] string file,
        [Description("Which transaction, counting from 1. Default 1.")] int transaction = 1) =>
        Run(() =>
        {
            var x = files.Load(file);
            return Summaries.Of(Transaction(x, transaction), x.Delimiters);
        });

    [McpServerTool(Name = "check_file", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
    [Description("Checks the envelope's bookkeeping, the counts and control numbers a receiver checks first: SE01 against the transaction's segment count and SE02 against ST02, GE01 against the group's transactions and GE02 against GS06, IEA01 against the interchange's groups and IEA02 against ISA13, and that every ST, GS and ISA is closed. It doesn't check the implementation guide's rules for each transaction.")]
    public string CheckFile(
        [Description("The file, relative to the folder.")] string file) =>
        Run(() =>
        {
            var problems = Checks.Of(files.Load(file));
            return new JsonObject
            {
                ["file"] = file,
                ["ok"] = problems.Count == 0,
                ["problems"] = new JsonArray(problems.Select(p => (JsonNode)new JsonObject { ["segment"] = p.Segment, ["problem"] = p.Message }).ToArray()),
            };
        });

    // ───────────────────────────── helpers ─────────────────────────────

    private static Transaction Transaction(X12File x, int number)
    {
        if (x.Transactions.Count == 0) throw new ArgumentException("The file has no transactions (no ST segment).");
        if (number < 1 || number > x.Transactions.Count)
            throw new ArgumentException(x.Transactions.Count == 1
                ? $"The file has one transaction; there's no transaction {number}."
                : $"The file has transactions 1 to {x.Transactions.Count}; there's no transaction {number}.");
        return x.Transactions[number - 1];
    }

    private static JsonObject Line(Segment s) => new() { ["segment"] = s.Index + 1, ["text"] = s.Text };

    /// <summary>Adds the transaction and loop <paramref name="s"/> is in to <paramref name="o"/>.</summary>
    private static void Place(X12File x, Segment s, JsonObject o)
    {
        for (var i = 0; i < x.Transactions.Count; i++)
        {
            var t = x.Transactions[i];
            var last = t.Trailer?.Index ?? (t.Body.Count > 0 ? t.Body[^1].Index : t.Start.Index);
            if (s.Index < t.Start.Index || s.Index > last) continue;
            o["transaction"] = i + 1;
            if (t.Kind is { } kind)
            {
                var loops = t.SplitAt(kind.LoopStart, kind.Closing).Loops;
                for (var n = 0; n < loops.Count; n++)
                    if (loops[n].Contains(s)) o["loop"] = n + 1;
            }
            return;
        }
    }

    /// <summary>Runs a handler, turning what goes wrong into an MCP tool error an agent can act on.</summary>
    private static string Run(Func<JsonObject> work)
    {
        try
        {
            return work().ToJsonString();
        }
        catch (Exception e) when (e is ArgumentException or IOException or UnauthorizedAccessException)
        {
            throw new McpException(e.Message);
        }
    }
}
