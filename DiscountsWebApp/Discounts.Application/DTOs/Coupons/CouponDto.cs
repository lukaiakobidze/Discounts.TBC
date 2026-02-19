// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Enums;

namespace Discounts.Application.DTOs.Coupons
{
    public class CouponDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public CouponStatus Status { get; set; }
        public DateTime PurchaseDate { get; set; }
        public Guid OfferId { get; set; }
        public string OfferName { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
    }
}
