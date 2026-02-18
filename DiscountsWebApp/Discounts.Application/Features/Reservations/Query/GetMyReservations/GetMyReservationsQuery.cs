// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Reservation;
using MediatR;

namespace Discounts.Application.Features.Reservations.Query.GetMyReservations
{
    public record GetMyReservationsQuery : IRequest<IReadOnlyList<ReservationDto>>;
}
