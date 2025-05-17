using DotNetEnv;
using OpenAILambda;

namespace UTests;

public class TestOpenAiFunction
{
    private readonly Function _openAiFunction;
    
    public TestOpenAiFunction()
    {
        // Load environment variables from .env file
        Env.Load("../../../../.env");

        _openAiFunction = new Function();
    }

    [Fact]
    public async Task This_ShouldWork()
    {
        await _openAiFunction.FunctionHandler(string.Empty, null);
    }
}