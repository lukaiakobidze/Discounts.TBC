// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Enums;

namespace Discounts.Application.DTOs.Offers
{
    public class OfferDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int TotalCount { get; set; }
        public int RemainingCount { get; set; }
        public OfferStatus Status { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public string MerchantId { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int DiscountPercentage => OriginalPrice > 0 ? (int)((OriginalPrice - DiscountedPrice) / OriginalPrice * 100) : 0;
        public bool IsSoldOut => RemainingCount <= 0;
    }
}
