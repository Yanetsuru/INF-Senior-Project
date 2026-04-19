using INF_Senior_Project.Controllers;
using INF_Senior_Project.Data;
using INF_Senior_Project.Models;
using INF_Senior_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;

public class AccountControllerTests
{
    [Fact]
    public async Task Register_ValidUser_Redirects()
    {
        var db = TestDbFactory.Create();
        var controller = new AccountController(db);
        TestHelper.SetupSession(controller);

        var model = new RegisterViewModel
        {
            Username = "John",
            Email = "john@test.com",
            Password = "123456",
            Role = "Admin"
        };

        var result = await controller.Register(model);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Single(db.Users);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsView()
    {
        var db = TestDbFactory.Create();

        db.Users.Add(new User
        {
            Username = "Old",
            Email = "john@test.com",
            PasswordHash = "x"
        });

        db.SaveChanges();

        var controller = new AccountController(db);
        TestHelper.SetupSession(controller);

        var model = new RegisterViewModel
        {
            Username = "New",
            Email = "john@test.com",
            Password = "123"
        };

        var result = await controller.Register(model);

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Login_Admin_RedirectsDashboard()
    {
        var db = TestDbFactory.Create();

        db.Users.Add(new User
        {
            Username = "Admin",
            Email = "admin@test.com",
            PasswordHash = TestHelper.Hash("123"),
            Role = "Admin",
            IsActive = true
        });

        db.SaveChanges();

        var controller = new AccountController(db);
        TestHelper.SetupSession(controller);

        var result = controller.Login(new LoginViewModel
        {
            Email = "admin@test.com",
            Password = "123"
        });

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("AdminDashboard", redirect.ActionName);
    }

    [Fact]
    public void Login_WrongPassword_ReturnsView()
    {
        var db = TestDbFactory.Create();

        db.Users.Add(new User
        {
            Username = "Admin",
            Email = "admin@test.com",
            PasswordHash = TestHelper.Hash("123"),
            Role = "Admin",
            IsActive = true
        });

        db.SaveChanges();

        var controller = new AccountController(db);
        TestHelper.SetupSession(controller);

        var result = controller.Login(new LoginViewModel
        {
            Email = "admin@test.com",
            Password = "wrong"
        });

        Assert.IsType<ViewResult>(result);
    }
}