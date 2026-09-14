using PartnerAppPortal.Core.Interfaces.Repositories;
using PartnerAppPortal.Core.Interfaces.Services;
using PartnerAppPortal.Core.Models.Crm;

namespace PartnerAppPortal.Application.Services;

public class CrmService : ICrmService
{
    private readonly ICrmRepository _repo;

    public CrmService(ICrmRepository repo)
    {
        _repo = repo;
    }

    public Task<IReadOnlyList<Customer>> GetCustomersAsync() =>
        _repo.GetCustomersAsync();

    public Task<Customer?> FindCustomerByEmailDomainAsync(string emailOrDomain) =>
        _repo.FindCustomerByEmailDomainAsync(emailOrDomain);
}
