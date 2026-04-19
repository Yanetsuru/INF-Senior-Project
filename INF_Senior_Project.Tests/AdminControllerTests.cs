using INF_Senior_Project.Controllers;
using INF_Senior_Project.Models;
using Microsoft.AspNetCore.Http;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using INF_Senior_Project.Controllers;
using INF_Senior_Project.Data;
using INF_Senior_Project.Models;
using INF_Senior_Project.ViewModels;
using System.Text;
using System.Security.Cryptography;

public class AdminControllerTests
{
    [Fact]
    public void Users_ReturnsView()
    {
        var db = TestDbFactory.Create();

        db.Users.Add(new User { Username = "A" });
        db.SaveChanges();

        var controller = new AdminController(db);
        TestHelper.SetupSession(controller);
        controller.HttpContext.Session.SetString("UserRole", "Admin");

        var result = controller.Users();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task ToggleUser_ChangesStatus()
    {
        var db = TestDbFactory.Create();

        db.Users.Add(new User
        {
            Id = 1,
            Username = "User",
            IsActive = true
        });

        db.SaveChanges();

        var controller = new AdminController(db);

        await controller.ToggleUser(1);

        Assert.False(db.Users.First().IsActive);
    }

    [Fact]
    public void Dashboard_ReturnsView()
    {
        var db = TestDbFactory.Create();

        db.Users.Add(new User { Username = "A", IsActive = true });
        db.SaveChanges();

        var controller = new AdminController(db);
        TestHelper.SetupSession(controller);
        controller.HttpContext.Session.SetString("UserRole", "Admin");

        var result = controller.AdminDashboard();

        Assert.IsType<ViewResult>(result);
    }
}