using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;

namespace DeskNin.Services;

public sealed class EmailTemplateService(IOptions<ResendOptions> resendOptions) : IEmailTemplateService
{
    private static readonly HtmlEncoder Encoder = HtmlEncoder.Default;
    private readonly ResendOptions _resendOptions = resendOptions.Value;

    public string BuildTicketUpdateBody(
        string eventLabel,
        int ticketId,
        string ticketTitle,
        string actorName,
        string changeSummary,
        string? assigneeName)
    {
        var assigneeRow = string.IsNullOrWhiteSpace(assigneeName)
            ? string.Empty
            : $"<tr><td style='padding:6px 0;color:#64748B;font-size:14px;'>Assigned to</td><td style='padding:6px 0;color:#0F172A;font-size:14px;font-weight:600;text-align:right;'>{Encode(assigneeName)}</td></tr>";

        var content = $@"
            <p style='margin:0 0 16px 0;color:#334155;font-size:14px;'>A ticket has been updated in DeskNin.</p>
            <table role='presentation' width='100%' cellspacing='0' cellpadding='0' style='margin:0 0 16px 0;border-collapse:collapse;'>
                <tr><td style='padding:6px 0;color:#64748B;font-size:14px;'>Event</td><td style='padding:6px 0;color:#0F172A;font-size:14px;font-weight:600;text-align:right;'>{Encode(eventLabel)}</td></tr>
                <tr><td style='padding:6px 0;color:#64748B;font-size:14px;'>Ticket</td><td style='padding:6px 0;color:#0F172A;font-size:14px;font-weight:600;text-align:right;'>#{ticketId} - {Encode(ticketTitle)}</td></tr>
                <tr><td style='padding:6px 0;color:#64748B;font-size:14px;'>Updated by</td><td style='padding:6px 0;color:#0F172A;font-size:14px;font-weight:600;text-align:right;'>{Encode(actorName)}</td></tr>
                {assigneeRow}
            </table>
            <div style='background:#F8FAFC;border:1px solid #E2E8F0;border-radius:10px;padding:12px 14px;color:#334155;font-size:14px;line-height:1.5;'>
                <strong style='color:#0F172A;'>Change summary:</strong><br/>{Encode(changeSummary)}
            </div>";

        return Wrap("Ticket update", content);
    }

    public string BuildOnboardingBody(string userEmail, string resetPasswordUrl)
    {
        var content = $@"
            <p style='margin:0 0 16px 0;color:#334155;font-size:14px;'>Your DeskNin account has been created.</p>
            <table role='presentation' width='100%' cellspacing='0' cellpadding='0' style='margin:0 0 16px 0;border-collapse:collapse;'>
                <tr><td style='padding:6px 0;color:#64748B;font-size:14px;'>Email</td><td style='padding:6px 0;color:#0F172A;font-size:14px;font-weight:600;text-align:right;'>{Encode(userEmail)}</td></tr>
            </table>
            <p style='margin:0 0 16px 0;color:#334155;font-size:14px;'>Please create your password to access DeskNin.</p>
            <a href='{Encode(resetPasswordUrl)}' style='display:inline-block;background:#2563EB;color:#FFFFFF;text-decoration:none;padding:10px 16px;border-radius:8px;font-size:14px;font-weight:600;'>Set password</a>";

        return Wrap("Welcome to DeskNin", content);
    }

    public string BuildForgotPasswordBody(string resetUrl)
    {
        var content = $@"
            <p style='margin:0 0 16px 0;color:#334155;font-size:14px;'>You requested a password reset for your DeskNin account.</p>
            <a href='{Encode(resetUrl)}' style='display:inline-block;background:#2563EB;color:#FFFFFF;text-decoration:none;padding:10px 16px;border-radius:8px;font-size:14px;font-weight:600;'>Reset password</a>
            <p style='margin:16px 0 0 0;color:#64748B;font-size:13px;'>If you did not request this, you can safely ignore this email.</p>";

        return Wrap("Password reset", content);
    }

    private string Wrap(string heading, string innerHtml)
    {
        var brandBlock =
            "<div style='font:700 20px Inter,Segoe UI,Arial,sans-serif;line-height:1;'>" +
            "<span style='color:#E2E8F0;'>Desk</span>" +
            "<span style='color:#60A5FA;'>Nin</span>" +
            "</div>";

        return $@"
<!doctype html>
<html>
<body style='margin:0;padding:0;background:#F1F5F9;font-family:Inter,Segoe UI,Arial,sans-serif;'>
    <table role='presentation' width='100%' cellspacing='0' cellpadding='0' style='padding:24px 12px;'>
        <tr>
            <td align='center'>
                <table role='presentation' width='100%' cellspacing='0' cellpadding='0' style='max-width:620px;background:#FFFFFF;border:1px solid #E2E8F0;border-radius:14px;overflow:hidden;'>
                    <tr>
                        <td style='background:#0F172A;padding:18px 20px;'>
                            {brandBlock}
                            <div style='color:#CBD5E1;font-size:12px;margin-top:2px;'>Ticket management system</div>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding:22px 20px;'>
                            <h2 style='margin:0 0 16px 0;color:#0F172A;font-size:18px;'>{Encode(heading)}</h2>
                            {innerHtml}
                        </td>
                    </tr>
                    <tr>
                        <td style='border-top:1px solid #E2E8F0;padding:12px 20px;color:#64748B;font-size:12px;'>
                            This message was sent by DeskNin.
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    private static string Encode(string value) => Encoder.Encode(value);
}
