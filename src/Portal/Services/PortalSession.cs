namespace PartnerAppPortal.Portal.Services;

public sealed class PortalSession
{
    public string? Email { get; set; }
    public Guid? CustomerId { get; set; }
    public bool IsEmployee { get; set; }

    public void SetPartner(string email, Guid customerId)
    {
        Email = email;
        CustomerId = customerId;
        IsEmployee = false;
    }

    public void SetEmployee(string email)
    {
        Email = email;
        CustomerId = null;
        IsEmployee = true;
    }

    public void RememberCustomer(Guid customerId)
    {
        CustomerId = customerId;
        Changed?.Invoke();
    }

    public event Action? Changed;
}
