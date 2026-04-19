using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public static class TestHelper
{
    public static string Hash(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    public static void SetupSession(Controller controller)
    {
        controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
        };

        controller.HttpContext.Session = new TestSession();
    }
}
