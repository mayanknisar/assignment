namespace WorkRequestService.Models;

public enum WorkRequestPriority
{
    Low,
    Medium,
    High
}

public enum WorkRequestStatus
{
    New,
    InProgress,
    Blocked,
    Completed
}

public class WorkRequest
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string ClientName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public WorkRequestPriority Priority { get; set; }
    public WorkRequestStatus Status { get; set; }
    public DateTimeOffset DueDate { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public string? Notes { get; set; }
}
