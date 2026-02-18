// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Coupon;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Discounts.Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Application.Features.Coupons.Command.PurchaseCoupon
{
    public class PurchaseCouponCommandHandler : IRequestHandler<PurchaseCouponCommand, CouponDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeProvider _dateTime;
        public PurchaseCouponCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IDateTimeProvider dateTime)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }
        public async Task<CouponDto> Handle(PurchaseCouponCommand request, CancellationToken cancellationToken)
        {
            var user = _currentUserService.UserId ?? throw new ForbiddenAccessException();

            var offer = await _unitOfWork.Offers.GetByIdAsync(request.OfferId, cancellationToken).ConfigureAwait(false)
                ?? throw new NotFoundException(nameof(Offer), request.OfferId);

            if (offer.Status != OfferStatus.Active)
                throw new ConflictException("This offer is not available for purchase.");

            if (offer.RemainingCount <= 0)
                throw new ConflictException("All coupons are sold out");

            if (_dateTime.UtcNow > offer.ValidTo)
                throw new ConflictException("This offer has expired.");

            //should add method for getting a coupon by offer id and customer id to avoid loading all coupons of the customer
            var myCoupons = await _unitOfWork.Coupons.GetByCustomerIdAsync(user, cancellationToken).ConfigureAwait(false);
            if (myCoupons.Any(c => c.OfferId == offer.Id))
                throw new ConflictException($"Customer with id:{user} has already purchased a coupon for the offer with id:{offer.Id}");

            var coupon = new Coupon
            {
                Code = GenerateUniqueCode(),
                OfferId = offer.Id,
                CustomerId = user,
                PurchaseDate = _dateTime.UtcNow,
                Status = CouponStatus.Active
            };

            offer.RemainingCount -= 1;
            _unitOfWork.Offers.Update(offer);

            await _unitOfWork.Coupons.AddAsync(coupon, cancellationToken).ConfigureAwait(false);

            var reservation = await _unitOfWork.Reservations.GetByOfferIdAndCustomerId(offer.Id, user, cancellationToken).ConfigureAwait(false);
            if (reservation is not null)
            {
                _unitOfWork.Reservations.Delete(reservation);
            }

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConflictException("This offer was modified by another request. Please try again.");
            }

            var dto = coupon.Adapt<CouponDto>();
            dto.OfferName = offer.Name;
            return dto;
        }
        private static string GenerateUniqueCode()
        {
            return $"CPN-{Guid.NewGuid().ToString("N")[..12].ToUpper()}";
        }
    }
}
