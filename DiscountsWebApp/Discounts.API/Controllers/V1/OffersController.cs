// Copyright (C) TBC Bank. All Rights Reserved.

using Asp.Versioning;
using Discounts.Application.Features.Offers.Command.CreateOffer;
using Discounts.Application.Features.Offers.Command.UpdateOffer;
using Discounts.Application.Features.Offers.Query.GetOfferById;
using Discounts.Application.Features.Offers.Query.GetOffers;
using Discounts.Application.Features.Offers.Query.SearchOffers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TBC.WebApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class OffersController : ControllerBase
    {
        private readonly ISender _sender;

        public OffersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetOffers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetOffersQuery(pageNumber, pageSize), cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOffer(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetOfferByIdQuery(id), cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchOffers([FromQuery] string? searchTerm, [FromQuery] Guid? categoryId, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, CancellationToken cancellationToken, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _sender.Send(new SearchOffersQuery(searchTerm, categoryId, minPrice, maxPrice, pageNumber, pageSize), cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "MerchantOnly")]
        public async Task<IActionResult> CreateOffer([FromBody] CreateOfferCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetOffer), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "MerchantOnly")]
        public async Task<IActionResult> UpdateOffer(Guid id, [FromBody] UpdateOfferCommand command, CancellationToken cancellationToken)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch.");

            var result = await _sender.Send(command, cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
