using DeskNin.Data;
using DeskNin.Services;
using DeskNin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeskNin.Controllers;

[Authorize(Roles = "Admin")]
public class TeamController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IPasswordGenerator _passwordGenerator;

    public TeamController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IPasswordGenerator passwordGenerator)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _passwordGenerator = passwordGenerator;
    }

    [HttpGet]
    public async Task<IActionResult> GeneratePassword()
    {
        var password = await _passwordGenerator.GenerateIdentityCompliantPasswordAsync();
        return Json(new { password });
    }

    public async Task<IActionResult> Index()
    {
        var users = await (
            from user in _context.Users
            join userRole in _context.Set<IdentityUserRole<string>>() on user.Id equals userRole.UserId into urJoin
            from userRole in urJoin.DefaultIfEmpty()
            join role in _context.Roles on userRole.RoleId equals role.Id into rJoin
            from role in rJoin.DefaultIfEmpty()
            select new UserViewModel
            {
                Id = user.Id,
                Name = user.UserName ?? user.Email ?? "-",
                Email = user.Email ?? "",
                Role = role != null ? role.Name ?? "User" : "User",
                TicketCount = _context.Tickets.Count(t => t.AuthorId == user.Id || t.AssignedTechnicianId == user.Id)
            }
        )
        .GroupBy(u => u.Id)
        .Select(g => g.First())
        .ToListAsync();

        return View(new UserListViewModel { Users = users });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(UserForm form)
    {
        var user = new IdentityUser
        {
            UserName = form.Username,
            Email = form.Email
        };
        var result = await _userManager.CreateAsync(user, form.Password!);
        if (!result.Succeeded)
        {
            var msg = string.Join(" ", result.Errors.Select(e => e.Description));
            TempData["AddMemberError"] = msg;
            return RedirectToAction("Index");
        }
        await _userManager.AddToRoleAsync(user, form.Role);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        var roles = await _userManager.GetRolesAsync(user);
        var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        ViewBag.Roles = allRoles;
        return View(new UserForm
        {
            Id = user.Id,
            Username = user.UserName ?? "",
            Email = user.Email ?? "",
            Role = roles.FirstOrDefault() ?? "User"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserForm form)
    {
        if (string.IsNullOrEmpty(form.Id)) return NotFound();
        var user = await _userManager.FindByIdAsync(form.Id);
        if (user == null) return NotFound();

        user.UserName = form.Username;
        user.Email = form.Email;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var e in updateResult.Errors)
                ModelState.AddModelError(string.Empty, e.Description);
            ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return View(form);
        }

        if (!string.IsNullOrEmpty(form.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, form.Password);
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, form.Role);

        return RedirectToAction("Index");
    }
}
