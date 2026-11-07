namespace AgenticHarness.Domain;

/// <summary>
/// Distingue la posture de l'agent dans l'escouade :
/// - Builder : a le droit d'écrire/modifier du code (séquentiel ou branche isolée).
/// - Auditor : strictement en lecture seule, 100% parallélisable sans conflit.
/// </summary>
public enum AgentGroup
{
    Builder,
    Auditor
}
