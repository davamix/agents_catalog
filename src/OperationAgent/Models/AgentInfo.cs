namespace OperationAgent.Models;

public record AgentInfo(
    string Id,
    string Name,
    string Description,
    Dictionary<string, string[]> Urls);