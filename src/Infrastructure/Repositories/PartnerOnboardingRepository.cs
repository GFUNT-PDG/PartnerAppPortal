using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PartnerAppPortal.Core.Interfaces.Repositories;
using PartnerAppPortal.Core.Models.Onboarding;
using PartnerAppPortal.Infrastructure.Data;

namespace PartnerAppPortal.Infrastructure.Repositories;

public class PartnerOnboardingRepository : IPartnerOnboardingRepository
{
    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly PortalDbContext _db;

    public PartnerOnboardingRepository(PortalDbContext db)
    {
        _db = db;
    }

    public async Task<PartnerOnboardingDocument> GetAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var accountName = await AccountNameAsync(accountId, cancellationToken);
        var row = await _db.PartnerOnboarding
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AccountId == accountId, cancellationToken);

        return new PartnerOnboardingDocument
        {
            AccountId = accountId,
            AccountName = accountName,
            Fields = ReadMap(row?.ProfileJson),
            Checklist = ReadMap(row?.ChecklistJson),
            FolderCreated = row?.FolderCreated ?? false,
            ContactCardBlob = row?.ContactCardBlob
        };
    }

    public async Task<PartnerOnboardingChecklistView> SaveAsync(
        Guid accountId,
        PartnerOnboardingSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        await AccountNameAsync(accountId, cancellationToken);
        var row = await TrackedRowAsync(accountId, cancellationToken);
        row.ProfileJson = WriteMap(SanitizeFields(request.Fields));
        row.UpdatedOn = DateTime.UtcNow;
        row.UpdatedBy = Trim(request.UpdatedBy, 256);
        await _db.SaveChangesAsync(cancellationToken);
        return OnboardingCatalog.ToChecklistView(await GetAsync(accountId, cancellationToken));
    }

    public async Task<PartnerOnboardingChecklistView> UpdateChecklistItemAsync(
        Guid accountId,
        ChecklistItemUpdate update,
        CancellationToken cancellationToken = default)
    {
        if (!OnboardingCatalog.IsChecklistKey(update.Key))
            throw new InvalidOperationException("That activity is not on the checklist.");

        await AccountNameAsync(accountId, cancellationToken);
        var row = await TrackedRowAsync(accountId, cancellationToken);
        var checklist = ReadMap(row.ChecklistJson);
        checklist[update.Key] = OnboardingCatalog.NormalizeYesNo(update.Value);
        row.ChecklistJson = WriteMap(checklist);
        row.UpdatedOn = DateTime.UtcNow;
        row.UpdatedBy = Trim(update.UpdatedBy, 256);
        await _db.SaveChangesAsync(cancellationToken);
        return OnboardingCatalog.ToChecklistView(await GetAsync(accountId, cancellationToken));
    }

    public async Task<(string AccountName, string PrimaryContact, string Email, string Phone)> GetContactCardSourceAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        var rows = await _db.Database.SqlQueryRaw<ContactCardSource>(
                """
                SELECT Name, PrimaryContact, Email, Phone
                FROM dbo.CrmAccounts
                WHERE AccountId = {0}
                """,
                accountId)
            .ToListAsync(cancellationToken);

        var row = rows.FirstOrDefault()
            ?? throw new InvalidOperationException("Customer was not found.");

        return (
            row.Name?.Trim() ?? "",
            row.PrimaryContact?.Trim() ?? "",
            row.Email?.Trim() ?? "",
            row.Phone?.Trim() ?? "");
    }

    public async Task MarkFolderCreatedAsync(
        Guid accountId,
        string blobName,
        string? updatedBy,
        CancellationToken cancellationToken = default)
    {
        var row = await TrackedRowAsync(accountId, cancellationToken);
        row.FolderCreated = true;
        row.ContactCardBlob = Trim(blobName, 512);
        row.UpdatedOn = DateTime.UtcNow;
        row.UpdatedBy = Trim(updatedBy, 256);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task<string> AccountNameAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var name = await _db.Customers
            .AsNoTracking()
            .Where(c => c.Id == accountId)
            .Select(c => c.Name)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException("Customer was not found.");

        return name.Trim();
    }

    private async Task<PartnerOnboardingRecord> TrackedRowAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var row = await _db.PartnerOnboarding
            .FirstOrDefaultAsync(x => x.AccountId == accountId, cancellationToken);

        if (row is not null)
            return row;

        row = new PartnerOnboardingRecord
        {
            AccountId = accountId,
            ProfileJson = "{}",
            ChecklistJson = "{}"
        };
        _db.PartnerOnboarding.Add(row);
        return row;
    }

    private static Dictionary<string, string?> SanitizeFields(IReadOnlyDictionary<string, string?>? fields)
    {
        var clean = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        if (fields is null)
            return clean;

        foreach (var field in OnboardingCatalog.InputFields)
        {
            fields.TryGetValue(field.Key, out var value);
            clean[field.Key] = Trim(value, 2000);
        }

        return clean;
    }

    private static Dictionary<string, string?> ReadMap(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        var parsed = JsonSerializer.Deserialize<Dictionary<string, string?>>(json, Json)
            ?? new Dictionary<string, string?>();
        return new Dictionary<string, string?>(parsed, StringComparer.OrdinalIgnoreCase);
    }

    private static string WriteMap(Dictionary<string, string?> map) =>
        JsonSerializer.Serialize(map, Json);

    private static string Trim(string? value, int max)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        var trimmed = value.Trim();
        return trimmed.Length <= max ? trimmed : trimmed[..max];
    }

    private sealed class ContactCardSource
    {
        public string? Name { get; set; }
        public string? PrimaryContact { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
