// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using MediatR;

namespace Discounts.Application.Features.Reservations.Command.CancelReservation
{
    public class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CancelReservationCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var reservation = await _unitOfWork.Reservations.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);

            if (reservation == null)
                throw new NotFoundException(nameof(reservation), request.Id);

            if (reservation.CustomerId != userId)
                throw new ForbiddenAccessException("You can only cancel your own reservations");

            if (reservation.IsDeleted)
                throw new ConflictException("reservaiton is already canceled");

            reservation.Offer.RemainingCount += 1;
            _unitOfWork.Offers.Update(reservation.Offer);

            _unitOfWork.Reservations.Delete(reservation);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
