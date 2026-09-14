using System;
using System.Collections.Generic;

namespace PartnerAppPortal.Core.Models.Assessments
{
    public class AssessmentFinalResult
    {
        public Guid AssessmentId { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;

        public DateTime CompletedAt { get; set; }

        public int TotalScore { get; set; }

        public int TotalMaxScore { get; set; }

        public double TotalPercentage { get; set; }

        public List<CategoryScoreResult> CategoryScores { get; set; } = new();
    }

}
