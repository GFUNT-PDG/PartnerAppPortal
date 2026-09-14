using PartnerAppPortal.Portal.Models;

namespace PartnerAppPortal.Portal.Services;

public interface ICrmService
{
    Task<List<CustomerModel>> GetCustomersAsync();
}
