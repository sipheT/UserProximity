using System.Globalization;
using System.Text.Json;
using UserProximity.API.Services.Implementation;
using UserProximity.API.Services.Interface;
using UserProximity.Helpers;
using UserProximity.Models;
using UserProximity.Services.Implementation;
using UserProximity.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

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

app.MapGet("/api/users", async (IExternalUserService externalUserService) =>
{
    var result =  await externalUserService.GetUsers();
    return Results.Ok(result);
});

app.MapPost("/api/users", (User newUser, IUserService userService) =>
{
    var user = userService.AddUser(newUser);
    return Results.Created($"/api/users/{user.Id}", user);
});

app.MapPut("/api/users/{id:int}", (int id, User updatedUser, IUserService userService) =>
{
    var user = userService.UpdateUser(id, updatedUser);
    return user is null ? Results.NotFound() : Results.Ok(user);
});

app.MapDelete("/api/users/{id:int}", (int id, IUserService userService) =>
{
    var result = userService.DeleteUser(id);
    return result ? Results.NoContent() : Results.NotFound();
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
