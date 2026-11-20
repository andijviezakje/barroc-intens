using System;
using baroc_intens.Models;

namespace baroc_intens.Services;

public static class RoleAuthorization
{
    public static void RequireRole(AppUser? user, UserRole requiredRole)
    {
        if (user is null || user.Role != requiredRole)
        {
            throw new UnauthorizedAccessException("Je hebt geen toegang tot deze functie.");
        }
    }
}
