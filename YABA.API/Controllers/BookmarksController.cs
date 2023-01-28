﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using YABA.API.ViewModels;
using YABA.Common.DTOs.Bookmarks;
using YABA.Common.DTOs.Tags;
using YABA.Service.Interfaces;

namespace YABA.API.Controllers
{
    [ApiVersion("1")]
    [Authorize, Route("api/v{version:apiVersion}/[controller]")]
    public class BookmarksController : ControllerBase
    {
        private readonly IBookmarkService _bookmarkService;

        public BookmarksController(IBookmarkService bookmarkService)
        {
            _bookmarkService = bookmarkService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(GenericResponse<CreateBookmarkRequestDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateBookmarkRequestDTO request) 
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _bookmarkService.CreateBookmark(request);

            if(!result.IsSuccessful) return BadRequest();

            return Ok(new GenericResponse<CreateBookmarkRequestDTO>(result));
        }

        [HttpPost("{id}/Tags")]
        [ProducesResponseType(typeof(IEnumerable<GenericResponse<string>>),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateBookmarkTags(int id, [FromBody] UpdateBookmarkTagRequest request)
        {
            if (request.Tags == null) return BadRequest();

            var result = await _bookmarkService.UpdateBookmarkTags(id, request.Tags);

            if (result.All(x => !x.IsSuccessful)) return NotFound();

            return Ok(result.Select(x => new GenericResponse<string>(x)));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(GenericResponse<UpdateBookmarkRequestDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateBookmark(int id, [FromBody] UpdateBookmarkRequestDTO request)
        {
            // TODO: Add support for HTTP PATCH
            var result = await _bookmarkService.UpdateBookmark(id, request);

            if (!result.IsSuccessful) return NotFound();

            return Ok(new GenericResponse<UpdateBookmarkRequestDTO>(result));
        }

        [HttpGet]
        [ProducesResponseType(typeof(GenericResponse<IEnumerable<BookmarkDTO>>), (int)HttpStatusCode.OK)]
        public IActionResult GetAll() 
        {
            var result = _bookmarkService.GetAll();
            return Ok(new GenericResponse<IEnumerable<BookmarkDTO>>(result));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GenericResponse<BookmarkDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(int id) 
        {
            var result = await _bookmarkService.Get(id);

            if (!result.IsSuccessful) return NotFound();

            return Ok(new GenericResponse<BookmarkDTO>(result));
        }

        [HttpGet("{id}/Tags")]
        [ProducesResponseType(typeof(GenericResponse<IEnumerable<TagSummaryDTO>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetBookmarkTags(int id) 
        { 
            var result = _bookmarkService.GetBookmarkTags(id);

            if (!result.IsSuccessful) return NotFound();

            return Ok(new GenericResponse<IEnumerable<TagSummaryDTO>>(result));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(GenericResponse<int>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(int id) 
        {
            var result = await _bookmarkService.DeleteBookmark(id);

            if (!result.IsSuccessful) return NotFound();

            return Ok(new GenericResponse<int>(result));
        }

        [HttpDelete]
        [ProducesResponseType(typeof(IEnumerable<GenericResponse<int>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteBookmarks([FromBody] DeleteBookmarksRequest request) 
        { 
            var result = await _bookmarkService.DeleteBookmarks(request.Ids);

            if(result.All(x => !x.IsSuccessful)) return NotFound();

            return Ok(result.Select((x) => new GenericResponse<int>(x)));
        }
    }
}