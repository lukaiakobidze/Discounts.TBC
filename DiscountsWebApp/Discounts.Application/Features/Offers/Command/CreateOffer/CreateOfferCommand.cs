// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Offers;
using MediatR;

namespace Discounts.Application.Features.Offers.Command.CreateOffer
{
    public record CreateOfferCommand(
        string Name,
        string Description,
        string? ImagePath,
        decimal OriginalPrice,
        decimal DiscountedPrice,
        int TotalCount,
        DateTime ValidFrom,
        DateTime ValidTo,
        Guid CategoryId
    ) : IRequest<OfferDto>;
}
