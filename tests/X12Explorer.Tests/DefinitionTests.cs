using System.Text.Json.Nodes;
using X12Explorer.Core;
using X12Explorer.Mcp;

namespace X12Explorer.Tests;

/// <summary>
/// Where the summaries' element positions and code meanings were checked against X12 005010 (the segment layouts and
/// code lists, and external code list 507 for claim status categories).
/// </summary>
public sealed class DefinitionTests
{
    private static JsonObject Summary(string body)
    {
        var text = "ISA*00*          *00*          *ZZ*SENDER         *ZZ*RECEIVER       *260101*1200*^*00501*000000001*0*P*:~"
            + "GS*HC*SENDER*RECEIVER*20260101*1200*1*X*005010X222A1~" + body + "GE*1*1~IEA*1*000000001~";
        var file = X12File.Parse(text);
        return Summaries.Of(file.Transactions[0], file.Delimiters);
    }

    [Fact]
    public void Each_kind_of_service_line_takes_its_units_from_its_own_quantity_element()
    {
        var claim = Summary("ST*837*0001*005010X224A2~BHT*0019*00*1*20260101*1200*CH~HL*1**20*1~HL*2*1*22*0~"
            + "CLM*ACCT1*300***11:B:1~SV1*HC:99213*100*UN*2***1~SV2*0450*HC:99284*120*UN*3~SV3*AD:D0120*80****4~SE*9*0001~");
        var lines = claim["claims"]![0]!["lines"]!.AsArray();
        Assert.Equal(["2", "3", "4"], lines.Select(l => (string)l!["units"]!));   // SV104, SV205, SV306
    }

    [Fact]
    public void A_999_reads_a_transaction_and_a_group_with_their_own_code_lists()
    {
        var ack = Summary("ST*999*0001*005010X231A1~AK1*HC*1*005010X222A1~AK2*837*0001~IK5*P~AK9*P*1*1*0~SE*6*0001~");
        Assert.Null(ack["transactions"]![0]!["result"]!["meaning"]);            // P isn't an IK501 code (717)
        Assert.Equal("Partially Accepted, At Least One Transaction Set Was Rejected", (string)ack["group_result"]!["result"]!["meaning"]!);
    }

    [Fact]
    public void Claim_status_categories_mean_what_list_507_says()
    {
        Assert.Equal("Finalized/Adjudication Complete - No payment forthcoming", Codes.StatusCategory["F4"]);
        Assert.Equal("Requests for additional Information/Documentation", Codes.StatusCategory["R4"]);
        Assert.False(Codes.StatusCategory.ContainsKey("F5"));                    // stopped in 2003
    }
}
