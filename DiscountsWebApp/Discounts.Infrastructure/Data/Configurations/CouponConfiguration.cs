// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Infrastructure.Data.Configurations
{
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
            builder.HasIndex(x => x.Code).IsUnique();

            builder.Property(x => x.PurchaseDate).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.CustomerId).IsRequired().HasMaxLength(500);

            builder.Navigation(x => x.Offer).AutoInclude();

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
