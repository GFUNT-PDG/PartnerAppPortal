using PartnerAppPortal.Core.Models.SalesAccelerator;

namespace PartnerAppPortal.Core.Interfaces.Repositories;

public interface ISalesAcceleratorRepository
{
    Task<SalesAcceleratorAccount?> FindByNormalizedDomainAsync(string normalizedDomain);
    Task<IReadOnlyList<SalesAcceleratorAccount>> FindByNormalizedDomainsAsync(IEnumerable<string> normalizedDomains);
    Task AddSubmissionWithAccountsAsync(
        SalesAcceleratorSubmission submission,
        IEnumerable<SalesAcceleratorAccount> accounts);
    Task<IReadOnlyList<SalesAcceleratorSubmissionListItem>> ListSubmissionsAsync(Guid? partnerCustomerId);
    Task<SalesAcceleratorFileDownload?> GetSubmissionFileAsync(Guid submissionId);
    Task<IReadOnlyList<SalesAcceleratorAccount>> ListAccountsAsync(Guid? partnerCustomerId);
    Task UpdateAccountFieldsAsync(IEnumerable<SalesAcceleratorAccountUpdate> updates);
}
