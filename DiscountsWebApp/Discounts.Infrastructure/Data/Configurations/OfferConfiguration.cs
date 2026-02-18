// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Infrastructure.Data.Configurations
{
    public class OfferConfiguration : IEntityTypeConfiguration<Offer>
    {
        public void Configure(EntityTypeBuilder<Offer> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(2000);
            builder.Property(x => x.ImagePath).HasMaxLength(500);
            builder.Property(x => x.OriginalPrice).HasColumnType("decimal(18,2)");
            builder.Property(x => x.DiscountedPrice).HasColumnType("decimal(18,2)");
            builder.Property(x => x.TotalCount).IsRequired();
            builder.Property(x => x.RemainingCount).IsRequired();
            builder.Property(x => x.ValidFrom).IsRequired();
            builder.Property(x => x.ValidTo).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.MerchantId).IsRequired().HasMaxLength(500);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Offers)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(x => x.Category).AutoInclude();

            builder.HasMany(x => x.Coupons)
                .WithOne(x => x.Offer)
                .HasForeignKey(x => x.OfferId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Reservations)
                .WithOne(x => x.Offer)
                .HasForeignKey(x => x.OfferId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
