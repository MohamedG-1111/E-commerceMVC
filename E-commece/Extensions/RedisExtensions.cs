using StackExchange.Redis;

namespace E_commece.Extensions
{
    public static class RedisExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConnectionMultiplexer>(cfg =>
            {
                return ConnectionMultiplexer
                    .Connect(configuration.GetConnectionString("RedisConnection")!);
            });
            return services;
        }
    }
}
