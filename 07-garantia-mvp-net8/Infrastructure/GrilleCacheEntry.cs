namespace GarantIA.Infrastructure;

/// <summary>
/// Représente une entrée de grille de garanties standard mise en cache par son empreinte cryptographique SHA-256
/// afin d'éviter tout appel OCR/LLM redondant (coût FinOps IA = 0,00 €, latence &lt; 5 ms).
/// </summary>
public sealed record GrilleCacheEntry(
    string DocumentSha256,
    string NomAssureur,
    string NomContrat,
    decimal CotisationMensuelleHt,
    DateTimeOffset DateIndexation);
