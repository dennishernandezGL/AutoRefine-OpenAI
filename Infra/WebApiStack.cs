using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.S3.Deployment;
using System.Collections.Generic;

namespace Infra;

public class WebApiStack : Stack
{
    internal WebApiStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
        // Create a VPC
        var vpc = new Vpc(this, "WebApiVpc", new VpcProps
        {
            MaxAzs = 2,  // Use 2 Availability Zones for high availability
            NatGateways = 1  // Use 1 NAT Gateway to save costs
        });

        // Create Security Group for the EC2 instance
        var securityGroup = new SecurityGroup(this, "WebApiSecurityGroup", new SecurityGroupProps
        {
            Vpc = vpc,
            Description = "Allow HTTP and HTTPS traffic",
            AllowAllOutbound = true
        });

        // Allow inbound HTTP and HTTPS traffic
        securityGroup.AddIngressRule(Peer.AnyIpv4(), Port.Tcp(80), "Allow HTTP");
        securityGroup.AddIngressRule(Peer.AnyIpv4(), Port.Tcp(443), "Allow HTTPS");
        securityGroup.AddIngressRule(Peer.AnyIpv4(), Port.Tcp(22), "Allow SSH");

        // Create a role for the EC2 instance
        var role = new Role(this, "WebApiInstanceRole", new RoleProps
        {
            AssumedBy = new ServicePrincipal("ec2.amazonaws.com"),
            ManagedPolicies = new IManagedPolicy[]
            {
                ManagedPolicy.FromAwsManagedPolicyName("AmazonSSMManagedInstanceCore"),
                ManagedPolicy.FromAwsManagedPolicyName("AmazonS3ReadOnlyAccess"),
            }
        });

        // Create S3 bucket for deployment artifacts
        var deploymentBucket = new Bucket(this, "WebApiDeploymentBucket", new BucketProps
        {
            RemovalPolicy = RemovalPolicy.DESTROY,
            AutoDeleteObjects = true,
            Versioned = false
        });

        // Upload deployment package to S3
        var deployment = new BucketDeployment(this, "DeploymentAssets", new BucketDeploymentProps
        {
            Sources = new[] { Source.Asset("../Services/bin/Release/net9.0/publish") },
            DestinationBucket = deploymentBucket,
            DestinationKeyPrefix = "webapp",
            Prune = true
        });

        // Create UserData script to install .NET, download app from S3, and set up as service
        var userData = UserData.ForLinux();
        userData.AddCommands(
            "#!/bin/bash",
            "sudo apt-get update",
            "sudo apt-get install -y apt-transport-https",

            // Install .NET 6
            "wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb",
            "sudo dpkg -i packages-microsoft-prod.deb",
            "sudo apt-get update",
            "sudo apt-get install -y dotnet-sdk-6.0",
            "sudo apt-get install -y aspnetcore-runtime-6.0",
            
            // Install Nginx
            "sudo apt-get install -y nginx",

            // Create app directory
            "mkdir -p /var/www/webapi",

            // Download app from S3
            $"aws s3 cp s3://{deploymentBucket.BucketName}/webapp/ /var/www/webapi/ --recursive",

            // Create service file
            "cat > /etc/systemd/system/webapi.service << 'EOL'\n" +
            "[Unit]\n" +
            "Description=.NET Web API App\n" +
            "After=network.target\n\n" +
            "[Service]\n" +
            "WorkingDirectory=/var/www/webapi\n" +
            "ExecStart=/usr/bin/dotnet /var/www/webapi/Services.dll --urls=\"http://0.0.0.0:5000\"\n" +
            "Restart=always\n" +
            "RestartSec=10\n" +
            "SyslogIdentifier=webapi\n" +
            "User=www-data\n" +
            "Environment=ASPNETCORE_ENVIRONMENT=Production\n\n" +
            "[Install]\n" +
            "WantedBy=multi-user.target\n" +
            "EOL",

            // Configure Nginx as reverse proxy
            "cat > /etc/nginx/sites-available/default << 'EOL'\n" +
            "server {\n" +
            "    listen 80;\n" +
            "    location / {\n" +
            "        proxy_pass http://localhost:5000;\n" +
            "        proxy_http_version 1.1;\n" +
            "        proxy_set_header Upgrade $http_upgrade;\n" +
            "        proxy_set_header Connection keep-alive;\n" +
            "        proxy_set_header Host $host;\n" +
            "        proxy_cache_bypass $http_upgrade;\n" +
            "        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;\n" +
            "        proxy_set_header X-Forwarded-Proto $scheme;\n" +
            "    }\n" +
            "}\n" +
            "EOL",

            // Set permissions and enable services
            "sudo chmod +x /var/www/webapi/Services.dll",
            "sudo chown -R www-data:www-data /var/www/webapi",
            "sudo systemctl enable webapi.service",
            "sudo systemctl start webapi.service",
            "sudo systemctl restart nginx"
        );

        // Create EC2 instance
        var instance = new Instance_(this, "WebApiInstance", new InstanceProps
        {
            Vpc = vpc,
            VpcSubnets = new SubnetSelection { SubnetType = SubnetType.PUBLIC },
            SecurityGroup = securityGroup,
            InstanceType = InstanceType.Of(InstanceClass.T3, InstanceSize.MICRO),
            MachineImage = MachineImage.LatestAmazonLinux(),
            Role = role,
            UserData = userData,
            //KeyName = "webapi-key"
        });

        // Output the public URL
        /*new CfnOutput(this, "WebApiUrl", new CfnOutputProps
        {
            Value = $"http://{instance.InstancePublicDnsName}",
            Description = "URL of the Web API"
        });*/
    }
}