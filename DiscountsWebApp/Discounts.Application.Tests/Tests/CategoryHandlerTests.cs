// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Exceptions;
using Discounts.Application.Features.Categories.Command.CreateCategory;
using Discounts.Application.Features.Categories.Command.DeleteCategory;
using Discounts.Application.Features.Categories.Command.UpdateCateogry;
using Discounts.Application.Features.Categories.Query.GetCategoryById;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Application.Tests.Mocks;
using Discounts.Domain.Entities;
using Discounts.Domain.Enums;
using FluentValidation.TestHelper;
using MediatR;
using Moq;

namespace Discounts.Application.Tests.Categories;

public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICategoryRepository> _categoriesRepoMock;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        var mocks = MockUnitOfWork.Create();
        _unitOfWorkMock = mocks.UnitOfWork;
        _categoriesRepoMock = mocks.Categories;

        _handler = new CreateCategoryCommandHandler(
            MockCurrentUserService.Create().Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenCategoryNameDoesNotExist_ShouldCreateCategory()
    {
        var command = new CreateCategoryCommand("Electronics", "Description");
        _categoriesRepoMock.Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);
        _categoriesRepoMock.Setup(x => x.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category c, CancellationToken _) => c);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        Assert.Equal("Electronics", result.Name);
        Assert.Equal(0, result.OfferCount);
        _categoriesRepoMock.Verify(x => x.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCategoryNameAlreadyExists_ShouldThrowConflictException()
    {
        var command = new CreateCategoryCommand("Electronics", null);
        _categoriesRepoMock.Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Category { Name = "Electronics" });

        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
        _categoriesRepoMock.Verify(x => x.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldHaveNoErrors()
    {
        var command = new CreateCategoryCommand("Electronics", null);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldHaveError()
    {
        var command = new CreateCategoryCommand("", null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithNameExceeding60Chars_ShouldHaveError()
    {
        var command = new CreateCategoryCommand(new string('A', 61), null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}

public class DeleteCategoryCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICategoryRepository> _categoriesRepoMock;
    private readonly DeleteCateogryCommandHandler _handler;

    public DeleteCategoryCommandHandlerTests()
    {
        var mocks = MockUnitOfWork.Create();
        _unitOfWorkMock = mocks.UnitOfWork;
        _categoriesRepoMock = mocks.Categories;

        _handler = new DeleteCateogryCommandHandler(
            MockCurrentUserService.Create().Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ShouldThrowNotFoundException()
    {
        var command = new DeleteCategoryCommand(Guid.NewGuid());
        _categoriesRepoMock.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WhenCategoryHasActiveOffers_ShouldThrowConflictException()
    {
        var activeOffer = new Offer { Status = OfferStatus.Active, IsDeleted = false };
        var category = new Category { Id = Guid.NewGuid(), Offers = new List<Offer> { activeOffer } };
        var command = new DeleteCategoryCommand(category.Id);
        _categoriesRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WhenCategoryHasNoPendingOrActiveOffers_ShouldDeleteCategory()
    {
        var category = new Category { Id = Guid.NewGuid(), Offers = new List<Offer>() };
        var command = new DeleteCategoryCommand(category.Id);
        _categoriesRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        _categoriesRepoMock.Verify(x => x.Delete(category), Times.Once);
        Assert.Equal(Unit.Value, result);
    }
}
public class UpdateCategoryCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICategoryRepository> _categoriesRepoMock;
    private readonly UpdateCategoryCommandHandler _handler;

    public UpdateCategoryCommandHandlerTests()
    {
        var mocks = MockUnitOfWork.Create();
        _unitOfWorkMock = mocks.UnitOfWork;
        _categoriesRepoMock = mocks.Categories;

        _handler = new UpdateCategoryCommandHandler(
            MockCurrentUserService.Create().Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ShouldThrowNotFoundException()
    {
        var command = new UpdateCategoryCommand(Guid.NewGuid(), "NewName", null);
        _categoriesRepoMock.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_ShouldUpdateAndReturnDto()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Old", Offers = new List<Offer>() };
        var command = new UpdateCategoryCommand(categoryId, "NewName", "New description");
        _categoriesRepoMock.Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _categoriesRepoMock.Setup(x => x.Update(category));
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        Assert.Equal("NewName", category.Name);
        Assert.Equal("New description", category.Description);
        _categoriesRepoMock.Verify(x => x.Update(category), Times.Once);
    }
}

public class GetCategoryByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICategoryRepository> _categoriesRepoMock;
    private readonly GetCategoryByIdQueryHandler _handler;

    public GetCategoryByIdQueryHandlerTests()
    {
        var mocks = MockUnitOfWork.Create();
        _unitOfWorkMock = mocks.UnitOfWork;
        _categoriesRepoMock = mocks.Categories;

        _handler = new GetCategoryByIdQueryHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ShouldThrowNotFoundException()
    {
        var query = new GetCategoryByIdQuery(Guid.NewGuid());
        _categoriesRepoMock.Setup(x => x.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_ShouldReturnDtoWithActiveOfferCount()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            Name = "Tech",
            Offers = new List<Offer>
            {
                new() { Status = OfferStatus.Active },
                new() { Status = OfferStatus.Active },
                new() { Status = OfferStatus.Pending }
            }
        };
        _categoriesRepoMock.Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var result = await _handler.Handle(new GetCategoryByIdQuery(categoryId), CancellationToken.None).ConfigureAwait(true);

        Assert.Equal("Tech", result.Name);
        Assert.Equal(2, result.OfferCount);
    }
}
