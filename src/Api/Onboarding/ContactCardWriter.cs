using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using PartnerAppPortal.Core.Interfaces.Repositories;

namespace PartnerAppPortal.Api.Onboarding;

public sealed class ContactCardWriter
{
    private readonly IPartnerOnboardingRepository _onboarding;
    private readonly IConfiguration _configuration;

    public ContactCardWriter(IPartnerOnboardingRepository onboarding, IConfiguration configuration)
    {
        _onboarding = onboarding;
        _configuration = configuration;
    }

    public async Task<string> WriteAsync(Guid accountId, string? updatedBy, CancellationToken cancellationToken)
    {
        var source = await _onboarding.GetContactCardSourceAsync(accountId, cancellationToken);
        if (string.IsNullOrWhiteSpace(source.AccountName) ||
            source.AccountName.Contains('/') ||
            source.AccountName.Contains('\\'))
        {
            throw new InvalidOperationException("This account name cannot be used as a customer folder.");
        }

        var accountName = _configuration["SiteReports:AccountName"];
        var containerName = _configuration["SiteReports:Container"] ?? "sitereports";
        if (string.IsNullOrWhiteSpace(accountName))
            throw new InvalidOperationException("Site report storage is not configured.");

        var domain = DomainFromEmail(source.Email);
        var body =
            $"Company name: {source.AccountName}{Environment.NewLine}" +
            $"Address: {Environment.NewLine}" +
            $"Domain: {domain}{Environment.NewLine}" +
            $"Primary Contact: {source.PrimaryContact}{Environment.NewLine}" +
            $"Telephone number: {source.Phone}{Environment.NewLine}";

        var blobName = source.AccountName.Trim() + "/contactcard.txt";
        var container = new BlobContainerClient(
            new Uri($"https://{accountName}.blob.core.windows.net/{containerName}"),
            new DefaultAzureCredential());
        var blob = container.GetBlobClient(blobName);
        await using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(body));
        await blob.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = "text/plain; charset=utf-8" }
        }, cancellationToken);

        await _onboarding.MarkFolderCreatedAsync(accountId, blobName, updatedBy, cancellationToken);
        return blobName;
    }

    private static string DomainFromEmail(string email)
    {
        var at = email.LastIndexOf('@');
        return at >= 0 && at < email.Length - 1 ? email[(at + 1)..].Trim() : "";
    }
}
