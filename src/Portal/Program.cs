using PartnerAppPortal.Portal.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

// Wizard state
builder.Services.AddScoped<WizardState>();

// Typed HttpClient for Assessment API calls (interface registration — required by dashboard/wizard)
builder.Services.AddHttpClient<IAssessmentApiClient, AssessmentApiClient>(client =>
{
    var apiBase = builder.Configuration["ApiBaseUrl"]
        ?? throw new InvalidOperationException(
            "ApiBaseUrl is not configured. Set it in appsettings, App Service settings, or Key Vault.");
    client.BaseAddress = new Uri(apiBase.EndsWith('/') ? apiBase : apiBase + "/");
    client.Timeout = TimeSpan.FromSeconds(60);
});

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
