using backend.Data;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static backend.Features.Auth.AuthDto;

namespace backend.Features.Auth
{
    public static class AuthEndpoints
    {
        public static WebApplication MapAuthEndpoints(this WebApplication app)
        {
            // Input validation with FluentValidation
            app.MapPost("/api/users/register", async (
                RegistrationDto req,
                MariaDbContext db,
                IPasswordHasher<User> passwordHasher,
                IValidator<RegistrationDto> validator) =>
            {
                // Manual validation
                var validationResult = await validator.ValidateAsync(req);
                if (!validationResult.IsValid)
                    return Results.ValidationProblem(validationResult.ToDictionary());

                // SQL Injection protection: Parameterized queries via EF Core
                bool userExists = await db.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.Email == req.Email); // Safe

                if (userExists)
                    return Results.Conflict("User exists");

                var user = new User
                {
                    Name = req.Name.Trim(), // Input sanitization
                    Email = req.Email.ToLower().Trim(),
                    Password = passwordHasher.HashPassword(null!, req.Password),
                    Role = "Student" // Default role
                };

                db.Users.Add(user);
                await db.SaveChangesAsync(); // Safe: EF Core uses parameterization

                return Results.Created("", new { user.ID, user.Email });
            });

            return app;
        }
    }

    // FluentValidation rules
    public class RegistrationValidator : AbstractValidator<RegistrationDto>
    {
        public RegistrationValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(2, 50);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        }
    }
}