// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Auth;
using Moq;

namespace Discounts.Application.Tests.Mocks
{
    public static class MockCurrentUserService
    {
        public static Mock<ICurrentUserService> Create(string userId = "default-user-id")
        {
            var mock = new Mock<ICurrentUserService>();
            mock.Setup(x => x.UserId).Returns(userId);
            return mock;
        }
    }
}
