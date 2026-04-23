namespace DeskNin.Services;

public interface IEmailTemplateService
{
    string BuildTicketUpdateBody(
        string eventLabel,
        int ticketId,
        string ticketTitle,
        string actorName,
        string changeSummary,
        string? assigneeName);

    string BuildOnboardingBody(string userEmail, string resetPasswordUrl);

    string BuildForgotPasswordBody(string resetUrl);
}
