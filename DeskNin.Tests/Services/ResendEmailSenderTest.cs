using DeskNin.Services;
using Microsoft.Extensions.Options;

namespace DeskNin.Tests.Services;

public class ResendEmailSenderTest
{
    [Fact]
    public async Task SendEmailAsync_With_Local_FromEmail_Throws_Clear_Error()
    {
        var options = Options.Create(new ResendOptions
        {
            ApiKey = "re_test",
            FromEmail = "admin@desknin.local"
        });

        using var http = new HttpClient(new StubHandler());
        var sender = new ResendEmailSender(http, options);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sender.SendEmailAsync("user@example.com", "subject", "<p>body</p>"));

        Assert.Contains("invalid for production sending", ex.Message);
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
        }
    }
}
