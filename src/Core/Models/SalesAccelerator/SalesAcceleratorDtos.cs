namespace PartnerAppPortal.Core.Models.SalesAccelerator;

public class SalesAcceleratorSubmitResult
{
    public Guid SubmissionId { get; set; }
    public int InsertedCount { get; set; }
    public int DuplicateCount { get; set; }
    public int InvalidCount { get; set; }
    public List<SalesAcceleratorDuplicate> Duplicates { get; set; } = new();
    public List<string> InvalidRows { get; set; } = new();
}

public class SalesAcceleratorDuplicate
{
    public string Domain { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string CurrentPartner { get; set; } = string.Empty;
}

public class SalesAcceleratorSubmissionListItem
{
    public Guid Id { get; set; }
    public Guid PartnerCustomerId { get; set; }
    public string Partner { get; set; } = string.Empty;
    public string SubmittedBy { get; set; } = string.Empty;
    public DateTime PartnerUploadDate { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public int InsertedCount { get; set; }
    public int DuplicateCount { get; set; }
}

public class SalesAcceleratorAccountUpdate
{
    public Guid Id { get; set; }
    public string? Tpid { get; set; }
    public DateTime? SentToMitch { get; set; }
    public DateTime? DateSubmitted { get; set; }
    public DateTime? DateVerified { get; set; }
}

public class SalesAcceleratorFileDownload
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "text/csv";
    public byte[] Content { get; set; } = Array.Empty<byte>();
}
