namespace GarantIA.Security;

/// <summary>
/// État du quota mensuel FinOps d'un cabinet courtier (Tenant).
/// </summary>
public readonly record struct TenantQuotaStatus(
    Guid TenantCabinetId,
    int ConsommationMoisCourant,
    int PlafondMensuel,
    bool EstAutorise);
