namespace X12Explorer.Core;

/// <summary>
/// One of the HIPAA transactions the explorer has a tab for: its ST01 code, the tab's name, and the segment that
/// starts each loop shown in the tab's middle box (ENT for the 820, INS for the 834, CLP for the 835, HL for the
/// rest). <see cref="Expanded"/> is how the tab first shows each loop: one segment per line, or the whole loop on one.
/// </summary>
public sealed record TransactionKind(string Code, string Name, string LoopStart, bool Expanded)
{
    public static readonly TransactionKind Capitation = new("820", "Capitation", "ENT", false);
    public static readonly TransactionKind Membership = new("834", "Membership", "INS", false);
    public static readonly TransactionKind Remittance = new("835", "Remittance", "CLP", false);
    public static readonly TransactionKind BenefitInfo = new("271", "Benefit Info", "HL", false);
    public static readonly TransactionKind Claim = new("837", "Claim", "HL", true);
    public static readonly TransactionKind ClaimStatus = new("277", "Claim Status Response", "HL", true);

    /// <summary>The tabs, in the order the window shows them.</summary>
    public static readonly IReadOnlyList<TransactionKind> All =
        [Capitation, Membership, Remittance, BenefitInfo, Claim, ClaimStatus];

    /// <summary>The kind whose ST01 is <paramref name="code"/>, or null when the explorer has no tab for it.</summary>
    public static TransactionKind? ForCode(string code) => All.FirstOrDefault(k => k.Code == code.Trim());

    /// <summary>The tab's title, as the old program had it: "Capitation (820)".</summary>
    public string Title => $"{Name} ({Code})";
}
