// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Application.DTOs.Offers;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Favourites.Queries.GetMyFavourites
{
    [ApplicationAuthorize(Role = Roles.Customer)]
    public record GetMyFavouritesQuery : IRequest<IReadOnlyList<OfferDto>>;
}
