// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Application.DTOs.Offers;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Offers.Command.UpdateOffer
{
    [ApplicationAuthorize(Role = Roles.Merchant)]
    public record UpdateOfferCommand(
        Guid Id,
        string Title,
        string? Description,
        string? ImagePath,
        decimal OriginalPrice,
        decimal DiscountedPrice,
        int CouponQuantity,
        DateTime ValidFrom,
        DateTime ValidTo,
        Guid CategoryId
    ) : IRequest<OfferDto>;
}
