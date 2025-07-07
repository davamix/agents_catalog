using AddAgent.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using OperationAgent.Models;

// Agent Info
var agentInfo = new AgentInfo(
    "ed260ed4-adb5-4206-bd30-0ad0e52a4c43",
    "Operation Agent",
    "An agent that performs operations",
    new() {
        { "http://localhost:5000/call/add", new[] { "number1", "number2" } },
        { "http://localhost:5000/call/subtract", new[] { "number1", "number2" } }
    }
);

var builder = WebApplication.CreateBuilder();

builder.Services.AddSingleton<IOperationService, OperationService>();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "Operation Agent";
    config.Title = "Operation Agent API";
    config.Version = "v1";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "Operation Agent API";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
    app.MapOpenApi();
}

app.MapGet("/call/add", (IOperationService service, [FromQuery] int a, [FromQuery] int b) => service.Add(a, b));
app.MapGet("/call/subtract", (IOperationService service, [FromQuery] int a, [FromQuery] int b) => service.Subtract(a, b));

try
{
    using (var client = new HttpClient())
    {
        client.BaseAddress = new Uri("http://localhost:5227");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // HTTP POST
        var content = new StringContent(
            JsonSerializer.Serialize(agentInfo),
            Encoding.UTF8, "application/json");

        var response = client.PostAsync("/register", content).Result;

        response.EnsureSuccessStatusCode();

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Agent registered successfully.");
        }
        else
        {
            Console.WriteLine($"Failed to register agent: {response.ReasonPhrase}");
        }

    }
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred while registering the agent: {ex.Message}");
}

app.Run();

