// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Features.Admin.Command.UnblockUser;
using Discounts.Application.Interfaces.Auth;
using MediatR;
using Moq;

namespace Discounts.Application.Tests.Admin;

public class UnblockUserCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly UnblockUserCommandHandler _handler;

    public UnblockUserCommandHandlerTests()
    {
        _handler = new UnblockUserCommandHandler(_identityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCallUnblockUserAsync_WithCorrectUserId()
    {
        var userId = "user-456";
        var command = new UnblockUserCommand(userId);
        _identityServiceMock.Setup(x => x.UnblockUserAsync(userId)).Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        _identityServiceMock.Verify(x => x.UnblockUserAsync(userId), Times.Once);
        Assert.Equal(Unit.Value, result);
    }
}
