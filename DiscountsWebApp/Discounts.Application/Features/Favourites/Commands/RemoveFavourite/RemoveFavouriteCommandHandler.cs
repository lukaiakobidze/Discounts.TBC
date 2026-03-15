// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Features.Favourites.Commands.RemoveFavourite
{
    public class RemoveFavouriteCommandHandler : IRequestHandler<RemoveFavouriteCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public RemoveFavouriteCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task Handle(RemoveFavouriteCommand request, CancellationToken cancellationToken)
        {
            var customerId = _currentUserService.UserId!;

            var favourite = await _unitOfWork.Favourites.GetByCustomerAndOfferIdAsync(customerId, request.OfferId, cancellationToken).ConfigureAwait(false)
                ?? throw new NotFoundException(nameof(Favourite), request.OfferId);

            _unitOfWork.Favourites.Remove(favourite);

            var offer = await _unitOfWork.Offers.GetByIdAsync(request.OfferId, cancellationToken).ConfigureAwait(false);
            if (offer != null)
            {
                offer.FavouriteCount = Math.Max(0, offer.FavouriteCount - 1);
                _unitOfWork.Offers.Update(offer);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
