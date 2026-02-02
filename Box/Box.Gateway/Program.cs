using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Box.Shared.Auth.Extensions;
using Box.Shared.Auth.Constants;
using Box.Shared.Auth.Interfaces;
using Box.Shared.Auth.Services;

var builder = WebApplication.CreateBuilder(args);

// Config
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration.GetConnectionString("RedisConnection");

    options.InstanceName = "box-session:";
});

builder.Services.AddBoxSharedAuth();

// JWT Authentication

var jwt = builder.Configuration.GetSection("JwtSettings");
var key = Convert.FromBase64String(jwt["Secret"]!);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwt["Audience"],

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,

            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var sessionService = context.HttpContext
                    .RequestServices
                    .GetRequiredService<ISessionService>();

                var userIdStr = context.Principal!
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var jti = context.Principal!
                    .FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                if (string.IsNullOrEmpty(userIdStr) || string.IsNullOrEmpty(jti))
                {
                    context.Fail("Invalid token claims");
                    return;
                }

                var valid = await sessionService.IsSessionValidAsync(
                    Guid.Parse(userIdStr),
                    jti
                );

                if (!valid)
                {
                    context.Fail("Session revoked or expired");
                }
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

app.Run();
