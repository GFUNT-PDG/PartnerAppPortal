using PartnerAppPortal.Core.Interfaces.Repositories;
using PartnerAppPortal.Core.Interfaces.Services;
using PartnerAppPortal.Core.Models.SalesAccelerator;
using PartnerAppPortal.Core.Services.SalesAccelerator;

namespace PartnerAppPortal.Application.Services;

public class SalesAcceleratorService : ISalesAcceleratorService
{
    private const int MaxFileBytes = 2 * 1024 * 1024;

    private readonly ISalesAcceleratorRepository _repo;
    private readonly ICrmRepository _crm;
    private readonly ITeamsNotificationService _teams;

    public SalesAcceleratorService(
        ISalesAcceleratorRepository repo,
        ICrmRepository crm,
        ITeamsNotificationService teams)
    {
        _repo = repo;
        _crm = crm;
        _teams = teams;
    }

    public async Task<SalesAcceleratorSubmitResult> SubmitCsvAsync(
        Guid partnerCustomerId,
        string submittedBy,
        string originalFileName,
        byte[] fileContent,
        CancellationToken cancellationToken = default)
    {
        if (partnerCustomerId == Guid.Empty)
            throw new InvalidOperationException("Partner customer is required.");
        if (string.IsNullOrWhiteSpace(submittedBy))
            throw new InvalidOperationException("Submitted by (login email) is required.");
        if (fileContent is null || fileContent.Length == 0)
            throw new InvalidOperationException("Upload a completed CSV template before submitting.");
        if (fileContent.Length > MaxFileBytes)
            throw new InvalidOperationException("CSV file is too large (2 MB max).");

        var fileName = string.IsNullOrWhiteSpace(originalFileName) ? "sales-accelerator.csv" : originalFileName.Trim();
        if (!fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Upload the CSV template (.csv), not an Excel workbook.");

        var partner = await RequirePartnerWithAccessAsync(partnerCustomerId);
        var uploadedAt = DateTime.UtcNow;
        var text = System.Text.Encoding.UTF8.GetString(fileContent);
        if (text.Length > 0 && text[0] == '\uFEFF')
            text = text[1..];

        IReadOnlyList<ParsedSalesAcceleratorRow> parsed;
        try
        {
            parsed = SalesAcceleratorCsvParser.Parse(text);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.Message);
        }

        if (parsed.Count == 0)
            throw new InvalidOperationException("The CSV did not contain any account rows.");

        var result = new SalesAcceleratorSubmitResult();
        var valid = parsed.Where(r => r.IsValid).ToList();
        foreach (var invalid in parsed.Where(r => !r.IsValid))
        {
            result.InvalidCount++;
            result.InvalidRows.Add($"Row {invalid.LineNumber}: {invalid.Error}");
        }

        var existing = await _repo.FindByNormalizedDomainsAsync(valid.Select(r => r.NormalizedDomain));
        var claimed = existing.ToDictionary(a => a.NormalizedDomain, StringComparer.OrdinalIgnoreCase);
        var seenInFile = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var toInsert = new List<SalesAcceleratorAccount>();

        foreach (var row in valid)
        {
            claimed.TryGetValue(row.NormalizedDomain, out var current);
            var alreadyInFile = !seenInFile.Add(row.NormalizedDomain);
            if (alreadyInFile || current is not null)
            {
                result.Duplicates.Add(new SalesAcceleratorDuplicate
                {
                    Domain = row.Domain,
                    CompanyName = row.CompanyName,
                    CurrentPartner = current?.Partner ?? partner
                });
                continue;
            }

            var account = new SalesAcceleratorAccount
            {
                Id = Guid.NewGuid(),
                PartnerCustomerId = partnerCustomerId,
                CompanyName = row.CompanyName,
                Domain = row.Domain,
                NormalizedDomain = row.NormalizedDomain,
                AccountType = row.AccountType,
                SubmittedBy = submittedBy.Trim(),
                Partner = partner,
                PartnerUploadDate = uploadedAt
            };
            toInsert.Add(account);
            claimed[row.NormalizedDomain] = account;
        }

        result.DuplicateCount = result.Duplicates.Count;
        result.InsertedCount = toInsert.Count;

        var submission = new SalesAcceleratorSubmission
        {
            Id = Guid.NewGuid(),
            PartnerCustomerId = partnerCustomerId,
            Partner = partner,
            SubmittedBy = submittedBy.Trim(),
            PartnerUploadDate = uploadedAt,
            OriginalFileName = Path.GetFileName(fileName),
            ContentType = "text/csv",
            FileContent = fileContent,
            InsertedCount = result.InsertedCount,
            DuplicateCount = result.DuplicateCount
        };

        foreach (var account in toInsert)
            account.SubmissionId = submission.Id;

        await _repo.AddSubmissionWithAccountsAsync(submission, toInsert);
        result.SubmissionId = submission.Id;

        await _teams.NotifyNewSubmissionAsync(partner, submittedBy, result, cancellationToken);

        if (result.Duplicates.Count > 0)
            await _teams.NotifyDuplicateDomainsAsync(partner, submittedBy, result.Duplicates, cancellationToken);

        return result;
    }

    public Task<IReadOnlyList<SalesAcceleratorSubmissionListItem>> ListSubmissionsAsync(Guid? partnerCustomerId) =>
        _repo.ListSubmissionsAsync(partnerCustomerId);

    public Task<SalesAcceleratorFileDownload?> GetSubmissionFileAsync(Guid submissionId) =>
        _repo.GetSubmissionFileAsync(submissionId);

    public Task<IReadOnlyList<SalesAcceleratorAccount>> ListAccountsAsync(Guid? partnerCustomerId) =>
        _repo.ListAccountsAsync(partnerCustomerId);

    public Task UpdateAccountFieldsAsync(IEnumerable<SalesAcceleratorAccountUpdate> updates) =>
        _repo.UpdateAccountFieldsAsync(updates);

    private async Task<string> RequirePartnerWithAccessAsync(Guid partnerCustomerId)
    {
        var customers = await _crm.GetCustomersAsync();
        var match = customers.FirstOrDefault(c => c.Id == partnerCustomerId);
        if (match is null || string.IsNullOrWhiteSpace(match.Name))
            throw new InvalidOperationException("Partner account was not found.");
        if (!match.SalesAcceleratorAccess)
            throw new UnauthorizedAccessException("This partner does not have Sales Accelerator access.");
        return match.Name;
    }
}
