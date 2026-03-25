using Microsoft.AspNetCore.Identity;

namespace DeskNin.Models;

public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public string AuthorId { get; set; } = string.Empty;
    public string? AssignedTechnicianId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public IdentityUser Author { get; set; } = null!;
    public IdentityUser? AssignedTechnician { get; set; }
    public ICollection<TicketComment> Comments { get; set; } = [];

    public bool IsClosed => Status == TicketStatus.Closed;

    public bool CanEditDetails => !IsClosed;

    public bool CanManageWorkflow => !IsClosed;

    public bool CanChangeStatusTo(TicketStatus newStatus) =>
        Status != TicketStatus.Closed || newStatus == TicketStatus.Closed;

    public void ChangeStatus(TicketStatus newStatus, DateTime utcNow)
    {
        if (!CanChangeStatusTo(newStatus))
            throw new InvalidOperationException("Closed ticket status is final.");

        Status = newStatus;
        UpdatedAtUtc = utcNow;
    }

    public void SetPriority(TicketPriority newPriority, DateTime utcNow)
    {
        Priority = newPriority;
        UpdatedAtUtc = utcNow;
    }

    public void AssignTechnician(string? technicianUserId, DateTime utcNow)
    {
        AssignedTechnicianId = technicianUserId;
        UpdatedAtUtc = utcNow;
    }

    public void Touch(DateTime utcNow) => UpdatedAtUtc = utcNow;
}
