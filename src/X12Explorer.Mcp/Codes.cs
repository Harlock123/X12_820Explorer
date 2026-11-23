namespace X12Explorer.Mcp;

/// <summary>
/// What the codes a summary reports mean, for the code lists small enough to carry here. An agent gets the code and
/// its meaning side by side; a code not listed comes back on its own.
/// </summary>
internal static class Codes
{
    /// <summary>CLP02, an 835 claim's status.</summary>
    public static readonly Dictionary<string, string> ClaimStatus = new()
    {
        ["1"] = "Processed as Primary",
        ["2"] = "Processed as Secondary",
        ["3"] = "Processed as Tertiary",
        ["4"] = "Denied",
        ["19"] = "Processed as Primary, Forwarded to Additional Payer(s)",
        ["20"] = "Processed as Secondary, Forwarded to Additional Payer(s)",
        ["21"] = "Processed as Tertiary, Forwarded to Additional Payer(s)",
        ["22"] = "Reversal of Previous Payment",
        ["23"] = "Not Our Claim, Forwarded to Additional Payer(s)",
        ["25"] = "Predetermination Pricing Only - No Payment",
    };

    /// <summary>CAS01, who an 835 adjustment falls to.</summary>
    public static readonly Dictionary<string, string> AdjustmentGroup = new()
    {
        ["CO"] = "Contractual Obligations",
        ["CR"] = "Correction and Reversals",
        ["OA"] = "Other Adjustments",
        ["PI"] = "Payor Initiated Reductions",
        ["PR"] = "Patient Responsibility",
    };

    /// <summary>INS03, an 834 member's maintenance type.</summary>
    public static readonly Dictionary<string, string> Maintenance = new()
    {
        ["001"] = "Change",
        ["021"] = "Addition",
        ["024"] = "Cancellation or Termination",
        ["025"] = "Reinstatement",
        ["030"] = "Audit or Compare",
    };

    /// <summary>INS02, an 834 member's relationship to the subscriber.</summary>
    public static readonly Dictionary<string, string> Relationship = new()
    {
        ["01"] = "Spouse",
        ["15"] = "Ward",
        ["18"] = "Self",
        ["19"] = "Child",
        ["20"] = "Employee",
        ["53"] = "Life Partner",
        ["G8"] = "Other Relationship",
    };

    /// <summary>HD05, an 834 coverage level.</summary>
    public static readonly Dictionary<string, string> CoverageLevel = new()
    {
        ["EMP"] = "Employee Only",
        ["ESP"] = "Employee and Spouse",
        ["ECH"] = "Employee and Children",
        ["FAM"] = "Family",
        ["IND"] = "Individual",
    };

    /// <summary>IK501 (element 717), a 999's verdict on one transaction.</summary>
    public static readonly Dictionary<string, string> TransactionAcknowledgment = new()
    {
        ["A"] = "Accepted",
        ["E"] = "Accepted But Errors Were Noted",
        ["M"] = "Rejected, Message Authentication Code (MAC) Failed",
        ["R"] = "Rejected",
        ["W"] = "Rejected, Assurance Failed Validity Tests",
        ["X"] = "Rejected, Content After Decryption Could Not Be Analyzed",
    };

    /// <summary>AK901 (element 715), a 999's verdict on the functional group: as IK501, and P for partly accepted.</summary>
    public static readonly Dictionary<string, string> GroupAcknowledgment = new()
    {
        ["A"] = "Accepted",
        ["E"] = "Accepted But Errors Were Noted",
        ["M"] = "Rejected, Message Authentication Code (MAC) Failed",
        ["P"] = "Partially Accepted, At Least One Transaction Set Was Rejected",
        ["R"] = "Rejected",
        ["W"] = "Rejected, Assurance Failed Validity Tests",
        ["X"] = "Rejected, Content After Decryption Could Not Be Analyzed",
    };

    /// <summary>STC01-1, a 277 claim status category (X12 external code list 507, the codes still in use).</summary>
    public static readonly Dictionary<string, string> StatusCategory = new()
    {
        ["A0"] = "Acknowledgement/Forwarded",
        ["A1"] = "Acknowledgement/Receipt",
        ["A2"] = "Acknowledgement/Acceptance into adjudication system",
        ["A3"] = "Acknowledgement/Returned as unprocessable claim",
        ["A4"] = "Acknowledgement/Not Found",
        ["A5"] = "Acknowledgement/Split Claim",
        ["A6"] = "Acknowledgement/Rejected for Missing Information",
        ["A7"] = "Acknowledgement/Rejected for Invalid Information",
        ["A8"] = "Acknowledgement/Rejected for relational field in error",
        ["P0"] = "Pending: Adjudication/Details",
        ["P1"] = "Pending/In Process",
        ["P2"] = "Pending/Payer Review",
        ["P3"] = "Pending/Provider Requested Information",
        ["P4"] = "Pending/Patient Requested Information",
        ["P5"] = "Pending/Payer Administrative/System hold",
        ["F0"] = "Finalized",
        ["F1"] = "Finalized/Payment",
        ["F2"] = "Finalized/Denial",
        ["F3"] = "Finalized/Revised",
        ["F4"] = "Finalized/Adjudication Complete - No payment forthcoming",
        ["R0"] = "Requests for additional Information/General Requests",
        ["R1"] = "Requests for additional Information/Entity Requests",
        ["R3"] = "Requests for additional Information/Claim/Line",
        ["R4"] = "Requests for additional Information/Documentation",
        ["R5"] = "Request for additional information/more specific detail",
        ["R6"] = "Requests for additional information – Regulatory requirements",
        ["R7"] = "Requests for additional information – Confirm care is consistent with Health Plan policy coverage",
        ["R8"] = "Requests for additional information – Confirm care is consistent with health plan coverage exceptions",
        ["R9"] = "Requests for additional information – Determination of medical necessity",
        ["E0"] = "Response not possible - error on submitted request data",
        ["E1"] = "Response not possible - System Status",
        ["E2"] = "Information Holder is not responding; resubmit at a later time",
        ["E3"] = "Correction required - relational fields in error",
        ["E4"] = "Trading partner agreement specific requirement not met: Data correction required",
        ["D0"] = "Data Search Unsuccessful",
    };

    /// <summary>EB01, a 271 benefit's eligibility or benefit information.</summary>
    public static readonly Dictionary<string, string> Benefit = new()
    {
        ["1"] = "Active Coverage",
        ["2"] = "Active - Full Risk Capitation",
        ["3"] = "Active - Services Capitated",
        ["6"] = "Inactive",
        ["7"] = "Inactive - Pending Eligibility Update",
        ["8"] = "Inactive - Pending Investigation",
        ["A"] = "Co-Insurance",
        ["B"] = "Co-Payment",
        ["C"] = "Deductible",
        ["D"] = "Benefit Description",
        ["F"] = "Limitations",
        ["G"] = "Out of Pocket (Stop Loss)",
        ["I"] = "Non-Covered",
        ["L"] = "Primary Care Provider",
        ["R"] = "Other or Additional Payor",
        ["U"] = "Contact Following Entity for Eligibility or Benefit Information",
        ["V"] = "Cannot Process",
    };

    /// <summary>HL03, the level a hierarchical loop describes.</summary>
    public static readonly Dictionary<string, string> Level = new()
    {
        ["19"] = "Provider of Service",
        ["20"] = "Information Source",
        ["21"] = "Information Receiver",
        ["22"] = "Subscriber",
        ["23"] = "Dependent",
        ["PT"] = "Patient",
    };

    /// <summary>The meaning of <paramref name="code"/> in <paramref name="list"/>, or null.</summary>
    public static string? Meaning(this Dictionary<string, string> list, string code) =>
        list.TryGetValue(code.Trim(), out var meaning) ? meaning : null;
}
