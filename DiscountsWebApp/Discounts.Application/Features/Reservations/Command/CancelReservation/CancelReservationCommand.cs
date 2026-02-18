// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Reservations.Command.CancelReservation
{
    [ApplicationAuthorize(Role = Roles.Customer)]
    public record CancelReservationCommand(Guid Id) : IRequest<Unit>;
}
