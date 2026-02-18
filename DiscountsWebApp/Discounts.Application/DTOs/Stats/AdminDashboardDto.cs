// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.DTOs.Stats
{
    public class AdminDashboardDto
    {
        public int TotalCustomers { get; set; }
        public int TotalMerchants { get; set; }
        public int ActiveOffers { get; set; }
        public int PendingOffers { get; set; }

    }
}
