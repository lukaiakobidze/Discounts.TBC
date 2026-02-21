// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Discounts.Infrastructure.BackgroundServices
{
    public class OfferExpirationWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OfferExpirationWorker> _logger;

        public OfferExpirationWorker(IServiceProvider serviceProvider, ILogger<OfferExpirationWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var expiredOffers = await unitOfWork.Offers.GetExpiredOffersAsync(DateTime.UtcNow, stoppingToken).ConfigureAwait(false);

                    if (expiredOffers.Any())
                    {
                        _logger.LogInformation("Found {Count} expired offers to mark", expiredOffers.Count);

                        foreach (var offer in expiredOffers)
                        {
                            offer.Status = OfferStatus.Expired;
                            unitOfWork.Offers.Update(offer);
                        }

                        await unitOfWork.SaveChangesAsync(stoppingToken).ConfigureAwait(false);
                        _logger.LogInformation("Marked {Count} offers as expired", expiredOffers.Count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while expiring offers");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken).ConfigureAwait(false);
            }
        }
    }
}
