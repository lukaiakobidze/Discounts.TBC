// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Features.Categories.Query.GetAllCategories;
using Discounts.Application.Features.Coupons.Query.GetMyCoupons;
using Discounts.Application.Features.Offers.Query.GetOfferById;
using Discounts.Application.Features.Offers.Query.SearchOffers;
using Discounts.Application.Features.Reservations.Query.GetMyReservations;
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

        public async Task<IActionResult> Index(string? searchTerm, Guid? categoryId, decimal? minPrice, decimal? maxPrice, int pageNumber = 1)
        {
            var categories = await _sender.Send(new GetAllCategoriesQuery()).ConfigureAwait(false);
            ViewBag.Categories = categories;

            var result = await _sender.Send(new SearchOffersQuery(searchTerm, categoryId, minPrice, maxPrice, pageNumber, 9)).ConfigureAwait(false);
            return View(result);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var myCoupons = await _sender.Send(new GetMyCouponsQuery()).ConfigureAwait(false);
            var myReservations = await _sender.Send(new GetMyReservationsQuery()).ConfigureAwait(false);

            ViewBag.IsReserved = myReservations.Any(x => x.OfferId == id);
            ViewBag.IsPurchased = myCoupons.Any(x => x.OfferId == id);

            var offer = await _sender.Send(new GetOfferByIdQuery(id)).ConfigureAwait(false);
            return View(offer);
        }
    }
}
