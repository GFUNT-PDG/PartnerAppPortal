using PartnerAppPortal.Core.Models.Crm;

namespace PartnerAppPortal.Core.Interfaces.Repositories;

public interface ICrmRepository
{
    Task<IReadOnlyList<Customer>> GetCustomersAsync();
    Task<Customer?> FindCustomerByEmailDomainAsync(string emailOrDomain);
}
