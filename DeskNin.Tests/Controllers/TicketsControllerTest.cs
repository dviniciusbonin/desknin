using DeskNin.Controllers;
using DeskNin.Data;
using DeskNin.Models;
using DeskNin.Services;
using DeskNin.Tests.TestHelpers;
using DeskNin.ViewModels;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace DeskNin.Tests.Controllers;

[TestSubject(typeof(TicketsController))]
public class TicketsControllerTest : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly Mock<ITicketNotificationService> _ticketNotificationService = new();

    public TicketsControllerTest()
    {
        (_context, _userManager, _) = IdentityTestHelpers.CreateIdentityServices();
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Index_Returns_ViewResult_With_TicketListViewModel()
    {
        var controller = new TicketsController(_context, _userManager, _ticketNotificationService.Object, Mock.Of<ILogger<TicketsController>>());
        controller.SetAnonymousUser();

        var result = await controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        Assert.IsType<TicketListViewModel>(view.Model);
    }

    [Fact]
    public async Task Create_Post_With_Valid_Form_Creates_Ticket_And_Redirects()
    {
        var user = new IdentityUser { UserName = "author", Email = "author@test.com" };
        await _userManager.CreateAsync(user, "Password123!");

        var controller = new TicketsController(_context, _userManager, _ticketNotificationService.Object, Mock.Of<ILogger<TicketsController>>());
        controller.SetUser(user);

        var form = new TicketFormViewModel
        {
            Title = "Cannot login",
            Description = "User reports invalid credentials loop.",
            Priority = TicketPriority.High
        };

        var result = await controller.Create(form);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
        Assert.NotNull(redirect.RouteValues);
        Assert.True(redirect.RouteValues!.ContainsKey("id"));

        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Title == "Cannot login");
        Assert.NotNull(ticket);
        Assert.Equal(TicketPriority.High, ticket!.Priority);
        Assert.Equal(TicketStatus.Open, ticket.Status);
        Assert.Equal(user.Id, ticket.AuthorId);
    }

    [Fact]
    public async Task Edit_Get_With_Invalid_Id_Returns_NotFound()
    {
        var controller = new TicketsController(_context, _userManager, _ticketNotificationService.Object, Mock.Of<ILogger<TicketsController>>());
        controller.SetAnonymousUser();

        var result = await controller.Edit(9999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateStatus_With_Valid_Id_Updates_Status()
    {
        var user = new IdentityUser { UserName = "author2", Email = "author2@test.com" };
        await _userManager.CreateAsync(user, "Password123!");
        var ticket = new Ticket
        {
            Title = "Email issue",
            Description = "Mailbox sync timeout",
            AuthorId = user.Id,
            Status = TicketStatus.Open,
            Priority = TicketPriority.Medium
        };
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        var controller = new TicketsController(_context, _userManager, _ticketNotificationService.Object, Mock.Of<ILogger<TicketsController>>());
        controller.SetUser(user);

        var result = await controller.UpdateStatus(ticket.Id, TicketStatus.Resolved);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);

        var updated = await _context.Tickets.FindAsync(ticket.Id);
        Assert.NotNull(updated);
        Assert.Equal(TicketStatus.Resolved, updated!.Status);
        _ticketNotificationService.Verify(n => n.NotifyTicketUpdatedAsync(
            It.IsAny<Ticket>(),
            user.Id,
            "status updated",
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddComment_With_Valid_Form_Creates_Comment()
    {
        var author = new IdentityUser { UserName = "author3", Email = "author3@test.com" };
        var commenter = new IdentityUser { UserName = "tech", Email = "tech@test.com" };
        await _userManager.CreateAsync(author, "Password123!");
        await _userManager.CreateAsync(commenter, "Password123!");

        var ticket = new Ticket
        {
            Title = "VPN down",
            Description = "Cannot connect to VPN",
            AuthorId = author.Id,
            Status = TicketStatus.Open,
            Priority = TicketPriority.Critical
        };
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        var controller = new TicketsController(_context, _userManager, _ticketNotificationService.Object, Mock.Of<ILogger<TicketsController>>());
        controller.SetUser(commenter);

        var form = new CommentFormViewModel
        {
            TicketId = ticket.Id,
            Content = "Investigating firewall and gateway logs."
        };

        var result = await controller.AddComment(form);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);

        var comment = await _context.TicketComments.FirstOrDefaultAsync(c => c.TicketId == ticket.Id);
        Assert.NotNull(comment);
        Assert.Equal(commenter.Id, comment!.AuthorId);
        Assert.Equal(form.Content, comment.Content);
        _ticketNotificationService.Verify(n => n.NotifyTicketUpdatedAsync(
            It.IsAny<Ticket>(),
            commenter.Id,
            "new comment",
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
