using backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using static backend.Features.Users.UserDto;

namespace backend.Features.Users
{
    public static class UserEndpoints
    {
        public static WebApplication MapUserEndpoints(this WebApplication app)
        {

            // Get Current User Profile
            app.MapGet("/api/users/me", [Authorize] async (
                MariaDbContext db,
                ClaimsPrincipal claims) =>
            {
                var userIdClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Results.Unauthorized();

                var userId = int.Parse(userIdClaim.Value);
                var user = await db.Users.FindAsync(userId);

                if (user == null)
                    return Results.NotFound();

                return Results.Ok(new { user.ID, user.Name, user.Email, user.LastLogin, user.IsActive });
            });

            // Update user endpoint
            app.MapPut("/api/users/{id:int}", [Authorize] async (
                int id,
                UserUpdateDto req,
                MariaDbContext db,
                ClaimsPrincipal claims) =>
            {
                var userIdClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || int.Parse(userIdClaim.Value) != id)
                    return Results.Forbid();

                var user = await db.Users.FindAsync(id);
                if (user == null)
                    return Results.NotFound();

                if (!string.IsNullOrWhiteSpace(req.Name))
                    user.Name = req.Name;

                if (!string.IsNullOrWhiteSpace(req.Email))
                    user.Email = req.Email;

                await db.SaveChangesAsync();
                return Results.Ok(new { user.ID, user.Name, user.Email });
            });

            // Delete user endpoint
            app.MapDelete("/api/users/{id:int}", [Authorize] async (
                int id,
                MariaDbContext db,
                ClaimsPrincipal claims) =>
            {
                var userIdClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || int.Parse(userIdClaim.Value) != id)
                    return Results.Forbid();

                var user = await db.Users.FindAsync(id);
                if (user == null)
                    return Results.NotFound();

                db.Users.Remove(user);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
            return app;
        }
    }
}
