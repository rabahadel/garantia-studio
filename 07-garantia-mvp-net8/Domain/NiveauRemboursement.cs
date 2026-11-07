using System.Globalization;

namespace GarantIA.Domain;

/// <summary>
/// Value Object représentant le niveau de prise en charge d'un acte médical (Tell, Don't Ask).
/// </summary>
public readonly record struct NiveauRemboursement(UniteRemboursement Unite, decimal Valeur)
{
    /// <summary>
    /// Compare ce niveau (offre proposée) avec le niveau du contrat actuel.
    /// </summary>
    public EcartGarantie ComparerAvec(NiveauRemboursement actuel)
    {
        if (Unite == actuel.Unite)
        {
            if (Unite is UniteRemboursement.FraisReels or UniteRemboursement.Panier100PourcentSante)
            {
                return new EcartGarantie(TendanceComparaison.Equivalence, 0m, "Maintien intégral");
            }

            decimal delta = Valeur - actuel.Valeur;
            if (delta == 0m)
            {
                return new EcartGarantie(TendanceComparaison.Equivalence, 0m, "Identique");
            }

            TendanceComparaison tendance = delta > 0m
                ? TendanceComparaison.Amelioration
                : TendanceComparaison.Regression;

            string signe = delta > 0m ? "+" : string.Empty;
            string uniteLabel = Unite == UniteRemboursement.PourcentageBrss ? "% BRSS" : " €";
            string valeurFormatee = delta.ToString("0.##", CultureInfo.InvariantCulture);

            return new EcartGarantie(tendance, delta, $"{signe}{valeurFormatee}{uniteLabel}");
        }

        if (Unite == UniteRemboursement.FraisReels)
        {
            return new EcartGarantie(TendanceComparaison.Amelioration, 0m, "Passage Frais Réels");
        }

        if (actuel.Unite == UniteRemboursement.FraisReels)
        {
            return new EcartGarantie(TendanceComparaison.Regression, 0m, "Perte Frais Réels");
        }

        if (Unite == UniteRemboursement.Panier100PourcentSante)
        {
            return new EcartGarantie(TendanceComparaison.Amelioration, 0m, "Passage 100% Santé");
        }

        return new EcartGarantie(TendanceComparaison.Equivalence, 0m, "Changement de structure");
    }

    public string Formater() => Unite switch
    {
        UniteRemboursement.PourcentageBrss => $"{Valeur.ToString("0.##", CultureInfo.InvariantCulture)}% BRSS",
        UniteRemboursement.ForfaitEuros => $"{Valeur.ToString("0.##", CultureInfo.InvariantCulture)} €",
        UniteRemboursement.FraisReels => "Frais Réels",
        UniteRemboursement.Panier100PourcentSante => "100% Santé",
        _ => Valeur.ToString("0.##", CultureInfo.InvariantCulture)
    };
}
