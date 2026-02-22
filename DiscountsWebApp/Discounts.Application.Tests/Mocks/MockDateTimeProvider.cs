// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Auth;
using Moq;

namespace Discounts.Application.Tests.Mocks
{
    public static class MockDateTimeProvider
    {
        public static Mock<IDateTimeProvider> Create(DateTime? utcNow = null)
        {
            var mock = new Mock<IDateTimeProvider>();
            mock.Setup(x => x.UtcNow).Returns(utcNow ?? DateTime.UtcNow);
            return mock;
        }
    }
}
