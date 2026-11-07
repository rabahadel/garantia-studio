using AgenticHarness.Domain;
using AgenticHarness.Orchestration;

namespace AgenticHarness;

/// <summary>
/// Fabrique l'escouade standard des 6 Agents Spécialistes (3 Builders + 3 Auditors)
/// avec leurs Skills respectifs. Montre comment les Rôles (Agents) sont découplés des Compétences (Skills).
/// </summary>
public static class SquadFactory
{
    public static void ConfigureDefaultEnterpriseSquad(MultiAgentSquadOrchestrator orchestrator)
    {
        ArgumentNullException.ThrowIfNull(orchestrator);

        // ====================================================================
        // GROUPE 1 : LES 3 BÂTISSEURS (Lecture + Écriture - Séquentiel / Branche)
        // ====================================================================
        orchestrator.RegisterAgent(new AgentProfile(
            id: "agent-maintenance-net4",
            roleTitle: "Spécialiste Maintenance Legacy (.NET 4 / MVC / EF6)",
            group: AgentGroup.Builder,
            systemPrompt: "Refactorer et corriger les bugs Legacy avec impact minimal et zéro régression.",
            initialSkills:
            [
                new SkillDefinition("csharp-refactoring", "Refactoring sécurisé sans changement de comportement", ["Maintenance-Net4"]),
                new SkillDefinition("systematic-debugging", "Diagnostic méthodique par hypothèses et tests", ["Debugging"])
            ]));

        orchestrator.RegisterAgent(new AgentProfile(
            id: "agent-builder-net8",
            roleTitle: "Bâtisseur API Moderne (.NET 8+ / Clean Architecture)",
            group: AgentGroup.Builder,
            systemPrompt: "Concevoir et coder des API .NET 8 modernes (1 Type = 1 Fichier, SOLID, GRASP, CQS).",
            initialSkills:
            [
                new SkillDefinition("dotnet-webapi", "Conception d'endpoints ASP.NET Core Web API", ["API-Net8"]),
                new SkillDefinition("mutuelle-sante-domain", "Règles métier Adhérents, Garanties, Prestations", ["Domaine-Metier"])
            ]));

        orchestrator.RegisterAgent(new AgentProfile(
            id: "agent-qa-tests",
            roleTitle: "Ingénieur Tests & Qualité (MSTest / Anti-patterns)",
            group: AgentGroup.Builder,
            systemPrompt: "Écrire les tests unitaires/intégration et éliminer les anti-patterns de tests.",
            initialSkills:
            [
                new SkillDefinition("writing-mstest-tests", "Rédaction de tests MSTest modernes", ["QA-Tests"]),
                new SkillDefinition("test-anti-patterns", "Détection des tests fragiles ou sans assertion", ["QA-Tests"])
            ]));

        // ====================================================================
        // GROUPE 2 : LES 3 AUDITEURS & EXPERTS (Lecture Seule - 100% Parallèle)
        // ====================================================================
        orchestrator.RegisterAgent(new AgentProfile(
            id: "agent-expert-data-sql-ef",
            roleTitle: "Expert Data, SQL Server & EF Code First",
            group: AgentGroup.Auditor,
            systemPrompt: "Auditer les requêtes SQL/LINQ, traquer les N+1, AsNoTracking et types SQL Server.",
            initialSkills:
            [
                new SkillDefinition("sqlserver-analysis", "Plans d'exécution, Index SQL Server, T-SQL", ["SQL-EF"]),
                new SkillDefinition("optimizing-ef-core-queries", "Optimisation EF Core/EF6, N+1, AsNoTracking", ["SQL-EF"])
            ]));

        orchestrator.RegisterAgent(new AgentProfile(
            id: "agent-expert-perf-archi",
            roleTitle: "Expert Performance Runtime .NET & Architecture",
            group: AgentGroup.Auditor,
            systemPrompt: "Auditer la performance CPU/Mémoire/Async .NET et l'architecture Clean Code (SOLID, GRASP).",
            initialSkills:
            [
                new SkillDefinition("analyzing-dotnet-performance", "Allocations mémoire, GC, Async/ThreadPool, LINQ", ["Perf-Runtime"]),
                new SkillDefinition("dotnet-architecture-review", "Clean Architecture, 1 Type = 1 Fichier, SOLID, GRASP", ["Architecture"])
            ]));

        orchestrator.RegisterAgent(new AgentProfile(
            id: "agent-auditeur-rssi-prod",
            roleTitle: "Auditeur RSSI (OWASP) & Exploitabilité Prod",
            group: AgentGroup.Auditor,
            systemPrompt: "Auditer les failles OWASP (IDOR, Mass Assignment, fuites JSON) et la résilience Prod.",
            initialSkills:
            [
                new SkillDefinition("owasp-security-audit", "Contrôle d'accès, IDOR, Mass Assignment, Secrets", ["RSSI-Securite"])
            ]));
    }
}
