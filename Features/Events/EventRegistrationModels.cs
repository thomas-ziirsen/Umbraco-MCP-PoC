namespace MyProject.Features.Events;

public sealed record EventRegistrationCreateRequest(
    Guid EventKey,
    string EventName,
    string Name,
    string Email,
    string? Phone,
    bool ConsentAccepted);

public sealed record EventRegistrationRow(
    Guid Id,
    Guid EventKey,
    string EventName,
    string Name,
    string Email,
    string? Phone,
    bool ConsentAccepted,
    DateTime CreatedUtc);

public enum EventRegistrationStatus
{
    Success,
    Duplicate,
    Invalid,
    Error
}

public sealed record EventRegistrationResult(
    EventRegistrationStatus Status,
    string Message,
    EventRegistrationRow? Registration = null);
