// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.Constants;

public static class CacheKeys
{
    public const string AllCategories     = "categories:all";
    public const string AdminDashboard    = "admin:dashboard";
    public const string PendingOffers     = "offers:pending";
    public const string OfferById         = "offers:id:";           // + offerId
    public const string OfferReviews      = "offers:reviews:";      // + offerId
    public const string MerchantDashboard = "merchant:dashboard:";  // + merchantId
    public const string MerchantOffers    = "merchant:offers:";     // + merchantId
}
