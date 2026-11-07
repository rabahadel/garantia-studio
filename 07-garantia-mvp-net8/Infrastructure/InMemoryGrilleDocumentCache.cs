using System.Collections.Concurrent;

namespace GarantIA.Infrastructure;

/// <summary>
/// Implémentation thread-safe du cache d'empreintes documentaires SHA-256 avec télémétrie FinOps en temps réel.
/// </summary>
public sealed class InMemoryGrilleDocumentCache : IGrilleDocumentCache
{
    private readonly ConcurrentDictionary<string, GrilleCacheEntry> _store = new(StringComparer.OrdinalIgnoreCase);
    private long _cacheHitsCount;

    /// <summary>
    /// Nombre total de hits de cache ayant évité une extraction OCR/LLM payante (0,00 € par hit).
    /// </summary>
    public long CacheHitsCount => Interlocked.Read(ref _cacheHitsCount);

    /// <inheritdoc />
    public GrilleCacheEntry? TryGetBySha256AsNoTracking(string sha256Hash)
    {
        if (string.IsNullOrWhiteSpace(sha256Hash))
        {
            return null;
        }

        string normalizedHash = sha256Hash.Trim();
        if (!_store.TryGetValue(normalizedHash, out GrilleCacheEntry? entry))
        {
            return null;
        }

        Interlocked.Increment(ref _cacheHitsCount);

        // Retourne une copie immuable (sémantique AsNoTracking / record value equality)
        return entry with { };
    }

    /// <inheritdoc />
    public void Store(GrilleCacheEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        if (string.IsNullOrWhiteSpace(entry.DocumentSha256))
        {
            throw new ArgumentException("L'empreinte SHA-256 du document est obligatoire.", nameof(entry));
        }

        string normalizedHash = entry.DocumentSha256.Trim().ToLowerInvariant();
        _store[normalizedHash] = entry with { DocumentSha256 = normalizedHash };
    }
}
