using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using PartnerAppPortal.Application;
using PartnerAppPortal.Infrastructure;
using PartnerAppPortal.Api.Endpoints.Crm;


namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Do not bind Kestrel to PORT/WEBSITES_PORT on Azure App Service.
            // OutOfProcess hosting (ANCM) sets ASPNETCORE_URLS; overriding it
            // causes the site to listen on the wrong port and return 503/504.

            // Layered services
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddHttpClient("Dataverse");
            builder.Services.AddHttpClient("CrmSync", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(45);
            });
            builder.Services.AddSingleton<PartnerAppPortal.Api.Onboarding.DataverseAccountProfile>();
            builder.Services.AddSingleton<PartnerAppPortal.Api.Crm.CrmSyncStarter>();
            builder.Services.AddScoped<PartnerAppPortal.Api.Onboarding.ContactCardWriter>();
            builder.Services.Configure<PartnerAppPortal.Core.Options.TeamsNotificationOptions>(
                builder.Configuration.GetSection(PartnerAppPortal.Core.Options.TeamsNotificationOptions.SectionName));

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 2 * 1024 * 1024;
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(
                    jwt =>
                    {
                        var clientId = builder.Configuration["AzureAd:ClientId"];
                        var configuredAudience = builder.Configuration["AzureAd:Audience"];
                        var audiences = new List<string>();
                        if (IsReal(clientId))
                        {
                            audiences.Add(clientId!);
                            audiences.Add("api://" + clientId);
                        }

                        if (IsReal(configuredAudience))
                            audiences.Add(configuredAudience!);

                        if (audiences.Count > 0)
                        {
                            jwt.TokenValidationParameters.ValidAudiences = audiences
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .ToArray();
                        }
                    },
                    identity =>
                    {
                        builder.Configuration.GetSection("AzureAd").Bind(identity);
                        // App-only tokens carry a roles claim. Accept the portal token
                        // even when the role check is stricter than a signed-in user token.
                        identity.AllowWebApiToBeAuthorizedByACL = true;
                    });

            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            // API plumbing. Health stays anonymous via [AllowAnonymous].
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add(new AuthorizeFilter());
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.MapCrmEndpoints();


            app.Run();
        }

        private static bool IsReal(string? value) =>
            !string.IsNullOrWhiteSpace(value) &&
            !value.Contains("YOUR_", StringComparison.OrdinalIgnoreCase);
    }
}
