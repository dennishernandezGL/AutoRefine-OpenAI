using Amazon.CDK;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.Lambda;

namespace Infra;

public class InfraStack : Stack
{
    internal InfraStack(App scope, string id, StackProps? props = null) : base(scope, id, props)
    {
        var lambdaCodePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "OpenAILambda", "bin", "Release", "net6.0", "publish"));
        
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
        
        var scheduleTime = System.Environment.GetEnvironmentVariable("SCHEDULE_TIME") ?? "5";
        var rule = new Rule(this, "ScheduleRule", new RuleProps
        {
            Schedule = Schedule.Rate(Duration.Minutes(Convert.ToDouble(scheduleTime))),
            Description = "Schedule for invoking OpenAI Lambda function every X minutes"
        });
        
        rule.AddTarget(new LambdaFunction(fn));
        
        /*new CfnOutput(this, "LogGroupUrl", new CfnOutputProps
        {
            Value = $"https://console.aws.amazon.com/cloudwatch/home?region={this.Region}#logsV2:log-groups/log-group/{fn.LogGroup.LogGroupName}",
            Description = "URL to CloudWatch Logs for the Lambda function"
        });*/
        
        new CfnOutput(this, "LambdaFunctionArn", new CfnOutputProps
        {
            Value = fn.FunctionArn,
            Description = "ARN of the Lambda function"
        });
        
        /*new CfnOutput(this, "ScheduleRuleName", new CfnOutputProps
        {
            Value = rule.RuleName,
            Description = "Name of the EventBridge rule that schedules the Lambda"
        });*/
    }
}