namespace AgenticHarness.Domain;

/// <summary>
/// Représente une anomalie ou recommandation détectée par un Agent Spécialiste.
/// </summary>
public sealed record AuditFinding(
    string AgentId,
    string Severity,
    string Category,
    string Description,
    string Recommendation);
