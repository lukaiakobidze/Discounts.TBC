// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Discounts.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastructure.Repositories
{
    public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
    {
        public ReservationRepository(DiscountsDbContext dbContext) : base(dbContext) { }

        public async Task<IReadOnlyList<Reservation>> GetByCustomerId(string customerId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(r => r.CustomerId == customerId).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        public async Task<IReadOnlyList<Reservation>> GetByOfferId(Guid offerId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(r => r.OfferId == offerId).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        public Task<Reservation?> GetByOfferIdAndCustomerId(Guid offerId, string customerId, CancellationToken cancellationToken = default)
        {
            return _dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.OfferId == offerId && r.CustomerId == customerId, cancellationToken);
        }
        public async Task<IReadOnlyList<Reservation>> GetExpiredAsync(DateTime expireThreshold, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(r => r.CreatedAt < expireThreshold).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
