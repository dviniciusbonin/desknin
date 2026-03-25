using DeskNin.Data;
using DeskNin.Models;
using DeskNin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeskNin.Controllers;

[Authorize]
public class DashboardController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IActionResult> Index()
    {
        var openCount = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Open);
        var inProgressCount = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.InProgress);
        var resolvedCount = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Resolved);

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
            RecentTickets = recentTickets
        });
    }
}
