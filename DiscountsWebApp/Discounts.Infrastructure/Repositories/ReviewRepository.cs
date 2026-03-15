// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Data.Context;
using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastructure.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(DiscountsDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Review>> GetByOfferIdAsync(Guid offerId, CancellationToken ct = default)
            => await _dbSet.Where(r => r.OfferId == offerId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(ct).ConfigureAwait(false);

        public async Task<Review?> GetByCouponIdAsync(Guid couponId, CancellationToken ct = default)
            => await _dbSet.FirstOrDefaultAsync(r => r.CouponId == couponId, ct).ConfigureAwait(false);
    }
}
