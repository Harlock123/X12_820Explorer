using System.Globalization;
using System.Text.Json.Nodes;
using X12Explorer.Core;

namespace X12Explorer.Mcp;

/// <summary>
/// What a transaction says, read the way its implementation guide lays it out: an 835's payment and claims, an 837's
/// claims and lines, an 834's members, an 820's payments, a 999's verdicts, a 277's statuses, a 271's benefits. The
/// requests (270, 276) and any other transaction get their hierarchy or their segment counts. Each lists at most
/// <see cref="MaxItems"/> entries and says when there were more.
/// </summary>
internal static class Summaries
{
    public const int MaxItems = 200;

    public static JsonObject Of(Transaction t, Delimiters d)
    {
        var s = new Reader(d);
        var result = t.Code switch
        {
            "835" => s.Remittance(t),
            "837" => s.Claims(t),
            "834" => s.Membership(t),
            "820" => s.Capitation(t),
            "999" => s.Acknowledgment(t),
            "277" => s.ClaimStatus(t),
            "271" => s.Benefits(t),
            "270" or "276" => s.Hierarchy(t),
            _ => s.Counts(t),
        };
        var head = new JsonObject
        {
            ["transaction_set"] = t.Code,
            ["name"] = t.Kind?.Name ?? "Not one the explorer has a tab for",
            ["control_number"] = t.Control,
            ["version"] = t.Start[3].Trim(),
        };
        foreach (var (key, value) in result.ToList())
        {
            result.Remove(key);
            head[key] = value;
        }
        return head;
    }

    private sealed class Reader(Delimiters d)
    {
        // ───────────────────────────── 835 ─────────────────────────────

        public JsonObject Remittance(Transaction t)
        {
            var result = new JsonObject();
            var claims = new List<JsonObject>();
            var providerAdjustments = new JsonArray();
            JsonObject? claim = null, line = null;
            decimal charged = 0, paid = 0, patient = 0, adjusted = 0;
            var denied = 0;

            foreach (var s in t.Body)
            {
                switch (s.Id)
                {
                    case "BPR":
                        result["payment"] = new JsonObject
                        {
                            ["amount"] = Money(s[2]),
                            ["method"] = s[4],
                            ["date"] = Date(s[16]),
                        };
                        break;
                    case "TRN":
                        result["trace_number"] = s[2];
                        break;
                    case "N1" when s[1] == "PR":
                        result["payer"] = s[2];
                        break;
                    case "N1" when s[1] == "PE":
                        result["payee"] = Party(s);
                        break;
                    case "CLP":
                        line = null;
                        claim = new JsonObject
                        {
                            ["segment"] = s.Index + 1,
                            ["claim_id"] = s[1],
                            ["status"] = Coded(s[2], Codes.ClaimStatus),
                            ["charged"] = Money(s[3]),
                            ["paid"] = Money(s[4]),
                            ["patient_responsibility"] = Money(s[5]),
                            ["payer_claim_number"] = s[7],
                        };
                        claims.Add(claim);
                        charged += Amount(s[3]);
                        paid += Amount(s[4]);
                        patient += Amount(s[5]);
                        if (s[2].Trim() == "4") denied++;
                        break;
                    case "NM1" when claim != null && s[1] == "QC":
                        claim["patient"] = Name(s);
                        claim["patient_id"] = s[9];
                        break;
                    case "SVC" when claim != null:
                        line = new JsonObject
                        {
                            ["procedure"] = Composite(s[1]),
                            ["charged"] = Money(s[2]),
                            ["paid"] = Money(s[3]),
                            ["units"] = s[5],
                        };
                        Add(claim, "services", line);
                        break;
                    case "CAS":
                        // Groups of three after the group code: reason, amount, quantity.
                        for (var i = 2; i < s.Elements.Length; i += 3)
                        {
                            if (s[i].Trim().Length == 0) continue;
                            adjusted += Amount(s[i + 1]);
                            Add(line ?? claim ?? result, "adjustments", new JsonObject
                            {
                                ["group"] = Coded(s[1], Codes.AdjustmentGroup),
                                ["reason"] = s[i],
                                ["amount"] = Money(s[i + 1]),
                            });
                        }
                        break;
                    case "PLB":
                        // Pairs from PLB03: a reason (with its reference) and an amount.
                        for (var i = 3; i < s.Elements.Length; i += 2)
                            if (s[i].Trim().Length > 0)
                                providerAdjustments.Add(new JsonObject { ["reason"] = Composite(s[i]), ["amount"] = Money(s[i + 1]) });
                        break;
                }
            }

            var plb = providerAdjustments.Sum(a => Amount(a!["amount"]?.ToString() ?? ""));
            result["totals"] = new JsonObject
            {
                ["claims"] = claims.Count,
                ["denied"] = denied,
                ["charged"] = charged,
                ["paid"] = paid,
                ["patient_responsibility"] = patient,
                ["adjusted"] = adjusted,
                ["provider_adjustments"] = plb,
            };
            if (result["payment"] is JsonObject payment)
            {
                // The payment is what the claims paid less the provider-level adjustments.
                var expected = paid - plb;
                var amount = Amount(payment["amount"]?.ToString() ?? "");
                result["balanced"] = amount == expected;
                if (amount != expected)
                    result["balance_note"] = $"BPR02 pays {amount:0.00}, but the claims paid {paid:0.00} less {plb:0.00} in provider adjustments is {expected:0.00}.";
            }
            if (providerAdjustments.Count > 0) result["provider_adjustments"] = providerAdjustments;
            result["claims"] = Capped(claims);
            return result;
        }

        // ───────────────────────────── 837 ─────────────────────────────

        public JsonObject Claims(Transaction t)
        {
            var result = new JsonObject();
            var claims = new List<JsonObject>();
            JsonObject? claim = null, line = null;
            string? provider = null, subscriber = null, patient = null;
            decimal total = 0;

            foreach (var s in t.Body)
            {
                switch (s.Id)
                {
                    case "BHT":
                        result["batch"] = s[3];
                        result["created"] = Date(s[4]);
                        break;
                    case "NM1" when s[1] == "41":
                        result["submitter"] = s[3];
                        break;
                    case "NM1" when s[1] == "40":
                        result["receiver"] = s[3];
                        break;
                    case "HL":
                        claim = line = null;
                        if (s[3] is "20") provider = null;
                        if (s[3] is "22" or "23") patient = null;
                        break;
                    case "NM1" when s[1] == "85":
                        provider = Name(s);
                        break;
                    case "NM1" when s[1] == "IL":
                        subscriber = Name(s);
                        break;
                    case "NM1" when s[1] == "QC":
                        patient = Name(s);
                        break;
                    case "CLM":
                        line = null;
                        var place = s[5].Split(d.Component);
                        claim = new JsonObject
                        {
                            ["segment"] = s.Index + 1,
                            ["claim_id"] = s[1],
                            ["charged"] = Money(s[2]),
                            ["place_of_service"] = place[0],
                            ["frequency"] = place.Length > 2 ? place[2] : "",
                            ["billing_provider"] = provider,
                            ["subscriber"] = subscriber,
                            ["patient"] = patient ?? subscriber,
                        };
                        claims.Add(claim);
                        total += Amount(s[2]);
                        break;
                    case "HI" when claim != null:
                        for (var i = 1; i < s.Elements.Length; i++)
                            if (s[i].Trim().Length > 0) Add(claim, "diagnoses", JsonValue.Create(Composite(s[i])));
                        break;
                    case "SV1" or "SV2" or "SV3" when claim != null:
                        // SV2 (institutional) puts the revenue code first and the procedure second.
                        var procedure = s.Id == "SV2" ? s[2] : s[1];
                        line = new JsonObject
                        {
                            ["procedure"] = Composite(procedure),
                            ["charged"] = Money(s.Id == "SV2" ? s[3] : s[2]),
                            ["units"] = s.Id == "SV2" ? s[5] : s[4],
                        };
                        if (s.Id == "SV2") line["revenue_code"] = s[1];
                        Add(claim, "lines", line);
                        break;
                    case "DTP" when s[1] == "472":
                        if ((line ?? claim) is { } dated) dated["service_date"] = Date(s[3]);
                        break;
                }
            }

            result["totals"] = new JsonObject { ["claims"] = claims.Count, ["charged"] = total };
            result["claims"] = Capped(claims);
            return result;
        }

        // ───────────────────────────── 834 ─────────────────────────────

        public JsonObject Membership(Transaction t)
        {
            var result = new JsonObject();
            var members = new List<JsonObject>();
            JsonObject? member = null, coverage = null;

            foreach (var s in t.Body)
            {
                switch (s.Id)
                {
                    case "BGN":
                        result["reference"] = s[2];
                        result["created"] = Date(s[3]);
                        break;
                    case "N1" when s[1] == "P5":
                        result["sponsor"] = s[2];
                        break;
                    case "N1" when s[1] == "IN":
                        result["payer"] = s[2];
                        break;
                    case "INS":
                        coverage = null;
                        member = new JsonObject
                        {
                            ["segment"] = s.Index + 1,
                            ["subscriber"] = s[1].Trim() == "Y",
                            ["relationship"] = Coded(s[2], Codes.Relationship),
                            ["maintenance"] = Coded(s[3], Codes.Maintenance),
                            ["benefit_status"] = s[5],
                        };
                        members.Add(member);
                        break;
                    case "REF" when member != null && s[1] == "0F":
                        member["subscriber_id"] = s[2];
                        break;
                    case "NM1" when member != null && s[1] == "IL":
                        member["name"] = Name(s);
                        if (s[9].Length > 0) member["member_id"] = s[9];
                        break;
                    case "DMG" when member != null:
                        member["birth_date"] = Date(s[2]);
                        member["gender"] = s[3];
                        break;
                    case "DTP" when member != null && coverage == null && s[1] is "356" or "357":
                        member[s[1] == "356" ? "eligibility_begins" : "eligibility_ends"] = Date(s[3]);
                        break;
                    case "HD" when member != null:
                        coverage = new JsonObject
                        {
                            ["maintenance"] = Coded(s[1], Codes.Maintenance),
                            ["insurance_line"] = s[3],
                            ["plan"] = s[4],
                            ["coverage_level"] = Coded(s[5], Codes.CoverageLevel),
                        };
                        Add(member, "coverages", coverage);
                        break;
                    case "DTP" when coverage != null && s[1] is "348" or "349":
                        coverage[s[1] == "348" ? "begins" : "ends"] = Date(s[3]);
                        break;
                }
            }

            result["totals"] = new JsonObject
            {
                ["members"] = members.Count,
                ["subscribers"] = members.Count(m => m["subscriber"]?.GetValue<bool>() == true),
                ["by_maintenance"] = Tally(members.Select(m => m["maintenance"]?["meaning"]?.ToString() ?? m["maintenance"]?["code"]?.ToString() ?? "")),
            };
            result["members"] = Capped(members);
            return result;
        }

        // ───────────────────────────── 820 ─────────────────────────────

        public JsonObject Capitation(Transaction t)
        {
            var result = new JsonObject();
            var entities = new List<JsonObject>();
            JsonObject? entity = null;
            decimal remitted = 0;

            foreach (var s in t.Body)
            {
                switch (s.Id)
                {
                    case "BPR":
                        result["payment"] = new JsonObject { ["amount"] = Money(s[2]), ["method"] = s[4], ["date"] = Date(s[16]) };
                        break;
                    case "TRN":
                        result["trace_number"] = s[2];
                        break;
                    case "N1" when s[1] == "PR":
                        result["payer"] = s[2];
                        break;
                    case "N1" when s[1] == "PE":
                        result["payee"] = s[2];
                        break;
                    case "DTM" when entity == null && s[1] == "582":
                        result["coverage_period"] = s[6];
                        break;
                    case "ENT":
                        entity = new JsonObject { ["segment"] = s.Index + 1, ["entity"] = s[1], ["id"] = s[4] };
                        entities.Add(entity);
                        break;
                    case "NM1" when entity != null:
                        entity["name"] = Name(s);
                        break;
                    case "RMR":
                        remitted += Amount(s[4]);
                        Add(entity ?? result, "remittances", new JsonObject { ["reference"] = s[2], ["action"] = s[3], ["amount"] = Money(s[4]) });
                        break;
                    case "DTM" when entity != null && s[1] == "582":
                        entity["period"] = s[6];
                        break;
                }
            }

            result["totals"] = new JsonObject { ["entities"] = entities.Count, ["remitted"] = remitted };
            if (result["payment"] is JsonObject payment)
            {
                var amount = Amount(payment["amount"]?.ToString() ?? "");
                result["balanced"] = amount == remitted;
                if (amount != remitted) result["balance_note"] = $"BPR02 pays {amount:0.00}, but the RMR amounts add up to {remitted:0.00}.";
            }
            result["entities"] = Capped(entities);
            return result;
        }

        // ───────────────────────────── 999 ─────────────────────────────

        public JsonObject Acknowledgment(Transaction t)
        {
            var result = new JsonObject();
            var sets = new List<JsonObject>();
            JsonObject? set = null, error = null;

            foreach (var s in t.Body)
            {
                switch (s.Id)
                {
                    case "AK1":
                        result["acknowledges_group"] = new JsonObject { ["functional_id"] = s[1], ["control_number"] = s[2], ["version"] = s[3] };
                        break;
                    case "AK2":
                        error = null;
                        set = new JsonObject { ["segment"] = s.Index + 1, ["transaction_set"] = s[1], ["control_number"] = s[2] };
                        sets.Add(set);
                        break;
                    case "IK3" when set != null:
                        error = new JsonObject
                        {
                            ["segment_id"] = s[1],
                            ["position"] = s[2],
                            ["loop"] = s[3],
                            ["error"] = Coded(s[4], SegmentErrors),
                        };
                        Add(set, "errors", error);
                        break;
                    case "CTX" when error != null:
                        Add(error, "context", JsonValue.Create(s.Text[(s.Id.Length + 1)..]));
                        break;
                    case "IK4" when error != null:
                        Add(error, "elements", new JsonObject
                        {
                            ["position"] = s[1],
                            ["element_reference"] = s[2],
                            ["error"] = Coded(s[3], ElementErrors),
                            ["bad_value"] = s[4],
                        });
                        break;
                    case "IK5" when set != null:
                        set["result"] = Coded(s[1], Codes.Acknowledgment);
                        var reasons = new JsonArray();
                        for (var i = 2; i < s.Elements.Length; i++)
                            if (s[i].Trim().Length > 0) reasons.Add(Coded(s[i], SetErrors));
                        if (reasons.Count > 0) set["reasons"] = reasons;
                        break;
                    case "AK9":
                        result["group_result"] = new JsonObject
                        {
                            ["result"] = Coded(s[1], Codes.Acknowledgment),
                            ["included"] = s[2],
                            ["received"] = s[3],
                            ["accepted"] = s[4],
                        };
                        break;
                }
            }

            result["totals"] = new JsonObject
            {
                ["transactions"] = sets.Count,
                ["by_result"] = Tally(sets.Select(x => x["result"]?["meaning"]?.ToString() ?? x["result"]?["code"]?.ToString() ?? "No IK5")),
            };
            result["transactions"] = Capped(sets);
            return result;
        }

        // IK304, IK403 and IK502: what was wrong with a segment, an element or a whole transaction.
        private static readonly Dictionary<string, string> SegmentErrors = new()
        {
            ["1"] = "Unrecognized segment ID",
            ["2"] = "Unexpected segment",
            ["3"] = "Required Segment Missing",
            ["4"] = "Loop Occurs Over Maximum Times",
            ["5"] = "Segment Exceeds Maximum Use",
            ["6"] = "Segment Not in Defined Transaction Set",
            ["7"] = "Segment Not in Proper Sequence",
            ["8"] = "Segment Has Data Element Errors",
            ["I4"] = "Implementation \"Not Used\" Segment Present",
            ["I6"] = "Implementation Dependent Segment Missing",
            ["I7"] = "Implementation Loop Occurs Under Minimum Times",
            ["I8"] = "Implementation Segment Below Minimum Use",
            ["I9"] = "Implementation Dependent \"Not Used\" Segment Present",
        };

        private static readonly Dictionary<string, string> ElementErrors = new()
        {
            ["1"] = "Required Data Element Missing",
            ["2"] = "Conditional Required Data Element Missing",
            ["3"] = "Too Many Data Elements",
            ["4"] = "Data Element Too Short",
            ["5"] = "Data Element Too Long",
            ["6"] = "Invalid Character In Data Element",
            ["7"] = "Invalid Code Value",
            ["8"] = "Invalid Date",
            ["9"] = "Invalid Time",
            ["10"] = "Exclusion Condition Violated",
            ["12"] = "Too Many Repetitions",
            ["13"] = "Too Many Components",
            ["I6"] = "Code Value Not Used in Implementation",
            ["I9"] = "Implementation Dependent Data Element Missing",
            ["I10"] = "Implementation \"Not Used\" Data Element Present",
            ["I11"] = "Implementation Too Few Repetitions",
            ["I12"] = "Implementation Pattern Match Failure",
            ["I13"] = "Implementation Dependent \"Not Used\" Data Element Present",
        };

        private static readonly Dictionary<string, string> SetErrors = new()
        {
            ["1"] = "Transaction Set Not Supported",
            ["2"] = "Transaction Set Trailer Missing",
            ["3"] = "Transaction Set Control Number in Header and Trailer Do Not Match",
            ["4"] = "Number of Included Segments Does Not Match Actual Count",
            ["5"] = "One or More Segments in Error",
            ["6"] = "Missing or Invalid Transaction Set Identifier",
            ["7"] = "Missing or Invalid Transaction Set Control Number",
        };

        // ───────────────────────────── 277 ─────────────────────────────

        public JsonObject ClaimStatus(Transaction t)
        {
            var result = new JsonObject();
            var statuses = new List<JsonObject>();
            JsonObject? last = null;
            string? level = null, name = null, trace = null;

            foreach (var s in t.Body)
            {
                switch (s.Id)
                {
                    case "HL":
                        level = Codes.Level.Meaning(s[3]) ?? s[3];
                        name = trace = null;
                        last = null;
                        break;
                    case "NM1":
                        name = Name(s);
                        break;
                    case "TRN" when s[1] == "2":
                        trace = s[2];
                        break;
                    case "STC":
                        var parts = s[1].Split(d.Component);
                        last = new JsonObject
                        {
                            ["segment"] = s.Index + 1,
                            ["level"] = level,
                            ["name"] = name,
                            ["trace"] = trace,
                            ["category"] = Coded(parts[0], Codes.StatusCategory),
                            ["status_code"] = parts.Length > 1 ? parts[1] : "",
                            ["date"] = Date(s[2]),
                            ["charged"] = Money(s[4]),
                            ["paid"] = Money(s[5]),
                        };
                        statuses.Add(last);
                        break;
                    case "REF" when last != null && s[1] == "1K":
                        last["payer_claim_number"] = s[2];
                        break;
                    case "DTP" when last != null && s[1] == "472":
                        last["service_dates"] = s[3];
                        break;
                }
            }

            result["totals"] = new JsonObject
            {
                ["statuses"] = statuses.Count,
                ["by_category"] = Tally(statuses.Select(x => x["category"]?["meaning"]?.ToString() ?? x["category"]?["code"]?.ToString() ?? "")),
            };
            result["statuses"] = Capped(statuses);
            return result;
        }

        // ───────────────────────────── 271 ─────────────────────────────

        public JsonObject Benefits(Transaction t)
        {
            var result = new JsonObject();
            var people = new List<JsonObject>();
            JsonObject? person = null;

            foreach (var s in t.Body)
            {
                switch (s.Id)
                {
                    case "NM1" when s[1] == "PR":
                        result["payer"] = s[3];
                        break;
                    case "NM1" when s[1] == "1P":
                        result["provider"] = s[3];
                        break;
                    case "NM1" when s[1] is "IL" or "03":
                        person = new JsonObject { ["segment"] = s.Index + 1, ["role"] = s[1] == "IL" ? "Subscriber" : "Dependent", ["name"] = Name(s), ["member_id"] = s[9] };
                        people.Add(person);
                        break;
                    case "DMG" when person != null:
                        person["birth_date"] = Date(s[2]);
                        person["gender"] = s[3];
                        break;
                    case "AAA":
                        Add(person ?? result, "rejections", new JsonObject { ["valid"] = s[1], ["reason"] = s[3], ["follow_up"] = s[4] });
                        break;
                    case "EB" when person != null:
                        var benefit = new JsonObject
                        {
                            ["information"] = Coded(s[1], Codes.Benefit),
                            ["coverage_level"] = s[2],
                            ["service_types"] = s[3],
                            ["insurance_type"] = s[4],
                            ["plan"] = s[5],
                        };
                        if (s[7].Length > 0) benefit["amount"] = Money(s[7]);
                        if (s[8].Length > 0) benefit["percent"] = s[8];
                        Add(person, "benefits", benefit);
                        break;
                }
            }

            result["people"] = Capped(people);
            return result;
        }

        // ───────────────────────── 270, 276 and the rest ─────────────────────────

        public JsonObject Hierarchy(Transaction t)
        {
            var levels = new List<JsonObject>();
            JsonObject? level = null;
            foreach (var s in t.Body)
            {
                switch (s.Id)
                {
                    case "HL":
                        level = new JsonObject { ["segment"] = s.Index + 1, ["id"] = s[1], ["parent"] = s[2], ["level"] = Coded(s[3], Codes.Level) };
                        levels.Add(level);
                        break;
                    case "NM1" when level != null:
                        Add(level, "names", new JsonObject { ["role"] = s[1], ["name"] = Name(s), ["id"] = s[9] });
                        break;
                    case "TRN" when level != null:
                        level["trace"] = s[2];
                        break;
                    case "EQ" when level != null:
                        Add(level, "inquiries", JsonValue.Create(s[1]));
                        break;
                    case "AMT" when level != null:
                        level["amount"] = Money(s[2]);
                        break;
                    case "REF" when level != null:
                        Add(level, "references", new JsonObject { ["qualifier"] = s[1], ["value"] = s[2] });
                        break;
                }
            }
            return new JsonObject { ["levels"] = Capped(levels) };
        }

        public JsonObject Counts(Transaction t) => new()
        {
            ["note"] = "The explorer doesn't know this transaction's layout, so here are its segments by ID. get_transaction shows them.",
            ["segments"] = Tally(t.Body.Select(s => s.Id)),
        };

        // ───────────────────────────── helpers ─────────────────────────────

        /// <summary>A composite element with its parts joined by a colon whatever the file's separator: "HC:99213".</summary>
        private string Composite(string value) => string.Join(':', value.Split(d.Component));

        /// <summary>An NM1's name: "DOE, JANE" for a person, the organisation's name otherwise.</summary>
        private static string Name(Segment s)
        {
            var last = s[3].Trim();
            var first = s[4].Trim();
            var middle = s[5].Trim();
            if (s[2].Trim() != "1" || first.Length == 0) return last;
            return middle.Length > 0 ? $"{last}, {first} {middle}" : $"{last}, {first}";
        }

        private static JsonObject Party(Segment s)
        {
            var o = new JsonObject { ["name"] = s[2] };
            if (s[4].Length > 0) o["id"] = s[4];
            return o;
        }
    }

    // ───────────────────────────── shared ─────────────────────────────

    private static JsonObject Coded(string code, Dictionary<string, string> list)
    {
        var o = new JsonObject { ["code"] = code.Trim() };
        if (list.Meaning(code) is { } meaning) o["meaning"] = meaning;
        return o;
    }

    /// <summary>An amount as the file wrote it, so nothing is lost to rounding: "150.00".</summary>
    private static string Money(string value) => value.Trim();

    private static decimal Amount(string value) =>
        decimal.TryParse(value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var d) ? d : 0;

    /// <summary>A CCYYMMDD date as ISO (2026-11-03); anything else (a range, a short date) as written.</summary>
    private static string Date(string value)
    {
        var v = value.Trim();
        return v.Length == 8 && DateOnly.TryParseExact(v, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            : v;
    }

    private static void Add(JsonObject parent, string list, JsonNode? item)
    {
        if (parent[list] is not JsonArray array) parent[list] = array = [];
        array.Add(item);
    }

    private static JsonObject Tally(IEnumerable<string> keys)
    {
        var o = new JsonObject();
        foreach (var g in keys.GroupBy(k => k).OrderByDescending(g => g.Count()).ThenBy(g => g.Key, StringComparer.Ordinal))
            o[g.Key.Length == 0 ? "(none)" : g.Key] = g.Count();
        return o;
    }

    private static JsonNode Capped(List<JsonObject> items)
    {
        var array = new JsonArray(items.Take(MaxItems).Select(JsonNode (i) => i).ToArray());
        if (items.Count <= MaxItems) return array;
        return new JsonObject
        {
            ["shown"] = MaxItems,
            ["total"] = items.Count,
            ["note"] = $"Only the first {MaxItems} are listed. find_segments and get_transaction reach the rest.",
            ["items"] = array,
        };
    }
}
