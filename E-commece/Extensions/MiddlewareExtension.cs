using E_commece.Middlewares;
using Hangfire;

namespace E_commece.Extensions
{
    public static class MiddlewareExtension
    {
        public static WebApplication UseApplicationMiddlewares(this WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseExceptionHandlingMiddleware();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHangfireDashboard("/hangfire");
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            return app;
        }
    }
}
