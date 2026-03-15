// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Exceptions;
using Discounts.Application.Features.Favourites.Commands.AddFavourite;
using Discounts.Application.Features.Favourites.Commands.RemoveFavourite;
using Discounts.Application.Features.Favourites.Queries.GetMyFavourites;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Application.Tests.Mocks;
using Discounts.Domain.Entities;
using Discounts.Domain.Enums;
using Moq;

namespace Discounts.Application.Tests.Favourites;

public class AddFavouriteCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IOfferRepository> _offersRepoMock;
    private readonly Mock<IFavouriteRepository> _favouritesRepoMock;
    private readonly AddFavouriteCommandHandler _handler;

    private const string UserId = "customer-user-id";

    public AddFavouriteCommandHandlerTests()
    {
        var mocks = MockUnitOfWork.Create();
        _unitOfWorkMock = mocks.UnitOfWork;
        _offersRepoMock = mocks.Offers;
        _favouritesRepoMock = mocks.Favourites;

        _handler = new AddFavouriteCommandHandler(
            _unitOfWorkMock.Object,
            MockCurrentUserService.Create(UserId).Object);
    }

    [Fact]
    public async Task Handle_WhenOfferNotFound_ShouldThrowNotFoundException()
    {
        var command = new AddFavouriteCommand(Guid.NewGuid());
        _offersRepoMock.Setup(x => x.GetByIdAsync(command.OfferId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Offer?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WhenAlreadyFavourited_ShouldThrowConflictException()
    {
        var offer = new Offer { Id = Guid.NewGuid() };
        var command = new AddFavouriteCommand(offer.Id);
        _offersRepoMock.Setup(x => x.GetByIdAsync(offer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);
        _favouritesRepoMock.Setup(x => x.GetByCustomerAndOfferIdAsync(UserId, offer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Favourite());

        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldAddFavouriteAndIncrementCount()
    {
        var offer = new Offer { Id = Guid.NewGuid(), FavouriteCount = 2 };
        var command = new AddFavouriteCommand(offer.Id);
        _offersRepoMock.Setup(x => x.GetByIdAsync(offer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);
        _favouritesRepoMock.Setup(x => x.GetByCustomerAndOfferIdAsync(UserId, offer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Favourite?)null);
        _favouritesRepoMock.Setup(x => x.AddAsync(It.IsAny<Favourite>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Favourite());

        await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        Assert.Equal(3, offer.FavouriteCount);
        _favouritesRepoMock.Verify(x => x.AddAsync(
            It.Is<Favourite>(f => f.CustomerId == UserId && f.OfferId == offer.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class RemoveFavouriteCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IOfferRepository> _offersRepoMock;
    private readonly Mock<IFavouriteRepository> _favouritesRepoMock;
    private readonly RemoveFavouriteCommandHandler _handler;

    private const string UserId = "customer-user-id";

    public RemoveFavouriteCommandHandlerTests()
    {
        var mocks = MockUnitOfWork.Create();
        _unitOfWorkMock = mocks.UnitOfWork;
        _offersRepoMock = mocks.Offers;
        _favouritesRepoMock = mocks.Favourites;

        _handler = new RemoveFavouriteCommandHandler(
            _unitOfWorkMock.Object,
            MockCurrentUserService.Create(UserId).Object);
    }

    [Fact]
    public async Task Handle_WhenFavouriteNotFound_ShouldThrowNotFoundException()
    {
        var command = new RemoveFavouriteCommand(Guid.NewGuid());
        _favouritesRepoMock.Setup(x => x.GetByCustomerAndOfferIdAsync(UserId, command.OfferId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Favourite?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldRemoveFavouriteAndDecrementCount()
    {
        var offer = new Offer { Id = Guid.NewGuid(), FavouriteCount = 3 };
        var favourite = new Favourite { OfferId = offer.Id };
        var command = new RemoveFavouriteCommand(offer.Id);
        _favouritesRepoMock.Setup(x => x.GetByCustomerAndOfferIdAsync(UserId, offer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(favourite);
        _offersRepoMock.Setup(x => x.GetByIdAsync(offer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        Assert.Equal(2, offer.FavouriteCount);
        _favouritesRepoMock.Verify(x => x.Remove(favourite), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenFavouriteCountIsZero_ShouldNotGoNegative()
    {
        var offer = new Offer { Id = Guid.NewGuid(), FavouriteCount = 0 };
        var favourite = new Favourite { OfferId = offer.Id };
        var command = new RemoveFavouriteCommand(offer.Id);
        _favouritesRepoMock.Setup(x => x.GetByCustomerAndOfferIdAsync(UserId, offer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(favourite);
        _offersRepoMock.Setup(x => x.GetByIdAsync(offer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        Assert.Equal(0, offer.FavouriteCount);
    }
}

public class GetMyFavouritesQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IFavouriteRepository> _favouritesRepoMock;
    private readonly Mock<IOfferRepository> _offersRepoMock;
    private readonly GetMyFavouritesQueryHandler _handler;

    private const string UserId = "customer-user-id";

    public GetMyFavouritesQueryHandlerTests()
    {
        var mocks = MockUnitOfWork.Create();
        _unitOfWorkMock = mocks.UnitOfWork;
        _favouritesRepoMock = mocks.Favourites;
        _offersRepoMock = mocks.Offers;

        _handler = new GetMyFavouritesQueryHandler(
            _unitOfWorkMock.Object,
            MockCurrentUserService.Create(UserId).Object);
    }

    private static Favourite MakeFavourite(DateTime? validTo = null, OfferStatus status = OfferStatus.Active)
    {
        var offerId = Guid.NewGuid();
        return new Favourite
        {
            OfferId = offerId,
            CustomerId = UserId,
            Offer = new Offer
            {
                Id = offerId,
                Name = "Test Offer",
                Status = status,
                ValidTo = validTo ?? DateTime.UtcNow.AddDays(7),
                Category = new Category { Name = "Test Category" }
            }
        };
    }

    [Fact]
    public async Task Handle_WhenNoFavourites_ShouldReturnEmptyList()
    {
        _favouritesRepoMock.Setup(x => x.GetByCustomerIdAsync(UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Favourite>());

        var result = await _handler.Handle(new GetMyFavouritesQuery(), CancellationToken.None).ConfigureAwait(true);

        Assert.Empty(result);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithActiveFavourites_ShouldReturnOfferDtos()
    {
        var fav1 = MakeFavourite();
        var fav2 = MakeFavourite();
        _favouritesRepoMock.Setup(x => x.GetByCustomerIdAsync(UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Favourite> { fav1, fav2 });

        var result = await _handler.Handle(new GetMyFavouritesQuery(), CancellationToken.None).ConfigureAwait(true);

        Assert.Equal(2, result.Count);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenExpiredFavouritesExist_ShouldRemoveThemAndDecrementCount()
    {
        var activeFav = MakeFavourite();
        var expiredOffer = new Offer
        {
            Id = Guid.NewGuid(),
            Name = "Expired Offer",
            FavouriteCount = 5,
            Status = OfferStatus.Active,
            ValidTo = DateTime.UtcNow.AddDays(-1),
            Category = new Category { Name = "Cat" }
        };
        var expiredFav = new Favourite { OfferId = expiredOffer.Id, CustomerId = UserId, Offer = expiredOffer };

        _favouritesRepoMock.Setup(x => x.GetByCustomerIdAsync(UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Favourite> { activeFav, expiredFav });

        var result = await _handler.Handle(new GetMyFavouritesQuery(), CancellationToken.None).ConfigureAwait(true);

        Assert.Single(result);
        _favouritesRepoMock.Verify(x => x.Remove(expiredFav), Times.Once);
        Assert.Equal(4, expiredOffer.FavouriteCount);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoExpiredFavourites_ShouldNotCallSaveChanges()
    {
        var activeFav = MakeFavourite();
        _favouritesRepoMock.Setup(x => x.GetByCustomerIdAsync(UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Favourite> { activeFav });

        await _handler.Handle(new GetMyFavouritesQuery(), CancellationToken.None).ConfigureAwait(true);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenOfferHasExpiredStatus_ShouldTreatAsExpired()
    {
        var expiredStatusFav = MakeFavourite(validTo: DateTime.UtcNow.AddDays(5), status: OfferStatus.Expired);

        _favouritesRepoMock.Setup(x => x.GetByCustomerIdAsync(UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Favourite> { expiredStatusFav });

        var result = await _handler.Handle(new GetMyFavouritesQuery(), CancellationToken.None).ConfigureAwait(true);

        Assert.Empty(result);
        _favouritesRepoMock.Verify(x => x.Remove(expiredStatusFav), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
