using DeskNin.Controllers;
using DeskNin.Data;
using DeskNin.Tests.TestHelpers;
using DeskNin.ViewModels;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

    [Fact]
    public void ChangePassword_Get_Returns_ViewResult()
    {
        var controller = new SettingsController(_userManager);
        controller.SetAnonymousUser();

        var result = controller.ChangePassword();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task ChangePassword_Post_With_Invalid_Model_Returns_View()
    {
        var controller = new SettingsController(_userManager);
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

        var controller = new SettingsController(_userManager);
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

        var controller = new SettingsController(_userManager);
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

        var controller = new SettingsController(_userManager);
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
