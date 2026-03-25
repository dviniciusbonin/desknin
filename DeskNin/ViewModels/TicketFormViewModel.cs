using System.ComponentModel.DataAnnotations;
using DeskNin.Models;

namespace DeskNin.ViewModels;

public sealed class TicketFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
}
