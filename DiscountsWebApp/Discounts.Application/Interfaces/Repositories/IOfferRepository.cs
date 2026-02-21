// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Entities;
using Discounts.Domain.Enums;

namespace Discounts.Application.Interfaces.Repositories
{
    public interface IOfferRepository : IRepository<Offer>
    {
        Task<IReadOnlyList<Offer>> GetByStatusAsync(OfferStatus status, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Offer>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Offer>> GetByCategoryId(Guid categoryId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Offer>> SearchAsync(string? searchTerm, Guid? categoryId, decimal? minPrice, decimal? maxPrice, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Offer>> GetExpiredOffersAsync(DateTime now, CancellationToken cancellationToken = default);
    }
}
