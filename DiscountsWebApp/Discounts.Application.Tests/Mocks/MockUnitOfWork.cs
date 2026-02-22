// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Moq;

namespace Discounts.Application.Tests.Mocks
{
    public static class MockUnitOfWork
    {
        public static (Mock<IUnitOfWork> UnitOfWork,
                       Mock<IOfferRepository> Offers,
                       Mock<ICategoryRepository> Categories,
                       Mock<ICouponRepository> Coupons,
                       Mock<IReservationRepository> Reservations,
                       Mock<IGlobalSettingRepository> GlobalSettings) Create()
        {
            var offers = new Mock<IOfferRepository>();
            var categories = new Mock<ICategoryRepository>();
            var coupons = new Mock<ICouponRepository>();
            var reservations = new Mock<IReservationRepository>();
            var globalSettings = new Mock<IGlobalSettingRepository>();
            var uow = new Mock<IUnitOfWork>();

            uow.Setup(u => u.Offers).Returns(offers.Object);
            uow.Setup(u => u.Categories).Returns(categories.Object);
            uow.Setup(u => u.Coupons).Returns(coupons.Object);
            uow.Setup(u => u.Reservations).Returns(reservations.Object);
            uow.Setup(u => u.GlobalSettings).Returns(globalSettings.Object);
            uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            return (uow, offers, categories, coupons, reservations, globalSettings);
        }
    }
}
