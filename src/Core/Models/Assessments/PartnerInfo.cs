namespace PartnerAppPortal.Core.Models.Assessments
{
    public class PartnerInfo
    {
        public Guid Id { get; set; }
        public Guid AssessmentId { get; set; }

        public string? PartnerName { get; set; }
        public string? MpnId { get; set; }
        public string? Country { get; set; }
        public string? Segment { get; set; }
        public int? Tenure { get; set; }
        public int? CommitmentLength { get; set; }
        public string? Region { get; set; }
        public string? Industry { get; set; }

    }

    public class PartnerInfoModel
    {
        public Guid AssessmentId { get; set; }

        public string? PartnerName { get; set; }
        public string? MpnId { get; set; }
        public string? Country { get; set; }
        public string? Segment { get; set; }
        public int? Tenure { get; set; }
        public int? CommitmentLength { get; set; }
        public string? Region { get; set; }
        public string? Industry { get; set; }

        
    }
}
