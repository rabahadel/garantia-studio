using GarantIA.Domain;
using GarantIA.Security;

namespace GarantIA.Application;

/// <summary>
/// Contrat de sortie du Web Service SaaS GarantIA, incluant le rapport comparatif DDA,
/// l'état FinOps du cache SHA-256 et le statut du quota Multi-Tenant.
/// </summary>
public sealed record AnalyseComparatifResult(
    RapportComparatif Rapport,
    string TexteAnonymiseRgpdApercu,
    string HashOffreProposeeSha256,
    bool CacheHitFinOpsZeroEuro,
    TenantQuotaStatus StatutQuotaTenant);
