// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Offers;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Enums;
using Mapster;
using MediatR;

namespace Discounts.Application.Features.Favourites.Queries.GetMyFavourites
{
    public class GetMyFavouritesQueryHandler : IRequestHandler<GetMyFavouritesQuery, IReadOnlyList<OfferDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetMyFavouritesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<IReadOnlyList<OfferDto>> Handle(GetMyFavouritesQuery request, CancellationToken cancellationToken)
        {
            var customerId = _currentUserService.UserId!;

            var favourites = await _unitOfWork.Favourites.GetByCustomerIdAsync(customerId, cancellationToken).ConfigureAwait(false);

            var now = DateTime.UtcNow;
            var expired = favourites.Where(f => f.Offer.ValidTo < now || f.Offer.Status == OfferStatus.Expired).ToList();
            var active = favourites.Where(f => f.Offer.ValidTo >= now && f.Offer.Status != OfferStatus.Expired).ToList();

            if (expired.Count > 0)
            {
                foreach (var f in expired)
                {
                    _unitOfWork.Favourites.Remove(f);
                    f.Offer.FavouriteCount = Math.Max(0, f.Offer.FavouriteCount - 1);
                    _unitOfWork.Offers.Update(f.Offer);
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            return active.Select(f =>
            {
                var dto = f.Offer.Adapt<OfferDto>();
                dto.CategoryName = f.Offer.Category?.Name ?? string.Empty;
                return dto;
            }).ToList().AsReadOnly();
        }
    }
}
