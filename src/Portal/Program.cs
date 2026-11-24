using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using PartnerAppPortal.Portal.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();

builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();
builder.Services.AddRazorPages().AddMicrosoftIdentityUI();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = true; })
    .AddHubOptions(options => { options.MaximumReceiveMessageSize = 10 * 1024 * 1024; })
    .AddMicrosoftIdentityConsentHandler();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddScoped<WizardState>();
builder.Services.AddScoped<PortalSession>();
builder.Services.AddScoped<ApiUserContext>();
builder.Services.AddScoped<ApiTokenHandler>();
builder.Services.AddSingleton<SiteReportFiles>();
builder.Services.AddScoped<SiteReportAccess>();
builder.Services.AddScoped<TeamsGraphClient>();
builder.Services.AddHttpClient("Graph", client =>
{
    client.BaseAddress = new Uri("https://graph.microsoft.com/v1.0/");
    client.Timeout = TimeSpan.FromSeconds(60);
});
builder.Services.AddHttpClient("GraphNoRedirect", client =>
{
    client.BaseAddress = new Uri("https://graph.microsoft.com/v1.0/");
    client.Timeout = TimeSpan.FromSeconds(60);
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    AllowAutoRedirect = false
});

static Uri? IntroScoreRoot(string? raw)
{
    if (string.IsNullOrWhiteSpace(raw))
        return null;

    raw = raw.Trim().Trim('"', '\'');
    if (!raw.Contains("://", StringComparison.Ordinal))
        raw = "https://" + raw.TrimStart('/');

    if (!Uri.TryCreate(raw, UriKind.Absolute, out var parsed) ||
        (parsed.Scheme != Uri.UriSchemeHttp && parsed.Scheme != Uri.UriSchemeHttps))
        return null;

    var path = parsed.AbsolutePath.TrimEnd('/');
    return new Uri($"{parsed.Scheme}://{parsed.Authority}{(string.IsNullOrEmpty(path) ? "" : path)}/");
}

static Uri ApiRoot(IConfiguration config)
{
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
}).AddHttpMessageHandler<ApiTokenHandler>();

builder.Services.AddHttpClient<ISalesAcceleratorApiClient, SalesAcceleratorApiClient>(client =>
{
    client.BaseAddress = apiRoot;
    client.Timeout = TimeSpan.FromSeconds(60);
}).AddHttpMessageHandler<ApiTokenHandler>();

builder.Services.AddHttpClient<IOnboardingApiClient, OnboardingApiClient>(client =>
{
    client.BaseAddress = apiRoot;
    client.Timeout = TimeSpan.FromSeconds(60);
}).AddHttpMessageHandler<ApiTokenHandler>();

builder.Services.AddHttpClient<IntroScoreResultsClient>(client =>
{
    var introRoot = IntroScoreRoot(builder.Configuration["IntroScore:BaseUrl"]);
    if (introRoot is not null)
        client.BaseAddress = introRoot;

    client.Timeout = TimeSpan.FromSeconds(60);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
