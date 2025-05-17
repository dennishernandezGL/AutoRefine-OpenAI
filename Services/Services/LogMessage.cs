using Services.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Services.Services
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogControllerService : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LogControllerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("LogInfo")]
        public IActionResult LogInfo([FromBody] string message, [FromBody] string @object)
        {
            var loggingFacade = LoggingFacade.GetInstance(LoggingConnections.MixPanel, _configuration); // Passing IConfiguration as second parameter
            loggingFacade.LogInfo(message, Newtonsoft.Json.JsonConvert.DeserializeObject<object>(@object), new Context()); // Assuming Context is a class you have defined
            return Ok("Info logged successfully.");
        }

        [HttpPost("LogWarning")]
        public IActionResult LogWarning([FromBody] string message, [FromBody] string @object)
        {
            var loggingFacade = LoggingFacade.GetInstance(LoggingConnections.MixPanel, _configuration); // Passing IConfiguration as second parameter
            loggingFacade.LogWarning(message, Newtonsoft.Json.JsonConvert.DeserializeObject<object>(@object), new Context());
            return Ok("Warning logged successfully.");
        }

        [HttpPost("LogError")]
        public IActionResult LogError([FromBody] ExceptionDetails message)
        {
            var loggingFacade = LoggingFacade.GetInstance(LoggingConnections.MixPanel, _configuration); // Passing IConfiguration as second parameter
            loggingFacade.LogError(message, new { test = "test" }, new Context());
            return Ok("Error logged successfully.");
        }
    }
}