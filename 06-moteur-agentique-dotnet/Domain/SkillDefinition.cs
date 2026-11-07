namespace AgenticHarness.Domain;

/// <summary>
/// Représente une compétence (Skill) modulaire qu'on peut brancher sur un Agent
/// sans avoir à créer un nouvel Agent à chaque nouveau sujet technique.
/// </summary>
public sealed record SkillDefinition(
    string Name,
    string Description,
    IReadOnlyCollection<string> FocusCategories);
