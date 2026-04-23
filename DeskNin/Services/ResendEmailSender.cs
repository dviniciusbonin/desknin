using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace DeskNin.Services;

public sealed class ResendEmailSender(
    HttpClient httpClient,
    IOptions<ResendOptions> options) : IAppEmailSender
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient = httpClient;
    private readonly ResendOptions _options = options.Value;

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            throw new InvalidOperationException("Resend API key is not configured.");
        if (string.IsNullOrWhiteSpace(_options.FromEmail))
            throw new InvalidOperationException("Resend from e-mail is not configured.");
        if (!IsValidFromEmail(_options.FromEmail))
            throw new InvalidOperationException(
                $"Resend from e-mail is invalid for production sending: '{_options.FromEmail}'. Use a verified domain sender (e.g. no-reply@your-domain.com).");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        var payload = new
        {
            from = _options.FromEmail,
            to = new[] { toEmail },
            subject,
            html = htmlBody,
            reply_to = string.IsNullOrWhiteSpace(_options.ReplyToEmail) ? null : _options.ReplyToEmail
        };

        var json = JsonSerializer.Serialize(payload, JsonOptions);
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Resend send failed with status {(int)response.StatusCode}: {body}");
        }
    }

    private static bool IsValidFromEmail(string fromEmail)
    {
        var normalized = fromEmail.Trim().ToLowerInvariant();
        if (normalized == "admin@desknin.local")
            return false;
        if (normalized.EndsWith("@desknin.local", StringComparison.Ordinal))
            return false;
        if (normalized.Contains(".local", StringComparison.Ordinal))
            return false;
        return normalized.Contains('@');
    }
}
