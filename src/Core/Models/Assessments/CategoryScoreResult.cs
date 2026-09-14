namespace PartnerAppPortal.Core.Models.Assessments
{
    public class CategoryScoreResult
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal TotalPointsEarned { get; set; }
        public decimal PotentialPoints { get; set; }
        public double Percentage { get; set; }

        /// <summary>First-submitted percentage for this section (if a revision exists).</summary>
        public double? BaselinePercentage { get; set; }

        public double? DeltaPercentage =>
            BaselinePercentage is null ? null : Math.Round(Percentage - BaselinePercentage.Value, 1);
    }
}
