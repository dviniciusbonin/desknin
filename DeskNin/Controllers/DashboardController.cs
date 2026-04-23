using DeskNin.Data;
using DeskNin.Models;
using DeskNin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeskNin.Controllers;

[Authorize(Roles = "Admin,Technical")]
public class DashboardController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IActionResult> Index()
    {
        var openCount = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Open);
        var inProgressCount = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.InProgress);
        var resolvedCount = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Resolved);

        var highPriorityCount = await _context.Tickets.CountAsync(t =>
            t.Priority == TicketPriority.High || t.Priority == TicketPriority.Critical);

        var lowPriorityCount = await _context.Tickets.CountAsync(t => t.Priority == TicketPriority.Low);
        var mediumPriorityCount = await _context.Tickets.CountAsync(t => t.Priority == TicketPriority.Medium);
        var highPriorityBucketCount = await _context.Tickets.CountAsync(t =>
            t.Priority == TicketPriority.High || t.Priority == TicketPriority.Critical);

        var recentActivityTickets = await _context.Tickets
            .AsNoTracking()
            .Include(t => t.Author)
            .Include(t => t.Comments)
            .ThenInclude(c => c.Author)
            .OrderByDescending(t => t.UpdatedAtUtc)
            .Take(10)
            .ToListAsync();

        var recentActivities = recentActivityTickets
            .Select(t =>
            {
                var latestComment = t.Comments
                    .OrderByDescending(c => c.CreatedAtUtc)
                    .FirstOrDefault();

                var hasComment = latestComment is not null;

                var label = t.Status switch
                {
                    TicketStatus.Open => hasComment ? "Ticket updated" : "New ticket created",
                    TicketStatus.InProgress => "Ticket updated",
                    TicketStatus.Resolved => "Ticket resolved",
                    TicketStatus.Closed => "Ticket closed",
                    _ => "Ticket updated"
                };

                var authorName = hasComment
                    ? (latestComment!.Author.UserName ?? latestComment.Author.Email ?? "-")
                    : (t.Author.UserName ?? t.Author.Email ?? "-");

                var eventTimeUtc = hasComment
                    ? latestComment!.CreatedAtUtc
                    : t.Status == TicketStatus.Open ? t.CreatedAtUtc : t.UpdatedAtUtc;

                return new DashboardRecentActivityViewModel
                {
                    AuthorName = authorName,
                    Label = label,
                    TicketId = t.Id,
                    EventTimeUtc = eventTimeUtc
                };
            })
            .OrderByDescending(a => a.EventTimeUtc)
            .Take(5)
            .ToList();

        var recentTickets = await _context.Tickets
            .AsNoTracking()
            .Include(t => t.Author)
            .Include(t => t.AssignedTechnician)
            .Include(t => t.Comments)
            .OrderByDescending(t => t.UpdatedAtUtc)
            .Take(5)
            .Select(t => new TicketItemViewModel
            {
                Id = t.Id,
                Title = t.Title,
                DescriptionPreview = t.Description.Length > 120 ? t.Description.Substring(0, 120) + "..." : t.Description,
                Status = t.Status,
                Priority = t.Priority,
                AuthorName = t.Author.UserName ?? t.Author.Email ?? "-",
                AssignedTechnicianId = t.AssignedTechnicianId,
                AssignedTechnicianName = t.AssignedTechnician != null ? t.AssignedTechnician.UserName ?? t.AssignedTechnician.Email : null,
                CreatedAtUtc = t.CreatedAtUtc,
                UpdatedAtUtc = t.UpdatedAtUtc,
                CommentCount = t.Comments.Count
            })
            .ToListAsync();

        return View(new DashboardViewModel
        {
            OpenCount = openCount,
            InProgressCount = inProgressCount,
            ResolvedCount = resolvedCount,
            HighPriorityCount = highPriorityCount,
            PriorityBreakdown =
            [
                new DashboardPriorityBucketViewModel { Label = "Low", Count = lowPriorityCount },
                new DashboardPriorityBucketViewModel { Label = "Medium", Count = mediumPriorityCount },
                new DashboardPriorityBucketViewModel { Label = "High", Count = highPriorityBucketCount }
            ],
            RecentActivities = recentActivities,
            RecentTickets = recentTickets
        });
    }
}
