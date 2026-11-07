using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ModernApi.Controllers;

// ============================================================================
// RÉSULTAT DU WORKFLOW MULTI-AGENTS (.NET 8 + EF Core 8 Code First + RSSI)
// ============================================================================

[ApiController]
[Route("api/v1/commandes")]
[Authorize]
public sealed class CommandesController(
    AppDbContext db,
    ILogger<CommandesController> logger) : ControllerBase
{
    [HttpGet("/api/v1/clients/{clientId:int}/commandes")]
    [ProducesResponseType<IReadOnlyList<CommandeResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<CommandeResponse>>> GetCommandesClient(
        int clientId,
        CancellationToken ct)
    {
        // 1. [Agent RSSI] Protection anti-IDOR : vérifier que le client connecté accède à ses propres données
        var connectedClientId = User.FindFirstValue("client_id");
        if (connectedClientId != clientId.ToString() && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        // 2. [Agent EF Code First] Zéro N+1 : AsNoTracking + Projection SQL directe + CancellationToken
        var commandes = await db.Commandes
            .AsNoTracking()
            .Where(c => c.ClientId == clientId)
            .Select(c => new CommandeResponse(
                c.Id,
                c.ClientId,
                c.Lignes.Sum(l => l.PrixUnitaire * l.Quantite),
                c.EstValidee,
                c.CodePromo)) // Masque CommentaireInterne
            .ToListAsync(ct);

        return Ok(commandes);
    }

    [HttpPost("{id:int}/valider")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ValiderCommande(
        int id,
        [FromBody] ValiderCommandeRequest request,
        CancellationToken ct)
    {
        // 3. [Agent RSSI & Architecte] Chargement de l'entité et modification ciblée (anti-Mass Assignment)
        var commande = await db.Commandes.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (commande is null)
            return NotFound();

        if (commande.EstValidee)
            return Conflict(new ProblemDetails { Title = "Cette commande est déjà validée." });

        commande.CodePromo = request.CodePromo;
        commande.EstValidee = true;

        await db.SaveChangesAsync(ct);
        logger.LogInformation("Commande {CommandeId} validée pour le client {ClientId}", commande.Id, commande.ClientId);

        return NoContent();
    }
}

// --- [Agent Architecte] Contrats DTO (Records C# 12 immuables) ---
public sealed record CommandeResponse(
    int Id,
    int ClientId,
    decimal MontantTotal,
    bool EstValidee,
    string? CodePromo);

public sealed record ValiderCommandeRequest(
    [property: System.ComponentModel.DataAnnotations.StringLength(30)] string? CodePromo);

// --- [Agent EF Code First] Configuration Fluent API explicite ---
public sealed class CommandeConfiguration : IEntityTypeConfiguration<Commande>
{
    public void Configure(EntityTypeBuilder<Commande> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.ClientId);
        builder.Property(c => c.CodePromo).HasMaxLength(30).IsUnicode(false);
        builder.Property(c => c.CommentaireInterne).HasMaxLength(500);
        builder.Property(c => c.MontantTotal).HasPrecision(18, 2);
    }
}
