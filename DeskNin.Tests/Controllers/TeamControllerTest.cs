using DeskNin.Controllers;
using DeskNin.Data;
using DeskNin.Tests.TestHelpers;
using DeskNin.ViewModels;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DeskNin.Tests.Controllers;

[TestSubject(typeof(TeamController))]
public class TeamControllerTest : IDisposable
{
    private ApplicationDbContext _context = null!;
    private UserManager<IdentityUser> _userManager = null!;
    private RoleManager<IdentityRole> _roleManager = null!;

    public TeamControllerTest()
    {
        (_context, _userManager, _roleManager) = IdentityTestHelpers.CreateIdentityServices();
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Index_Returns_ViewResult_With_Users()
    {
        var controller = new TeamController(_context, _userManager, _roleManager);
        controller.SetAnonymousUser();

        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UserListViewModel>(viewResult.Model);
        Assert.NotNull(model.Users);
    }

    [Fact]
    public async Task Index_Post_With_Valid_Form_Creates_User_And_Redirects()
    {
        var controller = new TeamController(_context, _userManager, _roleManager);
        controller.SetAnonymousUser();

        var form = new UserForm
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            Role = "User"
        };

        var result = await controller.Index(form);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        var user = await _userManager.FindByEmailAsync("test@example.com");
        Assert.NotNull(user);
        Assert.Equal("testuser", user.UserName);
    }

    [Fact]
    public async Task Index_Post_With_Invalid_Password_Redirects_With_Error()
    {
        var controller = new TeamController(_context, _userManager, _roleManager);
        controller.SetAnonymousUser();

        var form = new UserForm
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "1",
            ConfirmPassword = "1",
            Role = "User"
        };

        var result = await controller.Index(form);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.True(controller.TempData.ContainsKey("AddMemberError"));
    }

    [Fact]
    public async Task Edit_Get_With_Valid_Id_Returns_View_With_Form()
    {
        var user = new IdentityUser { UserName = "edituser", Email = "edit@example.com" };
        await _userManager.CreateAsync(user, "Password123!");
        await _userManager.AddToRoleAsync(user, "User");

        var controller = new TeamController(_context, _userManager, _roleManager);
        controller.SetAnonymousUser();

        var result = await controller.Edit(user.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UserForm>(viewResult.Model);
        Assert.Equal(user.Id, model.Id);
        Assert.Equal("edituser", model.Username);
        Assert.Equal("edit@example.com", model.Email);
        Assert.Equal("User", model.Role);
    }

    [Fact]
    public async Task Edit_Get_With_Invalid_Id_Returns_NotFound()
    {
        var controller = new TeamController(_context, _userManager, _roleManager);
        controller.SetAnonymousUser();

        var result = await controller.Edit("non-existent-id");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_With_Valid_Form_Updates_User_And_Redirects()
    {
        var user = new IdentityUser { UserName = "original", Email = "original@example.com" };
        await _userManager.CreateAsync(user, "Password123!");
        await _userManager.AddToRoleAsync(user, "User");

        var controller = new TeamController(_context, _userManager, _roleManager);
        controller.SetAnonymousUser();

        var form = new UserForm
        {
            Id = user.Id,
            Username = "updated",
            Email = "updated@example.com",
            Role = "Technical"
        };

        var result = await controller.Edit(form);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        var updated = await _userManager.FindByIdAsync(user.Id);
        Assert.NotNull(updated);
        Assert.Equal("updated", updated!.UserName);
        Assert.Equal("updated@example.com", updated.Email);

        var roles = await _userManager.GetRolesAsync(updated);
        Assert.Contains("Technical", roles);
    }

    [Fact]
    public async Task Edit_Post_With_Invalid_Id_Returns_NotFound()
    {
        var controller = new TeamController(_context, _userManager, _roleManager);
        controller.SetAnonymousUser();

        var form = new UserForm
        {
            Id = "non-existent",
            Username = "x",
            Email = "x@x.com",
            Role = "User"
        };

        var result = await controller.Edit(form);

        Assert.IsType<NotFoundResult>(result);
    }
}
