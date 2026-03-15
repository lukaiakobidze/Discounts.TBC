// Copyright (C) TBC Bank. All Rights Reserved.

using System.Globalization;
using Discounts.Application;
using Discounts.Data.Context;
using Discounts.Infrastructure;
using Discounts.Infrastructure.Data.Seed;
using Discounts.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Discounts.MVC
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
                .CreateBootstrapLogger();

            try
            {
                Log.Information("Starting TBC Web MVC");

                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog((context, services, configuration) =>
                    configuration.ReadFrom.Configuration(context.Configuration));

                builder.Services.AddApplication();
                builder.Services.AddInfrastructure(builder.Configuration);

                builder.Services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromHours(2);
                });

                builder.Services.Configure<SecurityStampValidatorOptions>(options =>
                {
                    options.ValidationInterval = TimeSpan.Zero;
                });

                builder.Services.AddControllersWithViews();

                var app = builder.Build();

                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DiscountsDbContext>();
                    await context.Database.EnsureCreatedAsync().ConfigureAwait(false);
                    await RoleSeed.SeedRolesAsync(scope.ServiceProvider).ConfigureAwait(false);
                    await AdminSeed.SeedAdminAsync(scope.ServiceProvider).ConfigureAwait(false);
                    await CategorySeed.SeedCategoriesAsync(scope.ServiceProvider).ConfigureAwait(false);
                    await GlobalSettingsSeed.SeedGlobalSettingsAsync(scope.ServiceProvider).ConfigureAwait(false);
                }

                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseSerilogRequestLogging();
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
