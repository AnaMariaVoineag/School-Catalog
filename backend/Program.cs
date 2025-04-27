using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend;
using backend.Data;
using backend.Features.Auth;
using backend.Features.Grades;
using backend.Features.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("Missing config: Jwt:Key");
var issuer = builder.Configuration["Jwt:Issuer"] ?? throw new Exception("Missing config: Jwt:Issuer");

// Configure services
builder.Services.AddDbContext<MariaDbContext>(opt =>
{
    var conString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
    opt.UseMySql(conString, ServerVersion.AutoDetect(conString));
});

// Add authentication with explicit scheme
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
            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = ClaimTypes.Role
        };
    });

// Add authorization with role policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireTeacherRole", policy =>
        policy.RequireRole("teacher"));

    options.AddPolicy("RequireStudentRole", policy =>
        policy.RequireRole("student"));

    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Add scoped password hasher (better for request lifecycle)
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Add CORS if needed (adjust origins as necessary)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

/*
    Middleware Pipeline
*/
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

/*
    Route Endpoints
*/
app.MapUserEndpoints();
app.MapAuthEndpoints();
app.MapGradeEndpoints();
app.MapCourseEndpoints();

app.Run();