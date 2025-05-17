using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;

namespace Infra;

public class InfraStack : Stack
{
    internal InfraStack(App scope, string id, StackProps? props = null) : base(scope, id, props)
    {
        var fn = new Function(this, "MyDotnetFunction", new FunctionProps
        {
            Runtime = Runtime.DOTNET_6,
            Handler = "AutoRefineOpenAI::MyLambda.Function::FunctionHandler",
            Code = Code.FromAsset(Path.Combine("..","lambda","MyLambda","bin","Release","net6.0","publish"))
        });
    }
}