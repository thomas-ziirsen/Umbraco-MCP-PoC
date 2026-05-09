
using System.Text;
using MyProject.Features.Events;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IEventRegistrationService, EventRegistrationService>();
builder.Services.AddControllersWithViews();

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .Build();

WebApplication app = builder.Build();


await app.BootUmbracoAsync();

using (IServiceScope scope = app.Services.CreateScope())
{
    IEventRegistrationService registrationService = scope.ServiceProvider.GetRequiredService<IEventRegistrationService>();
    await registrationService.EnsureStoreAsync();
}


app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

app.MapControllers();

app.MapPost("/events/register", async (HttpRequest request, IEventRegistrationService registrationService, CancellationToken cancellationToken) =>
{
    IFormCollection form = await request.ReadFormAsync(cancellationToken);

    Guid.TryParse(form["eventKey"], out Guid eventKey);
    string eventName = form["eventName"].ToString();
    string name = form["name"].ToString();
    string email = form["email"].ToString();
    string? phone = form["phone"].ToString();
    bool consentAccepted = string.Equals(form["consent"], "on", StringComparison.OrdinalIgnoreCase)
        || string.Equals(form["consent"], "true", StringComparison.OrdinalIgnoreCase);

    EventRegistrationCreateRequest createRequest = new(
        eventKey,
        eventName,
        name,
        email,
        phone,
        consentAccepted);

    EventRegistrationResult result = await registrationService.RegisterAsync(createRequest, cancellationToken);

    string returnUrl = form["returnUrl"].ToString();
    if (string.IsNullOrWhiteSpace(returnUrl) || !Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
    {
        returnUrl = "/";
    }

    string suffix = result.Status switch
    {
        EventRegistrationStatus.Success => "registration=success",
        EventRegistrationStatus.Duplicate => "registration=duplicate",
        EventRegistrationStatus.Invalid => "registration=invalid",
        _ => "registration=error"
    };

    string separator = returnUrl.Contains('?', StringComparison.Ordinal) ? "&" : "?";
    return Results.Redirect($"{returnUrl}{separator}{suffix}");
});

app.MapGet("/events/registrations", async (HttpContext httpContext, Guid? eventKey, IEventRegistrationService registrationService, CancellationToken cancellationToken) =>
{
    if (httpContext.User?.Identity?.IsAuthenticated != true)
    {
        return Results.Unauthorized();
    }

    IReadOnlyList<EventRegistrationRow> rows = await registrationService.ListAsync(eventKey, cancellationToken);
    return Results.Ok(rows);
});

app.MapGet("/events/registrations.csv", async (HttpContext httpContext, Guid? eventKey, IEventRegistrationService registrationService, CancellationToken cancellationToken) =>
{
    if (httpContext.User?.Identity?.IsAuthenticated != true)
    {
        return Results.Unauthorized();
    }

    IReadOnlyList<EventRegistrationRow> rows = await registrationService.ListAsync(eventKey, cancellationToken);

    StringBuilder csv = new();
    csv.AppendLine("Id,EventKey,EventName,Name,Email,Phone,ConsentAccepted,CreatedUtc");

    foreach (EventRegistrationRow row in rows)
    {
        static string Escape(string? value) => $"\"{(value ?? string.Empty).Replace("\"", "\"\"")}\"";
        csv.AppendLine(string.Join(",", [
            Escape(row.Id.ToString("D")),
            Escape(row.EventKey.ToString("D")),
            Escape(row.EventName),
            Escape(row.Name),
            Escape(row.Email),
            Escape(row.Phone),
            Escape(row.ConsentAccepted ? "true" : "false"),
            Escape(row.CreatedUtc.ToString("O"))
        ]));
    }

    return Results.File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "event-registrations.csv");
});

await app.RunAsync();
