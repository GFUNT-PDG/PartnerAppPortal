using Microsoft.AspNetCore.Mvc;
using PartnerAppPortal.Api.Onboarding;
using PartnerAppPortal.Core.Interfaces.Repositories;
using PartnerAppPortal.Core.Models.Onboarding;

namespace PartnerAppPortal.Api.Controllers;

[ApiController]
[Route("api/onboarding")]
public class OnboardingController : ControllerBase
{
    private readonly IPartnerOnboardingRepository _onboarding;
    private readonly ContactCardWriter _contactCards;
    private readonly DataverseAccountProfile _crm;

    public OnboardingController(
        IPartnerOnboardingRepository onboarding,
        ContactCardWriter contactCards,
        DataverseAccountProfile crm)
    {
        _onboarding = onboarding;
        _contactCards = contactCards;
        _crm = crm;
    }

    [HttpGet("{accountId:guid}")]
    public async Task<IActionResult> Get(Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            var document = await _onboarding.GetAsync(accountId, cancellationToken);
            var profile = await _crm.ReadAsync(accountId, cancellationToken);
            document.Fields = profile.Values;
            document.UnmappedLabels = profile.UnmappedLabels.ToList();
            document.FieldChoices = profile.Choices;
            await ApplyDynamicsPillAsync(accountId, document, updatedBy: null, cancellationToken);
            return Ok(document);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{accountId:guid}")]
    public async Task<IActionResult> Save(
        Guid accountId,
        [FromBody] PartnerOnboardingSaveRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var warning = await _crm.WriteAsync(accountId, request.Fields, cancellationToken);
            var document = await _onboarding.GetAsync(accountId, cancellationToken);
            var profile = await _crm.ReadAsync(accountId, cancellationToken);
            document.Fields = profile.Values;
            await ApplyDynamicsPillAsync(accountId, document, request.UpdatedBy, cancellationToken);
            var checklist = OnboardingCatalog.ToChecklistView(document);
            checklist.CrmWarning = warning;
            return Ok(checklist);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPatch("{accountId:guid}/checklist")]
    public async Task<IActionResult> UpdateChecklistItem(
        Guid accountId,
        [FromBody] ChecklistItemUpdate update,
        CancellationToken cancellationToken)
    {
        try
        {
            await _onboarding.UpdateChecklistItemAsync(accountId, update, cancellationToken);
            try
            {
                var document = await _onboarding.GetAsync(accountId, cancellationToken);
                var profile = await _crm.ReadAsync(accountId, cancellationToken);
                document.Fields = profile.Values;
                await ApplyDynamicsPillAsync(accountId, document, update.UpdatedBy, cancellationToken);
                return Ok(OnboardingCatalog.ToChecklistView(document));
            }
            catch (InvalidOperationException)
            {
                var stored = await _onboarding.GetAsync(accountId, cancellationToken);
                return Ok(OnboardingCatalog.ToChecklistView(stored));
            }
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("{accountId:guid}/contact-card")]
    public async Task<IActionResult> CreateContactCard(
        Guid accountId,
        [FromQuery] string? updatedBy,
        CancellationToken cancellationToken)
    {
        try
        {
            var blobName = await _contactCards.WriteAsync(accountId, updatedBy, cancellationToken);
            return Ok(new ContactCardResult
            {
                AccountName = blobName.Split('/')[0],
                BlobName = blobName
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Contact card upload failed: {ex.Message}");
        }
    }

    private async Task ApplyDynamicsPillAsync(
        Guid accountId,
        PartnerOnboardingDocument document,
        string? updatedBy,
        CancellationToken cancellationToken)
    {
        document.FieldsFromDynamics = true;
        var complete = OnboardingCatalog.DynamicsDetailsComplete(document.Fields);
        document.Checklist.TryGetValue(OnboardingCatalog.AllDynamicsDetailsKey, out var current);
        var normalized = OnboardingCatalog.NormalizeYesNo(current);
        var desired = complete ? "Yes" : "No";
        var markComplete = complete && !string.Equals(normalized, "Yes", StringComparison.Ordinal);
        var clearStale = !complete && string.Equals(normalized, "Yes", StringComparison.Ordinal);
        if (!markComplete && !clearStale)
            return;

        await _onboarding.UpdateChecklistItemAsync(accountId, new ChecklistItemUpdate
        {
            Key = OnboardingCatalog.AllDynamicsDetailsKey,
            Value = desired,
            UpdatedBy = updatedBy
        }, cancellationToken);
        document.Checklist[OnboardingCatalog.AllDynamicsDetailsKey] = desired;
    }
}
