using System.Collections.Concurrent;

namespace GarantIA.Security;

/// <summary>
/// Garde-fou FinOps anti-"Denial of Wallet" protégeant la marge brute cible (91 %)
/// via un quota mensuel d'analyses IA par cabinet (Tenant).
/// </summary>
public sealed class FinOpsTenantQuotaGuard
{
    public const int PlafondMensuelParDefaut = 300;

    private readonly int _plafondMensuel;
    private readonly ConcurrentDictionary<Guid, int> _consommationsParTenant = new();
    private readonly object _verrouQuota = new();

    public FinOpsTenantQuotaGuard(int plafondMensuel = PlafondMensuelParDefaut)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(plafondMensuel);
        _plafondMensuel = plafondMensuel;
    }

    /// <summary>
    /// Vérifie si le quota mensuel du Tenant est atteint et incrémente atomiquement la consommation si autorisé.
    /// </summary>
    public TenantQuotaStatus VerifierEtIncrementerQuota(Guid tenantCabinetId)
    {
        if (tenantCabinetId == Guid.Empty)
        {
            throw new ArgumentException("L'identifiant du Tenant (TenantCabinetId) ne peut pas être vide.", nameof(tenantCabinetId));
        }

        lock (_verrouQuota)
        {
            int consommationActuelle = _consommationsParTenant.GetOrAdd(tenantCabinetId, 0);
            if (consommationActuelle >= _plafondMensuel)
            {
                return new TenantQuotaStatus(
                    tenantCabinetId,
                    consommationActuelle,
                    _plafondMensuel,
                    EstAutorise: false);
            }

            int nouvelleConsommation = consommationActuelle + 1;
            _consommationsParTenant[tenantCabinetId] = nouvelleConsommation;

            return new TenantQuotaStatus(
                tenantCabinetId,
                nouvelleConsommation,
                _plafondMensuel,
                EstAutorise: true);
        }
    }

    /// <summary>
    /// Retourne la consommation du mois courant pour un Tenant donné.
    /// </summary>
    public int ObtenirConsommation(Guid tenantCabinetId)
    {
        if (tenantCabinetId == Guid.Empty)
        {
            throw new ArgumentException("L'identifiant du Tenant (TenantCabinetId) ne peut pas être vide.", nameof(tenantCabinetId));
        }

        return _consommationsParTenant.TryGetValue(tenantCabinetId, out int consommation)
            ? consommation
            : 0;
    }
}
