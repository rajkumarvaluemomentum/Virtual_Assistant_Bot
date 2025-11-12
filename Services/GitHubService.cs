using System.Net.Http.Headers;
using System.Text.Json;
using System.Net;

namespace VirtualAssistant.API.Services
{
    public class GitHubService
    {
        private readonly HttpClient _httpClient;
        private readonly string _token;
        private readonly string _username;

        public GitHubService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            // First try environment variable (for production)
            var envToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

            // If not set, fallback to appsettings.json (for local dev)
            _token = !string.IsNullOrEmpty(envToken)
                ? envToken
                : configuration["GitHub:Token"]
                  ?? throw new InvalidOperationException("GitHub token is not set in environment variables or appsettings.json");

            _username = configuration["GitHub:Username"] 
                        ?? throw new InvalidOperationException("GitHub username is not set in configuration");

            _httpClient.BaseAddress = new Uri("https://api.github.com/");
            _httpClient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("VirtualAssistantAPI", "1.0"));
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("token", _token);
        }

        #region Repository Methods

        // Fetch all repository names
        public async Task<IEnumerable<string>> GetRepositoriesAsync()
        {
            var response = await _httpClient.GetAsync($"users/{_username}/repos");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("GitHub API returned 401 Unauthorized - check your token or credentials.");
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"GitHub API error: {(int)response.StatusCode} {response.ReasonPhrase}: {body}");
            }

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);
            var repoNames = document.RootElement
                .EnumerateArray()
                .Select(repo => repo.GetProperty("name").GetString()!)
                .ToList();

            return repoNames;
        }

        // Fetch deployments for a specific repository
        public async Task<IEnumerable<object>> GetDeploymentsAsync(string repoName)
        {
            var response = await _httpClient.GetAsync($"repos/{_username}/{repoName}/deployments");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("GitHub API returned 401 Unauthorized - check your token or credentials.");
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"GitHub API error: {(int)response.StatusCode} {response.ReasonPhrase}: {body}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var deployments = JsonSerializer.Deserialize<IEnumerable<object>>(json);
            return deployments ?? Enumerable.Empty<object>();
        }

        #endregion
    }
}
