using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;

namespace Infra;

public class InfraStack : Stack
{
    internal InfraStack(App scope, string id, StackProps? props = null) : base(scope, id, props)
    {
        var lambdaCodePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "OpenAILambda", "bin", "Release", "net6.0", "publish"));
        
        Console.WriteLine($"Lambda code path: {lambdaCodePath}");
        Console.WriteLine($"Directory exists: {Directory.Exists(lambdaCodePath)}");
        
        if (!Directory.Exists(lambdaCodePath))
        {
            throw new DirectoryNotFoundException($"Lambda publish directory not found at: {lambdaCodePath}");
        }
        
        var fn = new Function(this, "OpenAILambda", new FunctionProps
        {
            Runtime = Runtime.DOTNET_6,
            Handler = "OpenAILambda::OpenAILambda.Function::FunctionHandler",
            Code = Code.FromAsset(lambdaCodePath),
            Timeout = Duration.Seconds(30),
            MemorySize = 256
        });
        
        new CfnOutput(this, "LambdaFunctionArn", new CfnOutputProps
        {
            Value = fn.FunctionArn,
            Description = "ARN of the Lambda function"
        });
    }
}