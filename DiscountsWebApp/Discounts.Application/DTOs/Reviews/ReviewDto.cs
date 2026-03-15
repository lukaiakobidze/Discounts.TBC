// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.DTOs.Reviews
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public int Stars { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
