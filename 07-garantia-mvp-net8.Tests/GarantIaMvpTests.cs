using GarantIA.Application;
using GarantIA.Domain;
using GarantIA.Infrastructure;
using GarantIA.Security;
using GarantIA.Tests.Theories;

namespace GarantIA.Tests;

/// <summary>
/// Suite de tests unitaires xUnit du MVP GarantIA suivant le standard Given/When/Then
/// et l'exploration combinatoire via TheoryData séparé (1 Type = 1 Fichier .cs).
/// </summary>
public sealed class GarantIaMvpTests
{
    [Theory(DisplayName = "1. Comparaison budgétaire et écarts de garanties DDA (Combinatoire)")]
    [ClassData(typeof(ComparaisonNiveauxTheoryData))]
    public void Generer_GivenCombinatoireBudgetsEtNiveaux_WhenGenererRapport_ThenCalculeEconomieEtSyntheseDda(
        string scenario,
        decimal cotisationActuelleHt,
        decimal cotisationProposeeHt,
        decimal economieAnnuelleAttendue,
        NiveauRemboursement niveauActuel,
        NiveauRemboursement niveauPropose,
        TendanceComparaison tendanceAttendue,
        string resumeEcartAttendu,
        string extraitParagrapheDdaAttendu)
    {
        // Given
        Guid tenantId = Guid.NewGuid();
        var contratActuel = new GrilleGaranties(
            $"Contrat Actuel ({scenario})",
            "Assureur Historique",
            cotisationActuelleHt,
            [
                new LigneGarantie(PosteSante.Hospitalisation, "Chirurgie & Séjour", niveauActuel)
            ]);

        var offreProposee = new GrilleGaranties(
            $"Offre Proposée ({scenario})",
            "Assureur Cible",
            cotisationProposeeHt,
            [
                new LigneGarantie(PosteSante.Hospitalisation, "Chirurgie & Séjour", niveauPropose)
            ]);

        // When
        RapportComparatif rapport = RapportComparatif.Generer(tenantId, contratActuel, offreProposee);

        // Then
        Assert.Equal(economieAnnuelleAttendue, rapport.EconomieAnnuelleEuros);
        ComparaisonPoste comparaison = Assert.Single(rapport.Comparaisons);
        Assert.Equal(tendanceAttendue, comparaison.Ecart.Tendance);
        Assert.Equal(resumeEcartAttendu, comparaison.Ecart.ResumeCourt);
        Assert.Contains(extraitParagrapheDdaAttendu, rapport.ParagrapheDevoirConseilDda, StringComparison.Ordinal);
    }

    [Theory(DisplayName = "2. Conformité ANI et Contrat Responsable aux valeurs limites (Combinatoire)")]
    [ClassData(typeof(ConformiteAniCombinatoireTheoryData))]
    public void Generer_GivenCombinatoireSeuilsAni_WhenGenererRapport_ThenDetecteAlertesExactesAuxFrontieres(
        string scenario,
        PosteSante poste,
        UniteRemboursement unite,
        decimal valeur,
        bool attenduConforme,
        string? codeAlerteAttendu)
    {
        // Given
        Guid tenantId = Guid.NewGuid();
        var contratActuel = new GrilleGaranties(
            "Contrat Référence Conforme",
            "Assureur Référence",
            100m,
            [
                new LigneGarantie(poste, $"Acte {scenario}", new NiveauRemboursement(unite, 300m))
            ]);

        var offreTeste = new GrilleGaranties(
            $"Offre Testée ({scenario})",
            "Assureur Test",
            90m,
            [
                new LigneGarantie(poste, $"Acte {scenario}", new NiveauRemboursement(unite, valeur))
            ]);

        // When
        RapportComparatif rapport = RapportComparatif.Generer(tenantId, contratActuel, offreTeste);

        // Then
        Assert.Equal(attenduConforme, rapport.EstConformeAni);
        if (codeAlerteAttendu is null)
        {
            Assert.Empty(rapport.AlertesConformite);
        }
        else
        {
            AlerteConformiteAni alerte = Assert.Single(rapport.AlertesConformite);
            Assert.Equal(codeAlerteAttendu, alerte.CodeRegle);
            Assert.Equal(poste, alerte.PosteConcerne);
        }
    }

    [Theory(DisplayName = "3. Anonymisation RGPD des NIR (Métropole + Corse 2A/2B) et Emails (Combinatoire)")]
    [ClassData(typeof(RgpdNirEtEmailCombinatoireTheoryData))]
    public void AnonymiserTexteBrut_GivenCombinatoireNirEtEmails_WhenAnonymiser_ThenMasquePiiSansFuite(
        string scenario,
        string rawText,
        string nirSensible,
        string emailSensible)
    {
        // Given
        Assert.False(string.IsNullOrWhiteSpace(scenario));

        // When
        string sanitized = RgpdPiiSanitizer.AnonymiserTexteBrut(rawText);

        // Then
        Assert.Contains(RgpdPiiSanitizer.NirMask, sanitized, StringComparison.Ordinal);
        Assert.Contains(RgpdPiiSanitizer.EmailMask, sanitized, StringComparison.Ordinal);
        Assert.DoesNotContain(nirSensible, sanitized, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(emailSensible, sanitized, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "4a. Cache FinOps SHA-256 à 0,00 € : second appel identique sans incrémenter le quota Tenant")]
    public void AnalyserEtComparerGrilles_GivenMemeOffreProposee_WhenSecondAppel_ThenActiveCacheFinOpsSansConsommerQuota()
    {
        // Given
        Guid tenantId = Guid.NewGuid();
        var quotaGuard = new FinOpsTenantQuotaGuard(FinOpsTenantQuotaGuard.PlafondMensuelParDefaut);
        var documentCache = new InMemoryGrilleDocumentCache();
        var orchestrator = new GarantIaSaasOrchestrator(quotaGuard, documentCache);

        var contratActuel = new GrilleGaranties(
            "Contrat Actuel",
            "Assureur A",
            cotisationMensuelleHt: 100m,
            [
                new LigneGarantie(PosteSante.Dentaire, "Prothèses", new NiveauRemboursement(UniteRemboursement.PourcentageBrss, 150m))
            ]);

        var offreProposee = new GrilleGaranties(
            "Offre Cible",
            "Assureur B",
            cotisationMensuelleHt: 90m,
            [
                new LigneGarantie(PosteSante.Dentaire, "Prothèses", new NiveauRemboursement(UniteRemboursement.PourcentageBrss, 200m))
            ]);

        // When
        AnalyseComparatifResult premierAppel = orchestrator.AnalyserEtComparerGrilles(
            tenantId,
            "NIR: 2 90 12 69 123 456 12",
            contratActuel,
            offreProposee);

        AnalyseComparatifResult secondAppel = orchestrator.AnalyserEtComparerGrilles(
            tenantId,
            "NIR: 2 90 12 69 123 456 12",
            contratActuel,
            offreProposee);

        // Then
        Assert.False(premierAppel.CacheHitFinOpsZeroEuro);
        Assert.Equal(1, premierAppel.StatutQuotaTenant.ConsommationMoisCourant);
        Assert.Equal(300, premierAppel.StatutQuotaTenant.PlafondMensuel);

        Assert.True(secondAppel.CacheHitFinOpsZeroEuro);
        Assert.Equal(1, secondAppel.StatutQuotaTenant.ConsommationMoisCourant);
        Assert.Equal(1L, documentCache.CacheHitsCount);
        Assert.Equal(premierAppel.HashOffreProposeeSha256, secondAppel.HashOffreProposeeSha256);
    }

    [Fact(DisplayName = "4b. Protection FinOps Anti-Denial of Wallet : blocage strict à la 301e analyse mensuelle")]
    public void AnalyserEtComparerGrilles_GivenQuota300Atteint_When301emeAnalyseDistincte_ThenThrowsInvalidOperationException()
    {
        // Given
        Guid tenantId = Guid.NewGuid();
        var quotaGuard = new FinOpsTenantQuotaGuard(FinOpsTenantQuotaGuard.PlafondMensuelParDefaut);
        var documentCache = new InMemoryGrilleDocumentCache();
        var orchestrator = new GarantIaSaasOrchestrator(quotaGuard, documentCache);

        var contratActuel = new GrilleGaranties(
            "Contrat Base",
            "Assureur Base",
            cotisationMensuelleHt: 100m,
            [
                new LigneGarantie(PosteSante.Optique, "Optique", new NiveauRemboursement(UniteRemboursement.ForfaitEuros, 150m))
            ]);

        for (int i = 1; i <= FinOpsTenantQuotaGuard.PlafondMensuelParDefaut; i++)
        {
            var offreUnique = new GrilleGaranties(
                $"Offre Unique #{i}",
                "Assureur Cible",
                cotisationMensuelleHt: 80m + i,
                [
                    new LigneGarantie(PosteSante.Optique, "Optique", new NiveauRemboursement(UniteRemboursement.ForfaitEuros, 200m))
                ]);

            orchestrator.AnalyserEtComparerGrilles(tenantId, "Texte", contratActuel, offreUnique);
        }

        var offre301 = new GrilleGaranties(
            "Offre Unique #301",
            "Assureur Cible",
            cotisationMensuelleHt: 500m,
            [
                new LigneGarantie(PosteSante.Optique, "Optique", new NiveauRemboursement(UniteRemboursement.ForfaitEuros, 200m))
            ]);

        // When
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
            () => orchestrator.AnalyserEtComparerGrilles(tenantId, "Texte", contratActuel, offre301));

        // Then
        Assert.Equal(300, quotaGuard.ObtenirConsommation(tenantId));
        Assert.Contains("FINOPS QUOTA EXCEEDED", exception.Message, StringComparison.Ordinal);
    }
}
