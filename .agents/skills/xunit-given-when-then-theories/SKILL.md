---
name: xunit-given-when-then-theories
description: >-
  Conception de tests unitaires .NET avec xUnit suivant le pattern Given/When/Then,
  l'exploration combinatoire avancée via [Theory] et TheoryData<T>, et la séparation stricte
  (1 Type = 1 Fichier .cs) entre le code du test unitaire et les classes de données de la Theory.
---

# Tests Unitaires xUnit : Pattern Given/When/Then & Combinatoire `TheoryData` Séparée

Ce Skill définit le standard d'excellence pour tous les tests unitaires C# / .NET créés ou refactorés par les agents.

## 1. Les 3 Règles d'Or
1. **Structure `Given / When / Then` obligatoire** :
   - `// Given` : Préparation du contexte, instanciation du SUT (*System Under Test*) et des entrées de la combinatoire.
   - `// When` : Exécution de l'action unique testée.
   - `// Then` : Assertions claires et déterministes vérifiant l'état ou le résultat.
2. **Puissance Combinatoire via xUnit `[Theory]`** :
   - Remplacer les tests répétitifs par des `[Theory]` qui explorent intelligemment l'espace des cas : cas nominaux, valeurs limites exactes (*off-by-one* : `99.99m`, `100.00m`, `124.99m`, `125.00m`), combinaisons multi-postes et cas métier atypiques.
3. **Séparation Physique « Code TU vs Définition des Données » (1 Type = 1 Fichier `.cs`)** :
   - **Interdiction absolue** de déclarer les tableaux `IEnumerable<object[]>` ou `TheoryData` à l'intérieur de la classe `*Tests.cs`.
   - Chaque jeu de données combinatoire DOIT être une classe fortement typée héritant de `TheoryData<T1, T2, ...>`, placée dans son propre fichier `*TheoryData.cs` (dans un sous-dossier `TestData/` ou `Theories/`) et injectée via `[ClassData(typeof(MonScenarioTheoryData))]`.

## 2. Exemple de Référence (2 Fichiers Séparés)

### Fichier 1 : `Theories/ConformiteAniCombinatoireTheoryData.cs` (Définition des données seule)
```csharp
using Xunit;
using GarantIA.Domain;

namespace GarantIA.Tests.Theories;

public sealed class ConformiteAniCombinatoireTheoryData
    : TheoryData<string, PosteSante, UniteRemboursement, decimal, bool, string?>
{
    public ConformiteAniCombinatoireTheoryData()
    {
        // Cas 1 : Dentaire juste sous le seuil légal ANI (124.99% < 125% BRSS) -> Alerte ANI-DENT-125
        Add("Frontière basse Dentaire 124.99% BRSS", PosteSante.Dentaire, UniteRemboursement.PourcentageBrss, 124.99m, false, "ANI-DENT-125");

        // Cas 2 : Dentaire pile sur le plancher légal ANI (125.00% BRSS) -> Conforme
        Add("Plancher exact Dentaire 125% BRSS", PosteSante.Dentaire, UniteRemboursement.PourcentageBrss, 125.00m, true, null);

        // Cas 3 : Optique sous le forfait minimum ANI (99€ < 100€) -> Alerte ANI-OPT-100
        Add("Frontière basse Optique 99€", PosteSante.Optique, UniteRemboursement.ForfaitEuros, 99m, false, "ANI-OPT-100");
    }
}
```

### Fichier 2 : `GrilleGarantiesTests.cs` (Code du test unitaire pur Given/When/Then)
```csharp
using Xunit;
using GarantIA.Domain;
using GarantIA.Tests.Theories;

namespace GarantIA.Tests;

public sealed class GrilleGarantiesTests
{
    [Theory(DisplayName = "Vérification combinatoire des seuils légaux ANI et Contrat Responsable")]
    [ClassData(typeof(ConformiteAniCombinatoireTheoryData))]
    public void VerifierConformite_GivenCombinatoirePosteEtNiveau_WhenVerification_ThenRespectePlanchersAni(
        string scenario,
        PosteSante poste,
        UniteRemboursement unite,
        decimal valeur,
        bool attenduConforme,
        string? codeAlerteAttendu)
    {
        // Given
        var ligne = new LigneGarantie(poste, $"Acte test ({scenario})", new NiveauRemboursement(unite, valeur));
        var grille = new GrilleGaranties("Contrat Test", "Assureur Test", 100m, [ligne]);

        // When
        var alertes = grille.VerifierConformiteResponsableEtAni();

        // Then
        Assert.Equal(attenduConforme, alertes.Count == 0);
        if (codeAlerteAttendu is not null)
        {
            Assert.Contains(alertes, a => a.CodeRegle == codeAlerteAttendu);
        }
    }
}
```
