// Copyright (C) TBC Bank. All Rights Reserved.

using MediatR;

namespace Discounts.Application.Features.Offers.Command.RejectOffer
{
    public record RejectOfferCommand(Guid OfferId) : IRequest<Unit>;
}
