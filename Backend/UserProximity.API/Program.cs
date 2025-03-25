using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserProximity.API.Data;
using UserProximity.API.Services.Implementation;
using UserProximity.API.Services.Interface;
using UserProximity.Models;
using UserProximity.Services.Implementation;
using UserProximity.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseInMemoryDatabase("UserDb"));

builder.Services.AddHttpClient();
builder.Services.AddScoped<IExternalUserService, ExternalUserService>();
builder.Services.AddScoped<INearestUserService, NearestUserService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/users", async (IExternalUserService externalUserService, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Fetching external users.");
        var result = await externalUserService.GetUsers();
        logger.LogInformation("Fetched {UserCount} external users.", users?.Count() ?? 0);
        return Results.Ok(result);
    }
    catch (Exception e)
    {
        logger.LogError(ex, "Error fetching external users.");
        return Results.Problem("An error occurred while fetching external users.");
    }
});

app.MapPost("/api/users", async (User newUser, IUserService userService, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Adding a new local user.");
        var addedUser = await userService.AddAsync(newUser);
        return Results.Created($"/api/localusers/{addedUser.Id}", addedUser);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error adding new local user.");
        return Results.Problem("An error occurred while adding the user.");
    }
});

app.MapPut("/api/users/{id:int}", async (int id, User updatedUser, IUserService userService, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Updating local user with id {UserId}.", id);
        // Ensure the ID is set
        updatedUser.Id = id;
        var user = await userService.UpdateAsync(updatedUser);
        if (user == null)
        {
            logger.LogWarning("Local user with id {UserId} not found.", id);
            return Results.NotFound();
        }
        return Results.Ok(user);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error updating local user with id {UserId}.", id);
        return Results.Problem("An error occurred while updating the user.");
    }
});

app.MapDelete("/api/users/{id:int}", async (int id, IUserService userService, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Deleting local user with id {UserId}.", id);
        var deleted = await userService.DeleteAsync(id);
        if (!deleted)
        {
            logger.LogWarning("Local user with id {UserId} not found.", id);
            return Results.NotFound();
        }
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error deleting local user with id {UserId}.", id);
        return Results.Problem("An error occurred while deleting the user.");
    }
});

app.MapGet("/api/users/nearest", async (INearestUserService nearestUserService, ILogger<Program> logger) =>
{
    try
    {
        var result = await nearestUserService.GetNearestUsersAsync();
        if (!result.Any())
        {
            logger.LogWarning("No nearest user result found.");
            return Results.NotFound("No users found.");
        }
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error calculating nearest users.");
        return Results.Problem("An error occurred while processing your request.");
    }
});

app.Run();
