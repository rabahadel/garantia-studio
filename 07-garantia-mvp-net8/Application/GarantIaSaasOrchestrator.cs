using GarantIA.Domain;
using GarantIA.Infrastructure;
using GarantIA.Security;

namespace GarantIA.Application;

/// <summary>
/// Service d'orchestration SaaS Multi-Tenant de GarantIA :
/// 1. Vérifie le quota FinOps du Tenant (Protection Marge Brute 91% / Anti-Denial of Wallet)
/// 2. Anonymise à la volée les PII/NIR en mémoire (Conformité RGPD / Évitement HDS)
/// 3. Vérifie le Cache d'empreinte SHA-256 (0,00 € si la grille assureur est déjà connue)
/// 4. Exécute le moteur déterministe C# (BRSS, ANI, Contrat Responsable, Synthèse DDA)
/// </summary>
public sealed class GarantIaSaasOrchestrator
{
    private readonly FinOpsTenantQuotaGuard _quotaGuard;
    private readonly IGrilleDocumentCache _documentCache;

    public GarantIaSaasOrchestrator(
        FinOpsTenantQuotaGuard quotaGuard,
        IGrilleDocumentCache documentCache)
    {
        ArgumentNullException.ThrowIfNull(quotaGuard);
        ArgumentNullException.ThrowIfNull(documentCache);

        _quotaGuard = quotaGuard;
        _documentCache = documentCache;
    }

    public AnalyseComparatifResult AnalyserEtComparerGrilles(
        Guid tenantCabinetId,
        string rawPdfTextToSanitize,
        GrilleGaranties contratActuel,
        GrilleGaranties offreProposee)
    {
        if (tenantCabinetId == Guid.Empty)
            throw new UnauthorizedAccessException("Accès refusé : TenantCabinetId obligatoire (Isolation Multi-Tenant).");

        ArgumentNullException.ThrowIfNull(contratActuel);
        ArgumentNullException.ThrowIfNull(offreProposee);

        // 1. Vérification du Cache FinOps SHA-256 AVANT de consommer du quota IA
        string signatureGrilleProposee = $"{offreProposee.NomAssureur}|{offreProposee.NomContrat}|{offreProposee.CotisationMensuelleHt}";
        string sha256 = DocumentHashCalculator.ComputeSha256Hex(signatureGrilleProposee);

        GrilleCacheEntry? cachedEntry = _documentCache.TryGetBySha256AsNoTracking(sha256);
        bool isCacheHit = cachedEntry is not null;

        TenantQuotaStatus quotaStatus;
        if (!isCacheHit)
        {
            // Nouveau document : on vérifie et consomme 1 crédit sur le quota mensuel (300/mois)
            quotaStatus = _quotaGuard.VerifierEtIncrementerQuota(tenantCabinetId);
            if (!quotaStatus.EstAutorise)
            {
                throw new InvalidOperationException(
                    $"[FINOPS QUOTA EXCEEDED] Le cabinet {tenantCabinetId} a atteint son plafond mensuel de {quotaStatus.PlafondMensuel} analyses IA.");
            }

            _documentCache.Store(new GrilleCacheEntry(
                sha256,
                offreProposee.NomAssureur,
                offreProposee.NomContrat,
                offreProposee.CotisationMensuelleHt,
                DateTimeOffset.UtcNow));
        }
        else
        {
            // Cache Hit (0,00 € de coût IA) : on ne décrémente même pas le quota LLM du courtier !
            int conso = _quotaGuard.ObtenirConsommation(tenantCabinetId);
            quotaStatus = new TenantQuotaStatus(tenantCabinetId, conso, FinOpsTenantQuotaGuard.PlafondMensuelParDefaut, EstAutorise: true);
        }

        // 2. Privacy-by-Design : Anonymisation RGPD en mémoire vive (évitement HDS)
        string sanitizedText = RgpdPiiSanitizer.AnonymiserTexteBrut(rawPdfTextToSanitize);

        // 3. Moteur Domaine Déterministe C# (Zéro hallucination sur BRSS, ANI et Devoir de Conseil DDA)
        RapportComparatif rapport = RapportComparatif.Generer(tenantCabinetId, contratActuel, offreProposee);

        return new AnalyseComparatifResult(
            rapport,
            sanitizedText,
            sha256,
            isCacheHit,
            quotaStatus);
    }
}
