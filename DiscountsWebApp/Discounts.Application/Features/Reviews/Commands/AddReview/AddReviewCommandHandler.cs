// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Constants;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Discounts.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Discounts.Application.Features.Reviews.Commands.AddReview
{
    public class AddReviewCommandHandler : IRequestHandler<AddReviewCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMemoryCache _cache;

        public AddReviewCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _cache = cache;
        }

        public async Task<Unit> Handle(AddReviewCommand request, CancellationToken cancellationToken)
        {
            var customerId = _currentUserService.UserId!;

            var coupon = await _unitOfWork.Coupons.GetByIdAsync(request.CouponId, cancellationToken).ConfigureAwait(false)
                ?? throw new NotFoundException(nameof(Coupon), request.CouponId);

            if (coupon.CustomerId != customerId)
                throw new ForbiddenAccessException();

            if (coupon.Status != CouponStatus.Used)
                throw new ConflictException("You can only review an offer after using your coupon.");

            var existing = await _unitOfWork.Reviews.GetByCouponIdAsync(request.CouponId, cancellationToken).ConfigureAwait(false);
            if (existing != null)
                throw new ConflictException("You have already reviewed this offer.");

            var review = new Review
            {
                CustomerId = customerId,
                OfferId = coupon.OfferId,
                CouponId = request.CouponId,
                Stars = request.Stars,
                Comment = request.Comment
            };

            await _unitOfWork.Reviews.AddAsync(review, cancellationToken).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            _cache.Remove($"{CacheKeys.OfferReviews}{coupon.OfferId}");

            return Unit.Value;
        }
    }
}
