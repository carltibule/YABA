using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using YABA.API.ViewModels.Tags;
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
        public IActionResult GetTags()
        {
            var result = _tagsService.GetAll();
            return Ok(_mapper.Map<IEnumerable<TagResponse>>(result));
        }
    }
}
