namespace PartnerAppPortal.Core.Models.Assessments
{
    public class AssessmentResult
    {
        // Total score across all categories
        public int TotalScore { get; set; }

        public List<AssessmentScore> SectionScores { get; set; } = new();
        public Dictionary<string, int> CategoryScores { get; set; }
            = new(StringComparer.OrdinalIgnoreCase);
    }
}
