using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
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
        var userStore = new Microsoft.AspNetCore.Identity.EntityFrameworkCore.UserStore<IdentityUser>(context);
        var options = new Mock<IOptions<IdentityOptions>>();
        options.Setup(o => o.Value).Returns(new IdentityOptions { Lockout = { AllowedForNewUsers = false } });

        var userValidators = new List<IUserValidator<IdentityUser>> { new UserValidator<IdentityUser>() };
        var pwdValidators = new List<IPasswordValidator<IdentityUser>> { new PasswordValidator<IdentityUser>() };

        var userManager = new UserManager<IdentityUser>(
            userStore,
            options.Object,
            new PasswordHasher<IdentityUser>(),
            userValidators,
            pwdValidators,
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            new ServiceCollection().BuildServiceProvider(),
            Mock.Of<ILogger<UserManager<IdentityUser>>>());

        return userManager;
    }

    public static RoleManager<IdentityRole> CreateRoleManager(ApplicationDbContext context)
    {
        var roleStore = new Microsoft.AspNetCore.Identity.EntityFrameworkCore.RoleStore<IdentityRole>(context);
        var roleValidators = new List<IRoleValidator<IdentityRole>> { new RoleValidator<IdentityRole>() };

        var roleManager = new RoleManager<IdentityRole>(
            roleStore,
            roleValidators,
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            Mock.Of<ILogger<RoleManager<IdentityRole>>>());

        return roleManager;
    }

    public static (ApplicationDbContext Context, UserManager<IdentityUser> UserManager, RoleManager<IdentityRole> RoleManager) CreateIdentityServices()
    {
        var context = CreateInMemoryContext();
        var userManager = CreateUserManager(context);
        var roleManager = CreateRoleManager(context);
        return (context, userManager, roleManager);
    }
}
