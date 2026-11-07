using AgenticHarness.Domain;

namespace AgenticHarness.Orchestration;

/// <summary>
/// Orchestrateur Multi-Agents : gère le registre des Agents Spécialistes (Builders & Auditors),
/// permet d'ajouter de nouveaux Skills ou Agents à la volée, et lance les Auditeurs en parallèle (Task.WhenAll).
/// </summary>
public sealed class MultiAgentSquadOrchestrator
{
    private readonly Dictionary<string, AgentProfile> _agents = new(StringComparer.OrdinalIgnoreCase);
    private readonly SpecialistAgentRunner _runner;

    public IReadOnlyCollection<AgentProfile> RegisteredAgents => _agents.Values.ToList().AsReadOnly();

    public MultiAgentSquadOrchestrator(SpecialistAgentRunner runner)
    {
        ArgumentNullException.ThrowIfNull(runner);
        _runner = runner;
    }

    public void RegisterAgent(AgentProfile agent)
    {
        ArgumentNullException.ThrowIfNull(agent);
        _agents[agent.Id] = agent;
    }

    public void AddSkillToExistingAgent(string agentId, SkillDefinition newSkill)
    {
        if (!_agents.TryGetValue(agentId, out var agent))
            throw new KeyNotFoundException($"Agent introuvable dans le Harness : {agentId}");

        agent.EquipSkill(newSkill);
    }

    /// <summary>
    /// Lance tous les Agents Auditeurs (Lecture Seule) simultanément en parallèle via Task.WhenAll.
    /// </summary>
    public async Task<IReadOnlyCollection<AgentExecutionReport>> RunParallelAuditSquadAsync(
        string targetFilePath,
        CancellationToken ct = default)
    {
        var auditors = _agents.Values
            .Where(a => a.Group == AgentGroup.Auditor)
            .ToList();

        var parallelTasks = auditors.Select(auditor =>
            _runner.RunAuditAsync(auditor, targetFilePath, ct));

        var reports = await Task.WhenAll(parallelTasks);
        return reports.ToList().AsReadOnly();
    }
}
