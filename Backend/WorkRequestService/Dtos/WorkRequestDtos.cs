using WorkRequestService.Models;

namespace WorkRequestService.Dtos;

public sealed record WorkRequestResponseDto(
    Guid Id,
    string Title,
    string ClientName,
    string Description,
    WorkRequestPriority Priority,
    WorkRequestStatus Status,
    DateTimeOffset DueDate,
    DateTimeOffset CreatedDate,
    DateTimeOffset UpdatedDate,
    string? Notes)
{
    public static WorkRequestResponseDto FromWorkRequest(WorkRequest request)
        => new(
            request.Id,
            request.Title,
            request.ClientName,
            request.Description,
            request.Priority,
            request.Status,
            request.DueDate,
            request.CreatedDate,
            request.UpdatedDate,
            request.Notes);
}

public sealed record WorkRequestCreateDto(
    string? Title,
    string? ClientName,
    string? Description,
    string? Priority,
    string? Status,
    DateTimeOffset? DueDate,
    string? Notes);

public sealed record WorkRequestUpdateDto(
    string? Title,
    string? ClientName,
    string? Description,
    string? Priority,
    string? Status,
    DateTimeOffset? DueDate,
    string? Notes);

public sealed record ApiErrorResponse(
    string Message,
    IDictionary<string, string[]>? Errors = null);
