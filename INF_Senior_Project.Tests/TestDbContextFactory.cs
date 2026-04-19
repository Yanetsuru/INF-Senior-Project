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


public static class TestDbFactory
{
    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}