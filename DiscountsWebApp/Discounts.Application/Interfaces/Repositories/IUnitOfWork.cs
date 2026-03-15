// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        IOfferRepository Offers { get; }
        IReservationRepository Reservations { get; }
        ICouponRepository Coupons { get; }
        ICategoryRepository Categories { get; }
        IGlobalSettingRepository GlobalSettings { get; }
        IFavouriteRepository Favourites { get; }
        IReviewRepository Reviews { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
