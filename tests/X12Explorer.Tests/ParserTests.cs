using X12Explorer.Core;

namespace X12Explorer.Tests;

/// <summary>
/// Reading files: the delimiters from the ISA, segments split however the lines are broken, transactions with their
/// envelopes, loops by each transaction's key segment, and the text of the three boxes.
/// </summary>
public class ParserTests
{
    public static string Sample(string name) => Path.Combine(AppContext.BaseDirectory, "Samples", name);

    private const string Isa = "ISA*00*          *00*          *ZZ*SAMPLESENDER   *ZZ*SAMPLERECEIVER *261103*1200*^*00501*000000001*0*T*:~";

    [Fact]
    public void Reads_the_delimiters_from_the_isa()
    {
        Assert.Equal(new Delimiters('~', '*', ':'), Delimiters.Detect(Isa));
        Assert.Equal(new Delimiters('\'', '|', '>'), Delimiters.Detect(Isa.Replace('*', '|').Replace(':', '>').Replace('~', '\'')));
        Assert.Equal(new Delimiters('\n', '*', ':'), Delimiters.Detect(Isa.Replace("~", "\n")));
    }

    [Fact]
    public void Reads_an_isa_whose_fields_were_not_padded()
    {
        Assert.Equal(new Delimiters('!', '^', '#'), Delimiters.Detect("ISA^00^^00^^ZZ^A^ZZ^B^261103^1200^U^00401^1^0^P^#!ST^835^1!"));
    }

    [Fact]
    public void Files_without_an_isa_use_the_usual_delimiters()
    {
        Assert.Equal(Delimiters.Standard, Delimiters.Detect("ST*835*0001~CLP*A~SE*3*0001~"));
        Assert.Equal(Delimiters.Standard, Delimiters.Detect(""));
    }

    [Fact]
    public void Splits_segments_whatever_the_line_breaks()
    {
        var oneLine = X12File.Parse("ST*835*1~CLP*A*1~CLP*B*1~SE*4*1~");
        var crlf = X12File.Parse("ST*835*1~\r\nCLP*A*1~\r\nCLP*B*1~\r\nSE*4*1~\r\n");
        // Some senders end a few segments with a bare line feed instead of the terminator.
        var mixed = X12File.Parse("ST*835*1~CLP*A*1\nCLP*B*1~SE*4*1~");

        foreach (var file in new[] { oneLine, crlf, mixed })
            Assert.Equal(["ST", "CLP", "CLP", "SE"], file.Segments.Select(s => s.Id));
    }

    [Fact]
    public void Reads_files_whose_segments_end_at_line_breaks()
    {
        var file = X12File.Parse(Isa.Replace("~", "\r\n") + "ST*271*1\r\nHL*1**20*1\r\nSE*3*1\r\nIEA*1*000000001\r\n");

        Assert.True(file.Delimiters.LineTerminated);
        Assert.Equal(["ISA", "ST", "HL", "SE", "IEA"], file.Segments.Select(s => s.Id));
        var views = Views.Of(file.Transactions, file.Delimiters, "HL", expanded: false);
        Assert.Equal("HL*1**20*1", views.Loops);
    }

    [Fact]
    public void Reads_elements_by_position()
    {
        var clp = X12File.Parse("ST*835*1~CLP*CLAIM1*1*200.00~SE*3*1~").Transactions[0].Body[0];

        Assert.Equal("CLP", clp.Id);
        Assert.Equal("CLAIM1", clp[1]);
        Assert.Equal("200.00", clp[3]);
        Assert.Equal("", clp[9]);
    }

    [Theory]
    [InlineData("capitation-820.edi", "820", 1, 3)]
    [InlineData("membership-834.edi", "834", 1, 2)]
    [InlineData("remittance-835.edi", "835", 1, 2)]
    [InlineData("benefit-info-271.edi", "271", 1, 3)]
    [InlineData("claim-837.edi", "837", 2, 4)]
    [InlineData("claim-status-277.edi", "277", 1, 4)]
    public void Reads_each_sample(string name, string code, int transactions, int loops)
    {
        var file = X12File.Load(Sample(name));
        var kind = TransactionKind.ForCode(code)!;

        Assert.Equal(code, file.Code);
        Assert.Equal(transactions, file.Transactions.Count);
        Assert.All(file.Transactions, t =>
        {
            Assert.NotNull(t.Trailer);
            Assert.Equal(t.Control, t.Trailer![2]);
            // SE01 counts the segments from ST to SE.
            Assert.Equal(t.Body.Count + 2, int.Parse(t.Trailer[1]));
            Assert.NotNull(t.GroupEnd);
            Assert.NotNull(t.InterchangeEnd);
        });
        Assert.Equal(loops, Views.Of(file.Transactions, file.Delimiters, kind.LoopStart, kind.Expanded).LoopCount);
        Assert.Empty(file.Outside);
    }

    [Fact]
    public void Header_has_the_envelope_and_everything_before_the_first_loop()
    {
        var file = X12File.Load(Sample("remittance-835.edi"));
        var views = Views.Of(file.Transactions, file.Delimiters, "CLP", expanded: false);
        var header = views.Header.Split(Environment.NewLine);

        Assert.StartsWith("ISA*00*", header[0]);
        Assert.StartsWith("GS*HP*", header[1]);
        Assert.Equal("ST*835*0001*005010X221A1~", header[2]);
        Assert.Equal("LX*1~", header[^1]);
        Assert.Equal(["SE*19*0001~", "GE*1*3~", "IEA*1*000000003~"], views.Trailer.Split(Environment.NewLine));
    }

    [Fact]
    public void Loops_are_one_line_each_or_a_segment_per_line()
    {
        var file = X12File.Load(Sample("remittance-835.edi"));

        var compact = Views.Of(file.Transactions, file.Delimiters, "CLP", expanded: false).Loops.Split(Environment.NewLine);
        Assert.Equal(2, compact.Length);
        Assert.StartsWith("CLP*CLAIM0001*", compact[0]);
        Assert.EndsWith("CAS*PR*2*25.00~", compact[0]);
        Assert.StartsWith("CLP*CLAIM0002*", compact[1]);

        var expanded = Views.Of(file.Transactions, file.Delimiters, "CLP", expanded: true).Loops.Split(Environment.NewLine);
        Assert.Equal(11, expanded.Length);
        Assert.StartsWith("CLP*CLAIM0001*", expanded[0]);
        Assert.Equal("    NM1*QC*1*DOE*JANE****MI*EX1000001~", expanded[1]);
        Assert.StartsWith("CLP*CLAIM0002*", expanded[6]);
    }

    [Fact]
    public void Several_transactions_share_one_envelope_in_the_views()
    {
        var file = X12File.Load(Sample("claim-837.edi"));
        var both = Views.Of(file.Transactions, file.Delimiters, "HL", expanded: true);

        Assert.Single(both.Header.Split(Environment.NewLine), l => l.StartsWith("ISA"));
        Assert.Single(both.Header.Split(Environment.NewLine), l => l.StartsWith("GS"));
        Assert.Equal(2, both.Header.Split(Environment.NewLine).Count(l => l.StartsWith("ST*837")));
        Assert.Equal(["SE*21*0001~", "", "SE*21*0002~", "GE*2*5~", "IEA*1*000000005~"], both.Trailer.Split(Environment.NewLine));

        var second = Views.Of([file.Transactions[1]], file.Delimiters, "HL", expanded: true);
        Assert.Contains("ROE*RICHARD", second.Loops);
        Assert.DoesNotContain("DOE*JANE", second.Loops);
        Assert.Equal(2, second.LoopCount);
    }

    [Fact]
    public void Lines_before_the_first_st_are_kept_with_the_header()
    {
        // The old program skipped whatever a sender put before ~ST; keep it visible instead.
        var file = X12File.Parse("SENDER BATCH 42~ST*834*1~INS*Y*18~SE*3*1~");

        Assert.Equal("834", file.Code);
        var views = Views.Of(file.Transactions, file.Delimiters, "INS", expanded: false);
        Assert.Equal(["SENDER BATCH 42~", "ST*834*1~"], views.Header.Split(Environment.NewLine));
        Assert.Equal("INS*Y*18~", views.Loops);
    }

    [Fact]
    public void A_file_cut_off_inside_a_transaction_still_shows()
    {
        var file = X12File.Parse("ST*835*1~CLP*A*1~CLP*B*1");
        var t = Assert.Single(file.Transactions);

        Assert.Null(t.Trailer);
        var views = Views.Of(file.Transactions, file.Delimiters, "CLP", expanded: false);
        Assert.Equal(2, views.LoopCount);
        Assert.Contains("no SE", views.Trailer);
    }

    [Fact]
    public void A_file_with_no_st_has_no_code()
    {
        Assert.Null(X12File.Parse("hello, world").Code);
        Assert.Null(X12File.Parse("").Code);
    }

    [Fact]
    public void Finds_the_segment_under_the_caret()
    {
        var d = Delimiters.Standard;
        const string text = "CLP*A*1~NM1*QC*1*DOE~\n    SVC*HC:99213*200~";

        Assert.Equal("CLP", Elements.At(text, 0, d)!.Id);
        Assert.Equal("CLP", Elements.At(text, 7, d)!.Id);   // on the terminator
        Assert.Equal("NM1", Elements.At(text, 8, d)!.Id);   // just after it
        Assert.Equal("NM1", Elements.At(text, 21, d)!.Id);  // end of the line
        Assert.Equal("SVC", Elements.At(text, 30, d)!.Id);  // an indented line
        Assert.Null(Elements.At("", 0, d));
        Assert.Null(Elements.At("A~\n\nB~", 3, d));         // a blank line
    }

    [Fact]
    public void Lists_elements_by_name_leaving_out_empty_ones()
    {
        var nm1 = new Segment("NM1*IL*1*DOE*JANE****MI*EX1000001", Delimiters.Standard);

        Assert.Equal(
            [("NM101", "IL"), ("NM102", "1"), ("NM103", "DOE"), ("NM104", "JANE"), ("NM108", "MI"), ("NM109", "EX1000001")],
            Elements.List(nm1));
        Assert.Equal("NM101 IL   ·   NM102 1   ·   NM103 DOE   ·   NM104 JANE   ·   NM108 MI   ·   NM109 EX1000001", Elements.Describe(nm1));
        Assert.Equal("SE: no elements", Elements.Describe(new Segment("SE", Delimiters.Standard)));
    }

    [Fact]
    public void Knows_the_six_transactions()
    {
        Assert.Equal(["820", "834", "835", "271", "837", "277"], TransactionKind.All.Select(k => k.Code));
        Assert.Equal("Remittance (835)", TransactionKind.ForCode(" 835 ")!.Title);
        Assert.Null(TransactionKind.ForCode("999"));
    }
}
