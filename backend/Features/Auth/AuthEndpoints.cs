using Microsoft.AspNetCore.Identity;
using static backend.Features.Users.UserDto;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using static backend.Features.Auth.AuthDto;
using backend.Data;


namespace backend.Features.Auth
{
    public static class AuthEndpoints
    {
        public static WebApplication MapAuthEndpoints(this WebApplication app)
        {
            // Registration Endpoint
            app.MapPost("/api/auth/register", async (
                RegistrationDto req,
                MariaDbContext db,
                IPasswordHasher<User> passwordHasher,
                IConfiguration config) =>
            {
                if (await db.Users.AnyAsync(u => u.Email == req.Email))
                    return Results.Conflict("User already exists");

                if (req.Role != "student" &&  req.Role != "teacher")
                {
                    return Results.BadRequest("Invalid UserType. Must be 'teacher' or 'student'.");
                }
                var user = new User
                {
                    Name = req.Name,
                    Email = req.Email,
                    IsActive = true,
                    LastLogin = DateTime.UtcNow,
                    Role = req.Role
                };

                user.Password = passwordHasher.HashPassword(user, req.Password);

                db.Users.Add(user);
                await db.SaveChangesAsync();

                // Create either a Teacher or Student record
                if (req.Role == "teacher")
                {
                    var teacher = new Teacher { User = user };
                    db.Teachers.Add(teacher);
                }
                else if (req.Role == "student")
                {
                    var student = new Student { User = user };
                    db.Students.Add(student);
          
                }
                await db.SaveChangesAsync();

                var token = Utils.GenerateJwtToken(user, config["Jwt:Key"]!, config["Jwt:Issuer"]!);

                return Results.Created($"/api/users/{user.ID}", new
                {
                    user.ID,
                    user.Name,
                    user.Email,
                    user.Role,
                    Token = token,

                });
            });
            // Login Endpoint
            app.MapPost("/api/auth/login", async (
                LoginDto req,
                MariaDbContext db,
                IPasswordHasher<User> passwordHasher,
                IConfiguration config) =>
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);

                if (user == null)
                    return Results.Unauthorized();

                var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, req.Password);

                if (passwordVerificationResult == PasswordVerificationResult.Failed)
                    return Results.Unauthorized();

                user.LastLogin = DateTime.UtcNow;
                await db.SaveChangesAsync();

                var token = Utils.GenerateJwtToken(user, config["Jwt:Key"]!, config["Jwt:Issuer"]!);

                return Results.Ok(new
                {
                    Token = token
                });
            });
            return app;
        }
    }
}
