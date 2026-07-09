using Ecommerce.Utility.Settings;
using Hangfire;

namespace E_commece.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            services.AddHangfire(config =>
          config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
         .UseSimpleAssemblyNameTypeSerializer()
         .UseRecommendedSerializerSettings()
         .UseSqlServerStorage(
             configuration.GetConnectionString("DefaultConnection")));

            services.AddHangfireServer();
            return services;
        }
    }
}
