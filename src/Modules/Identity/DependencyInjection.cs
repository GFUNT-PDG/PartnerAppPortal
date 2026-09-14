using Microsoft.Extensions.DependencyInjection;

namespace PartnerAppPortal.Modules.Identity;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services)
    {
        return services;
    }
}
