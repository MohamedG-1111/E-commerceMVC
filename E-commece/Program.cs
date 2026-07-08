using E_commece.Extensions;
namespace E_commece
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDatabase(builder.Configuration)
             .AddIdentity()
             .AddRedis(builder.Configuration)
             .AddConfiguration(builder.Configuration)
             .AddDependenics();

            var app = builder.Build();


            await app.SeedDatabaseAsync();

            app.UseApplicationMiddlewares();
            app.Run();
        }
    }
}
