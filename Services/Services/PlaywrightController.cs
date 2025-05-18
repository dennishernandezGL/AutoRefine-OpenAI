using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Playwright;

namespace Services.Services;

[Route("api/[controller]")]
[ApiController]
public class PlaywrightController : ControllerBase
{
    private static readonly string[] Options = ["--no-sandbox", "--disable-dev-shm-usage"];
    private readonly IAmazonS3 _s3Client;

    public PlaywrightController()
    {
        var credentials = new BasicAWSCredentials(Environment.GetEnvironmentVariable("ACCESS_KEY") ,Environment.GetEnvironmentVariable("SECRET_KEY"));
        _s3Client = new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName("us-east-2"));
    }
    
    private static async Task InstallBrowsersIfNeeded()
    {
        const string executablePath = "/tmp/playwright-browsers";
        
        Environment.SetEnvironmentVariable("PLAYWRIGHT_BROWSERS_PATH", executablePath);
        
        try
        {
            if (!Directory.Exists(executablePath))
            {
                Directory.CreateDirectory(executablePath);
                
                var exitCode = Microsoft.Playwright.Program.Main(["install", "chromium"]);
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
    
    [HttpGet("/playwright/browsers")]
    public async Task<string> ExecuteTests()
    {
        await InstallBrowsersIfNeeded();
        
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            Args = Options
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
            await System.IO.File.WriteAllTextAsync(metricsPath, metrics);

            await browserContext.CloseAsync();
            
            var path = await page.Video?.PathAsync() ?? string.Empty;
            if (System.IO.File.Exists(path))
            {
                var videoBytes = await System.IO.File.ReadAllBytesAsync(path);
                var videoUrl = await UploadToS3(videoBytes, "recording.webm", "video/webm");
            }
            
            var result = new { success = formVisible, metricsPath };
            
            return string.Empty;
        }
        catch (Exception ex)
        {
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
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var key = $"playwright-outputs/{timestamp}-{fileName}";
            
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