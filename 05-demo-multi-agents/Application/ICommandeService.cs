using ModernApi.Contracts;

namespace ModernApi.Application;

/// <summary>
/// Contrat applicatif de gestion des commandes (découplé d'ASP.NET Core et d'EF Core).
/// </summary>
public interface ICommandeService
{
    Task<IReadOnlyList<CommandeResponse>> GetByClientIdAsync(int clientId, CancellationToken ct = default);
    Task<ValidationCommandeResult> ValiderAsync(int commandeId, string? codePromo, CancellationToken ct = default);
}
