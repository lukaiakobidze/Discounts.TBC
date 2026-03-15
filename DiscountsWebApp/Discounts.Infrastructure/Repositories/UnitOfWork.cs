// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Data.Context;

namespace Discounts.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DiscountsDbContext _dbContext;
        public UnitOfWork(DiscountsDbContext dbContext, IOfferRepository offers, IReservationRepository reservations,
            ICouponRepository coupons, ICategoryRepository categories, IGlobalSettingRepository globalSettings,
            IFavouriteRepository favourites, IReviewRepository reviews)
        {
            _dbContext = dbContext;
            Offers = offers;
            Reservations = reservations;
            Coupons = coupons;
            Categories = categories;
            GlobalSettings = globalSettings;
            Favourites = favourites;
            Reviews = reviews;
        }

        public IOfferRepository Offers { get; }

        public IReservationRepository Reservations { get; }

        public ICouponRepository Coupons { get; }

        public ICategoryRepository Categories { get; }

        public IGlobalSettingRepository GlobalSettings { get; }

        public IFavouriteRepository Favourites { get; }

        public IReviewRepository Reviews { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
