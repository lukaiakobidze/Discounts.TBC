// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Auth;
using Discounts.Application.Features.Admin.Query.AdminDashboard;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Application.Tests.Mocks;
using Discounts.Domain.Constants;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Discounts.Application.Tests.Admin;

public class AdminDashboardQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IOfferRepository> _offersRepoMock;
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly AdminDashboardQueryHandler _handler;

    public AdminDashboardQueryHandlerTests()
    {
        var mocks = MockUnitOfWork.Create();
        _unitOfWorkMock = mocks.UnitOfWork;
        _offersRepoMock = mocks.Offers;

        _handler = new AdminDashboardQueryHandler(
            MockCurrentUserService.Create().Object,
            _unitOfWorkMock.Object,
            _identityServiceMock.Object,
            new MemoryCache(new MemoryCacheOptions()));
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectDashboardStats()
    {
        var customers = new List<UserDto> { new(), new() };
        var merchants = new List<UserDto> { new() };
        _identityServiceMock.Setup(x => x.GetUsersAsync(Roles.Customer)).ReturnsAsync(customers);
        _identityServiceMock.Setup(x => x.GetUsersAsync(Roles.Merchant)).ReturnsAsync(merchants);
        _offersRepoMock.Setup(x => x.CountAsync(
                It.Is<System.Linq.Expressions.Expression<Func<Discounts.Domain.Entities.Offer, bool>>>(e => true),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        var result = await _handler.Handle(new AdminDashboardQuery(), CancellationToken.None).ConfigureAwait(true);

        Assert.Equal(2, result.TotalCustomers);
        Assert.Equal(1, result.TotalMerchants);
    }
}
