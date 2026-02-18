// Copyright (C) TBC Bank. All Rights Reserved.

using Asp.Versioning;
using Discounts.Application.Features.Categories.Command.CreateCategory;
using Discounts.Application.Features.Categories.Command.DeleteCategory;
using Discounts.Application.Features.Categories.Command.UpdateCateogry;
using Discounts.Application.Features.Categories.Query.GetAllCategories;
using Discounts.Application.Features.Categories.Query.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ISender _sender;

        public CategoriesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _sender.Send(new GetAllCategoriesQuery()).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _sender.Send(new GetCategoryByIdQuery(id)).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
        {
            var result = await _sender.Send(command).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch.");

            var result = await _sender.Send(command).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _sender.Send(new DeleteCategoryCommand(id)).ConfigureAwait(false);
            return NoContent();
        }
    }
}
