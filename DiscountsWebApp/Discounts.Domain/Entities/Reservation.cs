// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Domain.Entities
{
    public class Reservation : BaseEntity
    {
        public Guid OfferId { get; set; }
        public Offer Offer { get; set; } = null!;

        public string CustomerId { get; set; } = null!;
        public DateTime ExpirationDate { get; set; }
    }
}
