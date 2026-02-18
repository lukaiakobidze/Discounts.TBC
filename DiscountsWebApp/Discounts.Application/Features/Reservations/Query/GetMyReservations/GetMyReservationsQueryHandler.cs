// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Reservation;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Mapster;
using MediatR;

namespace Discounts.Application.Features.Reservations.Query.GetMyReservations
{
    public class GetMyReservationsQueryHandler : IRequestHandler<GetMyReservationsQuery, IReadOnlyList<ReservationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetMyReservationsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }
        public async Task<IReadOnlyList<ReservationDto>> Handle(GetMyReservationsQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
                throw new ForbiddenAccessException("You need to be logged in to see your reservations");

            var reservations = await _unitOfWork.Reservations.GetByCustomerId(_currentUser.UserId, cancellationToken).ConfigureAwait(false);

            var dtos = reservations.Select(x =>
            {
                var dto = x.Adapt<ReservationDto>();
                dto.OfferName = x.Offer.Name;
                dto.OfferDiscountedPrice = x.Offer.DiscountedPrice;
                return dto;
            }).ToList();

            return dtos;
        }
    }
}
