namespace PartnerAppPortal.Core.Models.Assessments
{
    public class AssessmentEntity
    {
        public Guid Id { get; set; }

        public string CustomerId { get; set; } = string.Empty;

        public DateTimeOffset? SubmittedAt { get; set; }

        public string AnswersJson { get; set; } = string.Empty;
        public double TotalScore { get; set; }

         // This is REQUIRED because your interface demands it
    public List<AssessmentResponseItem> Responses { get; set; } = new();


    }
}
