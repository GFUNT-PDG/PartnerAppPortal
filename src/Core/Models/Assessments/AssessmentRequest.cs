using System;

namespace PartnerAppPortal.Core.Models.Assessments
{
    public class AssessmentRequest
    {
        public Guid Id { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
