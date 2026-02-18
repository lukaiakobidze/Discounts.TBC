// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Reservation;
using MediatR;

namespace Discounts.Application.Features.Reservations.Command.CreateReservation
{
    public record CreateReservationCommand(Guid OfferId) : IRequest<ReservationDto>;
}
