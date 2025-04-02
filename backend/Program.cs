using System.Text;
using backend.Data;
using backend.Features.Auth;
using backend.Features.Users;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ===== SECURITY CONFIGURATION =====
// 1. HTTPS Enforcement
builder.Services.AddHttpsRedirection(options => options.HttpsPort = 443);

// 2. Rate Limiting (Prevent Brute Force)
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth", limiter =>
    {
        limiter.PermitLimit = 5;
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.QueueLimit = 0;
    });
});

// 3. Secure CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("SecurePolicy", policy =>
    {
        policy.WithOrigins("https://your-trusted-domain.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 4. Database Configuration (SQL Injection Protection)
builder.Services.AddDbContext<MariaDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mysqlOptions =>
        {
            mysqlOptions.EnableRetryOnFailure(maxRetryCount: 5);
            mysqlOptions.CommandTimeout(30);
        });
});

// 5. Authentication (JWT)
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("Missing JWT Key");
var issuer = builder.Configuration["Jwt:Issuer"] ?? throw new Exception("Missing JWT Issuer");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

// 6. Input Validation (FluentValidation)
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// 7. Authorization & Password Hashing
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

// ===== MIDDLEWARE PIPELINE =====
// Order is critical!
app.UseHttpsRedirection();
app.UseCors("SecurePolicy");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// ===== ROUTES =====
app.MapAuthEndpoints();
app.MapUserEndpoints();

// Health Check
app.MapGet("/", () => "API is running securely").ExcludeFromDescription();

app.Run();