// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Application.DTOs.Reservations;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Reservations.Command.CreateReservation
{
    [ApplicationAuthorize(Role = Roles.Customer)]
    public record CreateReservationCommand(Guid OfferId) : IRequest<ReservationDto>;
}
