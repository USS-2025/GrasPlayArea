using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
var env = app.Environment;

app.MapGet("/", () => $"{env.ApplicationName} API is running: {builder.Configuration["ASPNETCORE_URLS"]}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/hello/{name}", (string name) => $"Hello, {name}!")
    .WithSummary("Get a personalized greeting")
    .WithDescription("This endpoint returns a personalized greeting based on the provided name.")
    .WithTags("Greetings");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

Console.WriteLine($"{env.ApplicationName} is running: {builder.Configuration["ASPNETCORE_URLS"]}");
