using System.Globalization;
using System.Text;

namespace GarantIA.Domain;

/// <summary>
/// Agrégat Racine représentant le rapport d'audit comparatif entre un contrat en place et une offre proposée,
/// incluant la synthèse réglementaire de Devoir de Conseil (Directive sur la Distribution d'Assurances - DDA).
/// </summary>
public sealed class RapportComparatif
{
    private readonly List<ComparaisonPoste> _comparaisons;
    private readonly List<AlerteConformiteAni> _alertesConformite;

    public Guid Id { get; private set; }
    public Guid TenantCabinetId { get; private set; }
    public DateTime DateGenerationUtc { get; private set; }
    public string NomContratActuel { get; private set; }
    public string NomOffreProposee { get; private set; }
    public decimal CotisationActuelleMensuelleHt { get; private set; }
    public decimal CotisationProposeeMensuelleHt { get; private set; }
    public decimal EconomieAnnuelleEuros { get; private set; }
    public bool EstConformeAni { get; private set; }
    public string ParagrapheDevoirConseilDda { get; private set; }

    public IReadOnlyCollection<ComparaisonPoste> Comparaisons => _comparaisons.AsReadOnly();
    public IReadOnlyCollection<AlerteConformiteAni> AlertesConformite => _alertesConformite.AsReadOnly();

    private RapportComparatif(
        Guid id,
        Guid tenantCabinetId,
        DateTime dateGenerationUtc,
        string nomContratActuel,
        string nomOffreProposee,
        decimal cotisationActuelleMensuelleHt,
        decimal cotisationProposeeMensuelleHt,
        decimal economieAnnuelleEuros,
        List<ComparaisonPoste> comparaisons,
        List<AlerteConformiteAni> alertesConformite,
        string paragrapheDevoirConseilDda)
    {
        Id = id;
        TenantCabinetId = tenantCabinetId;
        DateGenerationUtc = dateGenerationUtc;
        NomContratActuel = nomContratActuel;
        NomOffreProposee = nomOffreProposee;
        CotisationActuelleMensuelleHt = cotisationActuelleMensuelleHt;
        CotisationProposeeMensuelleHt = cotisationProposeeMensuelleHt;
        EconomieAnnuelleEuros = economieAnnuelleEuros;
        _comparaisons = comparaisons;
        _alertesConformite = alertesConformite;
        EstConformeAni = alertesConformite.Count == 0;
        ParagrapheDevoirConseilDda = paragrapheDevoirConseilDda;
    }

    /// <summary>
    /// Génère de manière déterministe le rapport comparatif et la synthèse de Devoir de Conseil DDA.
    /// </summary>
    public static RapportComparatif Generer(
        Guid tenantCabinetId,
        GrilleGaranties contratActuel,
        GrilleGaranties offreProposee)
    {
        if (tenantCabinetId == Guid.Empty)
        {
            throw new ArgumentException("L'identifiant du cabinet courtier (Tenant) est obligatoire.", nameof(tenantCabinetId));
        }

        ArgumentNullException.ThrowIfNull(contratActuel);
        ArgumentNullException.ThrowIfNull(offreProposee);

        decimal economieAnnuelle = (contratActuel.CotisationMensuelleHt - offreProposee.CotisationMensuelleHt) * 12m;

        var comparaisons = new List<ComparaisonPoste>(offreProposee.Lignes.Count);
        foreach (LigneGarantie ligneProposee in offreProposee.Lignes)
        {
            LigneGarantie? ligneActuelle = TrouverLigneCorrespondante(contratActuel.Lignes, ligneProposee);
            NiveauRemboursement niveauActuel = ligneActuelle?.Niveau
                ?? new NiveauRemboursement(ligneProposee.Niveau.Unite, 0m);

            EcartGarantie ecart = ligneProposee.Niveau.ComparerAvec(niveauActuel);

            comparaisons.Add(new ComparaisonPoste(
                ligneProposee.Poste,
                ligneProposee.LibelleActe,
                niveauActuel,
                ligneProposee.Niveau,
                ecart));
        }

        var alertes = new List<AlerteConformiteAni>(offreProposee.VerifierConformiteResponsableEtAni());
        string paragrapheDda = ConstruireParagrapheDda(
            contratActuel,
            offreProposee,
            economieAnnuelle,
            comparaisons,
            alertes);

        return new RapportComparatif(
            Guid.NewGuid(),
            tenantCabinetId,
            DateTime.UtcNow,
            contratActuel.NomContrat,
            offreProposee.NomContrat,
            contratActuel.CotisationMensuelleHt,
            offreProposee.CotisationMensuelleHt,
            economieAnnuelle,
            comparaisons,
            alertes,
            paragrapheDda);
    }

    private static LigneGarantie? TrouverLigneCorrespondante(
        IReadOnlyCollection<LigneGarantie> lignesActuelles,
        LigneGarantie ligneProposee)
    {
        foreach (LigneGarantie candidate in lignesActuelles)
        {
            if (candidate.Poste == ligneProposee.Poste
                && string.Equals(candidate.LibelleActe, ligneProposee.LibelleActe, StringComparison.OrdinalIgnoreCase))
            {
                return candidate;
            }
        }

        foreach (LigneGarantie candidate in lignesActuelles)
        {
            if (candidate.Poste == ligneProposee.Poste)
            {
                return candidate;
            }
        }

        return null;
    }

    private static string ConstruireParagrapheDda(
        GrilleGaranties actuel,
        GrilleGaranties propose,
        decimal economieAnnuelle,
        List<ComparaisonPoste> comparaisons,
        List<AlerteConformiteAni> alertes)
    {
        int nbAmeliorations = 0;
        int nbRegressions = 0;
        foreach (ComparaisonPoste comp in comparaisons)
        {
            if (comp.Ecart.Tendance == TendanceComparaison.Amelioration)
            {
                nbAmeliorations++;
            }
            else if (comp.Ecart.Tendance == TendanceComparaison.Regression)
            {
                nbRegressions++;
            }
        }

        var sb = new StringBuilder(320);
        sb.Append("SYNTHÈSE DEVOIR DE CONSEIL (Article L.521-4 Code des assurances - DDA) : ");
        sb.Append($"La comparaison entre le contrat actuel '{actuel.NomContrat}' ({actuel.NomAssureur}) ");
        sb.Append($"et l'offre proposée '{propose.NomContrat}' ({propose.NomAssureur}) ");

        if (economieAnnuelle > 0m)
        {
            sb.Append($"dégage une économie budgétaire annuelle de {economieAnnuelle.ToString("0.##", CultureInfo.InvariantCulture)} € HT. ");
        }
        else if (economieAnnuelle < 0m)
        {
            decimal surcout = Math.Abs(economieAnnuelle);
            sb.Append($"représente un investissement supplémentaire annuel de {surcout.ToString("0.##", CultureInfo.InvariantCulture)} € HT. ");
        }
        else
        {
            sb.Append("s'effectue à budget annuel strictement iso-cotisation. ");
        }

        sb.Append($"Sur le plan des garanties médicales, l'offre présente {nbAmeliorations} poste(s) en amélioration ");
        sb.Append($"et {nbRegressions} poste(s) en retrait. ");

        if (alertes.Count == 0)
        {
            sb.Append("L'offre proposée respecte intégralement les planchers du Panier Minimum ANI (Décret n° 2014-1025) et du Contrat Responsable.");
        }
        else
        {
            sb.Append($"ATTENTION : {alertes.Count} alerte(s) de non-conformité réglementaire ANI/Contrat Responsable ont été détectées et doivent être régularisées avant souscription collective.");
        }

        return sb.ToString();
    }
}
