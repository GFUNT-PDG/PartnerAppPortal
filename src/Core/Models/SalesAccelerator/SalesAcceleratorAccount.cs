namespace PartnerAppPortal.Core.Models.SalesAccelerator;

public class SalesAcceleratorAccount
{
    public Guid Id { get; set; }
    public Guid SubmissionId { get; set; }
    public Guid PartnerCustomerId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string NormalizedDomain { get; set; } = string.Empty;
    public string? AccountType { get; set; }
    public string SubmittedBy { get; set; } = string.Empty;
    public string Partner { get; set; } = string.Empty;
    public DateTime PartnerUploadDate { get; set; }
    public string? Tpid { get; set; }
    public DateTime? SentToMitch { get; set; }
    public DateTime? DateSubmitted { get; set; }
    public DateTime? DateVerified { get; set; }
}
