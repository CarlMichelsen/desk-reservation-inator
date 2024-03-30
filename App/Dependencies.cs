using System.Reflection;
using App.Tasks;
using Domain.Configuration;
using Implementation.Client;
using Implementation.Handler;
using Implementation.Service;
using Interface.Client;
using Interface.Handler;
using Interface.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App;

public static class Dependencies
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuration
        services
            .AddOptions()
            .Configure<DiscordWebhookOptions>(configuration.GetSection(DiscordWebhookOptions.SectionName))
            .Configure<DeskReservationOptions>(configuration.GetSection(DeskReservationOptions.SectionName));

        // HostedServices
        services
            .AddHostedService<ReservationTask>();
        
        // Handlers
        services
            .AddScoped<IDeskReservationHandler, DeskReservationHandler>();
        
        // Services
        services
            .AddScoped<ICacheService, CacheService>()
            .AddScoped<ISessionDataService, SessionDataService>()
            .AddScoped<IDeskReservationService, DeskReservationService>()
            .AddScoped<IDiscordWebhookService, DiscordWebhookService>();
        
        // Clients
        services
            .AddScoped<IMydeskClient, MydeskClient>();

        // Logging
        services.AddLogging(config =>
            {
                config.ClearProviders();
                config.AddConsole();
                config.AddDebug();
            });

        // HttpClients
        services.AddHttpClient<DiscordWebhookService>();
        services.AddHttpClient<MydeskClient>();
        
        // Cache
        var redisConfiguration = configuration.GetSection(RedisOptions.SectionName);
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfiguration[nameof(RedisOptions.ConnectionString)];
            options.InstanceName = redisConfiguration[nameof(RedisOptions.InstanceName)];
        });
        
        return services;
    }

    public static IConfigurationBuilder RegisterApplicationConfiguration(this IConfigurationBuilder configurationBuilder)
    {
        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            ?? Directory.GetCurrentDirectory();
        
        configurationBuilder
            .SetBasePath(currentDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("secrets.json", optional: false, reloadOnChange: true);

        return configurationBuilder;
    }
}
