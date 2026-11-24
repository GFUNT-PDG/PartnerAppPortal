using System.Net.Http.Json;
using PartnerAppPortal.Core.Models.Onboarding;

namespace PartnerAppPortal.Portal.Services;

public interface IOnboardingApiClient
{
    Task<PartnerOnboardingDocument> GetAsync(Guid accountId);
    Task<PartnerOnboardingChecklistView> SaveAsync(Guid accountId, PartnerOnboardingSaveRequest request);
    Task<PartnerOnboardingChecklistView> UpdateChecklistItemAsync(Guid accountId, ChecklistItemUpdate update);
    Task<ContactCardResult> CreateContactCardAsync(Guid accountId, string? updatedBy);
}

public sealed class OnboardingApiClient : IOnboardingApiClient
{
    private readonly HttpClient _http;
    private readonly ApiUserContext _user;

    public OnboardingApiClient(HttpClient http, ApiUserContext user)
    {
        _http = http;
        _user = user;
    }

    public async Task<PartnerOnboardingDocument> GetAsync(Guid accountId)
    {
        var response = await (await HttpAsync()).GetAsync($"api/onboarding/{accountId}");
        await EnsureSuccessAsync(response, "Load onboarding");
        return await response.Content.ReadFromJsonAsync<PartnerOnboardingDocument>()
            ?? new PartnerOnboardingDocument { AccountId = accountId };
    }

    public async Task<PartnerOnboardingChecklistView> SaveAsync(Guid accountId, PartnerOnboardingSaveRequest request)
    {
        var response = await (await HttpAsync()).PutAsJsonAsync($"api/onboarding/{accountId}", request);
        await EnsureSuccessAsync(response, "Save onboarding");
        return await response.Content.ReadFromJsonAsync<PartnerOnboardingChecklistView>()
            ?? new PartnerOnboardingChecklistView { AccountId = accountId };
    }

    public async Task<PartnerOnboardingChecklistView> UpdateChecklistItemAsync(Guid accountId, ChecklistItemUpdate update)
    {
        var response = await (await HttpAsync()).PatchAsJsonAsync($"api/onboarding/{accountId}/checklist", update);
        await EnsureSuccessAsync(response, "Update checklist");
        return await response.Content.ReadFromJsonAsync<PartnerOnboardingChecklistView>()
            ?? new PartnerOnboardingChecklistView { AccountId = accountId };
    }

    public async Task<ContactCardResult> CreateContactCardAsync(Guid accountId, string? updatedBy)
    {
        var url = string.IsNullOrWhiteSpace(updatedBy)
            ? $"api/onboarding/{accountId}/contact-card"
            : $"api/onboarding/{accountId}/contact-card?updatedBy={Uri.EscapeDataString(updatedBy)}";
        var response = await (await HttpAsync()).PostAsync(url, content: null);
        await EnsureSuccessAsync(response, "Create customer folder");
        return await response.Content.ReadFromJsonAsync<ContactCardResult>()
            ?? new ContactCardResult();
    }

    private async Task<HttpClient> HttpAsync()
    {
        await _user.EnsureAsync();
        return _http;
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, string action)
    {
        if (response.IsSuccessStatusCode)
            return;

        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"{action} failed ({(int)response.StatusCode}): {body}");
    }
}
