using System.Text;
using GarantIA.Application;
using GarantIA.Domain;
using GarantIA.Infrastructure;
using GarantIA.Security;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("================================================================================");
Console.WriteLine("🚀 GARANTIA MVP (.NET) — COMPARATEUR MUTUELLE/PRÉVOYANCE & CONFORMITÉ DDA EN 30s");
Console.WriteLine("================================================================================\n");

var quotaGuard = new FinOpsTenantQuotaGuard(plafondMensuel: 300);
var documentCache = new InMemoryGrilleDocumentCache();
var saasOrchestrator = new GarantIaSaasOrchestrator(quotaGuard, documentCache);

var cabinetCourtierId = Guid.NewGuid();
string documentBrutClient = "Adhérent : Jean Dupont - N° Sécu : 1 85 05 75 112 045 88 - Email : jean.dupont@entreprise.fr";

// Contrat Actuel du prospect (Plus cher, moins bien remboursé)
var contratActuel = new GrilleGaranties(
    nomContrat: "Mutuelle Historique Syntec Niveau 1",
    nomAssureur: "AssurLegacy",
    cotisationMensuelleHt: 145.00m,
    lignes:
    [
        new LigneGarantie(PosteSante.Hospitalisation, "Honoraires Chirurgie OPTAM", new NiveauRemboursement(UniteRemboursement.PourcentageBrss, 150m)),
        new LigneGarantie(PosteSante.SoinsCourants, "Consultations Spécialistes", new NiveauRemboursement(UniteRemboursement.PourcentageBrss, 125m)),
        new LigneGarantie(PosteSante.Optique, "Équipement Optique Classe B", new NiveauRemboursement(UniteRemboursement.ForfaitEuros, 150m)),
        new LigneGarantie(PosteSante.Dentaire, "Prothèses Dentaires Hors 100% Santé", new NiveauRemboursement(UniteRemboursement.PourcentageBrss, 150m))
    ]);

// Offre Proposée par le Courtier (Moins chère, meilleures garanties, 100% conforme ANI)
var offreProposee = new GrilleGaranties(
    nomContrat: "SwissLife Santé Entreprise Niveau 3",
    nomAssureur: "SwissLife",
    cotisationMensuelleHt: 122.00m,
    lignes:
    [
        new LigneGarantie(PosteSante.Hospitalisation, "Honoraires Chirurgie OPTAM", new NiveauRemboursement(UniteRemboursement.PourcentageBrss, 250m)),
        new LigneGarantie(PosteSante.SoinsCourants, "Consultations Spécialistes", new NiveauRemboursement(UniteRemboursement.PourcentageBrss, 200m)),
        new LigneGarantie(PosteSante.Optique, "Équipement Optique Classe B", new NiveauRemboursement(UniteRemboursement.ForfaitEuros, 300m)),
        new LigneGarantie(PosteSante.Dentaire, "Prothèses Dentaires Hors 100% Santé", new NiveauRemboursement(UniteRemboursement.PourcentageBrss, 250m))
    ]);

// 1er Appel : Extraction initiale (Mise en cache SHA-256)
var premierAppel = saasOrchestrator.AnalyserEtComparerGrilles(
    cabinetCourtierId,
    documentBrutClient,
    contratActuel,
    offreProposee);

// 2e Appel sur la même grille SwissLife : Cache Hit FinOps à 0,00 € !
var secondAppelCacheHit = saasOrchestrator.AnalyserEtComparerGrilles(
    cabinetCourtierId,
    documentBrutClient,
    contratActuel,
    offreProposee);

Console.WriteLine($"🔒 [SECURITY & RGPD] Texte anonymisé en mémoire (Évitement HDS) :");
Console.WriteLine($"   ↳ Avant : {documentBrutClient}");
Console.WriteLine($"   ↳ Après : {premierAppel.TexteAnonymiseRgpdApercu}\n");

Console.WriteLine($"💰 [FINOPS CACHE SHA-256] Empreinte Grille : {premierAppel.HashOffreProposeeSha256[..16]}...");
Console.WriteLine($"   ↳ 1er appel Cache Hit : {premierAppel.CacheHitFinOpsZeroEuro} (Quota consommé : {premierAppel.StatutQuotaTenant.ConsommationMoisCourant}/{premierAppel.StatutQuotaTenant.PlafondMensuel})");
Console.WriteLine($"   ↳ 2e appel Cache Hit  : {secondAppelCacheHit.CacheHitFinOpsZeroEuro} -> Coût IA = 0,00 € ! (Total Cache Hits : {documentCache.CacheHitsCount})\n");

var rapport = secondAppelCacheHit.Rapport;
Console.WriteLine($"📊 [AHA MOMENT 30s] VERDICT COMPARATIF POUR LE COURTIER :");
Console.WriteLine($"   • Contrat Actuel : {rapport.NomContratActuel} ({rapport.CotisationActuelleMensuelleHt:0.00} € HT/mois)");
Console.WriteLine($"   • Offre Proposée : {rapport.NomOffreProposee} ({rapport.CotisationProposeeMensuelleHt:0.00} € HT/mois)");
Console.WriteLine($"   • Gain Budgétaire : +{rapport.EconomieAnnuelleEuros:0.00} € HT / an d'économies");
Console.WriteLine($"   • Conformité ANI / Contrat Responsable : {(rapport.EstConformeAni ? "✅ 100% CONFORME" : "⚠️ ALERTE NON-CONFORMITÉ")}\n");

Console.WriteLine("   Détail Poste par Poste :");
foreach (var comp in rapport.Comparaisons)
{
    Console.WriteLine($"   - [{comp.Poste}] {comp.LibelleActe} : {comp.NiveauActuel.Formater()} ➔ {comp.NiveauPropose.Formater()} ({comp.Ecart.Tendance} : {comp.Ecart.ResumeCourt})");
}

Console.WriteLine($"\n📋 [DEVOIR DE CONSEIL DDA PRÊT À COPIER-COLLER] :");
Console.WriteLine($"   \"{rapport.ParagrapheDevoirConseilDda}\"");
