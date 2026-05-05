using Microsoft.EntityFrameworkCore;
using WorkRequestService.Dtos;
using WorkRequestService.Models;

namespace WorkRequestService.Endpoints;

public static class WorkRequestEndpoints
{
    public static void MapWorkRequestEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/work-requests")
            .WithName("WorkRequests");

        group.MapGet("/", GetAll)
            .WithName("GetAllWorkRequests")
            .WithDescription("Get all work requests");

        group.MapGet("/{id:guid}", GetById)
            .WithName("GetWorkRequestById")
            .WithDescription("Get a work request by ID");

        group.MapPost("/", Create)
            .WithName("CreateWorkRequest")
            .WithDescription("Create a new work request");

        group.MapPut("/{id:guid}", Update)
            .WithName("UpdateWorkRequest")
            .WithDescription("Update an existing work request");

        group.MapPatch("/{id:guid}", Patch)
            .WithName("PatchWorkRequest")
            .WithDescription("Partially update a work request");

        group.MapDelete("/{id:guid}", Delete)
            .WithName("DeleteWorkRequest")
            .WithDescription("Delete a work request");
    }

    private static async Task<IResult> GetAll(WorkRequestDbContext db)
    {
        var requests = await db.WorkRequests.AsNoTracking().ToListAsync();
        return Results.Ok(requests.Select(WorkRequestResponseDto.FromWorkRequest));
    }

    private static async Task<IResult> GetById(Guid id, WorkRequestDbContext db)
    {
        var workRequest = await db.WorkRequests.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
        return workRequest is null
            ? Results.NotFound(new ApiErrorResponse("Work request not found"))
            : Results.Ok(WorkRequestResponseDto.FromWorkRequest(workRequest));
    }

    private static async Task<IResult> Create(WorkRequestDbContext db, WorkRequestCreateDto dto)
    {
        var validationErrors = ValidateWorkRequestCreateDto(dto);
        if (validationErrors.Any())
        {
            return Results.BadRequest(new ApiErrorResponse("Validation failed", validationErrors));
        }

        var workRequest = new WorkRequest
        {
            Id = Guid.NewGuid(),
            Title = dto.Title!.Trim(),
            ClientName = dto.ClientName!.Trim(),
            Description = dto.Description!.Trim(),
            Priority = ParsePriority(dto.Priority!),
            Status = ParseStatus(dto.Status!),
            DueDate = dto.DueDate!.Value,
            CreatedDate = DateTimeOffset.UtcNow,
            UpdatedDate = DateTimeOffset.UtcNow,
            Notes = dto.Notes?.Trim()
        };

        db.WorkRequests.Add(workRequest);
        await db.SaveChangesAsync();

        return Results.Created($"/api/work-requests/{workRequest.Id}", WorkRequestResponseDto.FromWorkRequest(workRequest));
    }

    private static async Task<IResult> Update(Guid id, WorkRequestUpdateDto dto, WorkRequestDbContext db)
    {
        var workRequest = await db.WorkRequests.FindAsync(id);
        if (workRequest is null)
        {
            return Results.NotFound(new ApiErrorResponse("Work request not found"));
        }

        var validationErrors = ValidateWorkRequestUpdateDto(dto);
        if (validationErrors.Any())
        {
            return Results.BadRequest(new ApiErrorResponse("Validation failed", validationErrors));
        }

        workRequest.Title = dto.Title!.Trim();
        workRequest.ClientName = dto.ClientName!.Trim();
        workRequest.Description = dto.Description!.Trim();
        workRequest.Priority = ParsePriority(dto.Priority!);
        workRequest.Status = ParseStatus(dto.Status!);
        workRequest.DueDate = dto.DueDate!.Value;
        workRequest.UpdatedDate = DateTimeOffset.UtcNow;
        workRequest.Notes = dto.Notes?.Trim();

        await db.SaveChangesAsync();
        return Results.Ok(WorkRequestResponseDto.FromWorkRequest(workRequest));
    }

    private static async Task<IResult> Patch(Guid id, WorkRequestUpdateDto dto, WorkRequestDbContext db)
    {
        var workRequest = await db.WorkRequests.FindAsync(id);
        if (workRequest is null)
        {
            return Results.NotFound(new ApiErrorResponse("Work request not found"));
        }

        // Only update fields that are provided (not null)
        if (!string.IsNullOrWhiteSpace(dto.Title))
        {
            workRequest.Title = dto.Title.Trim();
        }

        if (!string.IsNullOrWhiteSpace(dto.ClientName))
        {
            workRequest.ClientName = dto.ClientName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(dto.Description))
        {
            workRequest.Description = dto.Description.Trim();
        }

        if (!string.IsNullOrWhiteSpace(dto.Priority))
        {
            workRequest.Priority = ParsePriority(dto.Priority);
        }

        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            workRequest.Status = ParseStatus(dto.Status);
        }

        if (dto.DueDate.HasValue)
        {
            workRequest.DueDate = dto.DueDate.Value;
        }

        if (dto.Notes != null)
        {
            workRequest.Notes = dto.Notes.Trim();
        }

        workRequest.UpdatedDate = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
        return Results.Ok(WorkRequestResponseDto.FromWorkRequest(workRequest));
    }

    private static async Task<IResult> Delete(Guid id, WorkRequestDbContext db)
    {
        var workRequest = await db.WorkRequests.FindAsync(id);
        if (workRequest is null)
        {
            return Results.NotFound(new ApiErrorResponse("Work request not found"));
        }

        db.WorkRequests.Remove(workRequest);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    private static IDictionary<string, string[]> ValidateWorkRequestCreateDto(WorkRequestCreateDto dto)
        => ValidateWorkRequestDtoCore(dto.Title, dto.ClientName, dto.Description, dto.Priority, dto.Status, dto.DueDate);

    private static IDictionary<string, string[]> ValidateWorkRequestUpdateDto(WorkRequestUpdateDto dto)
        => ValidateWorkRequestDtoCore(dto.Title, dto.ClientName, dto.Description, dto.Priority, dto.Status, dto.DueDate);

    private static IDictionary<string, string[]> ValidateWorkRequestDtoCore(
        string? title,
        string? clientName,
        string? description,
        string? priority,
        string? status,
        DateTimeOffset? dueDate)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(title))
        {
            errors["title"] = new[] { "Title is required." };
        }

        if (string.IsNullOrWhiteSpace(clientName))
        {
            errors["clientName"] = new[] { "Client name is required." };
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            errors["description"] = new[] { "Description is required." };
        }

        if (string.IsNullOrWhiteSpace(priority))
        {
            errors["priority"] = new[] { "Priority is required." };
        }
        else if (!Enum.TryParse<WorkRequestPriority>(priority, true, out _))
        {
            errors["priority"] = new[] { "Invalid priority. Allowed values: Low, Medium, High." };
        }

        if (string.IsNullOrWhiteSpace(status))
        {
            errors["status"] = new[] { "Status is required." };
        }
        else if (!Enum.TryParse<WorkRequestStatus>(status, true, out _))
        {
            errors["status"] = new[] { "Invalid status. Allowed values: New, InProgress, Blocked, Completed." };
        }

        if (dueDate is null)
        {
            errors["dueDate"] = new[] { "Due date is required." };
        }

        return errors;
    }

    private static WorkRequestPriority ParsePriority(string value)
        => Enum.Parse<WorkRequestPriority>(value, true);

    private static WorkRequestStatus ParseStatus(string value)
        => Enum.Parse<WorkRequestStatus>(value, true);
}
