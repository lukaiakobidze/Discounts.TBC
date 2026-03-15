// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Data.Context;
using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastructure.Repositories
{
    public class FavouriteRepository : GenericRepository<Favourite>, IFavouriteRepository
    {
        public FavouriteRepository(DiscountsDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Favourite>> GetByCustomerIdAsync(string customerId, CancellationToken ct = default)
            => await _dbSet.Where(f => f.CustomerId == customerId)
                .Include(f => f.Offer).ThenInclude(o => o.Category)
                .ToListAsync(ct).ConfigureAwait(false);

        public async Task<Favourite?> GetByCustomerAndOfferIdAsync(string customerId, Guid offerId, CancellationToken ct = default)
            => await _dbSet.FirstOrDefaultAsync(f => f.CustomerId == customerId && f.OfferId == offerId, ct).ConfigureAwait(false);

        public void Remove(Favourite favourite) => _dbSet.Remove(favourite);
    }
}
