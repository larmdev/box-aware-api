using Box.Application.Interfaces;
using Box.Application.Services;
using Box.Infrastructure.Data;
using Box.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Box.Infrastructure.ExternalApis;
using StackExchange.Redis;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Hangfire.Dashboard.BasicAuthorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Db
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DatabaseConnection"),
        b => b.MigrationsAssembly("Box.Infrastructure")
    ));

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = builder.Configuration.GetConnectionString("RedisConnection");
    return ConnectionMultiplexer.Connect(config!);
});

// Services & Repositories
builder.Services.AddScoped<PasswordHasher>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();

builder.Services.AddScoped<IRankService, RankService>();
builder.Services.AddScoped<ITodoService, TodoService>();

builder.Services.AddScoped<IEmailJobService, EmailJobService>();

builder.Services.AddHttpClient<ITodoApiClient, TodoApiClient>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(
        config["ExternalApis:TodoApi:BaseUrl"]!
    );
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Redis
builder.Services.AddScoped<ISessionService, RedisSessionService>();
builder.Services.AddScoped<IRefreshTokenService, RedisRefreshTokenService>();


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

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var sessionService = context.HttpContext
                .RequestServices
                .GetRequiredService<ISessionService>();

            var userIdStr = context.Principal!
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                ?.Value;

            var jti = context.Principal!
                .FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti)
                ?.Value;

            if (userIdStr == null || jti == null)
            {
                context.Fail("Invalid token");
                return;
            }

            var valid = await sessionService.IsSessionValidAsync(
                Guid.Parse(userIdStr),
                jti
            );

            if (!valid)
            {
                context.Fail("Session expired or revoked");
            }
        }
    };
});

// Hangfire Configuration
var hangfireRedis = builder.Configuration.GetConnectionString("HangfireConnection");

builder.Services.AddHangfire(config =>
{
    config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseRedisStorage(hangfireRedis, new RedisStorageOptions
        {
            Prefix = "box-hangfire:",
            Db = 1 // แยก DB จาก cache ปกติ
        });
});

builder.Services.AddHangfireServer(options =>
{
    options.ServerName = builder.Configuration.GetValue<string>("Hangfire:ServerName");
    options.WorkerCount = 1;
    options.Queues = new[]
    {
        "send-email"
    };
    options.ShutdownTimeout = TimeSpan.FromMinutes(2);
    options.HeartbeatInterval = TimeSpan.FromSeconds(20);
    options.ServerTimeout = TimeSpan.FromMinutes(2);
    options.ServerCheckInterval = TimeSpan.FromSeconds(25);
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

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[]
    {
        new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
        {
            SslRedirect = false,
            RequireSsl = false,
            LoginCaseSensitive = true,
            Users = new []
            {
                new BasicAuthAuthorizationUser
                {
                    Login = "admin",
                    PasswordClear = "admin"
                }
            }
        })
    }
});

app.MapControllers();
app.Run();
