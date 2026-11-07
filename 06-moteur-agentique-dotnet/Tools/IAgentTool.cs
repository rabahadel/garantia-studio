using AgenticHarness.Domain;

namespace AgenticHarness.Tools;

/// <summary>
/// Contrat d'un Outil (Tool) que le Harness met à disposition des Agents.
/// </summary>
public interface IAgentTool
{
    string Name { get; }
    string Description { get; }
    Task<IReadOnlyCollection<AuditFinding>> AnalyzeFileAsync(
        AgentProfile caller,
        string filePath,
        CancellationToken ct = default);
}
