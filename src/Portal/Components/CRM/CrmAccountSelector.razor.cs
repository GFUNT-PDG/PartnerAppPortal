using Microsoft.AspNetCore.Components;
using PartnerAppPortal.Core.Models.Crm;
using PartnerAppPortal.Portal.Services;

namespace PartnerAppPortal.Portal.Components.CRM;

public partial class CrmAccountSelector : ComponentBase
{
    [Inject] protected IAssessmentApiClient AssessmentApi { get; set; } = default!;

    [Parameter] public EventCallback<Customer> CustomerSelected { get; set; }

    protected List<Customer> Customers { get; set; } = new();
    protected string SelectedCustomerIdValue { get; set; } = string.Empty;
    protected string? LoadError { get; set; }
    protected string? StatusMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Customers = await AssessmentApi.GetCustomersAsync() ?? new();
        }
        catch (Exception ex)
        {
            LoadError = $"Could not load customers: {ex.Message}";
            Customers = new();
        }
    }

    protected Task OnSelectionChanged()
    {
        StatusMessage = null;
        return Task.CompletedTask;
    }

    protected async Task ConfirmCustomerSelection()
    {
        StatusMessage = null;

        if (!Guid.TryParse(SelectedCustomerIdValue, out var id) || id == Guid.Empty)
        {
            StatusMessage = "Please select a customer from the list, then click Confirm.";
            return;
        }

        var customer = Customers.FirstOrDefault(c => c.Id == id);
        if (customer is null)
        {
            StatusMessage = "Selected customer was not found. Refresh and try again.";
            return;
        }

        await CustomerSelected.InvokeAsync(customer);
    }
}
