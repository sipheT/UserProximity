using System.Globalization;
using System.Text.Json;
using UserProximity.Helpers;
using UserProximity.Models;
using UserProximity.Services.Implementation;
using UserProximity.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IUserService, UserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// GET: /api/users - Retrieve external users
app.MapGet("/api/users", async (IHttpClientFactory httpClientFactory) =>
{
    try
    {
        var externalUsers = await FetchExternalUsers(httpClientFactory);
        return Results.Ok(externalUsers);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
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

app.MapGet("/api/users/nearest", async (IHttpClientFactory httpClientFactory) =>
{
    // Predefined hotels
    var hotels = new List<(string Name, double Lat, double Lon)>
    {
        ("Hotel A", -43.9509, -34.4618),
        ("Hotel B", 40.7128, -74.0060),
        ("Hotel C", 34.0522, -118.2437),
        ("Hotel D", -25.2744, 133.7751)
    };

    IEnumerable<User> users;
    try
    {
        users = await FetchExternalUsers(httpClientFactory);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }

    if (users == null || !users.Any())
    {
        return Results.NotFound("No users found.");
    }

    var result = hotels.Select(hotel =>
    {
        // Use DistanceHelper.Distance to calculate distance
        var nearestUser = users.OrderBy(user =>
            DistanceHelper.Distance(
                hotel.Lat,
                hotel.Lon,
                double.Parse(user.Address.Geo.Lat.Trim(), CultureInfo.InvariantCulture),
                double.Parse(user.Address.Geo.Lng.Trim(), CultureInfo.InvariantCulture)
            )).FirstOrDefault();

        return new
        {
            Hotel = hotel.Name,
            NearestUser = nearestUser
        };
    });

    return Results.Ok(result);
});

async Task<IEnumerable<User>> FetchExternalUsers(IHttpClientFactory httpClientFactory)
{
    var httpClient = httpClientFactory.CreateClient();
    var response = await httpClient.GetAsync("https://jsonplaceholder.typicode.com/users");
    if (!response.IsSuccessStatusCode)
    {
        throw new Exception($"Failed to fetch users. Status code: {response.StatusCode}");
    }
    var content = await response.Content.ReadAsStringAsync();
    var users = JsonSerializer.Deserialize<IEnumerable<User>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    return users;
}


app.Run();
