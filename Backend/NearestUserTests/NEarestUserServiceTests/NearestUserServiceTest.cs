using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using UserProximity.API.Services.Implementation;
using UserProximity.Models;
using UserProximity.Services.Interface;

namespace NearestUserServiceTests
{
    public class NearestUserServiceTests
    {
        // Fake IUserFetcher that returns a predefined set of users.
        private class ExternalUserService : IExternalUserService
        {
            public Task<IEnumerable<User>> GetUsers()
            {
                var users = new List<User>
                {
                    new User
                    {
                        Id = 1,
                        Name = "User A",
                        Address = new Address
                        {
                            Geo = new Geo { Lat = "-43.9509", Lng = "-34.4618" }
                        }
                    },
                    new User
                    {
                        Id = 2,
                        Name = "User B",
                        Address = new Address
                        {
                            Geo = new Geo { Lat = "40.7128", Lng = "-74.0060" }
                        }
                    },
                    new User
                    {
                        Id = 3,
                        Name = "User C",
                        Address = new Address
                        {
                            Geo = new Geo { Lat = "34.0522", Lng = "-118.2437" }
                        }
                    },
                    new User
                    {
                        Id = 4,
                        Name = "User D",
                        Address = new Address
                        {
                            Geo = new Geo { Lat = "-25.2744", Lng = "133.7751" }
                        }
                    }
                };
                return Task.FromResult<IEnumerable<User>>(users);
            }
        }

        [Fact]
        public async Task GetNearestUsersAsync_ReturnsExpectedResults()
        {
            // Arrange
            var fakeUserFetcher = new ExternalUserService();

            var logger = NullLogger<NearestUserService>.Instance;
            var nearestUserService = new NearestUserService(fakeUserFetcher, logger);

            // Act
            var result = await nearestUserService.GetNearestUsersAsync();

            // Assert
            Assert.NotNull(result);
            // The service defines 4 hotels: "Hotel A", "Hotel B", "Hotel C", "Hotel D"
            // So we expect 4 results.
            Assert.Equal(4, result.Count());

            var resultList = result.ToList();

            // Validate Hotel A
            var hotelAResult = resultList.FirstOrDefault(r => r.Hotel == "Hotel A");
            Assert.NotNull(hotelAResult);
            Assert.Equal("User A", hotelAResult.NearestUser.Name);

            // Validate Hotel B
            var hotelBResult = resultList.FirstOrDefault(r => r.Hotel == "Hotel B");
            Assert.NotNull(hotelBResult);
            Assert.Equal("User B", hotelBResult.NearestUser.Name);

            // Validate Hotel C
            var hotelCResult = resultList.FirstOrDefault(r => r.Hotel == "Hotel C");
            Assert.NotNull(hotelCResult);
            Assert.Equal("User C", hotelCResult.NearestUser.Name);

            // Validate Hotel D
            var hotelDResult = resultList.FirstOrDefault(r => r.Hotel == "Hotel D");
            Assert.NotNull(hotelDResult);
            Assert.Equal("User D", hotelDResult.NearestUser.Name);
        }
    }
}