using System;
using OllamaSharp;
using System.Collections.Generic;

namespace AppSample.Services;

public interface IOllamaService
{
    IAsyncEnumerable<string> SendMessage(string message);
}

public class OllamaService : IOllamaService
{
    string _ollamaServer = "http://127.0.0.1:11434";
    string _model = "gemma3:12b";
    OllamaApiClient _ollamaClient;
    Chat _ollamaChat;
    // Implementation of the service

    public OllamaService()
    {
        _ollamaClient = new OllamaApiClient(new Uri(_ollamaServer));
        _ollamaClient.SelectedModel = _model;
        _ollamaChat = new Chat(_ollamaClient);
    }

    public async IAsyncEnumerable<string> SendMessage(string message)
    {
        await foreach (var token in _ollamaChat.SendAsync(message)) {
            yield return token;    
        }
    }
}