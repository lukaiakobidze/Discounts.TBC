// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Infrastructure.Data.Configurations
{
    public class FavouriteConfiguration : IEntityTypeConfiguration<Favourite>
    {
        public void Configure(EntityTypeBuilder<Favourite> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired().HasMaxLength(500);

            builder.HasOne(x => x.Offer)
                .WithMany(x => x.Favourites)
                .HasForeignKey(x => x.OfferId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.CustomerId, x.OfferId }).IsUnique();

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
