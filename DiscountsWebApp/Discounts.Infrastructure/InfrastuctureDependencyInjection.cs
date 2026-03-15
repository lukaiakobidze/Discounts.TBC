// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Data.Context;
using Discounts.Infrastructure.BackgroundServices;
using Discounts.Infrastructure.Data.Interceptors;
using Discounts.Infrastructure.HealthCheck;
using Discounts.Infrastructure.Identity;
using Discounts.Infrastructure.Repositories;
using Discounts.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Discounts.Infrastructure
{
    public static class InfrastuctureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<AuditFieldsInterceptor>();
            services.AddDbContext<DiscountsDbContext>((serviceProvider, options) =>
            {
                var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                    ?? configuration.GetConnectionString("DefaultConnection")
                    ?? "Server=localhost;Database=TBC_Discounts;Trusted_Connection=True;TrustServerCertificate=True";

                var interceptor = serviceProvider.GetRequiredService<AuditFieldsInterceptor>();
                options.UseSqlServer(connectionString,
                    sqlOptions => sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                    .AddInterceptors(interceptor);
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<DiscountsDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
            services.PostConfigure<JwtSettings>(settings =>
            {
                var envSecret = Environment.GetEnvironmentVariable("JwtSettings__SecretKey");
                if (envSecret is not null)
                    settings.SecretKey = envSecret;
            });
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            services.AddScoped<IOfferRepository, OfferRepository>();
            services.AddScoped<ICouponRepository, CouponRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IGlobalSettingRepository, GlobalSettingRepository>();
            services.AddScoped<IFavouriteRepository, FavouriteRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddHostedService<OfferExpirationWorker>();
            services.AddHostedService<ReservationCleanupWorker>();

            services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("database");

            services.AddHttpContextAccessor();

            return services;
        }
    }
}
