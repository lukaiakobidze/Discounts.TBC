// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Category;
using Discounts.Application.DTOs.Coupon;
using Discounts.Application.DTOs.Offers;
using Discounts.Application.DTOs.Reservation;
using Discounts.Domain.Entities;
using Mapster;

namespace Discounts.Application.Mappings
{
    public static class MappingConfig
    {
        public static void Configure()
        {
            TypeAdapterConfig<Offer, OfferDto>.NewConfig()
                .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : string.Empty);

            TypeAdapterConfig<Category, CategoryDto>.NewConfig();

            TypeAdapterConfig<Coupon, CouponDto>.NewConfig()
                .Map(dest => dest.OfferName, src => src.Offer != null ? src.Offer.Name : string.Empty);

            TypeAdapterConfig<Reservation, ReservationDto>.NewConfig();
        }
    }
}
