// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Discounts.Domain.Enums;
using Discounts.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastructure.Repositories
{
    public class OfferRepository : GenericRepository<Offer>, IOfferRepository
    {
        public OfferRepository(DiscountsDbContext dbContext) : base(dbContext) { }

        public async Task<IReadOnlyList<Offer>> GetByCategoryId(Guid categoryId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(o => o.CategoryId == categoryId).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        public async Task<IReadOnlyList<Offer>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(o => o.MerchantId == merchantId).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        public async Task<IReadOnlyList<Offer>> GetByStatusAsync(OfferStatus status, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(o => o.Status == status).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        public async Task<IReadOnlyList<Offer>> GetExpiredOffersAsync(DateTime now, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(o => o.ValidTo < now && o.Status != OfferStatus.Expired).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        public async Task<IReadOnlyList<Offer>> SearchAsync(string? searchTerm, Guid? categoryId, decimal? minPrice, decimal? maxPrice, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(x => x.Status == OfferStatus.Active).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(o => o.Name.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) || (o.Description != null && o.Description.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)));

            if (categoryId.HasValue)
                query = query.Where(o => o.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                query = query.Where(o => o.DiscountedPrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(o => o.DiscountedPrice <= maxPrice.Value);

            return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
