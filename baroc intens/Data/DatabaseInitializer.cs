using System;
using System.Threading.Tasks;
using baroc_intens.Models;
using baroc_intens.Services;
using Microsoft.EntityFrameworkCore;

namespace baroc_intens.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync()
    {
        await using var database = new BarrocDbContext();
        await database.Database.EnsureCreatedAsync();

        if (await database.Users.AnyAsync())
        {
            return;
        }

        await using var transaction = await database.Database.BeginTransactionAsync();

        var salesPassword = PasswordHasher.CreateHash("Sales123!");
        var maintenancePassword = PasswordHasher.CreateHash("Maintenance123!");
        var salesUser = new AppUser
        {
            Username = "sales",
            DisplayName = "Sanne de Vries",
            PasswordSalt = salesPassword.Salt,
            PasswordHash = salesPassword.Hash,
            Role = UserRole.Sales
        };
        var maintenanceUser = new AppUser
        {
            Username = "maintenance",
            DisplayName = "Pieter Bakker",
            PasswordSalt = maintenancePassword.Salt,
            PasswordHash = maintenancePassword.Hash,
            Role = UserRole.Maintenance
        };

        database.Users.AddRange(salesUser, maintenanceUser);
        await database.SaveChangesAsync();

        var customers = new[]
        {
            new Customer { Name = "Hotel De L'Europe", ContactName = "Mila Jansen" },
            new Customer { Name = "Gemeente Utrecht", ContactName = "Thomas Visser" },
            new Customer { Name = "Bakkerij Van Dijk", ContactName = "Noor van Dijk" }
        };
        database.Customers.AddRange(customers);
        await database.SaveChangesAsync();

        var now = DateTime.Now;
        database.IncidentRequests.AddRange(
            new IncidentRequest
            {
                CustomerId = customers[0].Id,
                CreatedByUserId = salesUser.Id,
                Title = "Koffiemachine geeft geen water",
                Description = "De machine start op, maar er komt geen water uit de uitloop.",
                Location = "Nieuwe Doelenstraat 2, Amsterdam",
                Status = IncidentStatus.Open,
                CreatedAt = now.AddDays(-2)
            },
            new IncidentRequest
            {
                CustomerId = customers[1].Id,
                CreatedByUserId = salesUser.Id,
                Title = "Onregelmatige druk op de espressomachine",
                Description = "De drukmeter schommelt tijdens het zetten van koffie.",
                Location = "Stadhuisbrug 1, Utrecht",
                Status = IncidentStatus.InProgress,
                CreatedAt = now.AddDays(-1)
            },
            new IncidentRequest
            {
                CustomerId = customers[2].Id,
                CreatedByUserId = salesUser.Id,
                Title = "Lekkage bij de aansluiting",
                Description = "Er lekt water bij de koppeling aan de achterzijde van de machine.",
                Location = "Voorstraat 18, Amersfoort",
                Status = IncidentStatus.Open,
                CreatedAt = now.AddHours(-5)
            }
        );
        await database.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}
