namespace PartnerAppPortal.Core.Models.Assessments
{
    public class AssessmentScoreBaseline
    {
        public Guid AssessmentId { get; set; }
        public DateTimeOffset CapturedAt { get; set; }
        public decimal OverallScore { get; set; }
        public string? SectionScoresJson { get; set; }
    }
}
