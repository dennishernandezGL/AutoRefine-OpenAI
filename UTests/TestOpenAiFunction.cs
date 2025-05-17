using DotNetEnv;
using Microsoft.Extensions.Configuration;
using OpenAILambda;

namespace UTests;

public class TestOpenAiFunction
{
    private readonly Function _openAiFunction;

    public TestOpenAiFunction()
    {
        // Load environment variables from .env file
        Env.Load("../../../../.env");

        
        // Load configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables() // Add environment variables
                .Build();

        _openAiFunction = new Function(configuration);
    }

    [Fact]
    public async Task This_ShouldWork()
    {
        await _openAiFunction.FunctionHandler(string.Empty, null);
    }
}