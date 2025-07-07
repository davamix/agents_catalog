using CatalogApi.Models;

namespace CatalogApi.Services;

public interface IAgentsService {
    IEnumerable<Agent> GetAllAgents();
    void RegisterAgent(Agent agent);
}

public class AgentsService : IAgentsService {
    private List<Agent> _agents = new();

    public AgentsService()
    {
        InitializeFakeAgents();
    }

    private void InitializeFakeAgents() {
        _agents.Add(new Agent("1", "Agent 1", "First agent", new[]{ "http://agent1.example.com" }));
        _agents.Add(new Agent("2", "Agent 2", "Second agent", new[]{"http://agent2.example.com" }));
        _agents.Add(new Agent("3", "Agent 3", "Third agent", new[]{"http://agent3.example.com" }));
    }

    public IEnumerable<Agent> GetAllAgents() {
        return _agents;
    }

    public void RegisterAgent(Agent agent) {
        if (_agents.Any(a => a.Id == agent.Id)) {
            _agents.RemoveAll(a=> a.Id == agent.Id);
        }
        
        _agents.Add(agent);
    }
}