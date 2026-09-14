using System.Net.Http.Json;
using PartnerAppPortal.Portal.Models;

namespace PartnerAppPortal.Portal.Services;

public class CrmService : ICrmService
{
    private readonly HttpClient _http;

    public CrmService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<CustomerModel>> GetCustomersAsync()
    {
        return await _http.GetFromJsonAsync<List<CustomerModel>>("/api/crm/customers");
    }
}
