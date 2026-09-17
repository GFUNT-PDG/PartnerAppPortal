using PartnerAppPortal.Core.Models.SalesAccelerator;

namespace PartnerAppPortal.Core.Interfaces.Services;

public interface ISalesAcceleratorService
{
    Task<SalesAcceleratorSubmitResult> SubmitCsvAsync(
        Guid partnerCustomerId,
        string submittedBy,
        string originalFileName,
        byte[] fileContent,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SalesAcceleratorSubmissionListItem>> ListSubmissionsAsync(Guid? partnerCustomerId);
    Task<SalesAcceleratorFileDownload?> GetSubmissionFileAsync(Guid submissionId);
    Task<IReadOnlyList<SalesAcceleratorAccount>> ListAccountsAsync(Guid? partnerCustomerId);
    Task UpdateAccountFieldsAsync(IEnumerable<SalesAcceleratorAccountUpdate> updates);
}
