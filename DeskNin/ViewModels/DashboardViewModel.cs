namespace DeskNin.ViewModels;

public sealed class DashboardViewModel
{
    public int OpenCount { get; set; }
    public int InProgressCount { get; set; }
    public int ResolvedCount { get; set; }
    public List<TicketItemViewModel> RecentTickets { get; set; } = [];
}
