using GarantIA.Domain;

namespace GarantIA.Tests.Theories;

/// <summary>
/// Jeu de données combinatoire pour l'évaluation des planchers réglementaires ANI (Décret n° 2014-1025)
/// et Contrat Responsable (valeurs limites exactes et cas nominaux).
/// </summary>
public sealed class ConformiteAniCombinatoireTheoryData
    : TheoryData<string, PosteSante, UniteRemboursement, decimal, bool, string?>
{
    public ConformiteAniCombinatoireTheoryData()
    {
        // 1. Poste Dentaire (Plancher ANI : 125% BRSS)
        Add("Dentaire juste sous le plancher légal (124.99% BRSS)", PosteSante.Dentaire, UniteRemboursement.PourcentageBrss, 124.99m, false, "ANI-DENT-125");
        Add("Dentaire au plancher exact ANI (125.00% BRSS)", PosteSante.Dentaire, UniteRemboursement.PourcentageBrss, 125.00m, true, null);
        Add("Dentaire confort supérieur (250% BRSS)", PosteSante.Dentaire, UniteRemboursement.PourcentageBrss, 250.00m, true, null);
        Add("Dentaire en Panier 100% Santé", PosteSante.Dentaire, UniteRemboursement.Panier100PourcentSante, 0m, true, null);

        // 2. Poste Optique (Plancher ANI : 100 €)
        Add("Optique sous le forfait minimum ANI (99.00 €)", PosteSante.Optique, UniteRemboursement.ForfaitEuros, 99.00m, false, "ANI-OPT-100");
        Add("Optique frontière basse centime (99.99 €)", PosteSante.Optique, UniteRemboursement.ForfaitEuros, 99.99m, false, "ANI-OPT-100");
        Add("Optique au plancher exact ANI (100.00 €)", PosteSante.Optique, UniteRemboursement.ForfaitEuros, 100.00m, true, null);
        Add("Optique forfait haut de gamme (300.00 €)", PosteSante.Optique, UniteRemboursement.ForfaitEuros, 300.00m, true, null);

        // 3. Poste Hospitalisation (Plancher ANI : 100% BRSS)
        Add("Hospitalisation sous le minimum légal (99.99% BRSS)", PosteSante.Hospitalisation, UniteRemboursement.PourcentageBrss, 99.99m, false, "ANI-HOSP-100");
        Add("Hospitalisation au plancher exact (100.00% BRSS)", PosteSante.Hospitalisation, UniteRemboursement.PourcentageBrss, 100.00m, true, null);
        Add("Hospitalisation aux Frais Réels", PosteSante.Hospitalisation, UniteRemboursement.FraisReels, 0m, true, null);
    }
}
