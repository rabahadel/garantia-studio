namespace AgenticHarness.Domain;

/// <summary>
/// Résultat de l'exécution autonome d'un Agent Spécialiste après sa boucle ReAct.
/// </summary>
public sealed record AgentExecutionReport(
    string AgentId,
    string RoleTitle,
    IReadOnlyCollection<string> ActiveSkills,
    IReadOnlyCollection<string> ExecutedToolSteps,
    IReadOnlyCollection<AuditFinding> Findings,
    TimeSpan Duration);
