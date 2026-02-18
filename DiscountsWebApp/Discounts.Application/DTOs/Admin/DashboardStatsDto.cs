// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.DTOs.Admin
{
    public class DashboardStatsDto
    {
        public int TotalOffers { get; set; }
        public int PendingOffers { get; set; }
        public int ApprovedOffers { get; set; }
        public int TotalCoupons { get; set; }
        public int ActiveCoupons { get; set; }
        public int TotalUsers { get; set; }
        public int TotalMerchants { get; set; }
        public int TotalCustomers { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
