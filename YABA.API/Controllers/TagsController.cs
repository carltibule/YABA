using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using YABA.API.ViewModels.Tags;
using YABA.Common.DTOs.Tags;
using YABA.Service.Interfaces;

namespace YABA.API.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITagsService _tagsService;

        public TagsController(IMapper mapper, ITagsService tagsService)
        {
            _mapper = mapper;
            _tagsService = tagsService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TagResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _tagsService.GetAll();

            if (result == null) return NotFound();

            return Ok(_mapper.Map<IEnumerable<TagResponse>>(result));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TagResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _tagsService.Get(id);

            if (result == null) return NotFound();

            return Ok(_mapper.Map<TagResponse>(result));
        }

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<TagResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateTag([FromBody]CreateTagDTO request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _tagsService.CreateTag(request);

            if (result == null) return BadRequest();

            return Ok(_mapper.Map<TagResponse>(result));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(IEnumerable<TagResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateTag(int id, [FromBody]UpdateTagDTO request)
        {
            var result = await _tagsService.UpdateTag(id, request);

            if (result == null) return NotFound();

            return Ok(_mapper.Map<TagResponse>(result));
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(IEnumerable<TagResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> PatchTag(int id, [FromBody] UpdateTagDTO request) => await UpdateTag(id, request);


        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType ((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteTags([FromBody] DeleteTagsRequest request)
        {
            if(request.Ids == null || !request.Ids.Any()) return BadRequest();

            var result = await _tagsService.DeleteTags(request.Ids);
            if (result == null) return NotFound();

            return NoContent();
        }

        [HttpPost("Hide")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> HideTags([FromBody] HideTagsRequest request)
        {
            if (request.Ids == null || !request.Ids.Any()) return BadRequest();

            var result = await _tagsService.HideTags(request.Ids);
            if (result == null) return NotFound();

            return NoContent();
        }
    }
}
