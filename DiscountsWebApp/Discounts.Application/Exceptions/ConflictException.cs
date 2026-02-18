// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException() : base() { }
        public ConflictException(string message) : base(message) { }
    }
}
