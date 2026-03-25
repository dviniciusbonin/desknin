using System.ComponentModel.DataAnnotations;

namespace DeskNin.ViewModels;

public sealed class CommentFormViewModel
{
    public int TicketId { get; set; }

    [Required(ErrorMessage = "Comment is required")]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;
}
