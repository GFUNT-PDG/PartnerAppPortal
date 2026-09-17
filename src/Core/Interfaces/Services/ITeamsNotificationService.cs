using PartnerAppPortal.Core.Models.SalesAccelerator;

namespace PartnerAppPortal.Core.Interfaces.Services;

public interface ITeamsNotificationService
{
    Task NotifyNewSubmissionAsync(
        string partner,
        string submittedBy,
        SalesAcceleratorSubmitResult result,
        CancellationToken cancellationToken = default);

    Task NotifyDuplicateDomainsAsync(
        string submittingPartner,
        string submittedBy,
        IReadOnlyList<SalesAcceleratorDuplicate> duplicates,
        CancellationToken cancellationToken = default);
}
