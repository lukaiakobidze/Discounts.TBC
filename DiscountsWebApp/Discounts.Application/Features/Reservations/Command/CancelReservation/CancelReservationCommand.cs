// Copyright (C) TBC Bank. All Rights Reserved.

using MediatR;

namespace Discounts.Application.Features.Reservations.Command.CancelReservation
{
    public record CancelReservationCommand(Guid Id) : IRequest<Unit>;
}
