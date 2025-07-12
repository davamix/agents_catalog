using CatalogApi.Models;
using CatalogApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder();

builder.Services.AddSingleton<IAgentsService, AgentsService>();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config => {
    config.DocumentName = "Agent Catalog";
    config.Title = "Agent Catalog API";
    config.Version = "v1";
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseOpenApi();
    app.UseSwaggerUi(config => {
        config.DocumentTitle = "Agent Catalog API";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
    app.MapOpenApi();
}

app.MapGet("/", (IAgentsService service) => service.GetAllAgents());

app.MapPost("/register", (IAgentsService service, [FromBody] Agent agent) =>
{
    Console.WriteLine($"Registering agent: {agent.Name}");

    service.RegisterAgent(agent);

    Console.WriteLine($"Agent registered successfully.");
    Console.WriteLine($"Agent ID: {agent.Id},\n" +
    $"Name: {agent.Name},\n" +
    $"Description: {agent.Description}\n" +
    $"URLs: {string.Join(", ", agent.Urls.Select(u => $"{u.Key} ({string.Join(", ", u.Value)})"))}");
});

app.Run();