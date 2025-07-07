using AddAgent.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Threading.Tasks;

// Agent Info
var agentId = "ed260ed4-adb5-4206-bd30-0ad0e52a4c43";
var agentName = "Operation Agent";
var agentDescription = "An agent that performs addition operations";
// var agentUrl = "http://localhost:5000/call";
var agentCallAddUrl = "http://localhost:5000/call/add";
var agentCallSubtractUrl = "http://localhost:5000/call/subtract";

var builder = WebApplication.CreateBuilder();

builder.Services.AddSingleton<IOperationService, OperationService>();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "Add Agent";
    config.Title = "Add Agent API";
    config.Version = "v1";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "Add Agent API";
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
            $"{{\"id\":\"{agentId}\",\"name\":\"{agentName}\",\"description\":\"{agentDescription}\",\"urls\":[\"{agentCallAddUrl}\", \"{agentCallSubtractUrl}\"]}}",
            System.Text.Encoding.UTF8, "application/json");

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

