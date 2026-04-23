using DeskNin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DeskNin.Services;

public class TicketNotificationService(
    UserManager<IdentityUser> userManager,
    IAppSettingsService appSettingsService,
    IAppEmailSender emailSender,
    IEmailTemplateService emailTemplateService,
    ILogger<TicketNotificationService> logger) : ITicketNotificationService
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly IAppSettingsService _appSettingsService = appSettingsService;
    private readonly IAppEmailSender _emailSender = emailSender;
    private readonly IEmailTemplateService _emailTemplateService = emailTemplateService;
    private readonly ILogger<TicketNotificationService> _logger = logger;

    public async Task NotifyTicketUpdatedAsync(
        Ticket ticket,
        string actorUserId,
        string eventLabel,
        string changeSummary,
        CancellationToken cancellationToken = default)
    {
        if (!await _appSettingsService.IsEmailEnabledAsync(cancellationToken))
            return;

        var actor = await _userManager.FindByIdAsync(actorUserId);
        var actorName = actor?.UserName ?? actor?.Email ?? "A user";

        var recipients = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(ticket.AuthorId))
        {
            var author = await _userManager.FindByIdAsync(ticket.AuthorId);
            if (!string.IsNullOrWhiteSpace(author?.Email))
                recipients.Add(author.Email);
        }

        string? assigneeName = null;
        if (!string.IsNullOrWhiteSpace(ticket.AssignedTechnicianId))
        {
            var assignee = await _userManager.FindByIdAsync(ticket.AssignedTechnicianId);
            assigneeName = assignee?.UserName ?? assignee?.Email;
            if (!string.IsNullOrWhiteSpace(assignee?.Email))
                recipients.Add(assignee.Email);
        }

        if (recipients.Count == 0)
            return;

        var safeTitle = ticket.Title.Length > 80 ? $"{ticket.Title[..77]}..." : ticket.Title;
        var subject = $"[DeskNin] {eventLabel}: {safeTitle} (#{ticket.Id})";
        var htmlBody = _emailTemplateService.BuildTicketUpdateBody(
            eventLabel,
            ticket.Id,
            ticket.Title,
            actorName,
            changeSummary,
            assigneeName);

        foreach (var email in recipients)
        {
            try
            {
                await _emailSender.SendEmailAsync(email, subject, htmlBody, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send ticket update email to {Email}", email);
            }
        }
    }
}
