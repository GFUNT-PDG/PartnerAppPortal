using Microsoft.EntityFrameworkCore;
using PartnerAppPortal.Core.Interfaces.Repositories;
using PartnerAppPortal.Core.Models.SalesAccelerator;
using PartnerAppPortal.Infrastructure.Data;

namespace PartnerAppPortal.Infrastructure.Repositories;

public class SalesAcceleratorRepository : ISalesAcceleratorRepository
{
    private readonly PortalDbContext _db;

    public SalesAcceleratorRepository(PortalDbContext db)
    {
        _db = db;
    }

    public Task<SalesAcceleratorAccount?> FindByNormalizedDomainAsync(string normalizedDomain)
    {
        return _db.SalesAcceleratorAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.NormalizedDomain == normalizedDomain);
    }

    public async Task<IReadOnlyList<SalesAcceleratorAccount>> FindByNormalizedDomainsAsync(
        IEnumerable<string> normalizedDomains)
    {
        var keys = normalizedDomains
            .Where(d => !string.IsNullOrWhiteSpace(d))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (keys.Count == 0)
            return Array.Empty<SalesAcceleratorAccount>();

        return await _db.SalesAcceleratorAccounts
            .AsNoTracking()
            .Where(a => keys.Contains(a.NormalizedDomain))
            .ToListAsync();
    }

    public async Task AddSubmissionWithAccountsAsync(
        SalesAcceleratorSubmission submission,
        IEnumerable<SalesAcceleratorAccount> accounts)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();
        _db.SalesAcceleratorSubmissions.Add(submission);
        await _db.SaveChangesAsync();

        var list = accounts.ToList();
        if (list.Count > 0)
        {
            _db.SalesAcceleratorAccounts.AddRange(list);
            await _db.SaveChangesAsync();
        }

        await tx.CommitAsync();
    }

    public async Task<IReadOnlyList<SalesAcceleratorSubmissionListItem>> ListSubmissionsAsync(
        Guid? partnerCustomerId)
    {
        var query = _db.SalesAcceleratorSubmissions.AsNoTracking();
        if (partnerCustomerId is Guid id && id != Guid.Empty)
            query = query.Where(s => s.PartnerCustomerId == id);

        return await query
            .OrderByDescending(s => s.PartnerUploadDate)
            .Select(s => new SalesAcceleratorSubmissionListItem
            {
                Id = s.Id,
                PartnerCustomerId = s.PartnerCustomerId,
                Partner = s.Partner,
                SubmittedBy = s.SubmittedBy,
                PartnerUploadDate = s.PartnerUploadDate,
                OriginalFileName = s.OriginalFileName,
                InsertedCount = s.InsertedCount,
                DuplicateCount = s.DuplicateCount
            })
            .ToListAsync();
    }

    public async Task<SalesAcceleratorFileDownload?> GetSubmissionFileAsync(Guid submissionId)
    {
        return await _db.SalesAcceleratorSubmissions
            .AsNoTracking()
            .Where(s => s.Id == submissionId)
            .Select(s => new SalesAcceleratorFileDownload
            {
                FileName = s.OriginalFileName,
                ContentType = s.ContentType,
                Content = s.FileContent
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<SalesAcceleratorAccount>> ListAccountsAsync(Guid? partnerCustomerId)
    {
        var query = _db.SalesAcceleratorAccounts.AsNoTracking();
        if (partnerCustomerId is Guid id && id != Guid.Empty)
            query = query.Where(a => a.PartnerCustomerId == id);

        return await query
            .OrderByDescending(a => a.PartnerUploadDate)
            .ThenBy(a => a.CompanyName)
            .ToListAsync();
    }

    public async Task UpdateAccountFieldsAsync(IEnumerable<SalesAcceleratorAccountUpdate> updates)
    {
        var list = updates.ToList();
        if (list.Count == 0)
            return;

        var ids = list.Select(u => u.Id).ToList();
        var entities = await _db.SalesAcceleratorAccounts
            .Where(a => ids.Contains(a.Id))
            .ToListAsync();

        var byId = list.ToDictionary(u => u.Id);
        foreach (var entity in entities)
        {
            if (!byId.TryGetValue(entity.Id, out var update))
                continue;

            entity.Tpid = string.IsNullOrWhiteSpace(update.Tpid) ? null : update.Tpid.Trim();
            entity.SentToMitch = update.SentToMitch;
            entity.DateSubmitted = update.DateSubmitted;
            entity.DateVerified = update.DateVerified;
        }

        await _db.SaveChangesAsync();
    }
}
