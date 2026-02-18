// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<Offer> Offers { get; set; } = new List<Offer>();

    }
}
