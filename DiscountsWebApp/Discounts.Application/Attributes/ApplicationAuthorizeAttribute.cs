// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ApplicationAuthorizeAttribute : Attribute
    {
        public string? Role { get; set; }
    }
}
