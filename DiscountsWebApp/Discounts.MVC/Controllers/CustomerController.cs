// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Features.Coupons.Command.PurchaseCoupon;
using Discounts.Application.Features.Coupons.Command.UseCoupon;
using Discounts.Application.Features.Coupons.Query.GetMyCoupons;
using Discounts.Application.Features.Reservations.Command.CreateReservation;
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

        public async Task<IActionResult> MyCoupons()
        {
            var coupons = await _sender.Send(new GetMyCouponsQuery()).ConfigureAwait(false);
            return View(coupons);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Purchase(Guid offerId)
        {
            try
            {
                await _sender.Send(new PurchaseCouponCommand(offerId)).ConfigureAwait(false);
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
        public async Task<IActionResult> Reserve(Guid offerId)
        {
            try
            {
                await _sender.Send(new CreateReservationCommand(offerId)).ConfigureAwait(false);
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
        public async Task<IActionResult> UseCoupon(string uniqueCode)
        {
            try
            {
                await _sender.Send(new UseCouponCommand(uniqueCode)).ConfigureAwait(false);
                TempData["Success"] = "Coupon marked as used!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("MyCoupons");
        }
    }
}
