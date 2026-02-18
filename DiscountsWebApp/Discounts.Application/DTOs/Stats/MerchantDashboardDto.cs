// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.DTOs.Stats
{
    public class MerchantDashboardDto
    {
        public int TotalOffers { get; set; }
        public int PendingOffers { get; set; }
        public int ActiveOffers { get; set; }
        public int TotalCouponsSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
