namespace X12Explorer.Core;

/// <summary>A problem with a file's structure: the segment it was found at (1 for the first) and what is wrong.</summary>
public sealed record Problem(int Segment, string Message);

/// <summary>
/// Checks the envelope's own bookkeeping: each SE counts its transaction's segments and repeats ST02, each GE counts
/// its group's transactions and repeats GS06, each IEA counts its interchange's groups and repeats ISA13, and every
/// ST, GS and ISA is closed. These are the counts a receiver's translator checks first, and the ones hand edits break.
/// </summary>
public static class Checks
{
    public static IReadOnlyList<Problem> Of(X12File file)
    {
        var problems = new List<Problem>();
        Segment? isa = null, gs = null, st = null;
        int groups = 0, sets = 0, inSet = 0;

        foreach (var s in file.Segments)
        {
            var at = s.Index + 1;
            if (st != null) inSet++;
            switch (s.Id)
            {
                case "ISA":
                    if (isa != null) problems.Add(new(at, $"ISA {Trim(isa[13])} has no IEA before the next ISA."));
                    if (s.Elements.Length != 17) problems.Add(new(at, $"ISA has {s.Elements.Length - 1} elements; it should have 16."));
                    isa = s;
                    groups = 0;
                    break;
                case "GS":
                    if (gs != null) problems.Add(new(at, $"GS {Trim(gs[6])} has no GE before the next GS."));
                    if (isa == null) problems.Add(new(at, "GS is outside any ISA interchange."));
                    gs = s;
                    sets = 0;
                    groups++;
                    break;
                case "ST":
                    if (st != null) problems.Add(new(at, $"ST {Trim(st[2])} has no SE before the next ST."));
                    if (gs == null) problems.Add(new(at, "ST is outside any GS group."));
                    st = s;
                    inSet = 1;
                    sets++;
                    break;
                case "SE" when st != null:
                    if (!int.TryParse(s[1], out var count)) problems.Add(new(at, $"SE01 \"{s[1]}\" is not a number."));
                    else if (count != inSet) problems.Add(new(at, $"SE01 says {count} segments; transaction {Trim(st[2])} has {inSet} (ST to SE)."));
                    if (Trim(s[2]) != Trim(st[2])) problems.Add(new(at, $"SE02 \"{Trim(s[2])}\" doesn't match ST02 \"{Trim(st[2])}\"."));
                    st = null;
                    break;
                case "GE" when gs != null:
                    if (st != null)
                    {
                        problems.Add(new(at, $"ST {Trim(st[2])} has no SE before GE."));
                        st = null;
                    }
                    if (!int.TryParse(s[1], out var setCount)) problems.Add(new(at, $"GE01 \"{s[1]}\" is not a number."));
                    else if (setCount != sets) problems.Add(new(at, $"GE01 says {setCount} transactions; group {Trim(gs[6])} has {sets}."));
                    if (Trim(s[2]) != Trim(gs[6])) problems.Add(new(at, $"GE02 \"{Trim(s[2])}\" doesn't match GS06 \"{Trim(gs[6])}\"."));
                    gs = null;
                    break;
                case "IEA" when isa != null:
                    if (gs != null)
                    {
                        problems.Add(new(at, $"GS {Trim(gs[6])} has no GE before IEA."));
                        gs = null;
                    }
                    if (!int.TryParse(s[1], out var groupCount)) problems.Add(new(at, $"IEA01 \"{s[1]}\" is not a number."));
                    else if (groupCount != groups) problems.Add(new(at, $"IEA01 says {groupCount} groups; interchange {Trim(isa[13])} has {groups}."));
                    if (Trim(s[2]) != Trim(isa[13])) problems.Add(new(at, $"IEA02 \"{Trim(s[2])}\" doesn't match ISA13 \"{Trim(isa[13])}\"."));
                    isa = null;
                    break;
                case "SE" or "GE" or "IEA":
                    problems.Add(new(at, $"{s.Id} closes nothing: there's no open {(s.Id == "SE" ? "ST" : s.Id == "GE" ? "GS" : "ISA")}."));
                    break;
            }
        }

        var end = file.Segments.Count;
        if (st != null) problems.Add(new(end, $"The file ends inside transaction {Trim(st[2])}: there's no SE."));
        if (gs != null) problems.Add(new(end, $"The file ends inside group {Trim(gs[6])}: there's no GE."));
        if (isa != null) problems.Add(new(end, $"The file ends inside interchange {Trim(isa[13])}: there's no IEA."));
        if (file.Segments.Count > 0 && file.Segments[0].Id != "ISA") problems.Add(new(1, $"The file starts with {file.Segments[0].Id}, not ISA."));
        return problems.OrderBy(p => p.Segment).ToList();
    }

    private static string Trim(string s) => s.Trim();
}
