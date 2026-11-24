using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace PartnerAppPortal.Portal.Services;

public sealed class ApiUserContext
{
    private static readonly AsyncLocal<ClaimsPrincipal?> Current = new();
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public ApiUserContext(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    public static ClaimsPrincipal? User => Current.Value;

    public async Task EnsureAsync()
    {
        if (Current.Value?.Identity?.IsAuthenticated == true)
            return;

        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        Current.Value = state.User;
    }
}
