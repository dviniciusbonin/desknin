using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace DeskNin.Tests.TestHelpers;

public static class ControllerTestHelpers
{
    private static readonly IUrlHelper MockUrlHelper;
    private static readonly IServiceProvider ServiceProvider;

    static ControllerTestHelpers()
    {
        var urlHelper = new Mock<IUrlHelper>();
        urlHelper.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns("/");
        MockUrlHelper = urlHelper.Object;

        var tempDataProvider = new Mock<ITempDataProvider>();
        var tempDataFactory = new Mock<ITempDataDictionaryFactory>();
        tempDataFactory.Setup(f => f.GetTempData(It.IsAny<HttpContext>()))
            .Returns((HttpContext ctx) => new TempDataDictionary(ctx, tempDataProvider.Object));

        var services = new ServiceCollection();
        services.AddMvcCore();
        services.AddSingleton(tempDataFactory.Object);
        ServiceProvider = services.BuildServiceProvider();
    }

    private static HttpContext CreateHttpContext(ClaimsPrincipal? user = null)
    {
        var httpContext = new DefaultHttpContext { User = user ?? new ClaimsPrincipal() };
        httpContext.RequestServices = ServiceProvider;
        return httpContext;
    }

    public static void SetUser(this Controller controller, IdentityUser user, params string[] roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? ""),
            new(ClaimTypes.Email, user.Email ?? "")
        };
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext { HttpContext = CreateHttpContext(principal) };
        controller.Url = MockUrlHelper;
    }

    public static void SetAnonymousUser(this Controller controller)
    {
        controller.ControllerContext = new ControllerContext { HttpContext = CreateHttpContext() };
        controller.Url = MockUrlHelper;
    }
}
