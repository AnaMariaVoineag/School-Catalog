using System.Runtime.InteropServices;
using backend;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MariaDbContext>(opt =>
{
    var conString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
    opt.UseMySql(conString, ServerVersion.AutoDetect(conString));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();