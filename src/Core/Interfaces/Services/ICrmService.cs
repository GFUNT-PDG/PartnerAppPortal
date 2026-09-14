using PartnerAppPortal.Core.Models.Crm;

namespace PartnerAppPortal.Core.Interfaces.Services;

public interface ICrmService
{
    Task<IReadOnlyList<Customer>> GetCustomersAsync();
    Task<Customer?> FindCustomerByEmailDomainAsync(string emailOrDomain);
}
