using System.ComponentModel.DataAnnotations;

namespace ModernApi.Contracts;

/// <summary>
/// Requête de validation d'une commande existante (protégée contre le Mass Assignment).
/// </summary>
/// <param name="CodePromo">Code promotionnel optionnel (max 30 caractères).</param>
public sealed record ValiderCommandeRequest(
    [property: StringLength(30)] string? CodePromo);
