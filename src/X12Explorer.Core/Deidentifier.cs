using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace X12Explorer.Core;

/// <summary>What <see cref="Deidentifier.Apply"/> replaced, and the text it made.</summary>
public sealed record Deidentified(string Text, int Names, int Identifiers, int Dates, int Addresses, int Contacts, int Notes)
{
    public int Total => Names + Identifiers + Dates + Addresses + Contacts + Notes;

    /// <summary>"2 names, 5 identifiers, 7 dates and 1 address": what was replaced, leaving out what wasn't.</summary>
    public string Summary
    {
        get
        {
            var parts = new[] { (Names, "name"), (Identifiers, "identifier"), (Dates, "date"), (Addresses, "address"), (Contacts, "contact number"), (Notes, "note") }
                .Where(p => p.Item1 > 0)
                .Select(p => $"{p.Item1} {p.Item2}{(p.Item1 == 1 ? "" : p.Item2.EndsWith("ss") ? "es" : "s")}")
                .ToList();
            return parts.Count switch
            {
                0 => "nothing to replace",
                1 => parts[0],
                _ => string.Join(", ", parts[..^1]) + " and " + parts[^1],
            };
        }
    }
}

/// <summary>
/// Replaces the identifiers HIPAA's Safe Harbor method lists, as they appear in X12 files, so a file can be shared,
/// used as test data or read by an AI model:
/// <list type="bullet">
/// <item>people's names (NM1 for the insured, patient, dependent and the like; organisations' names stay),</item>
/// <item>their identifiers: member IDs and SSNs (NM109, ENT04, person REFs), patient account and claim numbers
///   (CLM01, CLP01, CLP07, the patient's TRN, 999 CTX), policy references (an 820's RMR02),</item>
/// <item>their street addresses and cities (N3, N4 after a person; ZIP codes keep their first three digits, or none
///   where those cover fewer than 20,000 people),</item>
/// <item>dates (DTP, DTM, STC02), all moved back by the same number of days so intervals stay true; birth dates
///   (DMG02) keep only their year, and anyone over 89 is shown as 90,</item>
/// <item>phone numbers and e-mail addresses after a person (PER), and free text (NTE, MSG, K3).</item>
/// </list>
/// Replacements are consistent: the same member ID, name or account number gets the same stand-in everywhere this
/// instance sees it, across files too, so an 837's claims still match their 835 payments and 999 errors. They come
/// from a keyed hash with a random key, so they can't be reversed without it. Everything else (amounts, codes,
/// diagnoses, providers, payers, the envelope) is left as it was, and the file keeps its layout, delimiters and
/// segment counts.
/// </summary>
public sealed class Deidentifier
{
    // NM101 codes for people whose names and IDs are protected: the insured, the patient, a dependent, a corrected or
    // prior insured, a mailing address, a guardian or other responsible person, a parent, other insureds.
    private static readonly HashSet<string> PersonRoles =
        ["IL", "QC", "03", "70", "74", "31", "36", "E1", "EXS", "GB", "GD", "J6", "LR", "QD", "S1", "S3", "TZ"];

    // REF01 qualifiers whose REF02 identifies a person or their claim.
    private static readonly HashSet<string> PersonReferences =
        ["0F", "1L", "1W", "23", "3H", "6O", "ABB", "EA", "EJ", "F6", "IG", "Q4", "SY", "D9", "G1", "9F"];

    // REF01 qualifiers for the payer's claim number, matched with CLP07.
    private static readonly HashSet<string> PayerClaimReferences = ["1K", "F8"];

    // Three-digit ZIP prefixes covering 20,000 people or fewer, which Safe Harbor says to show as 000.
    private static readonly HashSet<string> SmallZips =
        ["036", "059", "063", "102", "203", "556", "692", "790", "821", "823", "830", "831", "878", "879", "884", "890", "893"];

    private static readonly string[] LastNames =
    [
        "ASHFORD", "BELLAMY", "CARVER", "DALTON", "ELLERY", "FAIRBANK", "GARLAND", "HADLEY", "IVERSON", "JARRETT",
        "KENDRICK", "LANGLEY", "MERCER", "NORWOOD", "OAKLEY", "PRESCOTT", "QUINLAN", "RADCLIFF", "SUTTON", "TALBOT",
        "UPTON", "VANCE", "WHITLOCK", "YARDLEY", "ABERNATHY", "BRADDOCK", "CHANDLER", "DRUMMOND", "EASTON", "FENWICK",
        "GRAYSON", "HOLLOWAY",
    ];

    private static readonly string[] FirstNames =
    [
        "ALEX", "BLAIR", "CAMERON", "DANA", "ELLIS", "FRANCIS", "GALE", "HAYDEN", "JORDAN", "KENDALL", "LANE", "MORGAN",
        "NOEL", "PARKER", "QUINN", "REESE", "SAWYER", "TAYLOR", "AVERY", "BAILEY", "CASEY", "DREW", "EMERSON", "FINLEY",
        "HARPER", "JESSIE", "KAI", "LOGAN", "MARLOWE", "PEYTON", "RILEY", "SKYLER",
    ];

    private static readonly string[] Streets =
    [
        "MAPLE", "OAK", "CEDAR", "ELM", "PINE", "BIRCH", "WILLOW", "ASPEN", "CHESTNUT", "SPRUCE", "LAUREL", "HAWTHORN",
    ];

    private readonly byte[] _key;
    private readonly DateOnly _today;

    /// <param name="key">The key replacements are made with; a random one when null. The same key gives the same stand-ins.</param>
    /// <param name="today">The date ages are worked out from; today when null.</param>
    public Deidentifier(byte[]? key = null, DateOnly? today = null)
    {
        _key = key ?? RandomNumberGenerator.GetBytes(32);
        _today = today ?? DateOnly.FromDateTime(DateTime.Today);
        // Between 30 and 180 days back, the same for every date.
        ShiftDays = -(30 + (int)(Hash("shift:") % 151));
    }

    /// <summary>How many days every date is moved by (negative: back).</summary>
    public int ShiftDays { get; }

    /// <summary>The file read and de-identified.</summary>
    public Deidentified Load(string path) => Apply(File.ReadAllText(path));

    /// <summary>De-identifies X12 <paramref name="text"/>, keeping its layout.</summary>
    public Deidentified Apply(string text) => new Pass(this, Delimiters.Detect(text)).Run(text);

    /// <summary>One file's worth of replacing: where in it we are, and the counts.</summary>
    private sealed class Pass(Deidentifier owner, Delimiters d)
    {
        private int _names, _ids, _dates, _addresses, _contacts, _notes;
        private string _code = "";
        private string _level = "";
        private bool _person;
        private bool _inEntity;

        public Deidentified Run(string text)
        {
            var output = new StringBuilder(text.Length);
            var i = 0;
            bool IsBreak(char c) => c is '\r' or '\n' || (!d.LineTerminated && c == d.Segment);

            while (i < text.Length)
            {
                // The terminators and line breaks between segments, as they were.
                var from = i;
                while (i < text.Length && IsBreak(text[i])) i++;
                output.Append(text, from, i - from);

                from = i;
                while (i < text.Length && !IsBreak(text[i])) i++;
                if (i == from) continue;

                // Spaces, tabs and a byte order mark around a segment stay too; only the segment itself changes.
                var raw = text[from..i];
                var core = raw.Trim('﻿', ' ', '\t');
                if (core.Length == 0)
                {
                    output.Append(raw);
                    continue;
                }
                var lead = raw.IndexOf(core, StringComparison.Ordinal);
                output.Append(raw, 0, lead).Append(Rewrite(core)).Append(raw, lead + core.Length, raw.Length - lead - core.Length);
            }

            return new Deidentified(output.ToString(), _names, _ids, _dates, _addresses, _contacts, _notes);
        }

        private string Rewrite(string text)
        {
            var e = text.Split(d.Element);
            var id = e[0].Trim();
            string At(int n) => n < e.Length ? e[n] : "";
            bool Has(int n) => n < e.Length && e[n].Trim().Length > 0;

            switch (id)
            {
                case "ISA" or "GS" or "GE" or "IEA" or "SE":
                    return text;
                case "ST":
                    _code = At(1).Trim();
                    _level = "";
                    _person = _inEntity = false;
                    return text;
                case "HL":
                    _level = At(3).Trim();
                    _person = false;
                    return text;
                case "INS":
                    _person = false;
                    return text;
                case "ENT":
                    _inEntity = true;
                    _person = false;
                    if (At(2).Trim() == "2J" && Has(4)) Replace(e, 4, "id", ref _ids);
                    break;
                case "N1":
                    _person = false;
                    return text;
                case "NM1":
                    _person = PersonRoles.Contains(At(1).Trim());
                    if (!_person) return text;
                    if (At(2).Trim() != "2")
                    {
                        // A person: last, first and middle names; the suffix (JR, III) says nothing on its own.
                        if (Has(3)) e[3] = owner.Pick(LastNames, "last:" + e[3].Trim());
                        if (Has(4)) e[4] = owner.Pick(FirstNames, "first:" + e[4].Trim());
                        if (Has(5)) e[5] = owner.Pick(FirstNames, "middle:" + e[5].Trim())[..1];
                        if (Has(3) || Has(4)) _names++;
                    }
                    if (Has(9)) Replace(e, 9, "id", ref _ids);
                    break;
                case "N3" when _person:
                    var house = 100 + (int)(owner.Hash("house:" + At(1)) % 900);
                    e[1] = $"{house} {owner.Pick(Streets, "street:" + At(1))} ST";
                    if (e.Length > 2) e = e[..2];
                    _addresses++;
                    break;
                case "N4" when _person:
                    if (Has(1)) e[1] = "ANYTOWN";
                    if (Has(3)) e[3] = Zip(e[3]);
                    if (Has(1) || Has(3)) _addresses++;
                    break;
                case "PER" when _person:
                    // Pairs from PER03: the kind of number, then the number.
                    for (var n = 4; n < e.Length; n += 2)
                    {
                        if (!Has(n)) continue;
                        e[n] = At(n - 1).Trim() == "EM" ? $"member{owner.Hash("mail:" + e[n]) % 10000:0000}@example.com" : owner.Code("tel:", e[n]);
                        _contacts++;
                    }
                    if (Has(2)) e[2] = "CONTACT";
                    break;
                case "DMG":
                    if (Has(2) && At(1).Trim() == "D8" && BirthYear(e[2]) is { } birth)
                    {
                        e[2] = birth;
                        _dates++;
                    }
                    break;
                case "DTP":
                    if (Has(3) && Shift(At(2).Trim(), e[3]) is { } shifted)
                    {
                        e[3] = shifted;
                        _dates++;
                    }
                    break;
                case "DTM":
                    if (Has(2) && Shift("D8", e[2]) is { } day)
                    {
                        e[2] = day;
                        _dates++;
                    }
                    if (Has(6) && Shift(At(5).Trim(), e[6]) is { } period)
                    {
                        e[6] = period;
                        _dates++;
                    }
                    break;
                case "STC":
                    if (Has(2) && Shift("D8", e[2]) is { } status)
                    {
                        e[2] = status;
                        _dates++;
                    }
                    break;
                case "REF":
                    var qualifier = At(1).Trim();
                    if (Has(2) && PersonReferences.Contains(qualifier)) Replace(e, 2, qualifier is "D9" or "G1" or "9F" ? "ref" : "id", ref _ids);
                    else if (Has(2) && PayerClaimReferences.Contains(qualifier)) Replace(e, 2, "payerclaim", ref _ids);
                    break;
                case "CLM":
                    if (Has(1)) Replace(e, 1, "account", ref _ids);
                    break;
                case "CLP":
                    if (Has(1)) Replace(e, 1, "account", ref _ids);
                    if (Has(7)) Replace(e, 7, "payerclaim", ref _ids);
                    break;
                case "TRN" when _level is "22" or "23" or "PT":
                    // At the subscriber or patient level a TRN carries the claim's patient account number.
                    if (Has(2)) Replace(e, 2, _code is "276" or "277" ? "account" : "trace", ref _ids);
                    break;
                case "RMR" when _inEntity && _code == "820":
                    if (Has(2)) Replace(e, 2, "id", ref _ids);
                    break;
                case "CTX":
                    // A 999's context: "CLM01:PATACCT0002" names the claim by its account number.
                    var parts = At(1).Split(d.Component);
                    if (parts.Length > 1 && parts[0].Trim() == "CLM01")
                    {
                        parts[1] = owner.Code("account:", parts[1]);
                        e[1] = string.Join(d.Component, parts);
                        _ids++;
                    }
                    break;
                case "NTE":
                    if (Has(2)) Blank(e, 2);
                    break;
                case "MSG" or "K3":
                    if (Has(1)) Blank(e, 1);
                    break;
                default:
                    return text;
            }
            return string.Join(d.Element, e);
        }

        private void Replace(string[] e, int n, string kind, ref int count)
        {
            e[n] = owner.Code(kind + ":", e[n]);
            count++;
        }

        private void Blank(string[] e, int n)
        {
            e[n] = "NOTE REMOVED";
            _notes++;
        }

        /// <summary>A ZIP code's first three digits, the rest zeros; all zeros where those three cover too few people.</summary>
        private static string Zip(string zip)
        {
            var z = zip.Trim();
            if (z.Length < 3) return new string('0', z.Length);
            var prefix = SmallZips.Contains(z[..3]) ? "000" : z[..3];
            return prefix + new string('0', z.Length - 3);
        }

        /// <summary>A birth date as January 1 of its year, and no earlier than 90 years ago.</summary>
        private string? BirthYear(string value)
        {
            if (!TryDate(value.Trim(), out var date)) return null;
            var year = owner._today.Year - date.Year > 89 ? owner._today.Year - 90 : date.Year;
            return $"{year:0000}0101";
        }

        /// <summary>A date (D8), range (RD8) or date and time (DT) moved by <see cref="ShiftDays"/>; null when it isn't one.</summary>
        private string? Shift(string format, string value)
        {
            var v = value.Trim();
            switch (format)
            {
                case "D8" when TryDate(v, out var date):
                    return Format(date.AddDays(owner.ShiftDays));
                case "RD8" when v.Split('-') is [var a, var b] && TryDate(a, out var start) && TryDate(b, out var end):
                    return $"{Format(start.AddDays(owner.ShiftDays))}-{Format(end.AddDays(owner.ShiftDays))}";
                case "DT" when v.Length > 8 && TryDate(v[..8], out var stamp):
                    return Format(stamp.AddDays(owner.ShiftDays)) + v[8..];
                default:
                    return null;
            }
        }

        private static bool TryDate(string v, out DateOnly date) =>
            DateOnly.TryParseExact(v, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

        private static string Format(DateOnly d) => d.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// A stand-in for <paramref name="value"/> of the same shape: each digit becomes a digit and each letter a letter
    /// of the same case, from a keyed hash, so the same value always gets the same stand-in.
    /// </summary>
    private string Code(string kind, string value)
    {
        var trimmed = value.Trim();
        var bytes = HMACSHA256.HashData(_key, Encoding.UTF8.GetBytes(kind + trimmed));
        var result = new StringBuilder(trimmed.Length);
        for (var i = 0; i < trimmed.Length; i++)
        {
            var c = trimmed[i];
            var b = bytes[i % bytes.Length] + i / bytes.Length;
            result.Append(c switch
            {
                >= '0' and <= '9' => (char)('0' + b % 10),
                >= 'A' and <= 'Z' => (char)('A' + b % 26),
                >= 'a' and <= 'z' => (char)('a' + b % 26),
                _ => c,
            });
        }
        return result.ToString();
    }

    private string Pick(string[] list, string value) => list[Hash(value.ToUpperInvariant()) % (ulong)list.Length];

    private ulong Hash(string value) => BitConverter.ToUInt64(HMACSHA256.HashData(_key, Encoding.UTF8.GetBytes(value)));
}
