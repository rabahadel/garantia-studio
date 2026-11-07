using GarantIA.Domain;

namespace GarantIA.Tests.Theories;

/// <summary>
/// Jeu de données combinatoire pour le calcul budgétaire (économies, surcoût, iso-cotisation)
/// et l'évaluation déterministe des écarts de garanties DDA.
/// </summary>
public sealed class ComparaisonNiveauxTheoryData
    : TheoryData<string, decimal, decimal, decimal, NiveauRemboursement, NiveauRemboursement, TendanceComparaison, string, string>
{
    public ComparaisonNiveauxTheoryData()
    {
        // 1. Économie budgétaire + Amélioration en % BRSS
        Add(
            "Économie 240€/an et hausse +100% BRSS en Hospitalisation",
            115.00m,
            95.00m,
            240.00m,
            new NiveauRemboursement(UniteRemboursement.PourcentageBrss, 150m),
            new NiveauRemboursement(UniteRemboursement.PourcentageBrss, 250m),
            TendanceComparaison.Amelioration,
            "+100% BRSS",
            "dégage une économie budgétaire annuelle de 240 € HT");

        // 2. Investissement supplémentaire (surcoût) + Passage aux Frais Réels
        Add(
            "Surcoût 180€/an avec passage aux Frais Réels",
            85.00m,
            100.00m,
            -180.00m,
            new NiveauRemboursement(UniteRemboursement.PourcentageBrss, 150m),
            new NiveauRemboursement(UniteRemboursement.FraisReels, 0m),
            TendanceComparaison.Amelioration,
            "Passage Frais Réels",
            "représente un investissement supplémentaire annuel de 180 € HT");

        // 3. Budget strictement iso-cotisation + Équivalence de forfait
        Add(
            "Iso-budget 0€/an et maintien du forfait Optique à 200 €",
            90.00m,
            90.00m,
            0.00m,
            new NiveauRemboursement(UniteRemboursement.ForfaitEuros, 200m),
            new NiveauRemboursement(UniteRemboursement.ForfaitEuros, 200m),
            TendanceComparaison.Equivalence,
            "Identique",
            "s'effectue à budget annuel strictement iso-cotisation");

        // 4. Économie budgétaire mais régression de garantie en €
        Add(
            "Économie 120€/an mais réduction de -50 € sur le forfait",
            100.00m,
            90.00m,
            120.00m,
            new NiveauRemboursement(UniteRemboursement.ForfaitEuros, 200m),
            new NiveauRemboursement(UniteRemboursement.ForfaitEuros, 150m),
            TendanceComparaison.Regression,
            "-50 €",
            "1 poste(s) en retrait");

        // 5. Perte des Frais Réels vers un plafond BRSS
        Add(
            "Perte des Frais Réels vers 200% BRSS",
            130.00m,
            110.00m,
            240.00m,
            new NiveauRemboursement(UniteRemboursement.FraisReels, 0m),
            new NiveauRemboursement(UniteRemboursement.PourcentageBrss, 200m),
            TendanceComparaison.Regression,
            "Perte Frais Réels",
            "1 poste(s) en retrait");
    }
}
