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
                var interceptor = serviceProvider.GetRequiredService<AuditFieldsInterceptor>();
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
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
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            services.AddScoped<IOfferRepository, OfferRepository>();
            services.AddScoped<ICouponRepository, CouponRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IGlobalSettingRepository, GlobalSettingRepository>();
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
