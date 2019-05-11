using Newtonsoft.Json;
using APIASYNC.ExternalModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace APIASYNC.Repositories
{
    public class GitHubRepository : IGitHubRepository, IDisposable
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private ConcurrentQueue<Contributor> contributors = new ConcurrentQueue<Contributor>();

        public GitHubRepository(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ??
               throw new ArgumentNullException(nameof(httpClientFactory));
        }

        // Obtenemos usuarios de BCN con mayor número de repositorios
        // y los guardamos en una cola concurrente

        private async Task<ConcurrentQueue<Contributor>> GetUsersAsync(string cityname)
        {
            var httpClient = SetHttpClient("application/vnd.github.v3+json");

            var users = await httpClient
                      .GetAsync("https://api.github.com/search/users?q=location:" + cityname + "&s=repositories")
                      .ConfigureAwait(false);

            if (users.IsSuccessStatusCode)
            {
                var usersJson = JsonConvert.DeserializeObject<RootContributors>(await users.Content.ReadAsStringAsync());

                foreach (var item in usersJson.items)
                {
                    contributors.Enqueue(item);
                }
            }
            return contributors;
        }

        // Para los usuarios de BCN con mas repositorios, obtenemos sus commits
        private async Task<TopContributor> GetCommitsFromUserAsync(Contributor item)
        {
            var httpClient = SetHttpClient("application/vnd.github.cloak-preview");

            var commits = await httpClient
                   .GetAsync("https://api.github.com/search/commits?q=author:" + item.login)
                   .ConfigureAwait(false);

            if (commits.IsSuccessStatusCode)
            {
                var commitsJson = JsonConvert.DeserializeObject<RootCommits>
                     (await commits.Content.ReadAsStringAsync().ConfigureAwait(false));

                return new TopContributor(item.login, commitsJson.total_count);
            }

            return null;
        }

        private HttpClient SetHttpClient(string acceptheaders)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(acceptheaders));
            httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            return httpClient;
        }

        public void Dispose()
        {
          // _httpClient.Dispose();
        }

        public async Task<List<TopContributor>> GetTopContributorsAsync(string cityname)
        {           
            contributors = await GetUsersAsync(cityname).ConfigureAwait(false);
            var listContributors = contributors.ToList();

            var TaskList = new List<Task<TopContributor>>();

            foreach (var c in listContributors)
            {
                TaskList.Add(GetCommitsFromUserAsync(c));
            }
            var result = await Task.WhenAll(TaskList.ToList()).ConfigureAwait(false);

            return result.ToList();
        }
    }
}