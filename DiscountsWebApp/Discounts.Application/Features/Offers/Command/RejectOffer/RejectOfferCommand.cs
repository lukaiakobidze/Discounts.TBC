// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Offers.Command.RejectOffer
{
    [ApplicationAuthorize(Role = Roles.Admin)]
    public record RejectOfferCommand(Guid OfferId, string? Reason = null) : IRequest<Unit>;
}
