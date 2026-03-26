using DeskNin.Controllers;
using DeskNin.Data;
using DeskNin.Models;
using DeskNin.Tests.TestHelpers;
using DeskNin.ViewModels;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DeskNin.Tests.Controllers;

[TestSubject(typeof(DashboardController))]
public class DashboardControllerTest : IDisposable
{
    private readonly ApplicationDbContext _context;

    public DashboardControllerTest()
    {
        (_context, _, _) = IdentityTestHelpers.CreateIdentityServices();
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Index_Returns_ViewResult_With_DashboardViewModel()
    {
        // Arrange: seed a few tickets + comments in the in-memory database.
        var alice = new IdentityUser { Id = "u1", UserName = "alice", Email = "alice@example.com" };
        var bob = new IdentityUser { Id = "u2", UserName = "bob", Email = "bob@example.com" };
        var carol = new IdentityUser { Id = "u3", UserName = "carol", Email = "carol@example.com" };
        _context.Users.AddRange(alice, bob, carol);

        var now = DateTime.UtcNow;

        // Open + High priority (no comments)
        _context.Tickets.Add(new Ticket
        {
            Id = 1,
            Title = "Open high priority",
            Description = "desc",
            Status = TicketStatus.Open,
            Priority = TicketPriority.High,
            AuthorId = alice.Id,
            CreatedAtUtc = now.AddMinutes(-30),
            UpdatedAtUtc = now.AddMinutes(-30)
        });

        // In progress + Low priority.
        _context.Tickets.Add(new Ticket
        {
            Id = 2,
            Title = "In progress low",
            Description = "desc",
            Status = TicketStatus.InProgress,
            Priority = TicketPriority.Low,
            AuthorId = bob.Id,
            CreatedAtUtc = now.AddMinutes(-120),
            UpdatedAtUtc = now.AddMinutes(-10)
        });

        // Resolved + Critical priority.
        _context.Tickets.Add(new Ticket
        {
            Id = 3,
            Title = "Resolved critical",
            Description = "desc",
            Status = TicketStatus.Resolved,
            Priority = TicketPriority.Critical,
            AuthorId = carol.Id,
            CreatedAtUtc = now.AddHours(-2),
            UpdatedAtUtc = now.AddMinutes(-5)
        });

        // Open + Medium priority with a comment.
        _context.Tickets.Add(new Ticket
        {
            Id = 4,
            Title = "Open with comment",
            Description = "desc",
            Status = TicketStatus.Open,
            Priority = TicketPriority.Medium,
            AuthorId = alice.Id,
            CreatedAtUtc = now.AddMinutes(-60),
            UpdatedAtUtc = now.AddMinutes(-2)
        });

        _context.TicketComments.Add(new TicketComment
        {
            Id = 1,
            TicketId = 4,
            AuthorId = bob.Id,
            Content = "comment",
            CreatedAtUtc = now.AddMinutes(-2)
        });

        await _context.SaveChangesAsync();

        var controller = new DashboardController(_context);
        controller.SetAnonymousUser();

        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<DashboardViewModel>(viewResult.Model);

        Assert.Equal(1, model.OpenCount);
        Assert.Equal(1, model.InProgressCount);
        Assert.Equal(1, model.ResolvedCount);
        Assert.Equal(2, model.HighPriorityCount); // High + Critical (tickets 1 and 3)

        Assert.NotNull(model.PriorityBreakdown);
        Assert.Equal(3, model.PriorityBreakdown.Count);

        var low = model.PriorityBreakdown.FirstOrDefault(b => b.Label == "Low");
        var medium = model.PriorityBreakdown.FirstOrDefault(b => b.Label == "Medium");
        var high = model.PriorityBreakdown.FirstOrDefault(b => b.Label == "High");
        Assert.NotNull(low);
        Assert.NotNull(medium);
        Assert.NotNull(high);
        Assert.Equal(1, low!.Count); // ticket 2
        Assert.Equal(1, medium!.Count); // ticket 4
        Assert.Equal(2, high!.Count); // tickets 1 and 3

        Assert.NotNull(model.RecentActivities);
        Assert.Equal(4, model.RecentActivities.Count); // seeded 4 tickets

        var mostRecent = model.RecentActivities[0];
        Assert.Equal(4, mostRecent.TicketId);
        Assert.Equal("bob", mostRecent.AuthorName); // author from latest comment
        Assert.Equal("Ticket updated", mostRecent.Label); // Open + comment => "Ticket updated"

        var resolvedActivity = model.RecentActivities.SingleOrDefault(a => a.TicketId == 3);
        Assert.NotNull(resolvedActivity);
        Assert.Equal("Ticket resolved", resolvedActivity!.Label);
    }
}
