namespace PartnerAppPortal.Core.Models.Assessments
{
    public class AssessmentSubmission
    {
        // The customer being assessed
        public string CustomerId { get; set; } = string.Empty;

        // All answers grouped by section and questionId
        // Example:
        // {
        //   "Solutions": { "SOL-001": "yes", "SOL-002": "no" },
        //   "Marketplace": { "MP-001": "yes" }
        // }
        public Dictionary<string, Dictionary<string, string>> Answers { get; set; }
            = new(StringComparer.OrdinalIgnoreCase);
    }
}
