namespace PartnerAppPortal.Core.Models.Assessments
{
    public class AssessmentResponseItem
    {
        public string Section { get; set; } = string.Empty;
        public string? Subsection { get; set; }
        public string QuestionText { get; set; } = string.Empty;

        // Response does NOT exist in DB → always null
        public string? Response { get; set; } = null;

        // Score from AssessmentScore (nullable)
        public int? Score { get; set; }

        public int MaxScore { get; set; }

    }
}
