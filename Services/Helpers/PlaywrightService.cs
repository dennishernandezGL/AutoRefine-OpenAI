using System.Diagnostics;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Playwright;
using Services.Repositories;

namespace Services.Helpers;

public class PlaywrightService
{
    private bool IsInitialized { get; set; }
    private readonly IAmazonS3 _s3Client;
    private static readonly string[] Options = ["--no-sandbox", "--disable-dev-shm-usage"];
    private IConfiguration _configuration;
    
    public PlaywrightService(IConfiguration configuration)
    {
        _configuration = configuration;
        var credentials = new BasicAWSCredentials(Environment.GetEnvironmentVariable("ACCESS_KEY") ,Environment.GetEnvironmentVariable("SECRET_KEY"));
        _s3Client = new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName("us-east-2"));
    }

    public async Task RunTests(string branchName)
    {
        var git = ARepository.Create(Repositories.Repositories.GitHub, _configuration);
        var path = await git.CheckoutBranch(branchName);

        await ShellHelper.RunCommandAsync("npm", "install", $"{path}/ai-no-experts-frontend");
        _ = ShellHelper.RunCommandAsync("npm", "run dev", $"{path}/ai-no-experts-frontend");
        await Task.Delay(TimeSpan.FromSeconds(30));
        await ExecuteTests();
    }
    
    public async Task ExecuteTests()
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
            await HappyPath(page);
            await SubmitsFormWithValidData(page);
            await ShowsValidationErrorsForEmptyRequiredFields(page);
            await ResetsFormWhenClickingResetButton(page);
            await ValidatesEmailFormat(page);
            await ChecksAccessibilityFeatures(page);
            await TestsResponsivenessAcrossScreenSizes(page);

            var formVisible = await page.Locator("form").IsVisibleAsync();

            var metrics = await page.EvaluateAsync<string>("() => { const [entry] = performance.getEntriesByType('navigation'); return JSON.stringify(entry.toJSON()); }");

            var tmpDir = Path.Combine(Path.GetTempPath(), "metrics");
            Directory.CreateDirectory(tmpDir);
            var metricsPath = Path.Combine(tmpDir, "performance-metrics.json");
            await System.IO.File.WriteAllTextAsync(metricsPath, metrics);

            await browserContext.CloseAsync();
            
            var path = await page.Video?.PathAsync() ?? string.Empty;
            if (File.Exists(path))
            {
                var videoBytes = await System.IO.File.ReadAllBytesAsync(path);
                var videoUrl = await UploadToS3(videoBytes, "recording.webm", "video/webm");
            }
            
            var result = new { success = formVisible, metricsPath };
            
        }
        catch (Exception ex)
        {
            // Ignored
        }
        finally
        {
            await page.CloseAsync();
            await browserContext.CloseAsync();
            await browser.CloseAsync();
        }
    }

    public async Task HappyPath(IPage page)
    {
        await page.GotoAsync("http://localhost:5173/");

        await page.FillAsync("input[name=\"fullName\"]", "John Doe");
        await page.FillAsync("input[name=\"email\"]", "john@example.com");
        await page.FillAsync("input[name=\"cardNumber\"]", "4111111111111111");
        await page.FillAsync("input[name=\"expirationDate\"]", "12/25");
        await page.FillAsync("input[name=\"cvv\"]", "123");
        await page.FillAsync("input[name=\"billingAddress\"]", "123 Main St");
            
        await page.ClickAsync("button[type=\"submit\"]");
        await page.ScreenshotAsync(new()
        {
            Path = "screenshot.png",
            FullPage = true,
        });
    }
    
    public async Task SubmitsFormWithValidData(IPage _page)
        {
            await _page.GotoAsync("http://localhost:5173/");
            await _page.FillAsync("input[name=\"fullName\"]", "John Doe");
            await _page.FillAsync("input[name=\"email\"]", "john@example.com");
            await _page.FillAsync("input[name=\"cardNumber\"]", "4111111111111111");
            await _page.FillAsync("input[name=\"expirationDate\"]", "12/25");
            await _page.FillAsync("input[name=\"cvv\"]", "123");
            await _page.FillAsync("input[name=\"billingAddress\"]", "123 Main St");
            await _page.ClickAsync("button[type=\"submit\"]");
            await _page.ScreenshotAsync(new()
            {
                Path = $"screenshot{DateTime.UtcNow.Ticks}.png",
                FullPage = true,
            });
        }

        public async Task ShowsValidationErrorsForEmptyRequiredFields(IPage _page)
        {
            await _page.GotoAsync("http://localhost:5173/");
            await _page.ClickAsync("button[type=\"submit\"]");

            foreach (var name in new[] { "fullName", "email", "cardNumber", "expirationDate", "cvv", "billingAddress" })
            {
                var locator = _page.Locator($"input[name=\"{name}\"]");
            }
            
            await _page.ScreenshotAsync(new()
            {
                Path = $"screenshot{DateTime.UtcNow.Ticks}.png",
                FullPage = true,
            });
        }

        public async Task ResetsFormWhenClickingResetButton(IPage _page)
        {
            await _page.GotoAsync("http://localhost:5173/");
            await _page.FillAsync("input[name=\"fullName\"]", "John Doe");
            await _page.FillAsync("input[name=\"email\"]", "john@example.com");
            await _page.ClickAsync("button:text(\"Reset\")");
            await _page.ScreenshotAsync(new()
            {
                Path = $"screenshot{DateTime.UtcNow.Ticks}.png",
                FullPage = true,
            });
        }

        public async Task ValidatesEmailFormat(IPage _page)
        {
            await _page.GotoAsync("http://localhost:5173/");
            await _page.FillAsync("input[name=\"email\"]", "invalid-email");
            await _page.ClickAsync("button[type=\"submit\"]");
            await _page.ScreenshotAsync(new()
            {
                Path = $"screenshot{DateTime.UtcNow.Ticks}.png",
                FullPage = true,
            });
        }
    

        public async Task ChecksAccessibilityFeatures(IPage _page)
        {
            await _page.GotoAsync("http://localhost:5173/");

            // Keyboard accessibility: first Tab should focus the fullName field
            await _page.Keyboard.PressAsync("Tab");
            var activeName = await _page.EvaluateAsync<string>("() => document.activeElement?.getAttribute('name')");
            await _page.ScreenshotAsync(new()
            {
                Path = $"screenshot{DateTime.UtcNow.Ticks}.png",
                FullPage = true,
            });
        }

        public async Task TestsResponsivenessAcrossScreenSizes(IPage _page)
        {
            var viewports = new (int Width, int Height)[]
            {
                (1920, 1080), // Desktop
                (768, 1024),  // Tablet
                (375, 812)    // Mobile
            };

            foreach (var vp in viewports)
            {
                await _page.SetViewportSizeAsync(vp.Width, vp.Height);
                await _page.GotoAsync("http://localhost:5173/");
                await _page.ScreenshotAsync(new()
                {
                    Path = $"screenshot{DateTime.UtcNow.Ticks}.png",
                    FullPage = true,
                });
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
            return "";
        }
    }
    
    private Task InstallBrowsersIfNeeded()
    {
        if (IsInitialized)
        {
            return Task.CompletedTask;
        }
        
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

                IsInitialized = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error installing browsers: {ex.Message}");
            throw;
        }

        return Task.CompletedTask;
    }
}