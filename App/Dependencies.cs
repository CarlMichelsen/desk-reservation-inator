using System.Reflection;
using App.Tasks;
using Domain.Configuration;
using Implementation.Client;
using Implementation.Handler;
using Implementation.Service;
using Interface.Client;
using Interface.Handler;
using Interface.Service;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace App;

public static class Dependencies
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuration
        services
            .AddOptions()
            .Configure<DiscordWebhookOptions>(configuration.GetSection(DiscordWebhookOptions.SectionName))
            .Configure<DeskReservationOptions>(configuration.GetSection(DeskReservationOptions.SectionName))
            .AddOptions<MemoryDistributedCacheOptions>().Configure(options => options.SizeLimit = 1024 * 1024 * 50); // 50 MB

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
        
        // Cache (will revert to in-memory cache if for any reason unable to connect to redis instance)
        services
            .AddMemoryCache()
            .RegisterCacheImplementation(configuration);
        
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

    private static void RegisterCacheImplementation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDistributedCache>(sp =>
        {
            try
            {
                var redisConfiguration = configuration.GetSection(RedisOptions.SectionName);
                var redisOptions = new RedisCacheOptions
                {
                    Configuration = redisConfiguration[nameof(RedisOptions.ConnectionString)],
                    InstanceName = redisConfiguration[nameof(RedisOptions.InstanceName)],
                };

                var redis = new RedisCache(redisOptions);
                redis.SetString("redis-connection-check", "I'm alive!");

                return redis;
            }
            catch (Exception ex)
            {
                // Log as warning
                var logger = sp.GetRequiredService<ILogger<Program>>();
                logger.LogWarning(ex, "Failed to connect to Redis. Falling back to in-memory cache.");

                var memoryCacheOptions = sp.GetRequiredService<IOptions<MemoryDistributedCacheOptions>>();
                return new MemoryDistributedCache(memoryCacheOptions);
            }
        });
    }
}
