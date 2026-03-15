// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.Repositories
{
    public interface IFavouriteRepository : IRepository<Favourite>
    {
        Task<IReadOnlyList<Favourite>> GetByCustomerIdAsync(string customerId, CancellationToken ct = default);
        Task<Favourite?> GetByCustomerAndOfferIdAsync(string customerId, Guid offerId, CancellationToken ct = default);
        void Remove(Favourite favourite);
    }
}
