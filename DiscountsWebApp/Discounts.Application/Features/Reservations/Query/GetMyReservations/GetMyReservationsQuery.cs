// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Application.DTOs.Reservation;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Reservations.Query.GetMyReservations
{
    [ApplicationAuthorize(Role = Roles.Customer)]
    public record GetMyReservationsQuery : IRequest<IReadOnlyList<ReservationDto>>;
}
