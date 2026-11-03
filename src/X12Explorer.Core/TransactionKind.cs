namespace X12Explorer.Core;

/// <summary>
/// One of the HIPAA transactions the explorer has a tab for: its ST01 code, the tab's name, and the segment that
/// starts each loop shown in the tab's middle box (ENT for the 820, INS for the 834, CLP for the 835, HL for the
/// rest; AK2 for the 999). <see cref="Expanded"/> is how the tab first shows each loop: one segment per line, or the
/// whole loop on one. <see cref="Closing"/>, when set, is a segment that ends the loops (the 999's AK9): it and what
/// follows it go to the trailer box instead of the last loop.
/// </summary>
public sealed record TransactionKind(string Code, string Name, string LoopStart, bool Expanded, string? Closing = null)
{
    public static readonly TransactionKind Capitation = new("820", "Capitation", "ENT", false);
    public static readonly TransactionKind Membership = new("834", "Membership", "INS", false);
    public static readonly TransactionKind Remittance = new("835", "Remittance", "CLP", false);
    public static readonly TransactionKind Eligibility = new("270", "Eligibility", "HL", false);
    public static readonly TransactionKind BenefitInfo = new("271", "Benefit Info", "HL", false);
    public static readonly TransactionKind Claim = new("837", "Claim", "HL", true);
    public static readonly TransactionKind StatusRequest = new("276", "Status Request", "HL", true);
    public static readonly TransactionKind ClaimStatus = new("277", "Claim Status", "HL", true);
    public static readonly TransactionKind Acknowledgment = new("999", "Acknowledgment", "AK2", true, "AK9");

    /// <summary>The tabs, in the order the window shows them: each request beside its response.</summary>
    public static readonly IReadOnlyList<TransactionKind> All =
        [Capitation, Membership, Remittance, Eligibility, BenefitInfo, Claim, StatusRequest, ClaimStatus, Acknowledgment];

    /// <summary>The codes in tab order, for messages: "820, 834, … and 999".</summary>
    public static string Codes => string.Join(", ", All.Take(All.Count - 1).Select(k => k.Code)) + " and " + All[^1].Code;

    /// <summary>The kind whose ST01 is <paramref name="code"/>, or null when the explorer has no tab for it.</summary>
    public static TransactionKind? ForCode(string code) => All.FirstOrDefault(k => k.Code == code.Trim());

    /// <summary>The tab's title, as the old program had it: "Capitation (820)".</summary>
    public string Title => $"{Name} ({Code})";
}
