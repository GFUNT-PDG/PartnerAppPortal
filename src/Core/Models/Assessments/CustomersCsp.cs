namespace PartnerAppPortal.Core.Models.Assessments
{
    public class CustomersCsp
    {
        public Guid Id { get; set; }
        public Guid AssessmentId { get; set; }

        public string? CspTier { get; set; }
        public int ActiveSubscriptions { get; set; }
        public decimal AzureConsumption { get; set; }
        public decimal MarketplaceUsage { get; set; }
        public decimal IncentivesUsage { get; set; }
    }

    public class CustomersCspModel
    {
        public Guid AssessmentId { get; set; }

        public string? CspTier { get; set; }
        public int ActiveSubscriptions { get; set; }
        public decimal AzureConsumption { get; set; }
        public decimal MarketplaceUsage { get; set; }
        public decimal IncentivesUsage { get; set; }
    }
}
