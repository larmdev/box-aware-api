using Box.Application.Interfaces;
using Box.Application.Services;
using Box.Infrastructure.Data;
using Box.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Box.Infrastructure.ExternalApis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Db
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseInMemoryDatabase("BoxDb"));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DatabaseConnection"),
        b => b.MigrationsAssembly("Box.Infrastructure")
    ));

// Services & Repositories
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IRankService, RankService>();
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddHttpClient<ITodoApiClient, TodoApiClient>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(
        config["ExternalApis:TodoApi:BaseUrl"]!
    );
});


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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(db);
}

// Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
