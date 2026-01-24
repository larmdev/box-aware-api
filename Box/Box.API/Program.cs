using Box.Application.Interfaces;
using Box.Application.Services;
using Box.Infrastructure.Data;
using Box.Infrastructure.Interfaces;
using Box.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Db
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("BoxDb"));

// N-Tier DI
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IRankService, RankService>();

// JWT
var jwt = builder.Configuration.GetSection("JwtSettings");
var key = Convert.FromBase64String(jwt["Secret"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey =
            new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(db);
}


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
