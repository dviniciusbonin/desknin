using DeskNin.Models;

namespace DeskNin.ViewModels;

public sealed class TicketListViewModel
{
    public List<TicketItemViewModel> Tickets { get; set; } = [];
    public TicketStatus? SelectedStatus { get; set; }
}
