using DeskNin.Models;

namespace DeskNin.Services;

public interface ITicketNotificationService
{
    Task NotifyTicketUpdatedAsync(
        Ticket ticket,
        string actorUserId,
        string eventLabel,
        string changeSummary,
        CancellationToken cancellationToken = default);
}
