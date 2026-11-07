namespace AgenticHarness.Domain;

/// <summary>
/// Garde-fou déterministe du Harness (Sandbox Policy).
/// Empêche tout agent (même s'il hallucine) d'accéder aux dossiers compilés
/// ou d'écrire des fichiers s'il est dans le groupe Auditor.
/// </summary>
public sealed class HarnessSandboxPolicy
{
    private static readonly string[] DefaultForbiddenSegments =
    [
        "bin", "obj", ".vs", "TestResults", "publish", "packages", "node_modules", "dist", "build", ".git"
    ];

    public int MaxReActIterations { get; private set; }
    public IReadOnlyCollection<string> ForbiddenSegments => DefaultForbiddenSegments;

    public HarnessSandboxPolicy(int maxReActIterations = 5)
    {
        if (maxReActIterations <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxReActIterations), "Le nombre max d'itérations doit être positif.");

        MaxReActIterations = maxReActIterations;
    }

    public bool IsPathAllowed(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return false;

        var normalized = filePath.Replace('\\', '/');
        var segments = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);

        return !segments.Any(segment =>
            DefaultForbiddenSegments.Any(forbidden =>
                segment.Equals(forbidden, StringComparison.OrdinalIgnoreCase)));
    }

    public void EnsureOperationAllowed(AgentProfile agent, string filePath, bool isWriteOperation)
    {
        if (!IsPathAllowed(filePath))
            throw new InvalidOperationException(
                $"[SANDBOX BLOCKED] L'agent '{agent.Id}' a tenté de lire un dossier interdit (bin/obj/.git) : {filePath}");

        if (isWriteOperation && !agent.CanWriteFiles)
            throw new InvalidOperationException(
                $"[SANDBOX BLOCKED] L'agent '{agent.Id}' ({agent.RoleTitle}) est en LECTURE SEULE et ne peut pas modifier {filePath}.");
    }
}
