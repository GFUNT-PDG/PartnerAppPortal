using Microsoft.Extensions.DependencyInjection;

namespace PartnerAppPortal.Modules.CRM;

public static class DependencyInjection
{
    public static IServiceCollection AddCrmModule(this IServiceCollection services)
    {
        // services.AddScoped<ICrmService, CrmService>();
        // services.AddScoped<ICrmRepository, CrmRepository>();
        return services;
    }
}
