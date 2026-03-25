using DeskNin.Models;

namespace DeskNin.ViewModels;

public sealed class TicketItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string DescriptionPreview { get; set; } = string.Empty;
    public TicketStatus Status { get; set; }
    public TicketPriority Priority { get; set; }
    public string AuthorName { get; set; } = "-";
    public string? AssignedTechnicianId { get; set; }
    public string? AssignedTechnicianName { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public int CommentCount { get; set; }
}
