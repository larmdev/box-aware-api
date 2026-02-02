using Box.Shared.Auth.Interfaces;
using Box.Shared.Auth.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Box.Shared.Auth.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBoxSharedAuth(this IServiceCollection services)
    {
        services.AddScoped<ISessionService, RedisSessionService>();
        return services;
    }
}
