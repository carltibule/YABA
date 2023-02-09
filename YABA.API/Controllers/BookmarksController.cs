using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using YABA.API.ViewModels.Bookmarks;
using YABA.API.ViewModels.Tags;
using YABA.Common.DTOs.Bookmarks;
using YABA.Service.Interfaces;

namespace YABA.API.Controllers
{
    [ApiVersion("1")]
    [Authorize, Route("api/v{version:apiVersion}/[controller]")]
    public class BookmarksController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IBookmarkService _bookmarkService;

        public BookmarksController(IMapper mapper, IBookmarkService bookmarkService)
        {
            _mapper = mapper;
            _bookmarkService = bookmarkService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(BookmarkResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateBookmarkRequestDTO request) 
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _bookmarkService.CreateBookmark(request);

            if(result == null) return BadRequest();

            return CreatedAtAction(nameof(Create), _mapper.Map<BookmarkResponse>(result));
        }

        [HttpPost("{id}/Tags")]
        [ProducesResponseType(typeof(IEnumerable<TagResponse>),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateBookmarkTags(int id, [FromBody] UpdateBookmarkTagRequest request)
        {
            if (request.Tags == null || !request.Tags.Any()) return BadRequest();

            var result = await _bookmarkService.UpdateBookmarkTags(id, request.Tags);

            if (result == null) return NotFound();

            return Ok(_mapper.Map<IEnumerable<TagResponse>>(result));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BookmarkResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateBookmark(int id, [FromBody] UpdateBookmarkRequestDTO request)
        {
            // TODO: Add support for HTTP PATCH
            var result = await _bookmarkService.UpdateBookmark(id, request);

            if (result == null) return NotFound();

            return Ok(_mapper.Map<BookmarkResponse>(result));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookmarkResponse>), (int)HttpStatusCode.OK)]
        public IActionResult GetAll() 
        {
            var result = _bookmarkService.GetAll();
            return Ok(_mapper.Map<IEnumerable<BookmarkResponse>>(result));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BookmarkResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(int id) 
        {
            var result = await _bookmarkService.Get(id);

            if (result == null) return NotFound();

            return Ok(_mapper.Map<BookmarkResponse>(result));
        }

        [HttpGet("{id}/Tags")]
        [ProducesResponseType(typeof(IEnumerable<TagResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetBookmarkTags(int id) 
        { 
            var result = _bookmarkService.GetBookmarkTags(id);

            if (result == null) return NotFound();

            return Ok(_mapper.Map<IEnumerable<TagResponse>>(result));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(int id) 
        {
            var result = await _bookmarkService.DeleteBookmark(id);

            if (!result.HasValue) return NotFound();

            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteBookmarks([FromBody] DeleteBookmarksRequest request) 
        {
            if (request.Ids == null || !request.Ids.Any()) return BadRequest();

            var result = await _bookmarkService.DeleteBookmarks(request.Ids);

            if(result == null) return NotFound();

            return NoContent();
        }
    }
}