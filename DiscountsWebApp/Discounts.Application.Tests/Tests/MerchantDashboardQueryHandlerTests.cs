// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Features.Stats.Query.MerchantDashboard;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Discounts.Domain.Enums;
using MockQueryable;
using Moq;

namespace Discounts.Application.Tests.Stats;

public class MerchantDashboardQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICurrentUserService> _currentUserMock = new();
    private readonly Mock<IOfferRepository> _offersRepoMock = new();
    private readonly MerchantDashboardQueryHandler _handler;
    private const string MerchantId = "merchant-123";

    public MerchantDashboardQueryHandlerTests()
    {
        _unitOfWorkMock.Setup(x => x.Offers).Returns(_offersRepoMock.Object);
        _currentUserMock.Setup(x => x.UserId).Returns(MerchantId);
        _handler = new MerchantDashboardQueryHandler(_currentUserMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCalculateCorrectStats()
    {
        var offers = new List<Offer>
        {
            new()
            {
                MerchantId = MerchantId,
                Status = OfferStatus.Active,
                DiscountedPrice = 50m,
                Coupons = new List<Coupon> { new(), new() }
            },
            new()
            {
                MerchantId = MerchantId,
                Status = OfferStatus.Pending,
                DiscountedPrice = 30m,
                Coupons = new List<Coupon> { new() }
            }
        };

        _offersRepoMock.Setup(x => x.Query()).Returns(offers.BuildMock());

        var result = await _handler.Handle(new MerchantDashboardQuery(), CancellationToken.None).ConfigureAwait(true);

        Assert.Equal(2, result.TotalOffers);
        Assert.Equal(1, result.ActiveOffers);
        Assert.Equal(1, result.PendingOffers);
        Assert.Equal(3, result.TotalCouponsSold);
        Assert.Equal(130m, result.TotalRevenue);
    }
}
