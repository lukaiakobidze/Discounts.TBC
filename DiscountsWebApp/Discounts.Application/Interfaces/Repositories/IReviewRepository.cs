// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.Repositories
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IReadOnlyList<Review>> GetByOfferIdAsync(Guid offerId, CancellationToken ct = default);
        Task<Review?> GetByCouponIdAsync(Guid couponId, CancellationToken ct = default);
    }
}
