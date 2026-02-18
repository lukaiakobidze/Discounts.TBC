// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Discounts.Domain.Enums;
using MediatR;

namespace Discounts.Application.Features.Coupons.Command.UseCoupon
{
    public class UseCouponCommandHandler : IRequestHandler<UseCouponCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        public UseCouponCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Unit> Handle(UseCouponCommand request, CancellationToken cancellationToken)
        {
            var coupon = await _unitOfWork.Coupons.GetByCodeAsync(request.Code, cancellationToken).ConfigureAwait(false)
                ?? throw new NotFoundException(nameof(Coupon), request.Code);

            var customerId = _currentUser.UserId ?? throw new ForbiddenAccessException("You must be logged in to use a coupon.");

            if (coupon.CustomerId != customerId)
                throw new ForbiddenAccessException("You can only use your own coupons.");

            if (coupon.Status != CouponStatus.Active)
                throw new ConflictException("This coupon is not active.");

            coupon.Status = CouponStatus.Used;

            _unitOfWork.Coupons.Update(coupon);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
