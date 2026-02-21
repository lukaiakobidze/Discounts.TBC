// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Features.Categories.Query.GetAllCategories;
using Discounts.Application.Features.Coupons.Query.GetMyCoupons;
using Discounts.Application.Features.Offers.Query.GetOfferById;
using Discounts.Application.Features.Offers.Query.SearchOffers;
using Discounts.Application.Features.Reservations.Query.GetMyReservations;
using Discounts.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Controllers
{
    public class OfferBrowseController : Controller
    {
        private readonly ISender _sender;

        public OfferBrowseController(ISender sender)
        {
            _sender = sender;
        }

        public async Task<IActionResult> Index(string? searchTerm, Guid? categoryId, decimal? minPrice, decimal? maxPrice, CancellationToken cancellationToken, int pageNumber = 1)
        {
            var categories = await _sender.Send(new GetAllCategoriesQuery(), cancellationToken).ConfigureAwait(false);
            ViewBag.Categories = categories;

            var result = await _sender.Send(new SearchOffersQuery(searchTerm, categoryId, minPrice, maxPrice, pageNumber, 9), cancellationToken).ConfigureAwait(false);
            return View(result);
        }

        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            if (User.IsInRole(Roles.Customer))
            {
                var myCoupons = await _sender.Send(new GetMyCouponsQuery(), cancellationToken).ConfigureAwait(false);
                var myReservations = await _sender.Send(new GetMyReservationsQuery(), cancellationToken).ConfigureAwait(false);

                ViewBag.IsReserved = myReservations.Any(x => x.OfferId == id);
                ViewBag.IsPurchased = myCoupons.Any(x => x.OfferId == id);
            }

            var offer = await _sender.Send(new GetOfferByIdQuery(id), cancellationToken).ConfigureAwait(false);
            return View(offer);
        }
    }
}
