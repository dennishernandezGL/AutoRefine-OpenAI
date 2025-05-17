using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Octokit;

namespace OpenAILambda;

public class GithubService
{
    private readonly IConfiguration _configuration;
    private readonly CreateBranchRequest GitHubConfiguration;
    private readonly GitHubClient _gitHubClient;

    public GithubService(IConfiguration configuration)
    {
        _configuration = configuration;

        this.GitHubConfiguration = new CreateBranchRequest()
        {
            GitHubToken = "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",
            Owner = _configuration["GitHub:Owner"] ?? throw new ArgumentNullException("GitHub:Owner"),
            Repository = _configuration["GitHub:Repository"] ?? throw new ArgumentNullException("GitHub:Repository"),
            BaseBranch = _configuration["GitHub:BaseBranch"] ?? throw new ArgumentNullException("GitHub:BaseBranch"),
            UserAgent = _configuration["GitHub:UserAgent"] ?? throw new ArgumentNullException("GitHub:UserAgent")
        };
        
        _gitHubClient = new GitHubClient(new ProductHeaderValue("AutoRefineOpenAI"))
        {
            Credentials = new Credentials(this.GitHubConfiguration.GitHubToken)
        };
    }
    
    public async Task<List<RepositoryFile>> GetRepositoryFilesAsync()
    {
        List<RepositoryFile> files = new List<RepositoryFile>();
            
        try
        {
            var rootContents = await _gitHubClient.Repository.Content.GetAllContents(this.GitHubConfiguration.Owner, 
                this.GitHubConfiguration.Repository, "ai-no-experts-frontend/src");
                
            await GetFilesRecursivelyAsync(rootContents, "ai-no-experts-frontend/src", files);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener archivos del repositorio: {ex.Message}");
            throw;
        }
            
        return files;
    }
    
    private async Task GetFilesRecursivelyAsync(IReadOnlyList<RepositoryContent> contents, string path, List<RepositoryFile> files)
    {
        foreach (var content in contents)
        {
            var currentPath = Path.Combine(path, content.Name);
                
            if (content.Type == ContentType.Dir)
            {
                var directoryContents = await _gitHubClient.Repository.Content.GetAllContents(this.GitHubConfiguration.Owner,
                    this.GitHubConfiguration.Repository, currentPath);
                await GetFilesRecursivelyAsync(directoryContents, currentPath, files);
            }
            else if (content.Type == ContentType.File)
            {
                if (!ShouldAnalyzeFile(content.Name))
                {
                    continue;
                }
                
                var fileContent = await _gitHubClient.Repository.Content.GetRawContent(this.GitHubConfiguration.Owner, this.GitHubConfiguration.Repository, currentPath);
                var contentString = Encoding.UTF8.GetString(fileContent);
                        
                files.Add(new RepositoryFile
                {
                    Path = currentPath,
                    Name = content.Name,
                    Content = contentString
                });
            }
        }
    }
    
    private bool ShouldAnalyzeFile(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
            
        var codeExtensions = new[] { ".tsx", ".js", ".html", ".css", ".scss", ".ts", ".json", ".env" };
            
        return Array.Exists(codeExtensions, ext => ext == extension);
    }
}

public class CreateBranchRequest
{
    public string GitHubToken { get; set; }
    public string Owner { get; set; }
    public string Repository { get; set; }
    public string BaseBranch { get; set; }
    public string UserAgent { get; set; }
}


public class GitHubBranchResponse
{
    public GitHubBranchObject Object { get; set; }
}

public class GitHubBranchObject
{
    public string Sha { get; set; }
}

public class FileMetadata
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("path")]
    public string Path { get; set; }

    [JsonPropertyName("sha")]
    public string Sha { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; }

    [JsonPropertyName("git_url")]
    public string GitUrl { get; set; }

    [JsonPropertyName("download_url")]
    public string DownloadUrl { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("encoding")]
    public string Encoding { get; set; }

    [JsonPropertyName("_links")]
    public Links Links { get; set; }
}

public class Links
{
    [JsonPropertyName("self")]
    public string Self { get; set; }

    [JsonPropertyName("git")]
    public string Git { get; set; }

    [JsonPropertyName("html")]
    public string Html { get; set; }
}

public class RepositoryFile
{
    public string Path { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
}