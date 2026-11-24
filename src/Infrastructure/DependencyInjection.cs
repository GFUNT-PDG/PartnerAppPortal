using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PartnerAppPortal.Core.Interfaces.Repositories;
using PartnerAppPortal.Core.Interfaces.Services;
using PartnerAppPortal.Infrastructure.Data;
using PartnerAppPortal.Infrastructure.Notifications;
using PartnerAppPortal.Infrastructure.Repositories;


namespace PartnerAppPortal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Use Azure App Service connection string
        var connString = configuration.GetConnectionString("DefaultConnection");

        // Fail fast if Azure config is missing
        if (string.IsNullOrWhiteSpace(connString))
            throw new InvalidOperationException("DefaultConnection is missing from Azure App Service configuration.");

        // Register DbContext
        services.AddDbContext<PortalDbContext>(options =>
            options.UseSqlServer(connString));

        // Register repositories
        services.AddScoped<IAssessmentRepository, AssessmentRepository>();
        services.AddScoped<IAssessmentSubmissionRepository, AssessmentSubmissionRepository>();
        services.AddScoped<ICrmRepository, CrmRepository>();
        services.AddScoped<IAssessmentScoringService, AssessmentScoringService>();
        services.AddScoped<ISalesAcceleratorRepository, SalesAcceleratorRepository>();
        services.AddScoped<IPartnerOnboardingRepository, PartnerOnboardingRepository>();
        services.AddHttpClient<ITeamsNotificationService, TeamsNotificationService>();


        return services;
    }
}
