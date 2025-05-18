using OpenAILambda;

namespace UTests;

public class PlaywrightTests
{
    [Fact]
    public async Task This_ShouldWork()
    {
        await new PlaywrightFunction().FunctionHandler(string.Empty, null);
    }
}