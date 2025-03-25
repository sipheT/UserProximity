using System.Globalization;
using UserProximity.API.Models;
using UserProximity.API.Services.Interface;
using UserProximity.Helpers;
using UserProximity.Services.Interface;

namespace UserProximity.API.Services.Implementation
{
    public class NearestUserService : INearestUserService
    {
        private readonly IExternalUserService _externalUserService;
        private readonly ILogger<NearestUserService> _logger;

        public NearestUserService(IExternalUserService externalUserService, ILogger<NearestUserService> logger)
        {
            _externalUserService = externalUserService;
            _logger = logger;
        }

        public async Task<IEnumerable<NearestUserResult>> GetNearestUsersAsync()
        {
            _logger.LogInformation("Fetching external users for nearest calculation.");
            var users = await _externalUserService.GetUsers();

            if (users == null || !users.Any())
            {
                _logger.LogWarning("No users found.");
                return Enumerable.Empty<NearestUserResult>();
            }

            var hotels = new List<(string Name, double Lat, double Lon)>
            {
                ("Hotel A", -43.9509, -34.4618),
                ("Hotel B", 40.7128, -74.0060),
                ("Hotel C", 34.0522, -118.2437),
                ("Hotel D", -25.2744, 133.7751)
            };

            _logger.LogInformation("Calculating nearest users for hotels.");

            var results = hotels.Select(hotel =>
            {
                var nearestUser = users.OrderBy(user =>
                    DistanceHelper.Distance(
                        hotel.Lat,
                        hotel.Lon,
                        double.Parse(user.Address.Geo.Lat.Trim(), CultureInfo.InvariantCulture),
                        double.Parse(user.Address.Geo.Lng.Trim(), CultureInfo.InvariantCulture)
                    )).FirstOrDefault();

                return new NearestUserResult
                {
                    Hotel = hotel.Name,
                    NearestUser = nearestUser
                };
            }).ToList();

            _logger.LogInformation("Successfully calculated nearest users for hotels.");
            return results;
        }
    }
}
