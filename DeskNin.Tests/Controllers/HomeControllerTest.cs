using DeskNin.Controllers;
using DeskNin.Data;
using DeskNin.Tests.TestHelpers;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DeskNin.Tests.Controllers;

[TestSubject(typeof(HomeController))]
public class HomeControllerTest : IDisposable
{
    private ApplicationDbContext _context = null!;
    private UserManager<IdentityUser> _userManager = null!;

    public HomeControllerTest()
    {
        (_context, _userManager, _) = IdentityTestHelpers.CreateIdentityServices();
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public void Index_When_Anonymous_Returns_ViewResult()
    {
        var controller = new HomeController();
        controller.SetAnonymousUser();

        var result = controller.Index();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Index_When_Authenticated_Redirects_To_Dashboard()
    {
        var user = new IdentityUser { UserName = "homeuser", Email = "home@example.com" };
        await _userManager.CreateAsync(user, "Password123!");

        var controller = new HomeController();
        controller.SetUser(user);

        var result = controller.Index();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Dashboard", redirect.ControllerName);
    }

    [Fact]
    public void Error_Returns_ViewResult_With_ErrorViewModel()
    {
        var controller = new HomeController();
        controller.SetAnonymousUser();

        var result = controller.Error();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult.Model);
        Assert.NotNull((viewResult.Model as Models.ErrorViewModel)?.RequestId);
    }
}
