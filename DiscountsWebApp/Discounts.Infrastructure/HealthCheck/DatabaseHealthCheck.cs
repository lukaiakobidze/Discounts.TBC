// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Data.Context;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Discounts.Infrastructure.HealthCheck
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly DiscountsDbContext _dbContext;

        public DatabaseHealthCheck(DiscountsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var dbName = context.Registration.Name ?? "defaultDB";

            try
            {
                await _dbContext.Database.CanConnectAsync(cancellationToken).ConfigureAwait(false);
                return HealthCheckResult.Healthy($"Successfully connected to the database: {dbName}");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Failed to connect to the database: {dbName}", ex);
            }
        }
    }
}
