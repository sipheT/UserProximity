using System.Text.Json;
using UserProximity.Models;
using UserProximity.Services.Implementation;
using UserProximity.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IUserService, UserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// GET: /api/users - Retrieve external users
app.MapGet("/api/users", async (IHttpClientFactory httpClientFactory) =>
{
    var httpClient = httpClientFactory.CreateClient();
    var response = await httpClient.GetAsync("https://jsonplaceholder.typicode.com/users");
    if (!response.IsSuccessStatusCode)
    {
        return Results.StatusCode((int)response.StatusCode);
    }
    var content = await response.Content.ReadAsStringAsync();
    var externalUsers = JsonSerializer.Deserialize<IEnumerable<User>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    return Results.Ok(externalUsers);
});

// POST: /api/users - Add a new user
app.MapPost("/api/users", (User newUser, IUserService userService) =>
{
    var user = userService.AddUser(newUser);
    return Results.Created($"/api/users/{user.Id}", user);
});

// PUT: /api/users/{id} - Update an existing user
app.MapPut("/api/users/{id:int}", (int id, User updatedUser, IUserService userService) =>
{
    var user = userService.UpdateUser(id, updatedUser);
    return user is null ? Results.NotFound() : Results.Ok(user);
});

// DELETE: /api/users/{id} - Delete a user
app.MapDelete("/api/users/{id:int}", (int id, IUserService userService) =>
{
    var result = userService.DeleteUser(id);
    return result ? Results.NoContent() : Results.NotFound();
});

// GET: /api/users/nearest - Find the nearest user for each hotel
app.MapGet("/api/users/nearest", (IUserService userService) =>
{
    var hotels = new List<(string Name, double Lat, double Lon)>
    {
        ("Hotel A", -43.9509, -34.4618),
        ("Hotel B", 40.7128, -74.0060),
        ("Hotel C", 34.0522, -118.2437),
        ("Hotel D", -25.2744, 133.7751)
    };

    var users = userService.GetUsers();

    // Local helper function to convert degrees to radians
    double ToRadians(double angle) => angle * (Math.PI / 180);

    // Haversine formula for calculating distance
    double Distance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth's radius in kilometers
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    var result = hotels.Select(hotel =>
    {
        var nearestUser = users.OrderBy(user => Distance(hotel.Lat, hotel.Lon, double.Parse(user.Address.Geo.Lat),double.Parse(user.Address.Geo.Lng)))
                               .FirstOrDefault();
        return new
        {
            Hotel = hotel.Name,
            NearestUser = nearestUser
        };
    });

    return Results.Ok(result);
});

app.Run();
