// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Offers.Command.ApproveOffer
{
    [ApplicationAuthorize(Role = Roles.Admin)]
    public record ApproveOfferCommand(Guid OfferId) : IRequest<Unit>;
}
