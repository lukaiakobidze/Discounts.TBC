// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Constants;
using Discounts.Application.DTOs.Offers;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Discounts.Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Discounts.Application.Features.Offers.Command.CreateOffer
{
    public class CreateOfferCommandHandler : IRequestHandler<CreateOfferCommand, OfferDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly IMemoryCache _cache;

        public CreateOfferCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _cache = cache;
        }

        public async Task<OfferDto> Handle(CreateOfferCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken).ConfigureAwait(false)
                ?? throw new NotFoundException(nameof(Category), request.CategoryId);

            var offer = new Offer
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                ImagePath = request.ImagePath,
                OriginalPrice = request.OriginalPrice,
                DiscountedPrice = request.DiscountedPrice,
                TotalCount = request.TotalCount,
                RemainingCount = request.TotalCount,
                Status = OfferStatus.Pending,
                ValidFrom = request.ValidFrom,
                ValidTo = request.ValidTo,
                MerchantId = _currentUser.UserId!,
                CategoryId = request.CategoryId
            };

            await _unitOfWork.Offers.AddAsync(offer, cancellationToken).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            _cache.Remove($"{CacheKeys.MerchantOffers}{_currentUser.UserId}");
            _cache.Remove(CacheKeys.AdminDashboard);

            var dto = offer.Adapt<OfferDto>();
            dto.CategoryName = category.Name;
            return dto;
        }
    }
}
