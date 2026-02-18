// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.Interfaces.Auth
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}
