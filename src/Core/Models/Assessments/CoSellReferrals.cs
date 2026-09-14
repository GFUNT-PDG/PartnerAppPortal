namespace PartnerAppPortal.Core.Models.Assessments
{
    public class CoSellReferrals
    {
        public Guid Id { get; set; }
        public Guid AssessmentId { get; set; }

        public int CoSellOpportunities { get; set; }
        public int ReferralsSubmitted { get; set; }
        public int ReferralsWon { get; set; }
        public decimal PipelineValue { get; set; }
    }

    public class CoSellReferralsModel
    {
        public Guid AssessmentId { get; set; }

        public int CoSellOpportunities { get; set; }
        public int ReferralsSubmitted { get; set; }
        public int ReferralsWon { get; set; }
        public decimal PipelineValue { get; set; }
    }
}
