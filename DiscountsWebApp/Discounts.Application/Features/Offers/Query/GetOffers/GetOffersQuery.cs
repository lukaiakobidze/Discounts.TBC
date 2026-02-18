// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Application.DTOs.Offers;
using Discounts.Application.Models;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Offers.Query.GetOffers
{
    [ApplicationAuthorize(Role = Roles.Admin)]
    public record GetOffersQuery(int PageNumber = 10, int PageSize = 10) : IRequest<PaginatedList<OfferDto>>;
}
