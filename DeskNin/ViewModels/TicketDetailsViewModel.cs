using DeskNin.Models;

namespace DeskNin.ViewModels;

public sealed class TicketDetailsViewModel
{
    public TicketItemViewModel Ticket { get; set; } = new();
    public string Description { get; set; } = string.Empty;
    public List<CommentViewModel> Comments { get; set; } = [];
    public CommentFormViewModel NewComment { get; set; } = new();
    public List<TechnicianOptionViewModel> Technicians { get; set; } = [];
    public bool CanEditTicket { get; set; }
    public bool CanManageWorkflow { get; set; }
}

public sealed class TechnicianOptionViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
