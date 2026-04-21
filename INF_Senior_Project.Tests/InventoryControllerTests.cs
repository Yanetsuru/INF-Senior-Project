using INF_Senior_Project.Controllers;
using INF_Senior_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InventoryControllerTests
{
    [Fact]
    public async Task Create_AddsProduct()
    {
        var db = TestDbFactory.Create();

        var controller = new InventoryController(db);
        TestHelper.SetupSession(controller);

        var product = new Product
        {
            Id = 2,
            Name = "Aspirin",
            Price = 5,
            Quantity = 10,
            Category = "drug",
            SupplierId = 2
        };

        await controller.Create(product);

        Assert.Single(db.Products);
    }

    [Fact]
    public async Task Edit_UpdatesProduct()
    {
        var db = TestDbFactory.Create();

        var old = new Product
        {
            Id = 1,
            Name = "Old",
            Price = 1,
            Quantity = 1,
            Category = "drug",
            SupplierId = 2
        };

        db.Products.Add(old);

        db.SaveChanges();

        var controller = new InventoryController(db);
        TestHelper.SetupSession(controller);

        var updated = new Product
        {
            Id = 1,
            Name = "New",
            Price = 10,
            Quantity = 5,
            Category = "drug",
            SupplierId = 2
        };

        await controller.Edit(1, updated);

        Assert.Equal("New", db.Products.First().Name);
    }

    [Fact]
    public async Task Delete_RemovesProduct()
    {
        var db = TestDbFactory.Create();

        db.Products.Add(new Product
        {
            Id = 3,
            Name = "New",
            Price = 10,
            Quantity = 5,
            Category = "drug",
            SupplierId = 2
        });

        db.SaveChanges();

        var controller = new InventoryController(db);
        TestHelper.SetupSession(controller);

        await controller.DeleteConfirmed(3);

        Assert.Empty(db.Products);
    }
}
