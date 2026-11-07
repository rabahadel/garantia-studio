namespace GarantIA.Domain;

/// <summary>
/// Ligne de garantie normalisée extraite d'un tableau de garanties mutuelle santé.
/// </summary>
public sealed record LigneGarantie(
    PosteSante Poste,
    string LibelleActe,
    NiveauRemboursement Niveau);
