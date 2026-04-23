using DeskNin.Models;
using DeskNin.Services;
using DeskNin.Tests.TestHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace DeskNin.Tests.Services;

public class TicketNotificationServiceTest : IDisposable
{
    private readonly DeskNin.Data.ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public TicketNotificationServiceTest()
    {
        (_context, _userManager, _) = IdentityTestHelpers.CreateIdentityServices();
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task NotifyTicketUpdatedAsync_Uses_Title_In_Subject_And_Assignee_In_Body()
    {
        var actor = new IdentityUser { UserName = "actor", Email = "actor@example.com" };
        var author = new IdentityUser { UserName = "author", Email = "author@example.com" };
        var assignee = new IdentityUser { UserName = "tech", Email = "tech@example.com" };
        await _userManager.CreateAsync(actor, "Password123!");
        await _userManager.CreateAsync(author, "Password123!");
        await _userManager.CreateAsync(assignee, "Password123!");

        var settings = new Mock<IAppSettingsService>();
        settings.Setup(s => s.IsEmailEnabledAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var template = new EmailTemplateService(
            Options.Create(new ResendOptions { LogoUrl = "https://example.com/logo.png" }));
        string? sentSubject = null;
        string? sentBody = null;

        var sender = new Mock<IAppEmailSender>();
        sender.Setup(s => s.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, string, string, CancellationToken>((_, subject, body, _) =>
            {
                sentSubject = subject;
                sentBody = body;
            })
            .Returns(Task.CompletedTask);

        var service = new TicketNotificationService(
            _userManager,
            settings.Object,
            sender.Object,
            template,
            Mock.Of<ILogger<TicketNotificationService>>());

        var ticket = new Ticket
        {
            Id = 100,
            Title = "Cannot access VPN from home",
            Description = "desc",
            AuthorId = author.Id,
            AssignedTechnicianId = assignee.Id,
            Status = TicketStatus.InProgress,
            Priority = TicketPriority.High
        };

        await service.NotifyTicketUpdatedAsync(ticket, actor.Id, "assignment updated", "Assigned technician: tech.");

        Assert.NotNull(sentSubject);
        Assert.Contains("Cannot access VPN from home", sentSubject);

        Assert.NotNull(sentBody);
        Assert.Contains("Assigned to", sentBody);
        Assert.Contains("tech", sentBody);
    }
}
