using PartnerAppPortal.Portal.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = true; })
    .AddHubOptions(options => { options.MaximumReceiveMessageSize = 10 * 1024 * 1024; });

builder.Services.AddScoped<WizardState>();
builder.Services.AddScoped<PortalSession>();

static Uri ApiRoot(IConfiguration config)
{
    // Production: set ApiBaseUrl in appsettings / App Service config / Key Vault.
    const string fallback = "https://localhost:7xxx/";
    try
    {
        var raw = config["ApiBaseUrl"];
        if (string.IsNullOrWhiteSpace(raw))
            raw = fallback;

        raw = raw.Trim().Trim('"', '\'');
        if (!Uri.TryCreate(raw, UriKind.Absolute, out var parsed) ||
            (parsed.Scheme != Uri.UriSchemeHttp && parsed.Scheme != Uri.UriSchemeHttps))
            return new Uri(fallback);

        var builder = new UriBuilder(parsed)
        {
            Query = string.Empty,
            Fragment = string.Empty
        };

        var path = builder.Path.TrimEnd('/');
        if (path.EndsWith("/api", StringComparison.OrdinalIgnoreCase))
            path = path[..^4];

        builder.Path = string.IsNullOrEmpty(path) ? "/" : path + "/";
        return builder.Uri;
    }
    catch
    {
        return new Uri(fallback);
    }
}

var apiRoot = ApiRoot(builder.Configuration);

builder.Services.AddHttpClient<IAssessmentApiClient, AssessmentApiClient>(client =>
{
    client.BaseAddress = apiRoot;
    client.Timeout = TimeSpan.FromSeconds(60);
});

builder.Services.AddHttpClient<ISalesAcceleratorApiClient, SalesAcceleratorApiClient>(client =>
{
    client.BaseAddress = apiRoot;
    client.Timeout = TimeSpan.FromSeconds(60);
});

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
