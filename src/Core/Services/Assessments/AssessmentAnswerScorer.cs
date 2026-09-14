using System.Collections.Generic;
using PartnerAppPortal.Core.Models.Assessments;

namespace PartnerAppPortal.Core.Services.Assessments
{
    /// <summary>
    /// Scores answers from the Excel QuestionID / Result / Score formulas.
    /// Portal IDs (SOL-001…) are mapped 1:1 to the workbook row order.
    /// N/A answers and Excel "Not Applicable" questions are never scored (0).
    /// </summary>
    public static class AssessmentAnswerScorer
    {
        // Excel rows marked Not Applicable — never contribute points
        private static readonly HashSet<string> NeverScored = new(StringComparer.OrdinalIgnoreCase)
        {
            "CSP-002", "CSP-003", "CSP-004", "CSP-005", "CSP-009",
            "CSP-016", "CSP-017", "CSP-018", "CSP-020",
            "COSELL-002", "COSELL-004", "COSELL-009", "COSELL-013",
            "COSELL-019", "COSELL-020", "COSELL-021", "COSELL-023", "COSELL-024", "COSELL-026",
            "MKT-006", "MKT-007", "MKT-008",
        };

        // "Going to target" depends on the matching "Does Partner have … Designation?" answer
        private static readonly Dictionary<string, string> TargetDependsOnHave = new(StringComparer.OrdinalIgnoreCase)
        {
            ["SOL-005"] = "SOL-003", // Business Applications
            ["SOL-008"] = "SOL-006", // Data & AI
            ["SOL-011"] = "SOL-009", // Digital & App Innovation
            ["SOL-014"] = "SOL-012", // Infrastructure
            ["SOL-017"] = "SOL-015", // Modern Work
            ["SOL-020"] = "SOL-018", // Security
        };

        public static int Score(AssessmentQuestion question, string? response)
            => Score(question.QuestionId, response, null);

        public static int Score(
            string questionId,
            string? response,
            IReadOnlyDictionary<string, string?>? allResponses)
        {
            if (string.IsNullOrWhiteSpace(questionId))
                return 0;

            if (NeverScored.Contains(questionId))
                return 0;

            if (IsNotApplicable(response))
                return 0;

            var answer = (response ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(answer) || IsPleaseChoose(answer))
                return 0;

            // --- Solutions & Specializations ---
            if (questionId is "SOL-001" or "SOL-002")
                return YesPoints(answer, 4);

            if (questionId is "SOL-003" or "SOL-006" or "SOL-009" or "SOL-012" or "SOL-015" or "SOL-018")
                return YesPoints(answer, 5);

            // Designation % score: only when Have=Yes OR Target=Yes
            if (questionId is "SOL-004" or "SOL-007" or "SOL-010" or "SOL-013" or "SOL-016" or "SOL-019")
            {
                if (allResponses is not null &&
                    !QuestionAnswerGate.IsDesignationScoreApplicable(questionId, allResponses))
                    return 0;

                return DesignationScoreBands(answer);
            }

            // "Going to target" itself earns no points; it only unlocks the score field
            if (TargetDependsOnHave.ContainsKey(questionId))
                return 0;

            if (IsAdvancedSpecialization(questionId))
                return YesPoints(answer, 1);

            // --- CSP ---
            if (questionId == "CSP-001")
            {
                if (Eq(answer, "Direct CSP")) return 10;
                if (Eq(answer, "Indirect CSP")) return 5;
                if (Eq(answer, "Not a CSP")) return 0;
                return 0;
            }

            if (questionId is "CSP-006" or "CSP-007" or "CSP-008" or "CSP-010" or "CSP-011"
                or "CSP-012" or "CSP-013" or "CSP-015")
                return YesPoints(answer, 5);

            if (questionId == "CSP-014")
            {
                if (Eq(answer, "Premier Support")) return 10;
                if (Eq(answer, "Advanced Support")) return 5;
                return 0; // None / other
            }

            // Net new customers past 12 months: MIN(1, value/20) → 4/8/12/16/20
            if (questionId == "CSP-019")
                return CountOver20Bands(answer);

            if (questionId == "CSP-021")
            {
                if (Eq(answer, "3-year Commit")) return 15;
                if (Eq(answer, "Yearly Commit")) return 10;
                if (Eq(answer, "Monthly Commit")) return 5;
                return 0;
            }

            if (questionId == "CSP-022")
            {
                if (Eq(answer, "3 or More Years")) return 5;
                if (Eq(answer, "2 Years")) return 3;
                if (Eq(answer, "1 year or less")) return 1;
                return 0;
            }

            // --- Co-Sell ---
            if (questionId is "COSELL-001" or "COSELL-003" or "COSELL-005" or "COSELL-006" or "COSELL-007"
                or "COSELL-008" or "COSELL-010" or "COSELL-011" or "COSELL-012" or "COSELL-014"
                or "COSELL-015" or "COSELL-016" or "COSELL-017" or "COSELL-018" or "COSELL-025")
                return YesPoints(answer, 5);

            // New referrals past 12 months
            if (questionId == "COSELL-022")
                return CountOver20Bands(answer);

            // --- Marketplace ---
            if (questionId is "MKT-001" or "MKT-002" or "MKT-003")
                return YesPoints(answer, 3);

            if (questionId == "MKT-004")
                return YesPoints(answer, 5);

            // How many marketplace offers
            if (questionId == "MKT-005")
                return CountOver20Bands(answer);

            if (questionId is "MKT-009" or "MKT-010" or "MKT-012")
                return YesPoints(answer, 5);

            // Private / Resell (either choice awards 5)
            if (questionId == "MKT-011")
            {
                if (Eq(answer, "Private") || Eq(answer, "Resell") || IsYes(answer))
                    return 5;
                return 0;
            }

            // --- Incentives ---
            if (questionId is "INC-001" or "INC-002")
                return YesPoints(answer, 10);

            if (questionId is "INC-003" or "INC-004")
                return YesPoints(answer, 5);

            if (questionId == "INC-005")
                return YesPoints(answer, 20);

            return 0;
        }

        /// <summary>
        /// Excel designation score: Result = MIN(1, value/100), then
        /// Score = IF(>=1,5, IF(>=0.8,4, IF(>=0.6,3, IF(>=0.4,2, IF(>=0.2,1,0)))))
        /// </summary>
        private static int DesignationScoreBands(string answer)
        {
            if (!decimal.TryParse(answer, out var raw))
                return 0;

            var ratio = raw > 1m ? Math.Min(1m, raw / 100m) : Math.Max(0m, raw);
            if (ratio >= 1.0m) return 5;
            if (ratio >= 0.8m) return 4;
            if (ratio >= 0.6m) return 3;
            if (ratio >= 0.4m) return 2;
            if (ratio >= 0.2m) return 1;
            return 0;
        }

        /// <summary>
        /// Excel count metrics: Result = MIN(1, value/20), Score 4/8/12/16/20 by 0.2 bands.
        /// </summary>
        private static int CountOver20Bands(string answer)
        {
            if (!decimal.TryParse(answer, out var raw))
                return 0;

            var ratio = Math.Min(1m, Math.Max(0m, raw / 20m));
            if (ratio >= 1.0m) return 20;
            if (ratio >= 0.8m) return 16;
            if (ratio >= 0.6m) return 12;
            if (ratio >= 0.4m) return 8;
            if (ratio >= 0.2m) return 4;
            return 0;
        }

        private static int YesPoints(string answer, int points)
            => IsYes(answer) ? points : 0;

        private static bool IsAdvancedSpecialization(string questionId)
        {
            // SOL-021 … SOL-052
            if (!questionId.StartsWith("SOL-", StringComparison.OrdinalIgnoreCase))
                return false;
            if (!int.TryParse(questionId.AsSpan(4), out var n))
                return false;
            return n is >= 21 and <= 52;
        }

        private static string? Lookup(IReadOnlyDictionary<string, string?>? map, string questionId)
        {
            if (map is null) return null;
            return map.TryGetValue(questionId, out var v) ? v : null;
        }

        public static bool IsNotApplicable(string? response)
        {
            if (string.IsNullOrWhiteSpace(response))
                return false;

            var a = response.Trim();
            return a.Equals("N/A", StringComparison.OrdinalIgnoreCase)
                || a.Equals("NA", StringComparison.OrdinalIgnoreCase)
                || a.Equals("Not Applicable", StringComparison.OrdinalIgnoreCase)
                || a.Equals("NotApplicable", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsPleaseChoose(string answer)
            => answer.StartsWith("Please Choose", StringComparison.OrdinalIgnoreCase);

        private static bool IsYes(string? answer)
            => !string.IsNullOrWhiteSpace(answer)
               && answer.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase);

        private static bool IsNo(string? answer)
            => !string.IsNullOrWhiteSpace(answer)
               && answer.Trim().Equals("No", StringComparison.OrdinalIgnoreCase);

        private static bool Eq(string answer, string expected)
            => answer.Equals(expected, StringComparison.OrdinalIgnoreCase);
    }
}
