using Microsoft.Extensions.DependencyInjection;
using PartnerAppPortal.Core.Interfaces.Services;
using PartnerAppPortal.Application.Services;

namespace PartnerAppPortal.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAssessmentService, AssessmentService>();
        services.AddScoped<ICrmService, CrmService>();
        services.AddScoped<IScoringEngine, ScoringEngine>();
        services.AddScoped<ISalesAcceleratorService, SalesAcceleratorService>();

        return services;
    }
}
