using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Services.Services;
using Services.Logging;
using AutoRefineOpenAI;
using AutoRefineOpenAI.Controllers;
using DotNetEnv; // Add this

namespace UTests
{
    public class IntegrationTest
    {
        private readonly LogControllerService _controller;
        private readonly MonitoringControllerService _monitoringController;
        private readonly RepositoryConnections _repositoryConnections;
        private readonly OpenAiController _openAiController;

        public IntegrationTest()
        {
            // Load environment variables from .env file
            Env.Load("../../../../.env");

            // Load configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables() // Add environment variables
                .Build();

            _controller = new LogControllerService(configuration);
            _monitoringController = new MonitoringControllerService(configuration, null);
            _openAiController = new OpenAiController(configuration);
            _repositoryConnections = new RepositoryConnections(configuration);
        }

        [Fact]
        public void LogInfo_ShouldReturnOkResult()
        {
            // Arrange
            string testMessage = "Test Info Message";

            // Act
            var result = _controller.LogInfo(new Request
            {
                Message = testMessage,
                Object = Newtonsoft.Json.JsonConvert.SerializeObject(new { test = "test" }),
                Context = new Context()
            });

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
            var result = _controller.LogWarning(new Request
            {
                Message = testMessage,
                Object = Newtonsoft.Json.JsonConvert.SerializeObject(new { test = "test" }),
                Context = new Context()
            });

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

        [Fact]
        public async Task CreateBranch_ShouldReturnOkResult()
        {
            // Arrange


            // Act
            var result = await _repositoryConnections.CreateBranch("AI-branch01") as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Branch created successfully.", result.Value);
        }

        [Fact]
        public async Task CreatePullRequest_ShouldReturnOkResult()
        {
            // Arrange

            // Act
            var result = await _repositoryConnections.CreatePullRequest(new CreatePullRequest
            {
                BranchName = "AI-branch01",
                BaseBranch = "main",
                Body = "This is a test pull request Body."
            }) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Pull request created successfully.", result.Value);
        }

        [Fact]
        public async Task AnalyzeCode_ShouldReturnOkResult()
        {
            // Arrange

            // Act
            var result = await _openAiController.FunctionHandler();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
        }



        [Fact]
        public async Task DownloadCode_ShouldReturnOkResult()
        {
            // Arrange

            // Act
            var result = await _repositoryConnections.CheckoutBranch("Ai-Analysis-20250518-083505") as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
        }
    }
}