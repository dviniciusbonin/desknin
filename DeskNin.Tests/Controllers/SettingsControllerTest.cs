using System.Threading;
using DeskNin.Controllers;
using DeskNin.Data;
using DeskNin.Models;
using DeskNin.Tests.TestHelpers;
using DeskNin.ViewModels;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeskNin.Tests.Controllers;

[TestSubject(typeof(SettingsController))]
public class SettingsControllerTest : IDisposable
{
    private ApplicationDbContext _context = null!;
    private UserManager<IdentityUser> _userManager = null!;

    public SettingsControllerTest()
    {
        (_context, _userManager, _) = IdentityTestHelpers.CreateIdentityServices();
    }

    public void Dispose() => _context.Dispose();

    private SettingsController CreateController() => new(_context, _userManager);

    [Fact]
    public async Task Index_Get_Returns_View_With_Model()
    {
        var controller = CreateController();
        controller.SetAnonymousUser();

        var result = await controller.Index(CancellationToken.None);

        var view = Assert.IsType<ViewResult>(result);
        var vm = Assert.IsType<SettingsIndexViewModel>(view.Model);
        Assert.False(vm.CanManageSystemEmail);
    }

    [Fact]
    public async Task SaveEmailNotifications_Post_As_Admin_Updates_Database()
    {
        var user = new IdentityUser { UserName = "adminsettings", Email = "adminsettings@example.com" };
        await _userManager.CreateAsync(user, "Password123!");

        var controller = CreateController();
        controller.SetUser(user, "Admin");

        var result = await controller.SaveEmailNotifications(true, CancellationToken.None);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(SettingsController.Index), redirect.ActionName);

        var stored = await _context.Settings.AsNoTracking()
            .SingleAsync(s => s.Key == SettingKeys.EmailNotificationsEnabled);
        Assert.True(SettingValue.AsBool(stored.Value));
    }

    [Fact]
    public void ChangePassword_Get_Returns_ViewResult()
    {
        var controller = CreateController();
        controller.SetAnonymousUser();

        var result = controller.ChangePassword();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task ChangePassword_Post_With_Invalid_Model_Returns_View()
    {
        var controller = CreateController();
        controller.SetAnonymousUser();
        controller.ModelState.AddModelError("CurrentPassword", "Required");

        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "old",
            NewPassword = "NewPass123!",
            ConfirmPassword = "NewPass123!"
        };

        var result = await controller.ChangePassword(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(model, viewResult.Model);
    }

    [Fact]
    public async Task ChangePassword_Post_With_Valid_Model_Changes_Password_And_Redirects()
    {
        var user = new IdentityUser { UserName = "settingsuser", Email = "settings@example.com" };
        await _userManager.CreateAsync(user, "OldPassword123!");

        var controller = CreateController();
        controller.SetUser(user);

        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
        };

        var result = await controller.ChangePassword(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(SettingsController.ChangePassword), redirect.ActionName);
        Assert.True(controller.TempData["PasswordChanged"] as bool? == true);

        var signInResult = await _userManager.CheckPasswordAsync(user, "NewPassword123!");
        Assert.True(signInResult);
    }

    [Fact]
    public async Task ChangePassword_Post_With_Wrong_Current_Password_Returns_View_With_Error()
    {
        var user = new IdentityUser { UserName = "wronguser", Email = "wrong@example.com" };
        await _userManager.CreateAsync(user, "CorrectPassword123!");

        var controller = CreateController();
        controller.SetUser(user);

        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "WrongPassword!",
            NewPassword = "NewPass123!",
            ConfirmPassword = "NewPass123!"
        };

        var result = await controller.ChangePassword(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(model, viewResult.Model);
        Assert.False(controller.ModelState.IsValid);
    }

    [Fact]
    public async Task ChangePassword_Post_When_User_Not_Found_Returns_NotFound()
    {
        var user = new IdentityUser { UserName = "ghost", Email = "ghost@example.com", Id = "ghost-id" };

        var controller = CreateController();
        controller.SetUser(user);

        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "x",
            NewPassword = "NewPass123!",
            ConfirmPassword = "NewPass123!"
        };

        var result = await controller.ChangePassword(model);

        Assert.IsType<NotFoundResult>(result);
    }
}
