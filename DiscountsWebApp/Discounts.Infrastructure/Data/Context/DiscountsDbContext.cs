// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Entities;
using Discounts.Infrastructure.Identity;
using Discounts.Infrastructure.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Data.Context
{
    public class DiscountsDbContext : IdentityDbContext<ApplicationUser>
    {
        public DiscountsDbContext(DbContextOptions<DiscountsDbContext> options) : base(options) { }

        public DbSet<Offer> Offers => Set<Offer>();
        public DbSet<Coupon> Coupons => Set<Coupon>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<GlobalSetting> GlobalSettings => Set<GlobalSetting>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(DiscountsDbContext).Assembly);
        }
        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            builder.Properties<DateTime>()
                .HaveConversion<UtcDateTimeConverter>();
        }
    }
}
