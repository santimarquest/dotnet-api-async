using APIASYNC.ExternalModels;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIASYNC.Repositories
{
    public interface IGitHubRepository
    {
        Task<List<TopContributor>> GetTopContributorsAsync(string cityname);
    }
}