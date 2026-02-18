// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Infrastructure.Data.Configurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CustomerId).IsRequired().HasMaxLength(500);
            builder.Property(x => x.ExpirationDate).IsRequired();

            builder.Navigation(x => x.Offer).AutoInclude();

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
