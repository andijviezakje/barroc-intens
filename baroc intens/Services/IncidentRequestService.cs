using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using baroc_intens.Data;
using baroc_intens.Models;
using Microsoft.EntityFrameworkCore;

namespace baroc_intens.Services;

public sealed class IncidentRequestService
{
    public async Task<List<IncidentRequest>> GetForMaintenanceAsync(AppUser user)
    {
        RoleAuthorization.RequireRole(user, UserRole.Maintenance);
        await using var database = new BarrocDbContext();
        return await database.IncidentRequests
            .Include(request => request.Customer)
            .Include(request => request.CreatedByUser)
            .OrderByDescending(request => request.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<IncidentRequest>> GetForSalesAsync(AppUser user)
    {
        RoleAuthorization.RequireRole(user, UserRole.Sales);
        await using var database = new BarrocDbContext();
        return await database.IncidentRequests
            .Include(request => request.Customer)
            .Include(request => request.CreatedByUser)
            .OrderByDescending(request => request.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Customer>> GetCustomersForSalesAsync(AppUser user)
    {
        RoleAuthorization.RequireRole(user, UserRole.Sales);
        await using var database = new BarrocDbContext();
        return await database.Customers.OrderBy(customer => customer.Name).ToListAsync();
    }

    public async Task CreateAsync(
        AppUser user,
        int customerId,
        string title,
        string description,
        string location)
    {
        RoleAuthorization.RequireRole(user, UserRole.Sales);
        ValidateFields(title, description, location);

        await using var database = new BarrocDbContext();
        var customerExists = await database.Customers.AnyAsync(customer => customer.Id == customerId);
        if (!customerExists)
        {
            throw new ArgumentException("Selecteer een bestaande klant.");
        }

        database.IncidentRequests.Add(new IncidentRequest
        {
            CustomerId = customerId,
            CreatedByUserId = user.Id,
            Title = title.Trim(),
            Description = description.Trim(),
            Location = location.Trim(),
            Status = IncidentStatus.Open,
            CreatedAt = DateTime.Now
        });
        await database.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        AppUser user,
        int requestId,
        int customerId,
        string title,
        string description,
        string location)
    {
        RoleAuthorization.RequireRole(user, UserRole.Sales);
        ValidateFields(title, description, location);

        await using var database = new BarrocDbContext();
        var request = await database.IncidentRequests.FindAsync(requestId);
        if (request is null)
        {
            throw new InvalidOperationException("De storingsaanvraag bestaat niet meer.");
        }

        var customerExists = await database.Customers.AnyAsync(customer => customer.Id == customerId);
        if (!customerExists)
        {
            throw new ArgumentException("Selecteer een bestaande klant.");
        }

        request.CustomerId = customerId;
        request.Title = title.Trim();
        request.Description = description.Trim();
        request.Location = location.Trim();
        request.UpdatedAt = DateTime.Now;
        await database.SaveChangesAsync();
    }

    private static void ValidateFields(string title, string description, string location)
    {
        ValidateRequired(title, "Titel", 120);
        ValidateRequired(description, "Omschrijving", 2000);
        ValidateRequired(location, "Locatie", 200);
    }

    private static void ValidateRequired(string value, string fieldName, int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{fieldName} is verplicht.");
        }

        if (value.Trim().Length > maximumLength)
        {
            throw new ArgumentException($"{fieldName} mag maximaal {maximumLength} tekens bevatten.");
        }
    }
}
