namespace PartnerAppPortal.Core.Models.Assessments
{
    public class GrowthMetrics
    {
        public Guid Id { get; set; }
        public Guid AssessmentId { get; set; }

        public decimal YearOverYear { get; set; }
        public int CustomerAdds { get; set; }
        public decimal RevenueGrowth { get; set; }
        public decimal PipelineGrowth { get; set; }
    }

    public class GrowthMetricsModel
    {
        public Guid AssessmentId { get; set; }

        public decimal YearOverYear { get; set; }
        public int CustomerAdds { get; set; }
        public decimal RevenueGrowth { get; set; }
        public decimal PipelineGrowth { get; set; }
    }
}
