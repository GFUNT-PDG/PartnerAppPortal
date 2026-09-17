using Microsoft.EntityFrameworkCore;
using PartnerAppPortal.Core.Interfaces.Repositories;
using PartnerAppPortal.Core.Models.Crm;
using PartnerAppPortal.Infrastructure.Data;

namespace PartnerAppPortal.Infrastructure.Repositories;

public class CrmRepository : ICrmRepository
{
    private readonly PortalDbContext _db;

    /// <summary>
    /// Shared consumer domains must never use domain-wide partner matching
    /// (many unrelated CRM accounts can share @outlook.com / @gmail.com).
    /// </summary>
    private static readonly HashSet<string> PublicEmailDomains = new(StringComparer.OrdinalIgnoreCase)
    {
        "outlook.com", "hotmail.com", "live.com", "msn.com",
        "gmail.com", "googlemail.com",
        "yahoo.com", "yahoo.co.uk",
        "aol.com", "icloud.com", "me.com", "mac.com",
        "proton.me", "protonmail.com",
        "mail.com", "email.com"
    };

    public CrmRepository(PortalDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Customer>> GetCustomersAsync()
    {
        return await _db.Customers
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new Customer
            {
                Id = c.Id,
                AccountId = c.Id.ToString(),
                Name = c.Name,
                Email = c.Email,
                PrimaryContact = c.PrimaryContact,
                SalesAcceleratorAccess = c.SalesAcceleratorAccess
            })
            .ToListAsync();
    }

    public async Task<Customer?> FindCustomerByEmailDomainAsync(string emailOrDomain)
    {
        var login = NormalizeEmail(emailOrDomain);
        if (string.IsNullOrWhiteSpace(login))
            return null;

        var domain = ExtractDomain(login);
        var accounts = await _db.Customers
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new Customer
            {
                Id = c.Id,
                AccountId = c.Id.ToString(),
                Name = c.Name,
                Email = c.Email,
                PrimaryContact = c.PrimaryContact,
                SalesAcceleratorAccess = c.SalesAcceleratorAccess
            })
            .ToListAsync();

        // 1) Exact email match on account (always preferred)
        var exactAccount = accounts.FirstOrDefault(c =>
            string.Equals(NormalizeEmail(c.Email), login, StringComparison.OrdinalIgnoreCase));
        if (exactAccount != null)
            return exactAccount;

        // 2) Exact email match on CrmContacts
        List<CrmContact> contacts;
        try
        {
            contacts = await _db.CrmContacts
                .AsNoTracking()
                .Where(c => c.Email != null && c.Email != "")
                .ToListAsync();
        }
        catch
        {
            contacts = new List<CrmContact>();
        }

        var exactContact = contacts.FirstOrDefault(c =>
            string.Equals(NormalizeEmail(c.Email), login, StringComparison.OrdinalIgnoreCase));
        if (exactContact != null)
        {
            var fromContact = ResolveAccount(accounts, exactContact.AccountId);
            if (fromContact != null)
                return fromContact;
        }

        // 3) Domain match only for company domains (never outlook/gmail/etc.)
        if (string.IsNullOrWhiteSpace(domain) || PublicEmailDomains.Contains(domain))
            return null;

        var domainAccount = accounts.FirstOrDefault(c =>
        {
            var customerDomain = ExtractDomain(c.Email);
            return !string.IsNullOrWhiteSpace(customerDomain)
                   && string.Equals(customerDomain, domain, StringComparison.OrdinalIgnoreCase);
        });
        if (domainAccount != null)
            return domainAccount;

        var domainContact = contacts.FirstOrDefault(c =>
        {
            var contactDomain = ExtractDomain(c.Email);
            return !string.IsNullOrWhiteSpace(contactDomain)
                   && string.Equals(contactDomain, domain, StringComparison.OrdinalIgnoreCase);
        });

        return domainContact is null
            ? null
            : ResolveAccount(accounts, domainContact.AccountId);
    }

    private static Customer? ResolveAccount(IReadOnlyList<Customer> accounts, string? accountId)
    {
        if (string.IsNullOrWhiteSpace(accountId))
            return null;

        if (Guid.TryParse(accountId, out var accountGuid))
        {
            return accounts.FirstOrDefault(a => a.Id == accountGuid)
                   ?? accounts.FirstOrDefault(a =>
                       string.Equals(a.AccountId, accountId, StringComparison.OrdinalIgnoreCase));
        }

        return accounts.FirstOrDefault(a =>
            string.Equals(a.AccountId, accountId, StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return string.Empty;

        return email.Trim().TrimStart('@');
    }

    private static string? ExtractDomain(string? emailOrDomain)
    {
        var value = NormalizeEmail(emailOrDomain);
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var at = value.LastIndexOf('@');
        return at >= 0 && at < value.Length - 1
            ? value[(at + 1)..].Trim()
            : value;
    }
}
