// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Exceptions;
using Discounts.Application.Features.Reservations.Command.CancelReservation;
using Discounts.Application.Features.Reservations.Command.CreateReservation;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Application.Tests.Mocks;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using FluentValidation.TestHelper;
using MediatR;
using Moq;

namespace Discounts.Application.Tests.Reservations;

public class CancelReservationCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IReservationRepository> _reservationsRepoMock;
    private readonly Mock<IOfferRepository> _offersRepoMock;
    private readonly CancelReservationCommandHandler _handler;
    private const string UserId = "user-abc";

    public CancelReservationCommandHandlerTests()
    {
        var mocks = MockUnitOfWork.Create();
        _unitOfWorkMock = mocks.UnitOfWork;
        _reservationsRepoMock = mocks.Reservations;
        _offersRepoMock = mocks.Offers;

        _handler = new CancelReservationCommandHandler(
            _unitOfWorkMock.Object,
            MockCurrentUserService.Create(UserId).Object);
    }

    [Fact]
    public async Task Handle_WhenReservationNotFound_ShouldThrowNotFoundException()
    {
        var command = new CancelReservationCommand(Guid.NewGuid());
        _reservationsRepoMock.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reservation?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WhenReservationBelongsToAnotherUser_ShouldThrowForbiddenException()
    {
        var reservation = new Reservation { Id = Guid.NewGuid(), CustomerId = "other-user" };
        var command = new CancelReservationCommand(reservation.Id);
        _reservationsRepoMock.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservation);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WhenReservationIsAlreadyCanceled_ShouldThrowConflictException()
    {
        var reservation = new Reservation { Id = Guid.NewGuid(), CustomerId = UserId, IsDeleted = true };
        var command = new CancelReservationCommand(reservation.Id);
        _reservationsRepoMock.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservation);

        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldDeleteReservationAndRestoreOfferCount()
    {
        var offer = new Offer { Id = Guid.NewGuid(), RemainingCount = 2 };
        var reservation = new Reservation { Id = Guid.NewGuid(), CustomerId = UserId, IsDeleted = false, Offer = offer };
        var command = new CancelReservationCommand(reservation.Id);
        _reservationsRepoMock.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservation);

        var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        Assert.Equal(3, offer.RemainingCount);
        _reservationsRepoMock.Verify(x => x.Delete(reservation), Times.Once);
        _offersRepoMock.Verify(x => x.Update(offer), Times.Once);
        Assert.Equal(Unit.Value, result);
    }
}

public class CreateReservationCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IOfferRepository> _offersRepoMock;
    private readonly Mock<IReservationRepository> _reservationsRepoMock;
    private readonly Mock<IGlobalSettingRepository> _globalSettingsMock;
    private readonly CreateReservationCommandHandler _handler;
    private const string UserId = "customer-xyz";

    public CreateReservationCommandHandlerTests()
    {
        var mocks = MockUnitOfWork.Create();
        _unitOfWorkMock = mocks.UnitOfWork;
        _offersRepoMock = mocks.Offers;
        _reservationsRepoMock = mocks.Reservations;
        _globalSettingsMock = mocks.GlobalSettings;

        _globalSettingsMock
            .Setup(x => x.GetIntValueAsync(GlobalSettingConstants.ReservationDurationMinutes, 30, It.IsAny<CancellationToken>()))
            .ReturnsAsync(30);

        _handler = new CreateReservationCommandHandler(
            MockCurrentUserService.Create(UserId).Object,
            _unitOfWorkMock.Object,
            MockDateTimeProvider.Create().Object);
    }

    [Fact]
    public async Task Handle_WhenOfferNotFound_ShouldThrowNotFoundException()
    {
        var command = new CreateReservationCommand(Guid.NewGuid());
        _offersRepoMock.Setup(x => x.GetByIdAsync(command.OfferId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Offer?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WhenUserHasMadeMoreThan2Reservations_ShouldThrowConflictException()
    {
        var offer = new Offer { Id = Guid.NewGuid(), RemainingCount = 5, Name = "Offer" };
        var command = new CreateReservationCommand(offer.Id);
        var existingReservations = new List<Reservation> { new(), new(), new() };
        _offersRepoMock.Setup(x => x.GetByIdAsync(offer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);
        _reservationsRepoMock.Setup(x => x.GetByOfferIdAndCustomerId(offer.Id, UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReservations);

        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WhenNoRemainingCoupons_ShouldThrowConflictException()
    {
        var offer = new Offer { Id = Guid.NewGuid(), RemainingCount = 0, Name = "Offer" };
        var command = new CreateReservationCommand(offer.Id);
        _offersRepoMock.Setup(x => x.GetByIdAsync(offer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);
        _reservationsRepoMock.Setup(x => x.GetByOfferIdAndCustomerId(offer.Id, UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation>());

        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateReservationAndDecreaseRemainingCount()
    {
        var offer = new Offer { Id = Guid.NewGuid(), RemainingCount = 5, Name = "Great Deal", DiscountedPrice = 49.99m };
        var command = new CreateReservationCommand(offer.Id);
        _offersRepoMock.Setup(x => x.GetByIdAsync(offer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);
        _reservationsRepoMock.Setup(x => x.GetByOfferIdAndCustomerId(offer.Id, UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation>());
        _reservationsRepoMock.Setup(x => x.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Reservation());

        var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        Assert.Equal(4, offer.RemainingCount);
        Assert.Equal("Great Deal", result.OfferName);
        Assert.Equal(49.99m, result.OfferDiscountedPrice);
        _reservationsRepoMock.Verify(x => x.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class CreateReservationCommandValidatorTests
{
    private readonly CreateReservationCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidOfferId_ShouldHaveNoErrors()
    {
        var command = new CreateReservationCommand(Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyOfferId_ShouldHaveError()
    {
        var command = new CreateReservationCommand(Guid.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.OfferId);
    }
}
