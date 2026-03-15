// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Infrastructure.Data.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Stars).IsRequired();
            builder.Property(x => x.Comment).HasMaxLength(1000);

            builder.HasOne(x => x.Offer)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.OfferId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Coupon>()
                .WithOne()
                .HasForeignKey<Review>(x => x.CouponId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(x => x.CouponId).IsUnique();

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
