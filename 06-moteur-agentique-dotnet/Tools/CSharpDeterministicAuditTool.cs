using System.Text.RegularExpressions;
using AgenticHarness.Domain;

namespace AgenticHarness.Tools;

/// <summary>
/// Outil de Backpressure Déterministe : analyse le code source C# selon les catégories
/// de compétences (SQL/EF, Performance Runtime, Architecture Clean Code, Sécurité RSSI).
/// </summary>
public sealed class CSharpDeterministicAuditTool : IAgentTool
{
    private readonly HarnessSandboxPolicy _sandboxPolicy;

    public string Name => "csharp_deterministic_analyzer";
    public string Description => "Inspecte un fichier .cs pour détecter les violations Architecture, SQL/EF, Perf Runtime et RSSI.";

    public CSharpDeterministicAuditTool(HarnessSandboxPolicy sandboxPolicy)
    {
        ArgumentNullException.ThrowIfNull(sandboxPolicy);
        _sandboxPolicy = sandboxPolicy;
    }

    public async Task<IReadOnlyCollection<AuditFinding>> AnalyzeFileAsync(
        AgentProfile caller,
        string filePath,
        CancellationToken ct = default)
    {
        _sandboxPolicy.EnsureOperationAllowed(caller, filePath, isWriteOperation: false);

        if (!File.Exists(filePath))
            return [];

        var sourceCode = await File.ReadAllTextAsync(filePath, ct);
        var findings = new List<AuditFinding>();

        CheckArchitectureRules(caller, sourceCode, findings);
        CheckSqlAndEfRules(caller, sourceCode, findings);
        CheckRuntimePerformanceRules(caller, sourceCode, findings);
        CheckRssiSecurityRules(caller, sourceCode, findings);

        // Chaque agent ne remonte QUE les constats couverts par ses Skills équipés !
        return findings
            .Where(f => caller.HandlesCategory(f.Category))
            .ToList()
            .AsReadOnly();
    }

    private static void CheckArchitectureRules(AgentProfile caller, string code, List<AuditFinding> findings)
    {
        var typeMatches = Regex.Matches(code, @"\bpublic\s+(?:sealed\s+|abstract\s+|partial\s+)?(?:class|record|interface|enum)\s+\w+");
        if (typeMatches.Count > 1)
        {
            findings.Add(new AuditFinding(
                caller.Id,
                "Haute",
                "Architecture",
                $"Violation '1 Type = 1 Fichier .cs' : {typeMatches.Count} types publics déclarés dans le même fichier.",
                "Extraire chaque class/record/interface/enum public dans son propre fichier .cs."));
        }

        if (code.Contains("new AppDbContext()", StringComparison.Ordinal))
        {
            findings.Add(new AuditFinding(
                caller.Id,
                "Haute",
                "Architecture",
                "Couplage fort (DIP violé) : instanciation directe 'new AppDbContext()' dans le contrôleur.",
                "Injecter une abstraction ou un service via le constructeur (Dependency Injection)."));
        }
    }

    private static void CheckSqlAndEfRules(AgentProfile caller, string code, List<AuditFinding> findings)
    {
        if (code.Contains("foreach", StringComparison.Ordinal) && code.Contains("_db.", StringComparison.Ordinal))
        {
            findings.Add(new AuditFinding(
                caller.Id,
                "Critique",
                "SQL-EF",
                "Problème N+1 détecté : requête Entity Framework exécutée à l'intérieur d'une boucle foreach.",
                "Utiliser une projection SQL unique (.Select) ou un chargement groupé en dehors de la boucle."));
        }

        if (code.Contains("_db.Commandes", StringComparison.Ordinal) && !code.Contains("AsNoTracking", StringComparison.Ordinal))
        {
            findings.Add(new AuditFinding(
                caller.Id,
                "Haute",
                "SQL-EF",
                "Absence de .AsNoTracking() sur une requête de lecture (Violation CQS).",
                "Ajouter .AsNoTracking() sur les Queries pour éviter le coût CPU/RAM du Change Tracker EF."));
        }

        if (code.Contains("public string CodePromo { get; set; }", StringComparison.Ordinal) && !code.Contains("HasMaxLength", StringComparison.Ordinal))
        {
            findings.Add(new AuditFinding(
                caller.Id,
                "Moyenne",
                "SQL-EF",
                "Colonne string sans MaxLength (génère du nvarchar(max) non indexable sous SQL Server).",
                "Configurer HasMaxLength(30).IsUnicode(false) dans IEntityTypeConfiguration<T>."));
        }
    }

    private static void CheckRuntimePerformanceRules(AgentProfile caller, string code, List<AuditFinding> findings)
    {
        if (code.Contains("public ActionResult", StringComparison.Ordinal) && !code.Contains("async Task", StringComparison.Ordinal))
        {
            findings.Add(new AuditFinding(
                caller.Id,
                "Haute",
                "Perf-Runtime",
                "Appels I/O synchrones bloquants (.ToList(), .SaveChanges()) sans async/await ni CancellationToken.",
                "Passer en async Task<IActionResult> avec ToListAsync(ct) et SaveChangesAsync(ct) pour éviter la famine du ThreadPool."));
        }

        if (code.Contains("new AppDbContext()", StringComparison.Ordinal) && !code.Contains("Dispose", StringComparison.Ordinal))
        {
            findings.Add(new AuditFinding(
                caller.Id,
                "Haute",
                "Perf-Runtime",
                "Fuite potentielle de ressources mémoire/connexions : AppDbContext instancié en champ privé sans Dispose.",
                "Laisser le conteneur DI gérer le cycle de vie Scoped du DbContext."));
        }
    }

    private static void CheckRssiSecurityRules(AgentProfile caller, string code, List<AuditFinding> findings)
    {
        if (!code.Contains("[Authorize]", StringComparison.Ordinal))
        {
            findings.Add(new AuditFinding(
                caller.Id,
                "Critique",
                "RSSI-Securite",
                "Absence d'attribut [Authorize] et contrôle d'accès manquant sur clientId (Faille IDOR OWASP A01).",
                "Ajouter [Authorize] et vérifier que le clientId demandé correspond au Claim de l'utilisateur connecté."));
        }

        if (code.Contains("EntityState.Modified", StringComparison.Ordinal))
        {
            findings.Add(new AuditFinding(
                caller.Id,
                "Critique",
                "RSSI-Securite",
                "Faille Mass Assignment / Over-Posting : attachement direct de l'entité reçue avec EntityState.Modified.",
                "Utiliser un record DTO d'entrée strict (ex: ValiderCommandeRequest) et charger l'entité depuis la base."));
        }

        if (code.Contains("JsonRequestBehavior.AllowGet", StringComparison.Ordinal) && code.Contains("CommentaireInterne", StringComparison.Ordinal))
        {
            findings.Add(new AuditFinding(
                caller.Id,
                "Haute",
                "RSSI-Securite",
                "Fuite de données sensibles : sérialisation JSON directe de l'entité exposant 'CommentaireInterne'.",
                "Projeter vers un DTO de sortie (CommandeResponse) excluant les champs internes."));
        }
    }
}
