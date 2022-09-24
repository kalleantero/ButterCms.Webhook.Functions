using ButterCms.Webhook.Functions.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ButterCms.Webhook.Functions.Services
{
    public class DevOpsService: IDevOpsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _personalAccessToken;
        private readonly ILogger<DevOpsService> _logger;

        public DevOpsService(
            IHttpClientFactory httpClientFactory,
            ILogger<DevOpsService> logger,
            string devOpsOrganizationName,
            string personalAccessToken)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri($"https://dev.azure.com/{devOpsOrganizationName}/");
            _personalAccessToken = personalAccessToken;
            _logger = logger;
        }

        public async Task<bool> StartPipeline()
        {
            try
            {
                //encode your personal access token                   
                string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _personalAccessToken)));

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                var data = new
                {
                    templateParameters = new object[] { }
                };

                var content = JsonContent.Create(data);

                var response = await _httpClient.PostAsync("YAML/_apis/pipelines/2/runs?api-version=6.0-preview.1", content);

                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while starting pipeline.");
                return false;
            }      
        }
    }
}
