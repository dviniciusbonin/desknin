using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DeskNin.Data;

namespace DeskNin.Tests.TestHelpers;

public static class IdentityTestHelpers
{
    public static ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static UserManager<IdentityUser> CreateUserManager(ApplicationDbContext context)
    {
        return CreateIdentityServices().UserManager;
    }

    public static RoleManager<IdentityRole> CreateRoleManager(ApplicationDbContext context)
    {
        return CreateIdentityServices().RoleManager;
    }

    public static (ApplicationDbContext Context, UserManager<IdentityUser> UserManager, RoleManager<IdentityRole> RoleManager) CreateIdentityServices()
    {
        var databaseName = Guid.NewGuid().ToString();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDataProtection();
        services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(databaseName));
        services.AddIdentityCore<IdentityUser>(options => { options.Lockout.AllowedForNewUsers = false; })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        var provider = services.BuildServiceProvider();
        var context = provider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
        var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
        return (context, userManager, roleManager);
    }
}
