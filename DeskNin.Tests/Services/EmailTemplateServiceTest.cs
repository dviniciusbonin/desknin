using DeskNin.Services;
using Microsoft.Extensions.Options;

namespace DeskNin.Tests.Services;

public class EmailTemplateServiceTest
{
    private readonly EmailTemplateService _service = new(
        Options.Create(new ResendOptions()));

    [Fact]
    public void BuildTicketUpdateBody_Includes_Branding_And_Assignee()
    {
        var html = _service.BuildTicketUpdateBody(
            "assignment updated",
            42,
            "VPN access issue",
            "alice",
            "Assigned technician: bob.",
            "bob");

        Assert.Contains("DeskNin", html);
        Assert.Contains("VPN access issue", html);
        Assert.Contains("Assigned to", html);
        Assert.Contains("bob", html);
        Assert.Contains("Ticket management system", html);
    }

    [Fact]
    public void BuildForgotPasswordBody_Includes_Reset_Cta()
    {
        var html = _service.BuildForgotPasswordBody("https://app/reset");
        Assert.Contains("Reset password", html);
        Assert.Contains("https://app/reset", html);
    }
}
