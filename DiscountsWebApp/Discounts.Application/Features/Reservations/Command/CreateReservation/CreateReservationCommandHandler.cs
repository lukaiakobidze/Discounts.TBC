// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Reservation;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Mapster;
using MediatR;

namespace Discounts.Application.Features.Reservations.Command.CreateReservation
{
    public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, ReservationDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CreateReservationCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<ReservationDto> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
        {
            if(_currentUserService.UserId == null)
                throw new ForbiddenAccessException("You need to log in to make a reservation");

            var offer = await _unitOfWork.Offers.GetByIdAsync(request.OfferId, cancellationToken).ConfigureAwait(false);

            if (offer == null)
                throw new NotFoundException(nameof(offer), request.OfferId);

            var existingReservation = await _unitOfWork.Reservations.GetByOfferIdAndCustomerId(request.OfferId, _currentUserService.UserId, cancellationToken).ConfigureAwait(false);

            if (existingReservation != null)
                throw new ConflictException("You can only make a reservation once for an offer.");

            if (offer.RemainingCount < 1)
                throw new ConflictException("No remaining coupons to reserve");

            var duration = await _unitOfWork.GlobalSettings.GetIntValueAsync("ReservationDurationMinutes", 30, cancellationToken).ConfigureAwait(false);

            var reservation = new Reservation()
            {
                Id = Guid.NewGuid(),
                OfferId = request.OfferId,
                CustomerId = _currentUserService.UserId,
                ExpirationDate = _dateTimeProvider.UtcNow.AddMinutes(duration),
            };

            await _unitOfWork.Reservations.AddAsync(reservation, cancellationToken).ConfigureAwait(false);

            offer.RemainingCount -= 1;
            _unitOfWork.Offers.Update(offer);

            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var dto = reservation.Adapt<ReservationDto>();
            dto.OfferName = offer.Name;
            dto.OfferDiscountedPrice = offer.DiscountedPrice;

            return dto;
        }
    }
}
