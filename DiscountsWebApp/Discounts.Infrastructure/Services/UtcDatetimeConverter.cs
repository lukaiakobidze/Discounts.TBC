// Copyright (C) TBC Bank. All Rights Reserved.

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Discounts.Infrastructure.Services
{
    public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcDateTimeConverter() : base(
            v => v,
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
        { }
    }
}
