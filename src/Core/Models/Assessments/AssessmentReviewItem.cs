namespace PartnerAppPortal.Core.Models.Assessments
{
    public class AssessmentReviewItem
    {
        public string Section { get; set; } = string.Empty;
        public string? Subsection { get; set; }
        public string QuestionText { get; set; } = string.Empty;

        public string? Response { get; set; }
        public decimal? Score { get; set; }
    }
}
