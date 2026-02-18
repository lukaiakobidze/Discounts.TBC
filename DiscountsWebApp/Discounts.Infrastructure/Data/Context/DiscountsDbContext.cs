// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Entities;
using Discounts.Infrastructure.Identity;
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(DiscountsDbContext).Assembly);
        }
    }
}
