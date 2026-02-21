// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Discounts.Infrastructure.BackgroundServices
{
    public class ReservationCleanupWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReservationCleanupWorker> _logger;

        public ReservationCleanupWorker(IServiceProvider serviceProvider, ILogger<ReservationCleanupWorker> logger)
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

                    var durationMinutes = await unitOfWork.GlobalSettings.GetIntValueAsync(GlobalSettingConstants.ReservationDurationMinutes, 30, stoppingToken).ConfigureAwait(false);
                    var expiryTime = DateTime.UtcNow.AddMinutes(-durationMinutes);

                    var expiredReservations = await unitOfWork.Reservations.GetExpiredAsync(expiryTime, stoppingToken).ConfigureAwait(false);

                    if (expiredReservations.Any())
                    {
                        _logger.LogInformation("Found {Count} expired reservations to clean up", expiredReservations.Count);

                        foreach (var reservation in expiredReservations)
                        {
                            var offer = await unitOfWork.Offers.GetByIdAsync(reservation.OfferId, stoppingToken).ConfigureAwait(false);
                            if (offer != null)
                            {
                                offer.RemainingCount += 1;
                                unitOfWork.Offers.Update(offer);
                            }
                            unitOfWork.Reservations.Delete(reservation);
                        }

                        await unitOfWork.SaveChangesAsync(stoppingToken).ConfigureAwait(false);
                        _logger.LogInformation("Cleaned up {Count} expired reservations", expiredReservations.Count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while cleaning up reservations");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken).ConfigureAwait(false);
            }
        }
    }
}
