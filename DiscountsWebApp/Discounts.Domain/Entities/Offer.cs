// Copyright (C) TBC Bank. All Rights Reserved.

using System.ComponentModel.DataAnnotations;
using Discounts.Domain.Enums;

namespace Discounts.Domain.Entities
{
    public class Offer : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int TotalCount { get; set; }
        public int RemainingCount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public OfferStatus Status { get; set; }

        public string MerchantId { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
    }
}
