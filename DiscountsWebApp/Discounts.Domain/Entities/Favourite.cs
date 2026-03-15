// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Domain.Entities
{
    public class Favourite : BaseEntity
    {
        public string CustomerId { get; set; } = string.Empty;
        public Guid OfferId { get; set; }
        public Offer Offer { get; set; } = null!;
    }
}
