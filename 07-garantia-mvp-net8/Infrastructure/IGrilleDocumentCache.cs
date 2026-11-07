namespace GarantIA.Infrastructure;

/// <summary>
/// Contrat d'accès au référentiel de cache FinOps des grilles d'assurance indexées par empreinte SHA-256.
/// Respecte strictement la séparation Command-Query (CQS).
/// </summary>
public interface IGrilleDocumentCache
{
    /// <summary>
    /// [Query Pure - Sans effet de bord métier] Recherche une grille connue par son empreinte SHA-256 (lecture seule AsNoTracking).
    /// </summary>
    GrilleCacheEntry? TryGetBySha256AsNoTracking(string sha256Hash);

    /// <summary>
    /// [Command] Enregistre ou met à jour une grille standardisée dans le cache FinOps.
    /// </summary>
    void Store(GrilleCacheEntry entry);
}
