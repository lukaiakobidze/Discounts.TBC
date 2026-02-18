// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.DTOs.Reservation
{
    public class ReservationDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid OfferId { get; set; }
        public string OfferName { get; set; } = string.Empty;
        public decimal OfferDiscountedPrice { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
    }
}
