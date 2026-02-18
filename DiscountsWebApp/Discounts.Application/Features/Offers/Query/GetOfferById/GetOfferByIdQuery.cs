// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Offers;
using MediatR;

namespace Discounts.Application.Features.Offers.Query.GetOfferById
{
    public record GetOfferByIdQuery(Guid Id) : IRequest<OfferDto>;

}
