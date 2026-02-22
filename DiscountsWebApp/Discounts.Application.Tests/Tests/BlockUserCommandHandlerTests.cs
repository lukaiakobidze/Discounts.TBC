// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Features.Admin.Command.BlockUser;
using Discounts.Application.Interfaces.Auth;
using MediatR;
using Moq;

namespace Discounts.Application.Tests.Admin;

public class BlockUserCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly BlockUserCommandHandler _handler;

    public BlockUserCommandHandlerTests()
    {
        _handler = new BlockUserCommandHandler(_identityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCallBlockUserAsync_WithCorrectUserId()
    {
        var userId = "user-123";
        var command = new BlockUserCommand(userId);
        _identityServiceMock.Setup(x => x.BlockUserAsync(userId)).Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        _identityServiceMock.Verify(x => x.BlockUserAsync(userId), Times.Once);
        Assert.Equal(Unit.Value, result);
    }
}
