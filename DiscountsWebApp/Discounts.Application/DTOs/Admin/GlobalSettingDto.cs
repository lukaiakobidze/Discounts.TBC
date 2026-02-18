// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.DTOs.Admin
{
    public class GlobalSettingDto
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
