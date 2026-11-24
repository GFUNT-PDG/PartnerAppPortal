using PartnerAppPortal.Core.Models.Onboarding;

namespace PartnerAppPortal.Core.Interfaces.Repositories;

public interface IPartnerOnboardingRepository
{
    Task<PartnerOnboardingDocument> GetAsync(Guid accountId, CancellationToken cancellationToken = default);

    Task<PartnerOnboardingChecklistView> SaveAsync(
        Guid accountId,
        PartnerOnboardingSaveRequest request,
        CancellationToken cancellationToken = default);

    Task<PartnerOnboardingChecklistView> UpdateChecklistItemAsync(
        Guid accountId,
        ChecklistItemUpdate update,
        CancellationToken cancellationToken = default);

    Task<(string AccountName, string PrimaryContact, string Email, string Phone)> GetContactCardSourceAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    Task MarkFolderCreatedAsync(
        Guid accountId,
        string blobName,
        string? updatedBy,
        CancellationToken cancellationToken = default);
}
