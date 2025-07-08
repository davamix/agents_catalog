using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AppSample.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder();

builder.Services.AddSingleton<IOllamaService, OllamaService>();

var app = builder.Build();

app.MapGet("/", async (HttpContext context, IOllamaService ollamaService) => {
    await foreach (var response in ollamaService.SendMessage("Hello, tell me a joke!"))
    {
        await context.Response.WriteAsync(response);
        await context.Response.Body.FlushAsync();
        await Task.Delay(1);
    }
});

app.Run();