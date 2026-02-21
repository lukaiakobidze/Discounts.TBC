// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Features.Categories.Query.GetAllCategories;
using Discounts.Application.Features.Offers.Query.SearchOffers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISender _sender;

        public HomeController(ISender sender)
        {
            _sender = sender;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var offers = await _sender.Send(new SearchOffersQuery(PageNumber: 1, PageSize: 6), cancellationToken).ConfigureAwait(false);
            var categories = await _sender.Send(new GetAllCategoriesQuery(), cancellationToken).ConfigureAwait(false);
            ViewBag.Categories = categories;
            return View(offers.Items);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
