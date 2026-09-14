namespace PartnerAppPortal.Core.Models.Assessments
{
    public class IncentivesUsage
    {
        public Guid Id { get; set; }
        public Guid AssessmentId { get; set; }

        public decimal Earned { get; set; }
        public decimal Redeemed { get; set; }
        public decimal Balance { get; set; }
        public decimal Growth { get; set; }
    }

    public class IncentivesUsageModel
    {
        public Guid AssessmentId { get; set; }

        public decimal Earned { get; set; }
        public decimal Redeemed { get; set; }
        public decimal Balance { get; set; }
        public decimal Growth { get; set; }
    }
}
