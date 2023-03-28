using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using YABA.API.Settings;
using YABA.API.ViewModels;
using YABA.Service.Interfaces;

namespace YABA.API.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Authorize, Route("api/v{version:apiVersion}/[controller]")]
    public class MiscController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMiscService _miscService;

        public MiscController(
            IMapper mapper,
            IMiscService miscService)
        {
            _mapper = mapper;
            _miscService = miscService;
        }

        [HttpGet]
        [DevOnly]
        [Route("GetWebsiteMetaData")]
        [ProducesResponseType(typeof(GetWebsiteMetaDataResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetWebsiteMetaData(string url)
        {
            if (string.IsNullOrEmpty(url)) return BadRequest();

            var response = _miscService.GetWebsiteMetaData(url);
            return Ok(_mapper.Map<GetWebsiteMetaDataResponse>(response));
        }
    }
}
