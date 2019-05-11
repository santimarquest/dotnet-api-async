using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APIASYNC.Repositories;
using System;
using System.Threading.Tasks;

namespace APIASYNC.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GitHubController : ControllerBase
    {
        private IGitHubRepository _gitHubRepository;
        private readonly ILogger<GitHubRepository> _logger;

        public GitHubController(IGitHubRepository gitHubRepository,
             ILogger<GitHubRepository> logger)
        {
            _gitHubRepository = gitHubRepository
                ?? throw new ArgumentNullException(nameof(gitHubRepository));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{cityname}")]
        public async Task<IActionResult> GetTopContributors(string cityname)
        {
            try
            {
                //throw new Exception();
                var topListContributors = await _gitHubRepository.GetTopContributorsAsync(cityname);
                return Ok(topListContributors);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error:" + ex.Message);
                return BadRequest(ex);
            }          
        }
    }
}