namespace CatalogApi.Models;

public record Agent(
    string Id,
    string Name,
    string Description,
    Dictionary<string, string[]> Urls);