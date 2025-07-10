using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AppSample.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System;

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

app.Run();

public class MyRequest
{
    public string Message { get; set; }
}