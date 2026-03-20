using DeskNin.Controllers;
using DeskNin.Tests.TestHelpers;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace DeskNin.Tests.Controllers;

[TestSubject(typeof(DashboardController))]
public class DashboardControllerTest
{
    [Fact]
    public void Index_Returns_ViewResult()
    {
        var controller = new DashboardController();
        controller.SetAnonymousUser();

        var result = controller.Index();

        Assert.IsType<ViewResult>(result);
    }
}
