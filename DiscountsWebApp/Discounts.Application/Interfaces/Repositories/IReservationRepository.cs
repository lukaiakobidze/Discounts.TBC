// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.Repositories
{
    public interface IReservationRepository : IRepository<Reservation>
    {
        Task<IReadOnlyList<Reservation>> GetByOfferId(Guid offerId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Reservation>> GetByCustomerId(string customerId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Reservation>> GetByOfferIdAndCustomerId(Guid offerId, string customerId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Reservation>> GetExpiredAsync(DateTime expireThreshold, CancellationToken cancellationToken = default);
    }
}
