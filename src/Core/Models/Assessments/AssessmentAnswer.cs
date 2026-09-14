namespace PartnerAppPortal.Core.Models.Assessments
{
    /// <summary>Persisted per-question answer for an assessment.</summary>
    public class AssessmentAnswer
    {
        public Guid Id { get; set; }
        public Guid AssessmentId { get; set; }
        public string QuestionId { get; set; } = string.Empty;
        public string? Response { get; set; }
        public int PointsEarned { get; set; }
    }

    public class AssessmentAnswerDto
    {
        public string QuestionId { get; set; } = string.Empty;
        public string? Response { get; set; }
        public int PointsEarned { get; set; }
    }
}
