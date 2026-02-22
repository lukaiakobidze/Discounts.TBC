// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Auth;
using Discounts.Application.Features.Admin.Query.GetUsers;
using Discounts.Application.Interfaces.Auth;
using Moq;

namespace Discounts.Application.Tests.Admin;

public class GetUsersQueryHandlerTests
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _handler = new GetUsersQueryHandler(_identityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUsersFromIdentityService()
    {
        var role = "Customer";
        var users = new List<UserDto> { new() { Email = "a@b.com" } };
        _identityServiceMock.Setup(x => x.GetUsersAsync(role)).ReturnsAsync(users);

        var result = await _handler.Handle(new GetUsersQuery(role), CancellationToken.None).ConfigureAwait(true);

        Assert.Equal(users, result);
        _identityServiceMock.Verify(x => x.GetUsersAsync(role), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRoleIsNull_ShouldPassNullToIdentityService()
    {
        _identityServiceMock.Setup(x => x.GetUsersAsync(null)).ReturnsAsync(new List<UserDto>());

        var result = await _handler.Handle(new GetUsersQuery(null), CancellationToken.None).ConfigureAwait(true);

        _identityServiceMock.Verify(x => x.GetUsersAsync(null), Times.Once);
    }
}
