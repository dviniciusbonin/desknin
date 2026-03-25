using DeskNin.Controllers;
using DeskNin.Data;
using DeskNin.Tests.TestHelpers;
using DeskNin.ViewModels;
using JetBrains.Annotations;
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
        var controller = new DashboardController(_context);
        controller.SetAnonymousUser();

        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<DashboardViewModel>(viewResult.Model);
    }
}
