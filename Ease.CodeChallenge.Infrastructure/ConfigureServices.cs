using Ease.CodeChallenge.Application.Interfaces;
using Ease.CodeChallenge.Infrastructure.Persistence.DbContexts;
using Ease.CodeChallenge.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MetadataDbContext>(options =>
            {
                options.UseInMemoryDatabase("MetadataDB");
            });

            services.AddScoped<IApplicationContext, MetadataDbContext>();

            // Add Redis configuration
            var redisConfiguration = configuration.GetSection("Redis")["ConnectionString"];
            var redis = ConnectionMultiplexer.Connect(redisConfiguration);
            services.AddSingleton<IConnectionMultiplexer>(redis);
            services.AddSingleton<ICacheService, RedisService>();

            return services;
        }
    }
}