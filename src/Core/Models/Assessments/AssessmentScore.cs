namespace PartnerAppPortal.Core.Models.Assessments
{
    public class AssessmentScore
    {
        public Guid Id { get; set; }
        public Guid AssessmentId { get; set; }
        public string Section { get; set; } = string.Empty;
        public int Score { get; set; }

        public int TotalScore { get; set; }
        public Guid QuestionId { get; set; }

        public string? Response { get; set; }
        public string? Result { get; set; }
        public string? Notes { get; set; }



        // JSON dictionary: { "Solutions": 12, "Marketplace": 8, ... }
        public string CategoryScoresJson { get; set; } = string.Empty;
    }
}
