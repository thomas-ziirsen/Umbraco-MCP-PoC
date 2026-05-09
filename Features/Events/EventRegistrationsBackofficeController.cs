using Microsoft.AspNetCore.Mvc;
using Umbraco.Extensions;

namespace MyProject.Features.Events;

[Route("umbraco/backoffice/registrations")]
public sealed class EventRegistrationsBackofficeController : Controller
{
    private readonly IEventRegistrationService _registrationService;

    public EventRegistrationsBackofficeController(IEventRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    private async Task<bool> IsBackOfficeAuthenticatedAsync()
    {
        var authenticateResult = await HttpContext.AuthenticateBackOfficeAsync();
        return authenticateResult.Succeeded;
    }

    [HttpGet("api")]
    public async Task<IActionResult> Api([FromQuery] Guid? eventKey, CancellationToken cancellationToken)
    {
        if (!await IsBackOfficeAuthenticatedAsync())
        {
            return Unauthorized();
        }

        IReadOnlyList<EventRegistrationRow> rows = await _registrationService.ListAsync(eventKey, cancellationToken);
        return Json(rows);
    }
}
