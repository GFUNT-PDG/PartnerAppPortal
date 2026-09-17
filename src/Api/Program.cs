using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            builder.Services.Configure<PartnerAppPortal.Core.Options.TeamsNotificationOptions>(
                builder.Configuration.GetSection(PartnerAppPortal.Core.Options.TeamsNotificationOptions.SectionName));

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 2 * 1024 * 1024;
            });

            // API plumbing
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.MapControllers();

            app.MapCrmEndpoints();


            app.Run();
        }
    }
}
