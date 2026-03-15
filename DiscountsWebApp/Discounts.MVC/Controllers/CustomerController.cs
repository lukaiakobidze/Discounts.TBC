// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Features.Coupons.Command.PurchaseCoupon;
using Discounts.Application.Features.Coupons.Command.UseCoupon;
using Discounts.Application.Features.Coupons.Query.GetMyCoupons;
using Discounts.Application.Features.Favourites.Commands.AddFavourite;
using Discounts.Application.Features.Favourites.Commands.RemoveFavourite;
using Discounts.Application.Features.Favourites.Queries.GetMyFavourites;
using Discounts.Application.Features.Reservations.Command.CancelReservation;
using Discounts.Application.Features.Reservations.Command.CreateReservation;
using Discounts.Application.Features.Reservations.Query.GetMyReservations;
using Discounts.Application.Features.Reviews.Commands.AddReview;
using Discounts.Application.Models;
using Discounts.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Controllers
{
    [Authorize(Roles = Roles.Customer)]
    public class CustomerController : Controller
    {
        private readonly ISender _sender;

        public CustomerController(ISender sender)
        {
            _sender = sender;
        }

        public async Task<IActionResult> MyCoupons(CancellationToken cancellationToken, int pageNumber = 1)
        {
            const int pageSize = 10;
            var all = await _sender.Send(new GetMyCouponsQuery(), cancellationToken).ConfigureAwait(false);
            var paged = new PaginatedList<Discounts.Application.DTOs.Coupons.CouponDto>(
                all.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
                all.Count, pageNumber, pageSize);
            return View(paged);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Purchase(Guid offerId, CancellationToken cancellationToken)
        {
            try
            {
                await _sender.Send(new PurchaseCouponCommand(offerId), cancellationToken).ConfigureAwait(false);
                TempData["Success"] = "Coupon purchased successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("MyCoupons");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(Guid offerId, CancellationToken cancellationToken)
        {
            try
            {
                await _sender.Send(new CreateReservationCommand(offerId), cancellationToken).ConfigureAwait(false);
                TempData["Success"] = "Offer reserved successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Details", "OfferBrowse", new { id = offerId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UseCoupon(string uniqueCode, CancellationToken cancellationToken)
        {
            try
            {
                await _sender.Send(new UseCouponCommand(uniqueCode), cancellationToken).ConfigureAwait(false);
                TempData["Success"] = "Coupon marked as used!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("MyCoupons");
        }

        public async Task<IActionResult> MyFavourites(CancellationToken cancellationToken, int pageNumber = 1)
        {
            const int pageSize = 9;
            var all = await _sender.Send(new GetMyFavouritesQuery(), cancellationToken).ConfigureAwait(false);
            var paged = new PaginatedList<Discounts.Application.DTOs.Offers.OfferDto>(
                all.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
                all.Count, pageNumber, pageSize);
            return View(paged);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFavourite(Guid offerId, CancellationToken cancellationToken)
        {
            try
            {
                await _sender.Send(new AddFavouriteCommand(offerId), cancellationToken).ConfigureAwait(false);
                TempData["Success"] = "Offer added to favourites!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Details", "OfferBrowse", new { id = offerId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFavourite(Guid offerId, CancellationToken cancellationToken)
        {
            try
            {
                await _sender.Send(new RemoveFavouriteCommand(offerId), cancellationToken).ConfigureAwait(false);
                TempData["Success"] = "Offer removed from favourites!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Details", "OfferBrowse", new { id = offerId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(Guid couponId, int stars, string? comment, CancellationToken cancellationToken)
        {
            try
            {
                await _sender.Send(new AddReviewCommand(couponId, stars, comment), cancellationToken).ConfigureAwait(false);
                TempData["Success"] = "Review submitted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("MyCoupons");
        }

        public async Task<IActionResult> MyReservations(CancellationToken cancellationToken, int pageNumber = 1)
        {
            const int pageSize = 10;
            var all = await _sender.Send(new GetMyReservationsQuery(), cancellationToken).ConfigureAwait(false);
            var paged = new PaginatedList<Discounts.Application.DTOs.Reservations.ReservationDto>(
                all.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
                all.Count, pageNumber, pageSize);
            return View(paged);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelReservation(Guid reservationId, CancellationToken cancellationToken)
        {
            try
            {
                await _sender.Send(new CancelReservationCommand(reservationId), cancellationToken).ConfigureAwait(false);
                TempData["Success"] = "Offer canceled successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("MyReservations");
        }
    }
}
