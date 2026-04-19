using INF_Senior_Project.Controllers;
using INF_Senior_Project.Models;
using INF_Senior_Project.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using INF_Senior_Project.Data;
using System.Security.Cryptography;

public class PharmacistControllerTests
{
    [Fact]
    public void Dashboard_ReturnsView()
    {
        var db = TestDbFactory.Create();

        var controller = new PharmacistController(db);
        TestHelper.SetupSession(controller);

        controller.HttpContext.Session.SetString("UserRole", "Pharmacist");

        var result = controller.Dashboard();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task CreateOrder_ReducesStock()
    {
        var db = TestDbFactory.Create();

        db.Products.Add(new Product
        {
            Id = 1,
            Name = "Paracetamol",
            Price = 5,
            Quantity = 20
        });

        db.SaveChanges();

        var controller = new PharmacistController(db);
        TestHelper.SetupSession(controller);

        controller.HttpContext.Session.SetInt32("UserId", 1);

        var model = new CreateOrderViewModel
        {
            Items = new List<OrderItemInputModel>
            {
                new OrderItemInputModel
                {
                    ProductId = 1,
                    Quantity = 5
                }
            }
        };

        await controller.CreateOrder(model);

        Assert.Equal(15, db.Products.First().Quantity);
    }

    [Fact]
    public async Task CreatePrescription_AddsPrescription()
    {
        var db = TestDbFactory.Create();

        db.Products.Add(new Product
        {
            Id = 1,
            Name = "Drug"
        });

        db.SaveChanges();

        var controller = new PharmacistController(db);
        TestHelper.SetupSession(controller);

        var model = new CreatePrescriptionViewModel
        {
            PatientName = "John",
            DoctorName = "Dr Smith",
            DateIssued = DateTime.Today,
            Notes = "Take daily",

            Items = new List<PrescriptionItemInput>
        {
            new PrescriptionItemInput
            {
                ProductId = 1,
                Quantity = 1
            }
        }
        };

        await controller.CreatePrescription(model);

        Assert.Single(db.Prescriptions);
    }

    [Fact]
    public async Task FulfillPrescription_ChangesStatus()
    {
        var db = TestDbFactory.Create();

        var product = new Product
        {
            Id = 1,
            Name = "Drug",
            Price = 10,
            Quantity = 20
        };

        db.Products.Add(product);

        var prescription = new Prescription
        {
            Id = 1,
            PatientName = "John",
            IsFulfilled = false
        };

        db.Prescriptions.Add(prescription);
        db.SaveChanges();

        db.PrescriptionItems.Add(new PrescriptionItem
        {
            PrescriptionId = 1,
            ProductId = 1,
            Quantity = 2
        });

        db.SaveChanges();

        var controller = new PharmacistController(db);
        TestHelper.SetupSession(controller);
        controller.HttpContext.Session.SetInt32("UserId", 1);

        await controller.FulfillPrescription(1);

        Assert.True(db.Prescriptions.First().IsFulfilled);
        Assert.Equal(18, db.Products.First().Quantity);
    }
}