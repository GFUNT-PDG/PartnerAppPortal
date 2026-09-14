using System.Collections.Generic;

namespace PartnerAppPortal.Core.Services.Assessments
{
    /// <summary>
    /// Designation family gates (Have / Score / Target).
    /// Rules:
    /// - Have = Yes → Target locked (No); designation Score is required/enabled
    /// - Have = No, Target = Yes → designation Score is required/enabled
    /// - Have = No, Target = No → Score not needed (locked/cleared)
    /// </summary>
    public static class QuestionAnswerGate
    {
        public sealed record DesignationFamily(string HaveId, string ScoreId, string TargetId);

        public static readonly IReadOnlyList<DesignationFamily> DesignationFamilies =
        [
            new("SOL-003", "SOL-004", "SOL-005"), // Business Applications
            new("SOL-006", "SOL-007", "SOL-008"), // Data & AI
            new("SOL-009", "SOL-010", "SOL-011"), // Digital & App Innovation
            new("SOL-012", "SOL-013", "SOL-014"), // Infrastructure
            new("SOL-015", "SOL-016", "SOL-017"), // Modern Work
            new("SOL-018", "SOL-019", "SOL-020"), // Security
        ];

        public static bool IsYes(string? response) =>
            !string.IsNullOrWhiteSpace(response) &&
            response.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase);

        public static bool IsNo(string? response) =>
            !string.IsNullOrWhiteSpace(response) &&
            response.Trim().Equals("No", StringComparison.OrdinalIgnoreCase);

        /// <summary>True when the designation score field should be answered/scored.</summary>
        public static bool IsDesignationScoreApplicable(
            string scoreQuestionId,
            IReadOnlyDictionary<string, string?> responses)
        {
            foreach (var f in DesignationFamilies)
            {
                if (!IdsEqual(scoreQuestionId, f.ScoreId))
                    continue;

                var have = Get(responses, f.HaveId);
                var target = Get(responses, f.TargetId);

                // Have designation → score needed
                if (IsYes(have))
                    return true;

                // Don't have it, but targeting → score needed
                if (IsYes(target))
                    return true;

                // Don't have it and not targeting → no score
                return false;
            }

            return true;
        }

        /// <summary>Question is visible but not editable.</summary>
        public static bool IsLocked(
            string questionId,
            IReadOnlyDictionary<string, string?> responses)
        {
            foreach (var f in DesignationFamilies)
            {
                var have = Get(responses, f.HaveId);
                var target = Get(responses, f.TargetId);

                if (IdsEqual(questionId, f.ScoreId))
                {
                    // Locked ONLY when both Have and Target are not Yes
                    return !IsYes(have) && !IsYes(target);
                }

                if (IdsEqual(questionId, f.TargetId))
                {
                    // Already have designation → targeting N/A
                    return IsYes(have);
                }
            }

            return false;
        }

        /// <summary>True when proposed Yes is not allowed for this question.</summary>
        public static bool IsResponseForbidden(
            string questionId,
            string? proposedResponse,
            IReadOnlyDictionary<string, string?> responses)
        {
            if (!IsYes(proposedResponse))
                return false;

            foreach (var f in DesignationFamilies)
            {
                if (!IdsEqual(questionId, f.TargetId))
                    continue;

                // Have = Yes → Target cannot be Yes
                if (IsYes(Get(responses, f.HaveId)))
                    return true;
            }

            return false;
        }

        /// <summary>Clears / coerces dependents after a controlling answer changes.</summary>
        public static void ApplySideEffects(
            string changedQuestionId,
            IDictionary<string, string?> responses)
        {
            foreach (var f in DesignationFamilies)
            {
                var have = Get(responses, f.HaveId);
                var target = Get(responses, f.TargetId);

                if (IdsEqual(changedQuestionId, f.HaveId))
                {
                    if (IsYes(have))
                    {
                        // Have it → targeting off; keep/require score
                        responses[f.TargetId] = "No";
                    }
                    else if (!IsYes(target))
                    {
                        // Don't have it and not targeting → clear score
                        responses[f.ScoreId] = null;
                    }
                }

                if (IdsEqual(changedQuestionId, f.TargetId))
                {
                    if (!IsYes(have) && !IsYes(target))
                    {
                        // Not targeting → score not needed
                        responses[f.ScoreId] = null;
                    }
                }
            }
        }

        /// <summary>Normalize gated answers before save.</summary>
        public static void NormalizeAll(IDictionary<string, string?> responses)
        {
            foreach (var f in DesignationFamilies)
            {
                var have = Get(responses, f.HaveId);
                var target = Get(responses, f.TargetId);

                if (IsYes(have))
                {
                    responses[f.TargetId] = "No";
                }
                else if (!IsYes(target))
                {
                    responses[f.ScoreId] = null;
                }
            }
        }

        private static string? Get(IDictionary<string, string?> map, string id) =>
            map.TryGetValue(id, out var v) ? v : null;

        private static string? Get(IReadOnlyDictionary<string, string?> map, string id) =>
            map.TryGetValue(id, out var v) ? v : null;

        private static bool IdsEqual(string a, string b) =>
            string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }
}
