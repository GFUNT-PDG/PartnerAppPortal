using PartnerAppPortal.Core.Models.SalesAccelerator;

namespace PartnerAppPortal.Portal.Services;

public interface ISalesAcceleratorApiClient
{
    Task<byte[]> DownloadTemplateAsync();
    Task<SalesAcceleratorSubmitResult> SubmitCsvAsync(
        Guid partnerCustomerId,
        string submittedBy,
        string fileName,
        byte[] fileContent);
    Task<List<SalesAcceleratorSubmissionListItem>> ListSubmissionsAsync(Guid? partnerCustomerId = null);
    Task<(string FileName, string ContentType, byte[] Content)> DownloadSubmissionFileAsync(Guid submissionId);
    Task<List<SalesAcceleratorAccount>> ListAccountsAsync(Guid? partnerCustomerId = null);
    Task SaveAccountFieldsAsync(IEnumerable<SalesAcceleratorAccountUpdate> updates);
}
