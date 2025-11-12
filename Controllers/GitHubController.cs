using Microsoft.AspNetCore.Mvc;
using VirtualAssistant.API.Services;

namespace VirtualAssistant.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GitHubController : ControllerBase
    {
        private readonly GitHubService _gitHubService;

        public GitHubController(GitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        [HttpGet("repositories")]
        public async Task<IActionResult> GetRepositories()
        {
            var repos = await _gitHubService.GetRepositoriesAsync();
            return Ok(repos);
        }

        [HttpGet("repositories/{repoName}/deployments")]
        public async Task<IActionResult> GetDeployments(string repoName)
        {
            var deployments = await _gitHubService.GetDeploymentsAsync(repoName);
            return Ok(deployments);
        }
    }
}
