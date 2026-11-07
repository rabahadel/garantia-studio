using System.Diagnostics;
using AgenticHarness.Domain;
using AgenticHarness.Tools;

namespace AgenticHarness.Orchestration;

/// <summary>
/// Exécute la boucle autonome ReAct (Reason + Act + Observe) pour un Agent Spécialiste donné,
/// sous le contrôle strict du HarnessSandboxPolicy.
/// </summary>
public sealed class SpecialistAgentRunner
{
    private readonly HarnessSandboxPolicy _sandboxPolicy;
    private readonly IReadOnlyCollection<IAgentTool> _tools;

    public SpecialistAgentRunner(HarnessSandboxPolicy sandboxPolicy, IEnumerable<IAgentTool> tools)
    {
        ArgumentNullException.ThrowIfNull(sandboxPolicy);
        ArgumentNullException.ThrowIfNull(tools);

        _sandboxPolicy = sandboxPolicy;
        _tools = tools.ToList().AsReadOnly();
    }

    public async Task<AgentExecutionReport> RunAuditAsync(
        AgentProfile agent,
        string targetFilePath,
        CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var executedSteps = new List<string>();
        var findings = new List<AuditFinding>();

        // Étape 1 (Reason) : Activation Progressive Disclosure des Skills de l'agent
        var activeSkills = agent.Skills.Select(s => s.Name).ToList().AsReadOnly();
        executedSteps.Add($"[Reason] Chargement des Skills : {string.Join(", ", activeSkills)}");

        // Étape 2 (Act) : Exécution des outils dans la limite MaxReActIterations du Harness
        var iteration = 0;
        foreach (var tool in _tools)
        {
            if (iteration >= _sandboxPolicy.MaxReActIterations)
                break;

            iteration++;
            executedSteps.Add($"[Act #{iteration}] Appel Tool '{tool.Name}' sur '{Path.GetFileName(targetFilePath)}'");

            var toolFindings = await tool.AnalyzeFileAsync(agent, targetFilePath, ct);
            findings.AddRange(toolFindings);

            executedSteps.Add($"[Observe #{iteration}] {toolFindings.Count} constat(s) détecté(s) dans le périmètre de {agent.Id}");
        }

        stopwatch.Stop();

        return new AgentExecutionReport(
            agent.Id,
            agent.RoleTitle,
            activeSkills,
            executedSteps.AsReadOnly(),
            findings.AsReadOnly(),
            stopwatch.Elapsed);
    }
}
