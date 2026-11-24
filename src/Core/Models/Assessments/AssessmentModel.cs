namespace PartnerAppPortal.Core.Models.Assessments
{
    public class AssessmentModel
    {
        public Guid Id { get; set; }
        public DateTime? SubmittedAt { get; set; }

        /// <summary>Original onboarding date. Stays put when the assessment is revised.</summary>
        public DateTime? OnboardingAt { get; set; }

        public Guid CustomerId { get; set; }
        public string? CustomerName { get; set; }

        /// <summary>Overall partner score (average of section percentages).</summary>
        public decimal TotalScore { get; set; }

        /// <summary>Overall score from the first submit (before later revisions).</summary>
        public decimal? BaselineOverallScore { get; set; }

        public decimal? OverallDelta =>
            BaselineOverallScore is null ? null : Math.Round(TotalScore - BaselineOverallScore.Value, 1);

        public string? Status { get; set; }

        /// <summary>Per-category breakdown (each section out of 100).</summary>
        public List<CategoryScoreResult> SectionScores { get; set; } = new();
    }
}
