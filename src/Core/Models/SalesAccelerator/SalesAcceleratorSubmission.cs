namespace PartnerAppPortal.Core.Models.SalesAccelerator;

public class SalesAcceleratorSubmission
{
    public Guid Id { get; set; }
    public Guid PartnerCustomerId { get; set; }
    public string Partner { get; set; } = string.Empty;
    public string SubmittedBy { get; set; } = string.Empty;
    public DateTime PartnerUploadDate { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "text/csv";
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public int InsertedCount { get; set; }
    public int DuplicateCount { get; set; }
}
