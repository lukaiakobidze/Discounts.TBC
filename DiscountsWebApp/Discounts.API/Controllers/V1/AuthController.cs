// Copyright (C) TBC Bank. All Rights Reserved.

using Asp.Versioning;
using Discounts.Application.Features.Auth.Command.Login;
using Discounts.Application.Features.Auth.Command.RefreshToken;
using Discounts.Application.Features.Auth.Command.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var result = await _sender.Send(command).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _sender.Send(command).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            var result = await _sender.Send(command).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
