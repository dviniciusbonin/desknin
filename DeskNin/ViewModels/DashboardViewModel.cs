namespace DeskNin.ViewModels;

public sealed class DashboardViewModel
{
    public int OpenCount { get; set; }
    public int InProgressCount { get; set; }
    public int ResolvedCount { get; set; }

    public int HighPriorityCount { get; set; }

    public List<DashboardPriorityBucketViewModel> PriorityBreakdown { get; set; } = [];

    public List<DashboardRecentActivityViewModel> RecentActivities { get; set; } = [];

    public List<TicketItemViewModel> RecentTickets { get; set; } = [];
}

public sealed class DashboardPriorityBucketViewModel
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}

public sealed class DashboardRecentActivityViewModel
{
    public string AuthorName { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public int TicketId { get; set; }
    public DateTime EventTimeUtc { get; set; }
}

public sealed class DashboardMetricCardViewModel
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }

    // Bootstrap background utility class for the small dot/icon (e.g. "bg-primary").
    public string AccentCssClass { get; set; } = "bg-primary";

    // Controls which right-side icon is rendered in the metric card.
    public string IconKind { get; set; } = string.Empty;
}
