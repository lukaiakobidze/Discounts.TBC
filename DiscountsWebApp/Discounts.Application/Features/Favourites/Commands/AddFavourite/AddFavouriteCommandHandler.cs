// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Features.Favourites.Commands.AddFavourite
{
    public class AddFavouriteCommandHandler : IRequestHandler<AddFavouriteCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AddFavouriteCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task Handle(AddFavouriteCommand request, CancellationToken cancellationToken)
        {
            var customerId = _currentUserService.UserId!;

            var offer = await _unitOfWork.Offers.GetByIdAsync(request.OfferId, cancellationToken).ConfigureAwait(false)
                ?? throw new NotFoundException(nameof(Offer), request.OfferId);

            var existing = await _unitOfWork.Favourites.GetByCustomerAndOfferIdAsync(customerId, request.OfferId, cancellationToken).ConfigureAwait(false);
            if (existing != null)
                throw new ConflictException("You have already favourited this offer.");

            var favourite = new Favourite
            {
                CustomerId = customerId,
                OfferId = request.OfferId
            };

            await _unitOfWork.Favourites.AddAsync(favourite, cancellationToken).ConfigureAwait(false);

            offer.FavouriteCount++;
            _unitOfWork.Offers.Update(offer);

            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
