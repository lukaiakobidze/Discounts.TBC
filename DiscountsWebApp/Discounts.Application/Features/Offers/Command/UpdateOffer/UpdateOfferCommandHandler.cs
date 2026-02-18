// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Offers;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using Mapster;
using MediatR;

namespace Discounts.Application.Features.Offers.Command.UpdateOffer
{
    public class UpdateOfferCommandHandler : IRequestHandler<UpdateOfferCommand, OfferDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly IDateTimeProvider _dateTime;

        public UpdateOfferCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IDateTimeProvider dateTime)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _dateTime = dateTime;
        }

        public async Task<OfferDto> Handle(UpdateOfferCommand request, CancellationToken cancellationToken)
        {
            var offer = await _unitOfWork.Offers.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false)
                ?? throw new NotFoundException(nameof(Offer), request.Id);

            if (offer.MerchantId != _currentUser.UserId)
                throw new ForbiddenAccessException("You can only edit your own offers.");

            var editWindowHours = await _unitOfWork.GlobalSettings.GetIntValueAsync(GlobalSettingConstants.MerchantEditWindowHours, 24, cancellationToken).ConfigureAwait(false);
            if ((_dateTime.UtcNow - offer.CreatedAt).TotalHours > editWindowHours)
                throw new ForbiddenAccessException($"Offers can only be edited within {editWindowHours} hours of creation.");

            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken).ConfigureAwait(false)
                ?? throw new NotFoundException(nameof(Category), request.CategoryId);

            offer.Name = request.Title;
            offer.Description = request.Description;
            offer.ImagePath = request.ImagePath;
            offer.OriginalPrice = request.OriginalPrice;
            offer.DiscountedPrice = request.DiscountedPrice;
            offer.TotalCount = request.CouponQuantity;
            offer.RemainingCount = request.CouponQuantity;
            offer.ValidFrom = request.ValidFrom;
            offer.ValidTo = request.ValidTo;
            offer.CategoryId = request.CategoryId;

            _unitOfWork.Offers.Update(offer);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var dto = offer.Adapt<OfferDto>();
            dto.CategoryName = category.Name;
            return dto;
        }
    }
}
