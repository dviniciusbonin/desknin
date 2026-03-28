using DeskNin.Data;
using DeskNin.Models;
using DeskNin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeskNin.Controllers;

[Authorize]
public class SettingsController(ApplicationDbContext context, UserManager<IdentityUser> userManager) : Controller
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<IdentityUser> _userManager = userManager;

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var row = await _context.Settings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == SettingKeys.EmailNotificationsEnabled, cancellationToken);

        return View(new SettingsIndexViewModel
        {
            EmailNotificationsEnabled = SettingValue.AsBool(row?.Value),
            CanManageSystemEmail = User.IsInRole("Admin")
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SaveEmailNotifications(bool emailNotificationsEnabled, CancellationToken cancellationToken)
    {
        var row = await _context.Settings
            .FirstOrDefaultAsync(s => s.Key == SettingKeys.EmailNotificationsEnabled, cancellationToken);

        if (row == null)
        {
            _context.Settings.Add(new Setting
            {
                Key = SettingKeys.EmailNotificationsEnabled,
                Value = SettingValue.FromBool(emailNotificationsEnabled)
            });
        }
        else
        {
            row.Value = SettingValue.FromBool(emailNotificationsEnabled);
        }

        await _context.SaveChangesAsync(cancellationToken);
        TempData["SettingsSaved"] = true;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword!, model.NewPassword!);
        if (!result.Succeeded)
        {
            foreach (var e in result.Errors)
                ModelState.AddModelError(string.Empty, e.Description);
            return View(model);
        }

        TempData["PasswordChanged"] = true;
        return RedirectToAction(nameof(ChangePassword));
    }
}
