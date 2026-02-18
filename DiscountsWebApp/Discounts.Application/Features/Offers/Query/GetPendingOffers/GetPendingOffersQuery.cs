// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Application.DTOs.Offers;
using Discounts.Application.Models;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Offers.Query.GetPendingOffers
{
    [ApplicationAuthorize(Role = Roles.Admin)]
    public record GetPendingOffersQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedList<OfferDto>>;
}
