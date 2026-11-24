using System.Security.Claims;
using PartnerAppPortal.Core.Models.Crm;

namespace PartnerAppPortal.Portal.Services;

public sealed class SiteReportAccess
{
    private readonly IAssessmentApiClient _customers;

    public SiteReportAccess(IAssessmentApiClient customers)
    {
        _customers = customers;
    }

    public async Task<Customer?> ResolveAsync(ClaimsPrincipal user, Guid? requestedCustomerId)
    {
        var email = FindEmail(user);
        if (string.IsNullOrWhiteSpace(email))
            return null;

        if (IsEmployee(email))
        {
            if (requestedCustomerId is not Guid id || id == Guid.Empty)
                return null;

            var customers = await _customers.GetCustomersAsync() ?? new List<Customer>();
            return customers.FirstOrDefault(c => c.Id == id);
        }

        var own = await _customers.FindCustomerByEmailAsync(email);
        if (own is null || own.Id == Guid.Empty)
            return null;

        if (requestedCustomerId is Guid requested && requested != Guid.Empty && requested != own.Id)
            return null;

        return own;
    }

    public static bool IsEmployee(string email) =>
        email.EndsWith("@partnerdevelopmentgroup.com", StringComparison.OrdinalIgnoreCase) ||
        email.EndsWith("@pdg.com", StringComparison.OrdinalIgnoreCase);

    public static string? FindEmail(ClaimsPrincipal user) =>
        user.FindFirst("preferred_username")?.Value
        ?? user.FindFirst("email")?.Value
        ?? user.FindFirst(ClaimTypes.Upn)?.Value
        ?? user.FindFirst(ClaimTypes.Email)?.Value;
}
