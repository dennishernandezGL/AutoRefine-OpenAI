using System.Text.Json;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Octokit;

namespace OpenAILambda;

public class PlaywrightFunction
{
    private static readonly string[] options = new[] { "--no-sandbox", "--disable-dev-shm-usage" };
    private readonly IAmazonS3 _s3Client;

    public PlaywrightFunction()
    {
        var credentials = new BasicAWSCredentials("AKIA3B5SO65UDLAZIUQG" ,"RvC0K88bJpJ5rZgmZeE9h3je+pUy2CixYpkD2Rc+");
        _s3Client = new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName("us-east-2"));
    }
    
    private static async Task InstallBrowsersIfNeeded()
    {
        string executablePath = "/tmp/playwright-browsers";
        
        Environment.SetEnvironmentVariable("PLAYWRIGHT_BROWSERS_PATH", executablePath);
        
        try
        {
            if (!Directory.Exists(executablePath))
            {
                Directory.CreateDirectory(executablePath);
                
                var exitCode = Microsoft.Playwright.Program.Main(new[] { "install", "chromium" });
                if (exitCode != 0)
                {
                    throw new Exception($"Failed to install browsers, exit code: {exitCode}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error installing browsers: {ex.Message}");
            throw;
        }
    }
    
    public async Task<string> FunctionHandler(string input, ILambdaContext context)
    {
        await InstallBrowsersIfNeeded();
        
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            Args = options
        });

        // New context and page
        var contextOptions = new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 800 },
            RecordVideoDir = "videos/"
        };
        await using var browserContext = await browser.NewContextAsync(contextOptions);
        var page = await browserContext.NewPageAsync();

        try
        {
            await page.GotoAsync("http://localhost:5173/");

            await page.FillAsync("input[name=\"fullName\"]", "John Doe");
            await page.FillAsync("input[name=\"email\"]", "john@example.com");
            await page.FillAsync("input[name=\"cardNumber\"]", "4111111111111111");
            await page.FillAsync("input[name=\"expirationDate\"]", "12/25");
            await page.FillAsync("input[name=\"cvv\"]", "123");
            await page.FillAsync("input[name=\"billingAddress\"]", "123 Main St");
            
            await page.ClickAsync("button[type=\"submit\"]");

            var screenshot = await page.ScreenshotAsync(new()
            {
                Path = "screenshot.png",
                FullPage = true,
            });
            
            await UploadToS3(screenshot, "screenshot.png", "image/png");

            bool formVisible = await page.Locator("form").IsVisibleAsync();

            var metrics = await page.EvaluateAsync<string>("() => { const [entry] = performance.getEntriesByType('navigation'); return JSON.stringify(entry.toJSON()); }");

            var tmpDir = Path.Combine(Path.GetTempPath(), "metrics");
            Directory.CreateDirectory(tmpDir);
            var metricsPath = Path.Combine(tmpDir, "performance-metrics.json");
            await File.WriteAllTextAsync(metricsPath, metrics);

            await browserContext.CloseAsync();
            
            var path = await page.Video?.PathAsync() ?? string.Empty;
            if (File.Exists(path))
            {
                var videoBytes = await File.ReadAllBytesAsync(path);
                var videoUrl = await UploadToS3(videoBytes, "recording.webm", "video/webm");
            }
            
            var result = new { success = formVisible, metricsPath };
            
            return string.Empty;
        }
        catch (Exception ex)
        {
            context.Logger.LogError(ex, "Error running Playwright script");
            
            return string.Empty;
        }
        finally
        {
            await page.CloseAsync();
            await browserContext.CloseAsync();
            await browser.CloseAsync();
        }
    }
    
    private async Task<string> UploadToS3(byte[] fileBytes, string fileName, string contentType)
    {
        try
        {
            // Generate a unique key for the file
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string key = $"playwright-outputs/{timestamp}-{fileName}";
            
            // Set up the request to upload the file
            var putRequest = new PutObjectRequest
            {
                BucketName = "ainoexperts",
                Key = key,
                InputStream = new MemoryStream(fileBytes),
                ContentType = contentType,
                Metadata =
                {
                    ["timestamp"] = timestamp,
                    ["source"] = "playwright-lambda"
                }
            };
            
            // Upload to S3
            await _s3Client.PutObjectAsync(putRequest);
            
            // Return the S3 URL
            return $"https://ainoexperts.s3.amazonaws.com/{key}";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error uploading to S3: {ex.Message}");
            throw;
        }
    }
}