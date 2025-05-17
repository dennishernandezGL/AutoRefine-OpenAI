using Services.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // Added missing using directive

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

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello World!");
        }
        
        [HttpPost("LogInfo")]
        public IActionResult LogInfo([FromBody] Request request)
        {
            var loggingFacade = LoggingFacade.GetInstance(LoggingConnections.MixPanel, _configuration); // Passing IConfiguration as second parameter
            loggingFacade.LogInfo(request.Message, Newtonsoft.Json.JsonConvert.DeserializeObject<object>(request.Object), request.Context); // Assuming Context is a class you have defined
            return Ok("Info logged successfully.");
        }

        [HttpPost("LogWarning")]
        public IActionResult LogWarning([FromBody] Request request)
        {
            var loggingFacade = LoggingFacade.GetInstance(LoggingConnections.MixPanel, _configuration); // Passing IConfiguration as second parameter
            loggingFacade.LogWarning(request.Message, Newtonsoft.Json.JsonConvert.DeserializeObject<object>(request.Object), request.Context);
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

    public class Request
    {
        public string Message { get; set; }
        public string Object { get; set; }
        public Context Context { get; set; }
    }
}