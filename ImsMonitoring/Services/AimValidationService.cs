using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace ImsMonitoring.Services
{
    public interface IAimValidationService
    {
        Task<bool> TestConnection(string baseUrl, string username, string password);
    }

    public class AimValidationService : IAimValidationService
    {
        private readonly HttpClient _httpClient;

        public AimValidationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> TestConnection(string baseUrl, string username, string password)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl.TrimEnd('/')}/authenticate");
                
                var content = new StringContent(
                    JsonSerializer.Serialize(new { username, password }),
                    Encoding.UTF8,
                    "application/aim.api.v1+json"
                );

                request.Content = content;
                request.Headers.Accept.ParseAdd("application/aim.api.v1+json;charset=UTF-8");

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
} 