// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Entities;
using Discounts.Domain.Enums;

namespace Discounts.Application.Interfaces.Repositories
{
    public interface ICouponRepository : IRepository<Coupon>
    {
        Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Coupon>> GetByStatusAsync(CouponStatus status, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Coupon>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Coupon>> GetByOfferIdAsync(Guid offerId, CancellationToken cancellationToken = default);
    }
}
