using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Services.Services;
using Services.Logging;

namespace UTests
{
    public class IntegrationTest
    {
        private readonly LogControllerService _controller;
        private readonly MonitoringControllerService _monitoringController;

        public IntegrationTest()
        {
            // Load configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _controller = new LogControllerService(configuration);
            _monitoringController = new MonitoringControllerService(configuration);
        }

        [Fact]
        public void LogInfo_ShouldReturnOkResult()
        {
            // Arrange
            string testMessage = "Test Info Message";

            // Act
            var result = _controller.LogInfo(testMessage, Newtonsoft.Json.JsonConvert.SerializeObject(new { test = "test" }));

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Info logged successfully.", ((OkObjectResult)result).Value);
        }

        [Fact]
        public void LogWarning_ShouldReturnOkResult()
        {
            // Arrange
            string testMessage = "Test Warning Message";

            // Act
            var result = _controller.LogWarning(testMessage, Newtonsoft.Json.JsonConvert.SerializeObject(new { test = "test" }));

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Warning logged successfully.", ((OkObjectResult)result).Value);
        }

        [Fact]
        public void LogError_ShouldReturnOkResult()
        {
            // Arrange
            string testMessage = "Test Error Message";

            // Act
            var exception = new Exception(testMessage)
            {
                Source = "Test Source"
            };

            var result = _controller.LogError(new ExceptionDetails(exception)
            {
                Message = exception.Message,
                StackTrace = "Test Stack Trace"
            });

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Error logged successfully.", ((OkObjectResult)result).Value);
        }



        [Fact]
        public async Task LogRetrieve_ShouldReturnOkResult()
        {
            // Arrange

            // Act
            var result = await _monitoringController.RetrieveLogs(new MonitoringControllerService.DateRange
            {
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow,
                EventName = "info"//LogType.Info.ToString()
            });

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
        }
    }
}