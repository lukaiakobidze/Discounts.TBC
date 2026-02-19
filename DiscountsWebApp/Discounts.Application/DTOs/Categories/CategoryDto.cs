// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.DTOs.Categories
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int OfferCount { get; set; }
    }
}
