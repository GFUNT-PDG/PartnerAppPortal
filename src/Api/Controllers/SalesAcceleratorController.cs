using Microsoft.AspNetCore.Mvc;
using PartnerAppPortal.Core.Interfaces.Services;
using PartnerAppPortal.Core.Models.SalesAccelerator;
using PartnerAppPortal.Core.Services.SalesAccelerator;

namespace PartnerAppPortal.Api.Controllers;

[ApiController]
[Route("api/sales-accelerator")]
public class SalesAcceleratorController : ControllerBase
{
    private readonly ISalesAcceleratorService _service;

    public SalesAcceleratorController(ISalesAcceleratorService service)
    {
        _service = service;
    }

    [HttpGet("template")]
    public IActionResult DownloadTemplate()
    {
        var bytes = System.Text.Encoding.UTF8.GetPreamble()
            .Concat(System.Text.Encoding.UTF8.GetBytes(SalesAcceleratorCsvParser.TemplateCsv))
            .ToArray();
        return File(bytes, "text/csv", "SalesAcceleratorAccounts-Template.csv");
    }

    [HttpPost("submissions")]
    [RequestSizeLimit(2 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 2 * 1024 * 1024)]
    public async Task<IActionResult> Submit(
        [FromForm] Guid partnerCustomerId,
        [FromForm] string submittedBy,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        try
        {
            if (file is null || file.Length == 0)
                return BadRequest("Upload a completed CSV template before submitting.");

            await using var stream = file.OpenReadStream();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms, cancellationToken);

            var result = await _service.SubmitCsvAsync(
                partnerCustomerId,
                submittedBy,
                file.FileName,
                ms.ToArray(),
                cancellationToken);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Submit failed: {ex.GetType().Name}: {ex.Message}");
        }
    }

    [HttpGet("submissions")]
    public async Task<IActionResult> ListSubmissions([FromQuery] Guid? partnerCustomerId)
    {
        try
        {
            var items = await _service.ListSubmissionsAsync(partnerCustomerId);
            return Ok(items);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"List submissions failed: {ex.Message}");
        }
    }

    [HttpGet("submissions/{id:guid}/file")]
    public async Task<IActionResult> DownloadSubmission(Guid id)
    {
        try
        {
            var file = await _service.GetSubmissionFileAsync(id);
            if (file is null)
                return NotFound("Submission file was not found.");

            var name = string.IsNullOrWhiteSpace(file.FileName)
                ? "sales-accelerator-submission.csv"
                : file.FileName;
            return File(file.Content, file.ContentType, name);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Download failed: {ex.Message}");
        }
    }

    [HttpGet("accounts")]
    public async Task<IActionResult> ListAccounts([FromQuery] Guid? partnerCustomerId)
    {
        try
        {
            var items = await _service.ListAccountsAsync(partnerCustomerId);
            return Ok(items);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"List accounts failed: {ex.Message}");
        }
    }

    [HttpPut("accounts")]
    public async Task<IActionResult> UpdateAccounts([FromBody] List<SalesAcceleratorAccountUpdate> updates)
    {
        try
        {
            await _service.UpdateAccountFieldsAsync(updates ?? new List<SalesAcceleratorAccountUpdate>());
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Save failed: {ex.Message}");
        }
    }
}
