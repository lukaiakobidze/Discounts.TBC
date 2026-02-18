// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Enums;

namespace Discounts.Domain.Entities
{
    public class Coupon : BaseEntity
    {
        public string Code { get; set; } = null!;
        public CouponStatus Status { get; set; }
        public DateTime PurchaseDate { get; set; }
        public Guid OfferId { get; set; }
        public Offer Offer { get; set; } = null!;

        public string CustomerId { get; set; } = null!;

    }
}
