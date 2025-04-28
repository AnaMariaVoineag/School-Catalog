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
using SchoolCatalogProject.Features.Courses;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("missing config: Jwt:Key");
var issuer = builder.Configuration["Jwt:Issuer"] ?? throw new Exception("missing config: Jwt:Issuer");

// Add this before configuring any other services
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// Configure services
builder.Services.AddDbContext<MariaDbContext>(opt =>
{
    var conString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
    opt.UseMySql(conString, ServerVersion.AutoDetect(conString));
});

// Add authentication services
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IPasswordHasher<User>>(new PasswordHasher<User>());

var app = builder.Build();
/*
    Middlewares 
*/
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
/*
    Routes 
*/
app.MapUserEndpoints();
app.MapAuthEndpoints();
app.MapCourseEndpoints();
app.MapGradeEndpoints();
app.Run();
// DTOs and User model remain the same