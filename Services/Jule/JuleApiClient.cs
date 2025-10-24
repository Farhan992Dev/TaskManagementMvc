using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TaskManagementMvc.Services.Jule
{
    public class JuleApiClient
    {
        private readonly HttpClient _httpClient;

        public JuleApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new System.Uri("https://jules.googleapis.com/v1alpha/");
        }

        public async Task<string> ListSourcesAsync(string apiKey)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "sources");
            request.Headers.Add("X-Goog-Api-Key", apiKey);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> CreateSessionAsync(string apiKey, string source, string title, string prompt)
        {
            var requestBody = new
            {
                prompt,
                sourceContext = new
                {
                    source,
                    githubRepoContext = new
                    {
                        startingBranch = "main"
                    }
                },
                automationMode = "AUTO_CREATE_PR",
                title
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "sessions");
            request.Headers.Add("X-Goog-Api-Key", apiKey);
            request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetSessionAsync(string apiKey, string sessionId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"sessions/{sessionId}");
            request.Headers.Add("X-Goog-Api-Key", apiKey);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
