using System.Text.Json;
using UserProximity.Models;
using UserProximity.Services.Interface;

namespace UserProximity.API.Services.Implementation
{
    public class ExternalUserService : IExternalUserService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ExternalUserService> _logger;

        public ExternalUserService(IHttpClientFactory httpClientFactory, ILogger<ExternalUserService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        public async Task<IEnumerable<User>> GetUsers()
        {
            try
            {
                return await FetchExternalUsers();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new List<User>();
            }
        }

        private async Task<IEnumerable<User>> FetchExternalUsers()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync("https://jsonplaceholder.typicode.com/users");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch users. Status code: {response.StatusCode}");
            }
            var content = await response.Content.ReadAsStringAsync();
            var users = JsonSerializer.Deserialize<IEnumerable<User>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return users;
        }
    }
}
