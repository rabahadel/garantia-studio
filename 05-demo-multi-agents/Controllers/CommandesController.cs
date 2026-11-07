using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModernApi.Application;
using ModernApi.Contracts;

namespace ModernApi.Controllers;

/// <summary>
/// Endpoints REST de gestion des commandes (.NET 8) — Couche Présentation pure (sans DbContext direct).
/// </summary>
[ApiController]
[Route("api/v1/commandes")]
[Authorize]
public sealed class CommandesController(
    ICommandeService commandeService,
    ILogger<CommandesController> logger) : ControllerBase
{
    [HttpGet("/api/v1/clients/{clientId:int}/commandes")]
    [ProducesResponseType<IReadOnlyList<CommandeResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<CommandeResponse>>> GetCommandesClient(
        int clientId,
        CancellationToken ct)
    {
        var connectedClientId = User.FindFirstValue("client_id");
        if (connectedClientId != clientId.ToString() && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var commandes = await commandeService.GetByClientIdAsync(clientId, ct);
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
        var result = await commandeService.ValiderAsync(id, request.CodePromo, ct);

        if (result == ValidationCommandeResult.NotFound)
            return NotFound();

        if (result == ValidationCommandeResult.AlreadyValidated)
            return Conflict(new ProblemDetails { Title = "Cette commande est déjà validée." });

        logger.LogInformation("Commande {CommandeId} validée", id);
        return NoContent();
    }
}
