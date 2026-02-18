// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Domain.Entities
{
    public class GlobalSetting : BaseEntity
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
