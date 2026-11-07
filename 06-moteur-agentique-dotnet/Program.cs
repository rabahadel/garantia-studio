using System.Text;
using AgenticHarness;
using AgenticHarness.Domain;
using AgenticHarness.Orchestration;
using AgenticHarness.Tools;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("================================================================================");
Console.WriteLine("🛠️  HARNESS MULTI-AGENTS .NET — DÉMONSTRATION ARCHITECTURE AGENTS × SKILLS");
Console.WriteLine("================================================================================\n");

// 1. Initialisation du Harness (Sandbox Policy + Outils Déterministes)
var sandboxPolicy = new HarnessSandboxPolicy(maxReActIterations: 5);
var tools = new IAgentTool[] { new CSharpDeterministicAuditTool(sandboxPolicy) };
var runner = new SpecialistAgentRunner(sandboxPolicy, tools);
var orchestrator = new MultiAgentSquadOrchestrator(runner);

// 2. Enregistrement de l'Escouade des 6 Agents Spécialistes (3 Builders + 3 Auditors)
SquadFactory.ConfigureDefaultEnterpriseSquad(orchestrator);

Console.WriteLine($"✅ Escouade initialisée avec {orchestrator.RegisteredAgents.Count} Agents Spécialistes :");
foreach (var agent in orchestrator.RegisteredAgents)
{
    var mode = agent.CanWriteFiles ? "✍️ Lecture/Écriture (Builder)" : "🔒 Lecture Seule (Auditor Parallèle)";
    var skills = string.Join(", ", agent.Skills.Select(s => s.Name));
    Console.WriteLine($"   • [{agent.Id}] ({mode})");
    Console.WriteLine($"     ↳ Rôle   : {agent.RoleTitle}");
    Console.WriteLine($"     ↳ Skills : {skills}");
}

// 3. Démonstration de l'évolutivité : Ajouter un nouveau Skill à un Agent existant sans créer un 20e agent !
Console.WriteLine("\n➕ [ÉVOLUTIVITÉ] Ajout à la volée du Skill 'dotnet-trace-collect' sur 'agent-expert-perf-archi'...");
orchestrator.AddSkillToExistingAgent(
    "agent-expert-perf-archi",
    new SkillDefinition("dotnet-trace-collect", "Collecte de traces de performance en production", ["Perf-Runtime"]));

// 4. Exécution parallèle des 3 Agents Auditeurs sur LegacyCommandeController.cs et CommandesController.cs
var legacyFile = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "05-demo-multi-agents", "LegacyCommandeController.cs"));
var modernFile = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "05-demo-multi-agents", "Controllers", "CommandesController.cs"));

await RunAndPrintAuditAsync(orchestrator, legacyFile, "AUDIT #1 : Contrôleur Legacy (.NET 4 / EF6)");
await RunAndPrintAuditAsync(orchestrator, modernFile, "AUDIT #2 : Contrôleur Moderne (.NET 8 / Clean Archi)");

static async Task RunAndPrintAuditAsync(
    MultiAgentSquadOrchestrator orchestrator,
    string filePath,
    string title)
{
    Console.WriteLine($"\n--------------------------------------------------------------------------------");
    Console.WriteLine($"🚀 {title} -> {Path.GetFileName(filePath)}");
    Console.WriteLine($"--------------------------------------------------------------------------------");

    var reports = await orchestrator.RunParallelAuditSquadAsync(filePath);

    foreach (var report in reports)
    {
        Console.WriteLine($"\n🤖 Agent : {report.RoleTitle} [{report.AgentId}] ({report.Duration.TotalMilliseconds:F1} ms)");
        Console.WriteLine($"   Skills activés : {string.Join(", ", report.ActiveSkills)}");

        if (report.Findings.Count == 0)
        {
            Console.WriteLine("   ✅ Aucun problème détecté dans son périmètre d'expertise.");
            continue;
        }

        foreach (var finding in report.Findings)
        {
            Console.WriteLine($"   ❌ [{finding.Severity}] ({finding.Category}) {finding.Description}");
            Console.WriteLine($"      💡 Remédiation : {finding.Recommendation}");
        }
    }
}
