// Copyright (C) TBC Bank. All Rights Reserved.

using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Discounts.API.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IEndpointRouteBuilder MapHealthCheckEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false
            });

            endpoints.MapHealthChecks("/health/ready");

            return endpoints;
        }
    }
}
