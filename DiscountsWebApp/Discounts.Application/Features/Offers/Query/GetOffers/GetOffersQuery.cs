// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Offers;
using Discounts.Application.Models;
using MediatR;

namespace Discounts.Application.Features.Offers.Query.GetOffers
{
    public record GetOffersQuery(int PageNumber = 10, int PageSize = 10) : IRequest<PaginatedList<OfferDto>>;
}
