using backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using static backend.Features.Users.UserDto;

namespace backend.Features.Users
{
    /// \class UserEndpoints
    /// \brief Contains API endpoint mappings for user operations like fetching, updating, and deleting users.
    public static class UserEndpoints
    {
        /// \brief Maps the user-related API endpoints to the WebApplication instance.
        /// \param app The WebApplication instance to which the endpoints are mapped.
        /// \return The WebApplication instance with the user-related endpoints mapped.
        public static WebApplication MapUserEndpoints(this WebApplication app)
        {
            // Get Current User Profile
            /// \brief Endpoint for retrieving the current user's profile information.
            /// 
            /// This endpoint fetches details such as ID, name, email, last login, and role of the currently logged-in user.
            /// \return HTTP 200 OK if the user is found, or HTTP 401 Unauthorized, or HTTP 404 Not Found if the user does not exist.
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
                return Results.Ok(new { user.ID, user.Name, user.Email, user.LastLogin, user.IsActive, user.Role });
            });

            // Update user endpoint
            /// \brief Endpoint for updating a user's profile information (name, email).
            /// 
            /// This endpoint allows updating a user's name and/or email, and ensures that the request is made by the user themselves.
            /// \param id The ID of the user to be updated.
            /// \param req The update request containing the optional new name and email.
            /// \return HTTP 200 OK with updated user details if successful, or HTTP 403 Forbidden, HTTP 404 Not Found if the user does not exist.
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
            /// \brief Endpoint for deleting a user's profile.
            /// 
            /// This endpoint allows the user to delete their own account.
            /// \param id The ID of the user to be deleted.
            /// \return HTTP 204 No Content if successful, or HTTP 403 Forbidden, HTTP 404 Not Found if the user does not exist.
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
