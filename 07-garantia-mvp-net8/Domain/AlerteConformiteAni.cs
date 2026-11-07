namespace GarantIA.Domain;

/// <summary>
/// Alerte réglementaire levée lorsqu'une garantie ne respecte pas les planchers ANI ou Contrat Responsable.
/// </summary>
public sealed record AlerteConformiteAni(
    string CodeRegle,
    PosteSante PosteConcerne,
    string MessageExplicatif);
