// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Exceptions;
using Discounts.Application.Features.Coupons.Command.PurchaseCoupon;
using Discounts.Application.Features.Coupons.Command.UseCoupon;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Application.Tests.Mocks;
using Discounts.Domain.Entities;
using Discounts.Domain.Enums;
using MediatR;
using Moq;

namespace Discounts.Application.Tests.Coupons;

public class PurchaseCouponCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IOfferRepository> _offersRepoMock;
    private readonly Mock<ICouponRepository> _couponsRepoMock;
    private readonly Mock<IReservationRepository> _reservationsRepoMock;
    private readonly Mock<IDateTimeProvider> _dateTimeMock;
    private readonly PurchaseCouponCommandHandler _handler;

    private const string UserId = "customer-user-id";

    public PurchaseCouponCommandHandlerTests()
    {
        var mocks = MockUnitOfWork.Create();
        _unitOfWorkMock = mocks.UnitOfWork;
        _offersRepoMock = mocks.Offers;
        _couponsRepoMock = mocks.Coupons;
        _reservationsRepoMock = mocks.Reservations;
        _dateTimeMock = MockDateTimeProvider.Create();

        _handler = new PurchaseCouponCommandHandler(
            _unitOfWorkMock.Object,
            MockCurrentUserService.Create(UserId).Object,
            _dateTimeMock.Object);
    }

    private static Offer CreateActiveOffer(int remaining = 5) => new()
    {
        Id = Guid.NewGuid(),
        Name = "Test Offer",
        Status = OfferStatus.Active,
        RemainingCount = remaining,
        ValidTo = DateTime.UtcNow.AddDays(1)
    };

    [Fact]
    public async Task Handle_WhenOfferNotFound_ShouldThrowNotFoundException()
    {
        var command = new PurchaseCouponCommand(Guid.NewGuid());
        _offersRepoMock.Setup(x => x.GetByIdAsync(command.OfferId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Offer?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WhenOfferIsNotActive_ShouldThrowConflictException()
    {
        var offer = CreateActiveOffer();
        offer.Status = OfferStatus.Pending;
        var command = new PurchaseCouponCommand(offer.Id);
        _offersRepoMock.Setup(x => x.GetByIdAsync(command.OfferId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WhenNoRemainingCoupons_ShouldThrowConflictException()
    {
        var offer = CreateActiveOffer(remaining: 0);
        var command = new PurchaseCouponCommand(offer.Id);
        _offersRepoMock.Setup(x => x.GetByIdAsync(command.OfferId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WhenOfferIsExpired_ShouldThrowConflictException()
    {
        var offer = CreateActiveOffer();
        offer.ValidTo = DateTime.UtcNow.AddDays(-1);
        var command = new PurchaseCouponCommand(offer.Id);
        _offersRepoMock.Setup(x => x.GetByIdAsync(command.OfferId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WhenCouponAlreadyPurchased_ShouldThrowConflictException()
    {
        var offer = CreateActiveOffer();
        var command = new PurchaseCouponCommand(offer.Id);
        _offersRepoMock.Setup(x => x.GetByIdAsync(offer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);
        _couponsRepoMock.Setup(x => x.GetByCustomerAndOfferIdAsync(offer.Id, UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Coupon());

        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateCouponAndDecreaseRemainingCount()
    {
        var offer = CreateActiveOffer(remaining: 3);
        var initialRemaining = offer.RemainingCount;
        var command = new PurchaseCouponCommand(offer.Id);
        _offersRepoMock.Setup(x => x.GetByIdAsync(offer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);
        _couponsRepoMock.Setup(x => x.GetByCustomerAndOfferIdAsync(offer.Id, UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coupon?)null);
        _couponsRepoMock.Setup(x => x.AddAsync(It.IsAny<Coupon>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Coupon());
        _reservationsRepoMock.Setup(x => x.GetByOfferIdAndCustomerId(offer.Id, UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation>());

        var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        Assert.Equal(initialRemaining - 1, offer.RemainingCount);
        Assert.Equal(offer.Name, result.OfferName);
        _couponsRepoMock.Verify(x => x.AddAsync(It.IsAny<Coupon>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenActiveReservationsExist_ShouldDeleteThemAndRestoreCounts()
    {
        var offer = CreateActiveOffer(remaining: 2);
        var command = new PurchaseCouponCommand(offer.Id);
        var reservation = new Reservation { IsDeleted = false, Offer = offer };
        _offersRepoMock.Setup(x => x.GetByIdAsync(offer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);
        _couponsRepoMock.Setup(x => x.GetByCustomerAndOfferIdAsync(offer.Id, UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coupon?)null);
        _couponsRepoMock.Setup(x => x.AddAsync(It.IsAny<Coupon>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Coupon());
        _reservationsRepoMock.Setup(x => x.GetByOfferIdAndCustomerId(offer.Id, UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { reservation });

        await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        _reservationsRepoMock.Verify(x => x.Delete(reservation), Times.Once);
    }
}

public class UseCouponCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICouponRepository> _couponsRepoMock;
    private readonly UseCouponCommandHandler _handler;

    private const string UserId = "the-current-user";

    public UseCouponCommandHandlerTests()
    {
        var mocks = MockUnitOfWork.Create();
        _unitOfWorkMock = mocks.UnitOfWork;
        _couponsRepoMock = mocks.Coupons;

        _handler = new UseCouponCommandHandler(
            _unitOfWorkMock.Object,
            MockCurrentUserService.Create(UserId).Object);
    }

    [Fact]
    public async Task Handle_WhenCouponNotFound_ShouldThrowNotFoundException()
    {
        _couponsRepoMock.Setup(x => x.GetByCodeAsync("INVALID", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coupon?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new UseCouponCommand("INVALID"), CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WhenCouponBelongsToDifferentUser_ShouldThrowForbiddenException()
    {
        var coupon = new Coupon { Code = "ABC", CustomerId = "another-user", Status = CouponStatus.Active };
        _couponsRepoMock.Setup(x => x.GetByCodeAsync("ABC", It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            _handler.Handle(new UseCouponCommand("ABC"), CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WhenCouponIsNotActive_ShouldThrowConflictException()
    {
        var coupon = new Coupon { Code = "ABC", CustomerId = UserId, Status = CouponStatus.Used };
        _couponsRepoMock.Setup(x => x.GetByCodeAsync("ABC", It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        await Assert.ThrowsAsync<ConflictException>(() =>
            _handler.Handle(new UseCouponCommand("ABC"), CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WithValidCoupon_ShouldMarkCouponAsUsed()
    {
        var coupon = new Coupon { Code = "ABC", CustomerId = UserId, Status = CouponStatus.Active };
        _couponsRepoMock.Setup(x => x.GetByCodeAsync("ABC", It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        var result = await _handler.Handle(new UseCouponCommand("ABC"), CancellationToken.None).ConfigureAwait(true);

        Assert.Equal(CouponStatus.Used, coupon.Status);
        _couponsRepoMock.Verify(x => x.Update(coupon), Times.Once);
        Assert.Equal(Unit.Value, result);
    }
}
