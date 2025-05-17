using Amazon.CDK;

namespace Infra;

public class Program
{
    public static void Main(string[] args)
    {
        var app = new App();
        _ = new InfraStack(app, "InfraStack");
        //_ = new WebApiStack(app, "WebApiStack");
        app.Synth();
    }
}