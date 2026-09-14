using Microsoft.AspNetCore.Mvc;
using PartnerAppPortal.Core.Interfaces.Services;

namespace PartnerAppPortal.Api.Controllers;

[ApiController]
[Route("api/crm")]
public class CrmController : ControllerBase
{
    private readonly ICrmService _service;

    public CrmController(ICrmService service)
    {
        _service = service;
    }

    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomers()
    {
        try
        {
            var customers = await _service.GetCustomersAsync();
            return Ok(customers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"GetCustomers failed: {ex.GetType().Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// Resolve a CRM customer whose Email domain matches the login email domain.
    /// Example: naveen@dynamicseshop.com → customer with Email *@dynamicseshop.com
    /// </summary>
    [HttpGet("customers/by-email")]
    public async Task<IActionResult> FindByEmail([FromQuery] string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("email is required.");

            var customer = await _service.FindCustomerByEmailDomainAsync(email);
            if (customer is null)
                return NotFound($"No customer found for email domain of '{email}'.");

            return Ok(customer);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"FindByEmail failed: {ex.GetType().Name}: {ex.Message}");
        }
    }
}
