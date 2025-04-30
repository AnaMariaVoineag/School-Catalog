using Microsoft.AspNetCore.Identity;
using static backend.Features.Users.UserDto;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using static backend.Features.Auth.AuthDto;
using backend.Data;

namespace backend.Features.Auth
{
    /// \class AuthEndpoints
    /// \brief Contains API endpoint mappings for authentication operations such as user registration and login.
    ///
    /// This class handles user registration and login, including token generation, password hashing, and user validation.
    public static class AuthEndpoints
    {
        /// \brief Maps the authentication-related API endpoints to the WebApplication instance.
        /// \param app The WebApplication instance to which the authentication endpoints are mapped.
        /// \return The WebApplication instance with the authentication-related endpoints mapped.
        public static WebApplication MapAuthEndpoints(this WebApplication app)
        {
            // Registration Endpoint
            /// \brief Endpoint for user registration.
            ///
            /// This endpoint allows new users to register by providing their name, email, password, and role (either 'teacher' or 'student').
            /// A JWT token is generated upon successful registration.
            /// \param req The registration request containing user details.
            /// \param db The MariaDbContext to interact with the database.
            /// \param passwordHasher The password hasher for hashing user passwords.
            /// \param config The configuration to retrieve JWT settings.
            /// \return HTTP 201 Created with the new user details and JWT token, or HTTP 409 Conflict if the user already exists, or HTTP 400 BadRequest if the role is invalid.
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
            /// \brief Endpoint for user login.
            ///
            /// This endpoint allows existing users to log in using their email and password.
            /// Upon successful login, a JWT token is generated and returned.
            /// \param req The login request containing the user's email and password.
            /// \param db The MariaDbContext to interact with the database.
            /// \param passwordHasher The password hasher to verify the user's password.
            /// \param config The configuration to retrieve JWT settings.
            /// \return HTTP 200 OK with a JWT token if successful, or HTTP 401 Unauthorized if the credentials are incorrect.
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
