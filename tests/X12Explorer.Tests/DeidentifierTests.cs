using X12Explorer.Core;

namespace X12Explorer.Tests;

/// <summary>
/// De-identifying: what is replaced, what is left alone, that stand-ins match across files, that dates keep their
/// intervals, and that the file keeps its layout and envelope. Also the --deidentify command line.
/// </summary>
public class DeidentifierTests
{
    private static readonly byte[] Key = Enumerable.Range(1, 32).Select(i => (byte)i).ToArray();
    private static readonly DateOnly Today = new(2026, 11, 3);

    private static Deidentifier Make(byte[]? key = null) => new(key ?? Key, Today);

    private static string Text(string sample) => File.ReadAllText(ParserTests.Sample(sample));

    public static TheoryData<string> Samples => new(Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Samples"), "*.edi").Select(Path.GetFileName).Order()!);

    [Theory]
    [MemberData(nameof(Samples))]
    public void Every_sample_keeps_its_segments_envelope_and_amounts(string sample)
    {
        var before = X12File.Parse(Text(sample));
        var after = X12File.Parse(Make().Apply(Text(sample)).Text);

        Assert.Equal(before.Segments.Select(s => s.Id), after.Segments.Select(s => s.Id));
        Assert.Equal(before.Segments.Select(s => s.Elements.Length), after.Segments.Select(s => s.Elements.Length));
        Assert.Empty(Checks.Of(after));
        // The envelope is untouched.
        Assert.Equal(before.Segments.Where(s => s.Id is "ISA" or "GS" or "ST" or "SE" or "GE" or "IEA").Select(s => s.Text),
            after.Segments.Where(s => s.Id is "ISA" or "GS" or "ST" or "SE" or "GE" or "IEA").Select(s => s.Text));
        // So are amounts, codes and payments.
        Assert.Equal(before.Segments.Where(s => s.Id is "BPR" or "SVC" or "CAS" or "HI" or "SV1" or "EB" or "HD" or "AMT").Select(s => s.Text),
            after.Segments.Where(s => s.Id is "BPR" or "SVC" or "CAS" or "HI" or "SV1" or "EB" or "HD" or "AMT").Select(s => s.Text));
    }

    [Theory]
    [MemberData(nameof(Samples))]
    public void No_sample_keeps_a_patients_name_ID_or_account(string sample)
    {
        var text = Make().Apply(Text(sample)).Text;
        foreach (var gone in new[] { "DOE", "JANE", "ROE", "RICHARD", "POE", "EX1000001", "EX1000002", "EX2000001", "PATACCT0001", "PATACCT0002", "PAYERCLM01", "19800115" })
            Assert.DoesNotContain(gone, text);
    }

    [Fact]
    public void Leaves_organisations_and_providers_alone()
    {
        var text = Make().Apply(Text("claim-837.edi")).Text;
        Assert.Contains("NM1*85*2*SAMPLE CLINIC*****XX*1999999984", text);
        Assert.Contains("N3*100 EXAMPLE STREET", text);
        Assert.Contains("NM1*PR*2*SAMPLE HEALTH PLAN*****PI*888888888", text);
    }

    [Fact]
    public void The_same_person_gets_the_same_stand_ins_in_every_file()
    {
        var d = Make();
        var claim = X12File.Parse(d.Apply(Text("claim-837.edi")).Text);
        var remittance = X12File.Parse(d.Apply(Text("remittance-835.edi")).Text);
        var status = X12File.Parse(d.Apply(Text("claim-status-277.edi")).Text);
        var acknowledgment = X12File.Parse(d.Apply(Text("acknowledgment-999.edi")).Text);

        var insured = claim.Segments.First(s => s.Id == "NM1" && s[1] == "IL");
        var patient = remittance.Segments.First(s => s.Id == "NM1" && s[1] == "QC");
        Assert.Equal(insured[3], patient[3]);
        Assert.Equal(insured[4], patient[4]);
        Assert.Equal(insured[9], patient[9]);

        // A claim's account number in the 837 is the one the 277 traces and the 999 names.
        var accounts = claim.Segments.Where(s => s.Id == "CLM").Select(s => s[1]).ToList();
        Assert.Equal(accounts[0], status.Segments.Last(s => s.Id == "TRN")[2]);
        Assert.Equal("CLM01:" + accounts[1], acknowledgment.Segments.First(s => s.Id == "CTX")[1]);

        // The payer's claim number in the 835 is the one the 277 refers to.
        Assert.Equal(remittance.Segments.First(s => s.Id == "CLP")[7], status.Segments.First(s => s.Id == "REF" && s[1] == "1K")[2]);
    }

    [Fact]
    public void Stand_ins_keep_the_shape_of_what_they_replace()
    {
        var text = Make().Apply(Text("claim-837.edi")).Text;
        var insured = X12File.Parse(text).Segments.First(s => s.Id == "NM1" && s[1] == "IL");
        Assert.Matches("^[A-Z]{2}[0-9]{7}$", insured[9]);
        var account = X12File.Parse(text).Segments.First(s => s.Id == "CLM")[1];
        Assert.Matches("^[A-Z]{7}[0-9]{4}$", account);
    }

    [Fact]
    public void Another_key_gives_other_stand_ins_and_the_same_key_the_same()
    {
        var other = Enumerable.Repeat((byte)7, 32).ToArray();
        Assert.Equal(Make().Apply(Text("remittance-835.edi")).Text, Make().Apply(Text("remittance-835.edi")).Text);
        Assert.NotEqual(Make().Apply(Text("remittance-835.edi")).Text, Make(other).Apply(Text("remittance-835.edi")).Text);
    }

    [Fact]
    public void Dates_move_back_together_and_birth_dates_keep_only_their_year()
    {
        var d = Make();
        Assert.InRange(-d.ShiftDays, 30, 180);
        var text = d.Apply("ST*837*0001~DMG*D8*19800115*F~DTP*472*RD8*20261020-20261023~DTM*232*20261020~DMG*D8*19300704*M~SE*6*0001~").Text;
        var segments = X12File.Parse(text).Segments;

        Assert.Equal("19800101", segments[1][2]);
        var range = segments[2][3].Split('-').Select(v => DateOnly.ParseExact(v, "yyyyMMdd")).ToArray();
        Assert.Equal(new DateOnly(2026, 10, 20).AddDays(d.ShiftDays), range[0]);
        Assert.Equal(3, range[1].DayNumber - range[0].DayNumber);
        Assert.Equal(new DateOnly(2026, 10, 20).AddDays(d.ShiftDays).ToString("yyyyMMdd"), segments[3][2]);
        // Born 96 years ago: shown as 90.
        Assert.Equal("19360101", segments[4][2]);
    }

    [Fact]
    public void A_persons_address_phone_and_notes_are_replaced()
    {
        var text = Make().Apply(
            "ST*834*0001~INS*Y*18*021*28*A~NM1*IL*1*DOE*JANE*Q~PER*IP**HP*7175550123*EM*JANE.DOE@MAIL.COM~N3*42 REAL ROAD*APT 9~N4*HARRISBURG*PA*171010001~" +
            "NM1*IL*1*ROE*RICHARD~N4*SMALLTOWN*VT*03601~NTE*ADD*PATIENT LIVES WITH DAUGHTER MARY ROE~SE*10*0001~").Text;
        var s = X12File.Parse(text).Segments;

        Assert.Equal(1, s[2][5].Length);
        Assert.DoesNotContain("7175550123", text);
        Assert.Matches(@"^member\d{4}@example\.com$", s[3][6]);
        Assert.Matches(@"^\d{3} [A-Z]+ ST$", s[4][1]);
        Assert.Equal(2, s[4].Elements.Length);
        Assert.Equal("ANYTOWN", s[5][1]);
        Assert.Equal("PA", s[5][2]);
        Assert.Equal("171000000", s[5][3]);
        // 036 covers too few people to keep.
        Assert.Equal("00000", s[7][3]);
        Assert.Equal("NOTE REMOVED", s[8][2]);
        Assert.DoesNotContain("MARY", text);
    }

    [Fact]
    public void Keeps_the_files_layout()
    {
        const string text = "ISA*00*          *00*          *ZZ*SENDER         *ZZ*RECEIVER       *261103*1200*^*00501*000000001*0*T*:~\r\n" +
                            "GS*HC*SENDER*RECEIVER*20261103*1200*1*X*005010X222A1~\r\nST*837*0001~\r\n  NM1*IL*1*DOE*JANE****MI*X1~\r\nSE*3*0001~\r\nGE*1*1~\r\nIEA*1*000000001~\r\n";
        var result = Make().Apply(text);
        Assert.Equal(text.Split("\r\n").Length, result.Text.Split("\r\n").Length);
        Assert.Contains("~\r\n  NM1*IL*1*", result.Text);
        Assert.EndsWith("IEA*1*000000001~\r\n", result.Text);
        Assert.Equal("1 name and 1 identifier", result.Summary);
    }

    [Fact]
    public void The_command_line_de_identifies_a_folder_with_one_key()
    {
        var root = Path.Combine(Path.GetTempPath(), "x12-deid-" + Guid.NewGuid().ToString("N"));
        var input = Path.Combine(root, "in");
        Directory.CreateDirectory(Path.Combine(input, "more"));
        try
        {
            File.Copy(ParserTests.Sample("claim-837.edi"), Path.Combine(input, "claim-837.edi"));
            File.Copy(ParserTests.Sample("acknowledgment-999.edi"), Path.Combine(input, "more", "ack.txt"));
            File.WriteAllText(Path.Combine(input, "readme.txt"), "not X12");
            var output = Path.Combine(root, "out");
            var said = new StringWriter();

            Assert.Equal(0, DeidentifyCommand.Run(["--deidentify", input, output], said, TextWriter.Null, Make()));
            Assert.True(File.Exists(Path.Combine(output, "claim-837.edi")));
            Assert.True(File.Exists(Path.Combine(output, "more", "ack.txt")));
            Assert.False(File.Exists(Path.Combine(output, "readme.txt")));
            Assert.Contains("Dates moved back", said.ToString());

            // One key: the 999's context names the 837's stand-in account number.
            var claim = X12File.Load(Path.Combine(output, "claim-837.edi"));
            var ack = X12File.Load(Path.Combine(output, "more", "ack.txt"));
            Assert.Equal("CLM01:" + claim.Segments.Where(s => s.Id == "CLM").ElementAt(1)[1], ack.Segments.First(s => s.Id == "CTX")[1]);

            // A second run writes over nothing.
            var errors = new StringWriter();
            Assert.Equal(1, DeidentifyCommand.Run(["--deidentify", input, output], TextWriter.Null, errors));
            Assert.Contains("exists; left it alone", errors.ToString());

            Assert.Equal(2, DeidentifyCommand.Run(["--deidentify", input], TextWriter.Null, TextWriter.Null));
            Assert.Equal(2, DeidentifyCommand.Run(["--deidentify", Path.Combine(root, "missing"), output], TextWriter.Null, TextWriter.Null));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }
}
