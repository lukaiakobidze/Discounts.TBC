// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Infrastructure.Data.Configurations
{
    public class GlobalSettingConfiguration : IEntityTypeConfiguration<GlobalSetting>
    {
        public void Configure(EntityTypeBuilder<GlobalSetting> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Key).IsRequired().HasMaxLength(100);
            builder.HasIndex(x => x.Key).IsUnique();
            builder.Property(x => x.Value).IsRequired().HasMaxLength(500);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
