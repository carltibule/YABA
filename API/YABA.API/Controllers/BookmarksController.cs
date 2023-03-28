using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using YABA.API.ViewModels.Bookmarks;
using YABA.API.ViewModels.Tags;
using YABA.Common.DTOs.Bookmarks;
using YABA.Service.Interfaces;

namespace YABA.API.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
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

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BookmarkResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateBookmark(int id, [FromBody] UpdateBookmarkRequestDTO request)
        {
            var result = await _bookmarkService.UpdateBookmark(id, request);

            if (result == null) return NotFound();

            return Ok(_mapper.Map<BookmarkResponse>(result));
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(BookmarkResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> PatchBookmark(int id, [FromBody] JsonPatchDocument<PatchBookmarkRequest> request)
        {
            if (request == null || !ModelState.IsValid) return BadRequest(ModelState);

            var entryToEdit = await _bookmarkService.Get(id);
            if(entryToEdit == null) return NotFound();

            var entryToEditAsPatchRequest = _mapper.Map<PatchBookmarkRequest>(entryToEdit);
            request.ApplyTo(entryToEditAsPatchRequest, ModelState);

            var updateRequest = _mapper.Map<UpdateBookmarkRequestDTO>(entryToEditAsPatchRequest);
            var result = await _bookmarkService.UpdateBookmark(id, updateRequest);

            if (result == null) return NotFound();

            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookmarkResponse>), (int)HttpStatusCode.OK)]
        public IActionResult GetAll(bool showHidden = false) 
        {
            var result = _bookmarkService.GetAll(showHidden);
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
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteBookmarks([FromBody] DeleteBookmarksRequest request) 
        {
            if (request.Ids == null || !request.Ids.Any()) return BadRequest();

            var result = await _bookmarkService.DeleteBookmarks(request.Ids);

            if(result == null) return NotFound();

            return NoContent();
        }

        [HttpPost("Hide")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> HideBookmarks([FromBody] HideBookmarksRequest request)
        {
            if (request.Ids == null || !request.Ids.Any()) return BadRequest();

            var result = await _bookmarkService.HideBookmarks(request.Ids);

            if (result == null) return NotFound();

            return NoContent();
        }

        [HttpGet("Tags")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult GetBookmarkTags(bool showHidden = false)
        {
            var result = _bookmarkService.GetAllBookmarkTags(showHidden);
            return Ok(_mapper.Map<IEnumerable<TagResponse>>(result));
        }

    }
}