using System.Security.Claims;
using DeskNin.Data;
using DeskNin.Models;
using DeskNin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeskNin.Controllers;

[Authorize]
public class TicketsController(ApplicationDbContext context, UserManager<IdentityUser> userManager) : Controller
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<IdentityUser> _userManager = userManager;

    [Authorize(Roles = "Admin,Technical")]
    public async Task<IActionResult> Index(TicketStatus? status = null, bool assignedToMe = false)
    {
        if (!Request.Query.ContainsKey("status"))
            status = TicketStatus.Open;

        var userId = GetCurrentUserId();

        var query = _context.Tickets
            .AsNoTracking()
            .Include(t => t.Author)
            .Include(t => t.AssignedTechnician)
            .Include(t => t.Comments)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (assignedToMe)
        {
            if (string.IsNullOrEmpty(userId))
                return Forbid();

            query = query.Where(t => t.AssignedTechnicianId == userId);
        }

        query = query
            .OrderByDescending(t => t.Priority)
            .ThenByDescending(t => t.CreatedAtUtc);

        var tickets = await query
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

        return View(new TicketListViewModel { Tickets = tickets, SelectedStatus = status, AssignedToMe = assignedToMe });
    }

    [HttpGet]
    public async Task<IActionResult> MyTickets(TicketStatus? status = null)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
            return Forbid();

        var query = _context.Tickets
            .AsNoTracking()
            .Include(t => t.Author)
            .Include(t => t.AssignedTechnician)
            .Include(t => t.Comments)
            .Where(t => t.AuthorId == userId);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        var tickets = await query
            .OrderByDescending(t => t.Priority)
            .ThenByDescending(t => t.CreatedAtUtc)
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

        return View("MyTickets", new TicketListViewModel
        {
            Tickets = tickets,
            SelectedStatus = status,
            AssignedToMe = false
        });
    }

    public IActionResult Create() => View(new TicketFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TicketFormViewModel form)
    {
        if (!ModelState.IsValid)
            return View(form);

        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
            return Forbid();

        var now = DateTime.UtcNow;
        var ticket = new Ticket
        {
            Title = form.Title.Trim(),
            Description = form.Description.Trim(),
            Priority = form.Priority,
            Status = TicketStatus.Open,
            AuthorId = userId,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = ticket.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var ticket = await _context.Tickets
            .AsNoTracking()
            .Include(t => t.Author)
            .Include(t => t.AssignedTechnician)
            .Include(t => t.Comments)
            .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null)
            return NotFound();

        var vm = new TicketDetailsViewModel
        {
            Ticket = new TicketItemViewModel
            {
                Id = ticket.Id,
                Title = ticket.Title,
                DescriptionPreview = ticket.Description.Length > 120 ? $"{ticket.Description[..120]}..." : ticket.Description,
                Status = ticket.Status,
                Priority = ticket.Priority,
                AuthorName = ticket.Author.UserName ?? ticket.Author.Email ?? "-",
                AssignedTechnicianId = ticket.AssignedTechnicianId,
                AssignedTechnicianName = ticket.AssignedTechnician?.UserName ?? ticket.AssignedTechnician?.Email,
                CreatedAtUtc = ticket.CreatedAtUtc,
                UpdatedAtUtc = ticket.UpdatedAtUtc,
                CommentCount = ticket.Comments.Count
            },
            Description = ticket.Description,
            Comments = ticket.Comments
                .OrderBy(c => c.CreatedAtUtc)
                .Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    AuthorName = c.Author.UserName ?? c.Author.Email ?? "-",
                    CreatedAtUtc = c.CreatedAtUtc
                })
                .ToList(),
            NewComment = new CommentFormViewModel { TicketId = ticket.Id, Content = string.Empty },
            CanEditTicket = CanEditTicket(ticket),
            CanManageWorkflow = CanManageWorkflow() && ticket.CanManageWorkflow
        };

        if (vm.CanManageWorkflow)
            vm.Technicians = await LoadTechniciansAsync();

        return View(vm);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
            return NotFound();
        if (!CanEditTicket(ticket))
            return Forbid();

        return View(new TicketFormViewModel
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Priority = ticket.Priority
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TicketFormViewModel form)
    {
        if (!form.Id.HasValue)
            return NotFound();

        var ticket = await _context.Tickets.FindAsync(form.Id.Value);
        if (ticket == null)
            return NotFound();
        if (!CanEditTicket(ticket))
            return Forbid();
        if (!ModelState.IsValid)
            return View(form);

        ticket.Title = form.Title.Trim();
        ticket.Description = form.Description.Trim();
        ticket.Priority = form.Priority;
        ticket.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = ticket.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Technical")]
    public async Task<IActionResult> Assign(int id, string? assignedTechnicianId)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
            return NotFound();

        if (!ticket.CanManageWorkflow)
            return RedirectToAction(nameof(Details), new { id });

        if (!string.IsNullOrWhiteSpace(assignedTechnicianId))
        {
            var exists = await _context.Users.AnyAsync(u => u.Id == assignedTechnicianId);
            if (!exists)
                return NotFound();
            ticket.AssignedTechnicianId = assignedTechnicianId;
        }
        else
        {
            ticket.AssignedTechnicianId = null;
        }

        ticket.UpdatedAtUtc = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Technical")]
    public async Task<IActionResult> UpdateStatus(int id, TicketStatus status)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
            return NotFound();

        if (!ticket.CanChangeStatusTo(status))
            return RedirectToAction(nameof(Details), new { id });

        ticket.ChangeStatus(status, DateTime.UtcNow);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Technical")]
    public async Task<IActionResult> UpdatePriority(int id, TicketPriority priority)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
            return NotFound();

        if (!ticket.CanManageWorkflow)
            return RedirectToAction(nameof(Details), new { id });

        ticket.Priority = priority;
        ticket.UpdatedAtUtc = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(CommentFormViewModel form)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Details), new { id = form.TicketId });

        var ticket = await _context.Tickets.FindAsync(form.TicketId);
        if (ticket == null)
            return NotFound();

        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
            return Forbid();

        _context.TicketComments.Add(new TicketComment
        {
            TicketId = form.TicketId,
            AuthorId = userId,
            Content = form.Content.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        });
        ticket.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = form.TicketId });
    }

    private string? GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

    private bool CanManageWorkflow() => User.IsInRole("Admin") || User.IsInRole("Technical");

    private bool CanEditTicket(Ticket ticket)
    {
        var userId = GetCurrentUserId();
        return !string.IsNullOrEmpty(userId) &&
               ticket.CanEditDetails &&
               (ticket.AuthorId == userId || User.IsInRole("Admin") || User.IsInRole("Technical"));
    }

    private async Task<List<TechnicianOptionViewModel>> LoadTechniciansAsync()
    {
        var technicalUsers = await _userManager.GetUsersInRoleAsync("Technical");
        var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");

        return technicalUsers
            .Concat(adminUsers)
            .GroupBy(u => u.Id)
            .Select(g => g.First())
            .OrderBy(u => u.UserName ?? u.Email)
            .Select(u => new TechnicianOptionViewModel
            {
                UserId = u.Id,
                Name = u.UserName ?? u.Email ?? u.Id
            })
            .ToList();
    }
}
