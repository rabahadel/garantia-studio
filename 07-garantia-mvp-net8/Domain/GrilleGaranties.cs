namespace GarantIA.Domain;

/// <summary>
/// Agrégat représentant une grille de garanties santé (Contrat Actuel ou Offre Proposée).
/// Encapsule les règles de conformité Contrat Responsable et Panier Minimum ANI.
/// </summary>
public sealed class GrilleGaranties
{
    private readonly List<LigneGarantie> _lignes;

    public string NomContrat { get; private set; }
    public string NomAssureur { get; private set; }
    public decimal CotisationMensuelleHt { get; private set; }
    public IReadOnlyCollection<LigneGarantie> Lignes => _lignes.AsReadOnly();

    public GrilleGaranties(
        string nomContrat,
        string nomAssureur,
        decimal cotisationMensuelleHt,
        IEnumerable<LigneGarantie> lignes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nomContrat);
        ArgumentException.ThrowIfNullOrWhiteSpace(nomAssureur);
        ArgumentOutOfRangeException.ThrowIfNegative(cotisationMensuelleHt);
        ArgumentNullException.ThrowIfNull(lignes);

        NomContrat = nomContrat.Trim();
        NomAssureur = nomAssureur.Trim();
        CotisationMensuelleHt = cotisationMensuelleHt;
        _lignes = new List<LigneGarantie>(lignes);
    }

    /// <summary>
    /// Vérifie déterministement la conformité de la grille avec le panier minimum ANI (Décret n° 2014-1025)
    /// et le cahier des charges du Contrat Responsable.
    /// </summary>
    public IReadOnlyCollection<AlerteConformiteAni> VerifierConformiteResponsableEtAni()
    {
        var alertes = new List<AlerteConformiteAni>();

        foreach (LigneGarantie ligne in _lignes)
        {
            if (ligne.Poste == PosteSante.Dentaire
                && ligne.Niveau.Unite == UniteRemboursement.PourcentageBrss
                && ligne.Niveau.Valeur < 125m)
            {
                alertes.Add(new AlerteConformiteAni(
                    "ANI-DENT-125",
                    PosteSante.Dentaire,
                    $"Le poste '{ligne.LibelleActe}' ({ligne.Niveau.Formater()}) est inférieur au plancher légal ANI de 125% BRSS pour les prothèses et soins dentaires."));
            }

            if (ligne.Poste == PosteSante.Optique
                && ligne.Niveau.Unite == UniteRemboursement.ForfaitEuros
                && ligne.Niveau.Valeur < 100m)
            {
                alertes.Add(new AlerteConformiteAni(
                    "ANI-OPT-100",
                    PosteSante.Optique,
                    $"Le poste '{ligne.LibelleActe}' ({ligne.Niveau.Formater()}) est inférieur au forfait minimum ANI de 100 € par équipement optique simple."));
            }

            if (ligne.Poste == PosteSante.Hospitalisation
                && ligne.Niveau.Unite == UniteRemboursement.PourcentageBrss
                && ligne.Niveau.Valeur < 100m)
            {
                alertes.Add(new AlerteConformiteAni(
                    "ANI-HOSP-100",
                    PosteSante.Hospitalisation,
                    $"Le poste '{ligne.LibelleActe}' ({ligne.Niveau.Formater()}) ne couvre pas le minimum réglementaire de 100% BRSS en hospitalisation."));
            }
        }

        return alertes.AsReadOnly();
    }
}
