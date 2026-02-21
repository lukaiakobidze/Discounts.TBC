// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Features.Coupons.Command.PurchaseCoupon;
using Discounts.Application.Features.Coupons.Command.UseCoupon;
using Discounts.Application.Features.Coupons.Query.GetMyCoupons;
using Discounts.Application.Features.Reservations.Command.CancelReservation;
using Discounts.Application.Features.Reservations.Command.CreateReservation;
using Discounts.Application.Features.Reservations.Query.GetMyReservations;
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

        public async Task<IActionResult> MyCoupons(CancellationToken cancellationToken)
        {
            var coupons = await _sender.Send(new GetMyCouponsQuery(), cancellationToken).ConfigureAwait(false);
            return View(coupons);
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

        public async Task<IActionResult> MyReservations(CancellationToken cancellationToken)
        {
            var reservations = await _sender.Send(new GetMyReservationsQuery(), cancellationToken).ConfigureAwait(false);
            return View(reservations);
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
