namespace ModernApi.Contracts;

/// <summary>
/// Représente le détail d'une commande client exposée par l'API (sans données internes sensibles).
/// </summary>
/// <param name="Id">Identifiant unique de la commande.</param>
/// <param name="ClientId">Identifiant du client propriétaire.</param>
/// <param name="MontantTotal">Montant total TTC calculé.</param>
/// <param name="EstValidee">Indique si la commande a été validée.</param>
/// <param name="CodePromo">Code promotionnel éventuel appliqué.</param>
public sealed record CommandeResponse(
    int Id,
    int ClientId,
    decimal MontantTotal,
    bool EstValidee,
    string? CodePromo);
