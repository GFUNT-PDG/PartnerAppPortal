namespace PartnerAppPortal.Core.Models.Assessments
{
    public class MarketPlaceUsage
    {
        public Guid Id { get; set; }
        public Guid AssessmentId { get; set; }

        public int Offers { get; set; }
        public int Transactions { get; set; }
        public decimal Revenue { get; set; }
        public decimal Growth { get; set; }
    }

    public class MarketPlaceUsageModel
    {
        public Guid AssessmentId { get; set; }

        public int Offers { get; set; }
        public int Transactions { get; set; }
        public decimal Revenue { get; set; }
        public decimal Growth { get; set; }
    }
}
