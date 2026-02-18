// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Discounts.Domain.Enums;
using Discounts.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastructure.Repositories
{
    public class CouponRepository : GenericRepository<Coupon>, ICouponRepository
    {
        public CouponRepository(DiscountsDbContext dbContext) : base(dbContext) { }

        public Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return _dbSet.FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
        }
        public async Task<IReadOnlyList<Coupon>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(c => c.CustomerId == customerId).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        public async Task<IReadOnlyList<Coupon>> GetByOfferIdAsync(Guid offerId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(c => c.OfferId == offerId).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        public async Task<IReadOnlyList<Coupon>> GetByStatusAsync(CouponStatus status, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(c => c.Status == status).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
