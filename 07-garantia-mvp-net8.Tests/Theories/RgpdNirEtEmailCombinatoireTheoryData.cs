namespace GarantIA.Tests.Theories;

/// <summary>
/// Jeu de données combinatoire pour l'anonymisation RGPD en mémoire vive (NIR métropole/Corse 2A-2B et emails).
/// </summary>
public sealed class RgpdNirEtEmailCombinatoireTheoryData
    : TheoryData<string, string, string, string>
{
    public RgpdNirEtEmailCombinatoireTheoryData()
    {
        // 1. NIR formaté avec espaces (15 chiffres avec clé) + email standard
        Add(
            "NIR standard avec espaces et clé + email entreprise",
            "Assuré : Jean Dupont - NIR : 1 84 05 75 112 045 88 - Contact : jean.dupont@entreprise-client.fr",
            "1 84 05 75 112 045 88",
            "jean.dupont@entreprise-client.fr");

        // 2. NIR compact sans espace (15 chiffres) + email avec alias '+'
        Add(
            "NIR compact 15 chiffres + email avec sous-adressage",
            "Dossier #441 : NIR=291076924501244 / Email=rh+mutuelle@holding-groupe.com",
            "291076924501244",
            "rh+mutuelle@holding-groupe.com");

        // 3. NIR compact 13 chiffres (sans clé de contrôle) + email sous-domaine
        Add(
            "NIR compact 13 chiffres sans clé + email sous-domaine",
            "Adhérent : 1781133014009 - Mail : contact.sante@courtage.assurance.fr",
            "1781133014009",
            "contact.sante@courtage.assurance.fr");

        // 4. NIR Corse-du-Sud (2A) avec tirets
        Add(
            "NIR Corse-du-Sud (2A) séparé par tirets",
            "Bénéficiaire Corse 2A : 1-85-08-2A-004-112-35 | Courriel : p.paoli@corse-sante.fr",
            "1-85-08-2A-004-112-35",
            "p.paoli@corse-sante.fr");

        // 5. NIR Haute-Corse (2B) avec points
        Add(
            "NIR Haute-Corse (2B) séparé par points",
            "Bénéficiaire Corse 2B : 2.92.03.2B.018.029.11 | Courriel : m.colonna@bastia-pro.org",
            "2.92.03.2B.018.029.11",
            "m.colonna@bastia-pro.org");
    }
}
