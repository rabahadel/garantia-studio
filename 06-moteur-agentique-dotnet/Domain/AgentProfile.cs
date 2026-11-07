namespace AgenticHarness.Domain;

/// <summary>
/// Représente un Agent Spécialiste avec son Rôle, sa Posture (Builder vs Auditor),
/// ses droits Sandbox et son sac à dos de Skills évolutif (GRASP Information Expert).
/// </summary>
public sealed class AgentProfile
{
    private readonly List<SkillDefinition> _skills = [];

    public string Id { get; private set; }
    public string RoleTitle { get; private set; }
    public AgentGroup Group { get; private set; }
    public bool CanWriteFiles { get; private set; }
    public string SystemPrompt { get; private set; }
    public IReadOnlyCollection<SkillDefinition> Skills => _skills.AsReadOnly();

    public AgentProfile(
        string id,
        string roleTitle,
        AgentGroup group,
        string systemPrompt,
        IEnumerable<SkillDefinition>? initialSkills = null)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("L'identifiant de l'agent est obligatoire.", nameof(id));

        if (string.IsNullOrWhiteSpace(roleTitle))
            throw new ArgumentException("Le titre du rôle est obligatoire.", nameof(roleTitle));

        Id = id;
        RoleTitle = roleTitle;
        Group = group;
        // Make Illegal States Unrepresentable : un Auditeur ne peut JAMAIS avoir les droits d'écriture
        CanWriteFiles = group == AgentGroup.Builder;
        SystemPrompt = systemPrompt;

        if (initialSkills is not null)
        {
            _skills.AddRange(initialSkills);
        }
    }

    /// <summary>
    /// Permet d'enrichir un agent existant avec un nouveau Skill au fil du temps
    /// sans multiplier le nombre d'agents ("Peu d'Agents, Beaucoup de Skills").
    /// </summary>
    public void EquipSkill(SkillDefinition skill)
    {
        ArgumentNullException.ThrowIfNull(skill);

        if (_skills.Any(s => s.Name.Equals(skill.Name, StringComparison.OrdinalIgnoreCase)))
            return;

        _skills.Add(skill);
    }

    public bool HandlesCategory(string category)
    {
        return _skills.Any(skill =>
            skill.FocusCategories.Any(c => c.Equals(category, StringComparison.OrdinalIgnoreCase)));
    }
}
