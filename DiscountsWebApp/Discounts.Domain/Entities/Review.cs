// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Domain.Entities
{
    public class Review : BaseEntity
    {
        public string CustomerId { get; set; } = string.Empty;
        public Guid OfferId { get; set; }
        public Offer Offer { get; set; } = null!;
        public Guid CouponId { get; set; }
        public int Stars { get; set; }
        public string? Comment { get; set; }
    }
}
