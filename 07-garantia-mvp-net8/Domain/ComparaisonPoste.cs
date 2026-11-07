namespace GarantIA.Domain;

/// <summary>
/// Résultat de la comparaison poste par poste entre le contrat actuel et l'offre proposée.
/// </summary>
public sealed record ComparaisonPoste(
    PosteSante Poste,
    string LibelleActe,
    NiveauRemboursement NiveauActuel,
    NiveauRemboursement NiveauPropose,
    EcartGarantie Ecart);
