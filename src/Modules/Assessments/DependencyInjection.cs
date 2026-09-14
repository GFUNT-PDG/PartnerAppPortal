using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using PartnerAppPortal.Core.Interfaces.Repositories;
using PartnerAppPortal.Core.Interfaces.Services;
using PartnerAppPortal.Application.Services;
using PartnerAppPortal.Infrastructure.Repositories;
using PartnerAppPortal.Infrastructure.Data;

namespace PartnerAppPortal.Modules.Assessments;

public static class DependencyInjection
{
    public static IServiceCollection AddAssessmentsModule(this IServiceCollection services, IConfiguration config)
    {
        var connString = config.GetConnectionString("DefaultConnection");

        services.AddScoped<IAssessmentRepository, AssessmentRepository>();
        services.AddScoped<IAssessmentService, AssessmentService>();
        services.AddScoped<IScoringEngine, ScoringEngine>();

        // FIXED: pass PortalDbContext instead of string
        services.AddScoped<IAssessmentSubmissionRepository>(sp =>
            new AssessmentSubmissionRepository(
                sp.GetRequiredService<PortalDbContext>()
            ));

        return services;
    }
}
