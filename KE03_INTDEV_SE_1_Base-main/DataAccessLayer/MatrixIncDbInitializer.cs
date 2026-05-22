using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public static class MatrixIncDbInitializer
    {
        public static void Initialize(MatrixIncDbContext context)
        {
            // If the database already has products, assume seeding already happened.
            if (context.Products.Any())
            {
                return;   // DB has been seeded
            }

            // TODO: Hier moet ik nog wat namen verzinnen die betrekking hebben op de matrix.
            // - Denk aan de m3 boutjes, moertjes en ringetjes.
            // - Denk aan namen van schepen
            // - Denk aan namen van vliegtuigen            
            var customers = new Customer[]
            {
                new Customer { Name = "Neo", Address = "123 Elm St" , Active=true},
                new Customer { Name = "Morpheus", Address = "456 Oak St", Active = true },
                new Customer { Name = "Trinity", Address = "789 Pine St", Active = true }
            };
            // Add customers if none exist
            if (!context.Customers.Any())
            {
                context.Customers.AddRange(customers);
                context.SaveChanges();  // Save changes before trying to use the customers
            }

            // Ensure we have customers from the database to link orders to
            var dbCustomers = context.Customers.ToArray();
            var orders = new Order[]
            {
                new Order { CustomerId = dbCustomers[0].Id, OrderDate = DateTime.Parse("2021-01-01")},
                new Order { CustomerId = dbCustomers[0].Id, OrderDate = DateTime.Parse("2021-02-01")},
                new Order { CustomerId = dbCustomers[1].Id, OrderDate = DateTime.Parse("2021-02-01")},
                new Order { CustomerId = dbCustomers[2].Id, OrderDate = DateTime.Parse("2021-03-01")}
            };
            if (!context.Orders.Any())
            {
                context.Orders.AddRange(orders);
                context.SaveChanges();
            }

            var products = new Product[]
            {
                new Product { Name = "Nebuchadnezzar", Type = "product", Description = "Het schip waarop Neo voor het eerst de echte wereld leert kennen", Price = 10000.00m, ImageUrl = "images/product/nebuchadnezzar.jpg"},
                new Product { Name = "Jack-in Chair", Type = "product", Description = "Stoel met een rugsteun en metalen armen waarin mensen zitten om ingeplugd te worden in de Matrix via een kabel in de nekpoort", Price = 500.50m, ImageUrl = "images/product/jackinchair.jpg"},
                new Product { Name = "EMP (Electro-Magnetic Pulse) Device", Type = "product", Description = "Wapentuig op de schepen van Zion", Price = 129.99m,  ImageUrl = "images/product/emp.jpg" },
                new Product { Name = "Blue Pill", Type = "product", Description = "De blauwe pil die Neo neemt in de Matrix", Price = 100.00m, ImageUrl="images/product/bluepill.jpg"},
                new Product { Name = "Tandwiel", Type = "onderdeel", Description = "Overdracht van rotatie in bijvoorbeeld de motor of luikmechanismen", Price = 10000.00m, ImageUrl = "images/product/nebuchadnezzar.jpg"},
                new Product { Name = "M5 Boutje", Type = "onderdeel", Description = "Bevestiging van panelen, buizen of interne modules.", Price = 10000.00m, ImageUrl = "images/product/nebuchadnezzar.jpg"},
                new Product { Name = "Hydraulische cilinder", Type = "onderdeel", Description = "Openen/sluiten van zware luchtsluizen of bewegende onderdelen.", Price = 10000.00m, ImageUrl = "images/product/nebuchadnezzar.jpg"},
                new Product { Name = "Koelvloeistofpomp", Type = "onderdeel", Description = "Koeling van de motor of elektronische systemen.", Price = 10000.00m, ImageUrl = "images/product/nebuchadnezzar.jpg"}
            };
            // Add seeded products to the context so they are persisted
            if (!context.Products.Any())
            {
                context.Products.AddRange(products);
                context.SaveChanges();
            }

            
        }
    }
}
