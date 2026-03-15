// Copyright (C) TBC Bank. All Rights Reserved.

using System.Globalization;
using Discounts.API.Extensions;
using Discounts.API.Middlewares;
using Discounts.Application;
using Discounts.Data.Context;
using Discounts.Infrastructure;
using Discounts.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Discounts.API
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
                Log.Information("Starting TBC Web API");

                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog((context, services, configuration) =>
                    configuration.ReadFrom.Configuration(context.Configuration));

                // Add services
                builder.Services.AddApplication();
                builder.Services.AddInfrastructure(builder.Configuration);
                builder.Services.AddJwtAuthentication(builder.Configuration);
                builder.Services.AddApiVersioningConfiguration();
                builder.Services.AddSwaggerDocumentation();
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
                });

                var app = builder.Build();

                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<DiscountsDbContext>();

                    Log.Information("Applying database migrations...");
                    await db.Database.MigrateAsync().ConfigureAwait(false);
                    Log.Information("Database migrations applied successfully");

                    await RoleSeed.SeedRolesAsync(scope.ServiceProvider).ConfigureAwait(false);
                    await AdminSeed.SeedAdminAsync(scope.ServiceProvider).ConfigureAwait(false);
                    await CategorySeed.SeedCategoriesAsync(scope.ServiceProvider).ConfigureAwait(false);
                    await GlobalSettingsSeed.SeedGlobalSettingsAsync(scope.ServiceProvider).ConfigureAwait(false);
                }

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwaggerDocumentation();
                }

                app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
                app.UseSerilogRequestLogging();
                app.UseCors("AllowAll");
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers();
                app.MapHealthCheckEndpoints();

                app.Run();
            }
            catch (Exception ex) when (ex is not HostAbortedException)
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
