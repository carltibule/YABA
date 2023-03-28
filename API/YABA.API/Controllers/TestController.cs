using Microsoft.AspNetCore.Mvc;
using System.Net;
using YABA.API.Settings;

namespace YABA.API.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [DevOnly]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet("TestLog")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult TestLog()
        {
            var testObject = new { id = 1, name = "Test Message" };
            _logger.LogDebug("Testing debug: {@TestObject}", testObject);
            return Ok(testObject);
        }

        [HttpGet("TestLogError")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult TestLogError()
        {
            var testObject = new { id = 1, name = "Test Message" };
            throw new Exception();
            return Ok(testObject);
        }
    }
}
