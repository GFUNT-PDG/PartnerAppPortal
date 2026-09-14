namespace PartnerAppPortal.Core.Models.Assessments
{
    public class PartnerScoreResult
    {
        public Guid AssessmentId { get; set; }
        public CategoryScoreResult SolutionsAndSpecializations { get; set; } = new();
        public CategoryScoreResult CustomersAndCsp { get; set; } = new();
        public CategoryScoreResult CoSellAndReferrals { get; set; } = new();
        public CategoryScoreResult MarketplaceUsage { get; set; } = new();
        public CategoryScoreResult IncentivesUsage { get; set; } = new();
        public decimal OverallPartnerScore { get; set; }
    }
}
