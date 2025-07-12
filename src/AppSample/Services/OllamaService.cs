using System;
using OllamaSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using AppSample.Models;

namespace AppSample.Services;

public interface IOllamaService
{
    IAsyncEnumerable<string> SendMessage(string message);
    Task<IEnumerable<Agent>> GetToolsAsync();

    Task LoadToolsCatalogAsync();
}

public class OllamaService : IOllamaService
{
    string agentsCatalogUrl = "http://localhost:5227";
    string _ollamaServer = "http://127.0.0.1:11434";
    string _model = "gemma3:12b";
    OllamaApiClient _ollamaClient;
    Chat _ollamaChat;

    List<Agent> _tools = new();
    public OllamaService()
    {
        _ollamaClient = new OllamaApiClient(new Uri(_ollamaServer));
        _ollamaClient.SelectedModel = _model;
        _ollamaChat = new Chat(_ollamaClient);
    }

    public async IAsyncEnumerable<string> SendMessage(string message)
    {
        await foreach (var token in _ollamaChat.SendAsync(message))
        {
            yield return token;
        }
    }

    public async Task<IEnumerable<Agent>> GetToolsAsync()
    {
        return _tools;
    }

    public async Task LoadToolsCatalogAsync()
    {
        using (var client = new HttpClient())
        {
            var response = await client.GetAsync(new Uri(agentsCatalogUrl));

            if (response.IsSuccessStatusCode)
            {
                var tools = await response.Content.ReadFromJsonAsync<List<Agent>>();

                if (tools != null)
                {
                    _tools = tools;
                }
            }
        }
    }
}