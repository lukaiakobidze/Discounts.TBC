// Copyright (C) TBC Bank. All Rights Reserved.

using Asp.Versioning;
using Discounts.Application.Features.Coupons.Command.PurchaseCoupon;
using Discounts.Application.Features.Coupons.Command.UseCoupon;
using Discounts.Application.Features.Coupons.Query.GetMyCoupons;
using Discounts.Application.Features.Coupons.Query.GetSalesHistory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class CouponsController : ControllerBase
    {
        private readonly ISender _sender;

        public CouponsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("purchase")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> PurchaseCoupon([FromBody] PurchaseCouponCommand command)
        {
            var result = await _sender.Send(command).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost("use")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> UseCoupon([FromBody] UseCouponCommand command)
        {
            await _sender.Send(command).ConfigureAwait(false);
            return NoContent();
        }

        [HttpGet("my")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> GetMyCoupons()
        {
            var result = await _sender.Send(new GetMyCouponsQuery()).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet("sales-history")]
        [Authorize(Policy = "MerchantOnly")]
        public async Task<IActionResult> GetSalesHistory([FromQuery] Guid? offerId)
        {
            var result = await _sender.Send(new GetSalesHistoryQuery(offerId)).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
