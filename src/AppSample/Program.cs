using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AppSample.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System;
using System.Linq;

var builder = WebApplication.CreateBuilder();

builder.Services.AddSingleton<IOllamaService, OllamaService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

app.MapGet("/tools", async (HttpContext context, IOllamaService ollamaService) =>
{
    var tools = await ollamaService.GetToolsAsync();
    await context.Response.WriteAsJsonAsync(tools);
});

app.MapPost("/", async (HttpContext context, IOllamaService ollamaService, HttpRequest request) =>
{
    var body = await request.ReadFromJsonAsync<MyRequest>();
    if (body == null)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync("Message cannot be empty.");
    }
    Console.WriteLine($"Received message: {body.Message}");

    await foreach (var response in ollamaService.SendMessage(body.Message))
    {
        await context.Response.WriteAsync(response);
        await context.Response.Body.FlushAsync();
        await Task.Delay(1);

        Console.Write(response);
    }
});

try
{
    using (var scope = app.Services.CreateScope())
    {
        Console.WriteLine("Loading tools catalog...");

        var ollamaService = scope.ServiceProvider.GetRequiredService<IOllamaService>();

        await ollamaService.LoadToolsCatalogAsync();

        var tools = await ollamaService.GetToolsAsync();

        if (tools.Any())
        {
            Console.WriteLine("Tools catalog loaded successfully.");
            foreach (var tool in tools)
            {
                Console.WriteLine($"Tool: {tool.Name}");
            }

        }
        else
        {
            Console.WriteLine("No tools found in the catalog.");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error loading tools catalog: {ex.Message}");
}

app.Run();

public class MyRequest
{
    public string Message { get; set; }
}