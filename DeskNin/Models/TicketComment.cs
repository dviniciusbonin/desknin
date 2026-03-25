using Microsoft.AspNetCore.Identity;

namespace DeskNin.Models;

public class TicketComment
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Ticket Ticket { get; set; } = null!;
    public IdentityUser Author { get; set; } = null!;
}
