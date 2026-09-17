using System.Net.Http.Headers;
using System.Net.Http.Json;
using PartnerAppPortal.Core.Models.SalesAccelerator;

namespace PartnerAppPortal.Portal.Services;

public class SalesAcceleratorApiClient : ISalesAcceleratorApiClient
{
    private readonly HttpClient _http;

    public SalesAcceleratorApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<byte[]> DownloadTemplateAsync()
    {
        var response = await _http.GetAsync("api/sales-accelerator/template");
        await EnsureSuccessAsync(response, "Download template");
        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task<SalesAcceleratorSubmitResult> SubmitCsvAsync(
        Guid partnerCustomerId,
        string submittedBy,
        string fileName,
        byte[] fileContent)
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(partnerCustomerId.ToString()), "partnerCustomerId");
        content.Add(new StringContent(submittedBy), "submittedBy");

        var file = new ByteArrayContent(fileContent);
        file.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
        content.Add(file, "file", fileName);

        var response = await _http.PostAsync("api/sales-accelerator/submissions", content);
        await EnsureSuccessAsync(response, "Submit CSV");
        return await response.Content.ReadFromJsonAsync<SalesAcceleratorSubmitResult>()
               ?? new SalesAcceleratorSubmitResult();
    }

    public async Task<List<SalesAcceleratorSubmissionListItem>> ListSubmissionsAsync(Guid? partnerCustomerId = null)
    {
        var url = partnerCustomerId is Guid id && id != Guid.Empty
            ? $"api/sales-accelerator/submissions?partnerCustomerId={id}"
            : "api/sales-accelerator/submissions";
        var result = await _http.GetFromJsonAsync<List<SalesAcceleratorSubmissionListItem>>(url);
        return result ?? new List<SalesAcceleratorSubmissionListItem>();
    }

    public async Task<(string FileName, string ContentType, byte[] Content)> DownloadSubmissionFileAsync(Guid submissionId)
    {
        var response = await _http.GetAsync($"api/sales-accelerator/submissions/{submissionId}/file");
        await EnsureSuccessAsync(response, "Download submission");
        var bytes = await response.Content.ReadAsByteArrayAsync();
        var fileName = "sales-accelerator-submission.csv";
        var disposition = response.Content.Headers.ContentDisposition;
        if (!string.IsNullOrWhiteSpace(disposition?.FileNameStar))
            fileName = disposition.FileNameStar;
        else if (!string.IsNullOrWhiteSpace(disposition?.FileName))
            fileName = disposition.FileName.Trim('"');

        var contentType = response.Content.Headers.ContentType?.MediaType ?? "text/csv";
        return (fileName, contentType, bytes);
    }

    public async Task<List<SalesAcceleratorAccount>> ListAccountsAsync(Guid? partnerCustomerId = null)
    {
        var url = partnerCustomerId is Guid id && id != Guid.Empty
            ? $"api/sales-accelerator/accounts?partnerCustomerId={id}"
            : "api/sales-accelerator/accounts";
        var result = await _http.GetFromJsonAsync<List<SalesAcceleratorAccount>>(url);
        return result ?? new List<SalesAcceleratorAccount>();
    }

    public async Task SaveAccountFieldsAsync(IEnumerable<SalesAcceleratorAccountUpdate> updates)
    {
        var response = await _http.PutAsJsonAsync("api/sales-accelerator/accounts", updates);
        await EnsureSuccessAsync(response, "Save account fields");
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, string action)
    {
        if (response.IsSuccessStatusCode)
            return;

        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"{action} failed ({(int)response.StatusCode}): {body}");
    }
}
