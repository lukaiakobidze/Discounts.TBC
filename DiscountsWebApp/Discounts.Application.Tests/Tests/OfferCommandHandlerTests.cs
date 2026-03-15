// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Exceptions;
using Discounts.Application.Features.Offers.Command.ApproveOffer;
using Discounts.Application.Features.Offers.Command.CreateOffer;
using Discounts.Application.Features.Offers.Command.RejectOffer;
using Discounts.Application.Features.Offers.Command.UpdateOffer;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using Discounts.Domain.Enums;
using FluentValidation.TestHelper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Discounts.Application.Tests.Offers;

public class ApproveOfferCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IOfferRepository> _offersRepoMock = new();
    private readonly ApproveOfferCommandHandler _handler;

    public ApproveOfferCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(x => x.Offers).Returns(_offersRepoMock.Object);
        _handler = new ApproveOfferCommandHandler(_unitOfWorkMock.Object, new MemoryCache(new MemoryCacheOptions()));
    }

    [Fact]
    public async Task Handle_WhenOfferNotFound_ShouldThrowNotFoundException()
    {
        var command = new ApproveOfferCommand(Guid.NewGuid());
        _offersRepoMock.Setup(x => x.GetByIdAsync(command.OfferId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Offer?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WhenOfferIsNotPending_ShouldThrowConflictException()
    {
        var offer = new Offer { Id = Guid.NewGuid(), Status = OfferStatus.Active };
        var command = new ApproveOfferCommand(offer.Id);
        _offersRepoMock.Setup(x => x.GetByIdAsync(command.OfferId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WithPendingOffer_ShouldSetStatusToActive()
    {
        var offer = new Offer { Id = Guid.NewGuid(), Status = OfferStatus.Pending };
        var command = new ApproveOfferCommand(offer.Id);
        _offersRepoMock.Setup(x => x.GetByIdAsync(command.OfferId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        Assert.Equal(OfferStatus.Active, offer.Status);
        _offersRepoMock.Verify(x => x.Update(offer), Times.Once);
        Assert.Equal(Unit.Value, result);
    }
}

// ─── RejectOffer ───────────────────────────────────────────────────────────────

public class RejectOfferCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IOfferRepository> _offersRepoMock = new();
    private readonly RejectOfferCommandHandler _handler;

    public RejectOfferCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(x => x.Offers).Returns(_offersRepoMock.Object);
        _handler = new RejectOfferCommandHandler(_unitOfWorkMock.Object, new MemoryCache(new MemoryCacheOptions()));
    }

    [Fact]
    public async Task Handle_WhenOfferNotFound_ShouldThrowNotFoundException()
    {
        var command = new RejectOfferCommand(Guid.NewGuid(), "Test reason");
        _offersRepoMock.Setup(x => x.GetByIdAsync(command.OfferId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Offer?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WhenOfferIsNotPending_ShouldThrowConflictException()
    {
        var offer = new Offer { Id = Guid.NewGuid(), Status = OfferStatus.Active };
        var command = new RejectOfferCommand(offer.Id, "Test reason");
        _offersRepoMock.Setup(x => x.GetByIdAsync(command.OfferId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WithPendingOffer_ShouldSetStatusToDeniedAndPersistReason()
    {
        var offer = new Offer { Id = Guid.NewGuid(), Status = OfferStatus.Pending };
        var command = new RejectOfferCommand(offer.Id, "Image quality is poor.");
        _offersRepoMock.Setup(x => x.GetByIdAsync(command.OfferId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        Assert.Equal(OfferStatus.Denied, offer.Status);
        Assert.Equal("Image quality is poor.", offer.RejectionReason);
        _offersRepoMock.Verify(x => x.Update(offer), Times.Once);
        Assert.Equal(Unit.Value, result);
    }
}
public class CreateOfferCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICurrentUserService> _currentUserMock = new();
    private readonly Mock<IOfferRepository> _offersRepoMock = new();
    private readonly Mock<ICategoryRepository> _categoriesRepoMock = new();
    private readonly CreateOfferCommandHandler _handler;
    private const string MerchantId = "merchant-1";

    public CreateOfferCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(x => x.Offers).Returns(_offersRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.Categories).Returns(_categoriesRepoMock.Object);
        _currentUserMock.Setup(x => x.UserId).Returns(MerchantId);
        _handler = new CreateOfferCommandHandler(_unitOfWorkMock.Object, _currentUserMock.Object, new MemoryCache(new MemoryCacheOptions()));
    }

    private static CreateOfferCommand ValidCommand(Guid categoryId) => new(
        "Offer Name", "Description", null, 100m, 60m, 10,
        DateTime.UtcNow, DateTime.UtcNow.AddDays(30), categoryId);

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ShouldThrowNotFoundException()
    {
        var command = ValidCommand(Guid.NewGuid());
        _categoriesRepoMock.Setup(x => x.GetByIdAsync(command.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateOfferWithPendingStatus()
    {
        var category = new Category { Id = Guid.NewGuid(), Name = "Tech" };
        var command = ValidCommand(category.Id);
        _categoriesRepoMock.Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _offersRepoMock.Setup(x => x.AddAsync(It.IsAny<Offer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Offer());
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        Assert.Equal("Offer Name", result.Name);
        Assert.Equal("Tech", result.CategoryName);
        _offersRepoMock.Verify(x => x.AddAsync(
            It.Is<Offer>(o => o.Status == OfferStatus.Pending && o.MerchantId == MerchantId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSetRemainingCountEqualToTotalCount()
    {
        var category = new Category { Id = Guid.NewGuid(), Name = "Tech" };
        var command = ValidCommand(category.Id);
        Offer? capturedOffer = null;
        _categoriesRepoMock.Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _offersRepoMock.Setup(x => x.AddAsync(It.IsAny<Offer>(), It.IsAny<CancellationToken>()))
            .Callback<Offer, CancellationToken>((o, _) => capturedOffer = o)
            .ReturnsAsync(new Offer());
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        Assert.Equal(command.TotalCount, capturedOffer!.RemainingCount);
    }
}

public class CreateOfferCommandValidatorTests
{
    private readonly CreateOfferCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldHaveNoErrors()
    {
        var command = new CreateOfferCommand(
            "Valid Name", "Valid Description", null, 100m, 60m, 5,
            DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenDiscountedPriceGreaterThanOriginal_ShouldHaveError()
    {
        var command = new CreateOfferCommand(
            "Name", "Desc", null, 50m, 100m, 1,
            DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DiscountedPrice);
    }

    [Fact]
    public void Validate_WhenValidToBeforeValidFrom_ShouldHaveError()
    {
        var validFrom = DateTime.UtcNow.AddDays(5);
        var validTo = DateTime.UtcNow.AddDays(1);
        var command = new CreateOfferCommand(
            "Name", "Desc", null, 100m, 60m, 1, validFrom, validTo, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ValidTo);
    }

    [Fact]
    public void Validate_WithZeroTotalCount_ShouldHaveError()
    {
        var command = new CreateOfferCommand(
            "Name", "Desc", null, 100m, 60m, 0,
            DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.TotalCount);
    }
}

public class UpdateOfferCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICurrentUserService> _currentUserMock = new();
    private readonly Mock<IDateTimeProvider> _dateTimeMock = new();
    private readonly Mock<IOfferRepository> _offersRepoMock = new();
    private readonly Mock<ICategoryRepository> _categoriesRepoMock = new();
    private readonly Mock<IGlobalSettingRepository> _globalSettingsMock = new();
    private readonly UpdateOfferCommandHandler _handler;
    private const string MerchantId = "merchant-1";

    public UpdateOfferCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(x => x.Offers).Returns(_offersRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.Categories).Returns(_categoriesRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.GlobalSettings).Returns(_globalSettingsMock.Object);
        _currentUserMock.Setup(x => x.UserId).Returns(MerchantId);
        _dateTimeMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);
        _globalSettingsMock
            .Setup(x => x.GetIntValueAsync(GlobalSettingConstants.MerchantEditWindowHours, 24, It.IsAny<CancellationToken>()))
            .ReturnsAsync(24);
        _handler = new UpdateOfferCommandHandler(
            _unitOfWorkMock.Object, _currentUserMock.Object, _dateTimeMock.Object, new MemoryCache(new MemoryCacheOptions()));
    }

    private static UpdateOfferCommand ValidCommand(Guid offerId, Guid categoryId) => new(
        offerId, "Updated Title", "Updated Desc", null, 100m, 60m, 10,
        DateTime.UtcNow, DateTime.UtcNow.AddDays(30), categoryId);

    [Fact]
    public async Task Handle_WhenOfferNotFound_ShouldThrowNotFoundException()
    {
        var command = ValidCommand(Guid.NewGuid(), Guid.NewGuid());
        _offersRepoMock.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Offer?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WhenOfferBelongsToAnotherMerchant_ShouldThrowForbiddenException()
    {
        var offer = new Offer { Id = Guid.NewGuid(), MerchantId = "other-merchant", CreatedAt = DateTime.UtcNow };
        var command = ValidCommand(offer.Id, Guid.NewGuid());
        _offersRepoMock.Setup(x => x.GetByIdAsync(offer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WhenEditWindowHasExpired_ShouldThrowForbiddenException()
    {
        var offer = new Offer
        {
            Id = Guid.NewGuid(),
            MerchantId = MerchantId,
            CreatedAt = DateTime.UtcNow.AddHours(-25)
        };
        var command = ValidCommand(offer.Id, Guid.NewGuid());
        _offersRepoMock.Setup(x => x.GetByIdAsync(offer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ShouldThrowNotFoundException()
    {
        var offer = new Offer { Id = Guid.NewGuid(), MerchantId = MerchantId, CreatedAt = DateTime.UtcNow };
        var command = ValidCommand(offer.Id, Guid.NewGuid());
        _offersRepoMock.Setup(x => x.GetByIdAsync(offer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);
        _categoriesRepoMock.Setup(x => x.GetByIdAsync(command.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldUpdateOfferFields()
    {
        var categoryId = Guid.NewGuid();
        var offer = new Offer { Id = Guid.NewGuid(), MerchantId = MerchantId, CreatedAt = DateTime.UtcNow };
        var category = new Category { Id = categoryId, Name = "Updated Category" };
        var command = ValidCommand(offer.Id, categoryId);
        _offersRepoMock.Setup(x => x.GetByIdAsync(offer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);
        _categoriesRepoMock.Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        Assert.Equal("Updated Title", offer.Name);
        Assert.Equal("Updated Category", result.CategoryName);
        _offersRepoMock.Verify(x => x.Update(offer), Times.Once);
    }
}

public class UpdateOfferCommandValidatorTests
{
    private readonly UpdateOfferCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldHaveNoErrors()
    {
        var command = new UpdateOfferCommand(
            Guid.NewGuid(), "Title", "Description", null, 100m, 60m, 5,
            DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenDiscountedPriceGreaterThanOriginal_ShouldHaveError()
    {
        var command = new UpdateOfferCommand(
            Guid.NewGuid(), "Title", "Desc", null, 50m, 100m, 5,
            DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DiscountedPrice);
    }

    [Fact]
    public void Validate_WhenValidToBeforeValidFrom_ShouldHaveError()
    {
        var command = new UpdateOfferCommand(
            Guid.NewGuid(), "Title", "Desc", null, 100m, 60m, 5,
            DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(1), Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ValidTo);
    }
}
