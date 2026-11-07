# 🌐 HARNESS GLOBAL MACHINE — Règles d'Ingénierie Universelles (.NET / C#)

> **Emplacement** : `C:\Users\<VotreNom>\.gemini\config\GEMINI.md` (et intégré à vos règles globales Antigravity).
> **Portée** : S'applique automatiquement à **tous** vos projets et **toutes** vos conversations, sans jamais polluer les dépôts Git des clients.

---

## 1. Exploration Token-Efficient (.NET & Web)
- **Ne jamais lire, indexer ou parcourir** : `bin/`, `obj/`, `.vs/`, `TestResults/`, `publish/`, `packages/`, `node_modules/`, `dist/`, `build/`, `.git/`.
- **Ordre de lecture obligatoire d'une solution .NET** :
  1. `*.sln` et `*/*.csproj` (structure, versions .NET, packages NuGet)
  2. `Program.cs` / `Startup.cs` / `Global.asax.cs` et `appsettings.json` / `Web.config`
  3. `DbContext` et configurations Entity Framework Code First
  4. Domaine (`Entities`) → Application (`Services` / `Handlers`) → Présentation (`Controllers` / `Pages`)

---

## 2. Clean Code, SOLID, GRASP & KISS/YAGNI (Zéro compromis)
- **Règle « 1 Type = 1 Fichier `.cs` »** : Chaque `class`, `record`, `interface` ou `enum` public DOIT impérativement avoir son propre fichier `.cs` portant exactement le nom du type. **Interdiction absolue** de regrouper plusieurs classes ou records dans un même fichier source.
- **KISS, YAGNI & Rule of Three (Anti-Over-Engineering)** :
  - Ne jamais créer d'abstraction spéculative (`IGenericRepository<T>`, `AbstractFactory`, `IStrategy`) tant qu'il n'y a pas au moins 2 implémentations concrètes ou un besoin explicite de test unitaire.
  - Privilégier le code simple, direct et lisible avec des *Guard Clauses* (retours anticipés, zéro `if/else` imbriqué profond).
- **GRASP & « Tell, Don't Ask » (Anti-Modèle Anémique)** :
  - **Information Expert (GRASP)** : Placer la règle métier et la validation dans l'entité ou le Value Object qui détient les données (ex. `commande.Valider(codePromo)`) plutôt que dans un Service procédural externe.
  - **Make Illegal States Unrepresentable** : Protéger les invariants métier via des setters privés (`private set;`), des collections en lecture seule (`IReadOnlyCollection<T>`) et des Value Objects typés.
- **CQS (Command-Query Separation) & Loi de Déméter** :
  - Séparer strictement les méthodes qui lisent des données (Queries : zéro effet de bord, `AsNoTracking()`) de celles qui modifient l'état (Commands).
  - Interdire les chaînages profonds (`a.B.C.D`).

---

## 3. Modélisation Graphique Systématique (Mermaid)
Avant d'écrire ou de modifier du code non trivial, toujours produire un diagramme **Mermaid** concis pour valider la conception :
- **Nouveau flux ou refactoring d'API** → `sequenceDiagram`
- **Création / modification d'entités EF Code First** → `erDiagram` (tables, types SQL, clés, index)
- **Architecture ou Design Pattern métier** → `flowchart LR` ou `classDiagram`

---

## 4. Standards de Tests Unitaires (xUnit, Given/When/Then & Combinatoire `TheoryData`)
- **Framework & Pattern** : Privilégier **xUnit** avec une structure explicite **`// Given` ( Contexte ) → `// When` ( Action ) → `// Then` ( Assertions )**.
- **Exploration Combinatoire (`[Theory]` & `TheoryData<...>`)** :
  - Utiliser `[Theory]` avec `[ClassData]` ou `[MemberData]` fortement typés (`TheoryData<T1, T2, ...>`) pour explorer systématiquement les combinatoires pertinentes, cas limites (boundary values) et scénarios métier innovants sans dupliquer le code de test.
- **Séparation Stricte « Code de Test vs Données de la Theory » (1 Type = 1 Fichier `.cs`)** :
  - **Interdiction** de polluer la classe de test avec d'énormes jeux de données en vrac : isoler les définitions de données de `[Theory]` dans leur propre classe dédiée (ex. `NomDuScenarioTheoryData.cs` héritant de `TheoryData<...>`) dans un fichier `.cs` séparé de la classe de test (`NomDeLaClasseTests.cs`).

---

## 5. Routage Automatique des Skills & Sous-Agents
Le développeur n'a pas à choisir quel Skill ou Sous-agent activer. Dès qu'il soumet un ticket Azure DevOps ou un problème :
1. **Ticket de BUG / Anomalie** → Activer automatiquement **`systematic-debugging`** + **`run-tests`** :
   - Tracer le chemin complet → Formuler 3 hypothèses → Écrire un test qui **reproduit** le bug → Corriger la cause racine → Vérifier le test au vert.
2. **Nouvelle API / Évolution (.NET 8)** → Activer automatiquement **`dotnet-architecture-review`** + **`dotnet-webapi`** + **`optimizing-ef-core-queries`**.
3. **Maintenance / Refactoring Legacy (.NET 4)** → Activer automatiquement **`csharp-refactoring`** (changement minimal sans régression).
4. **Revue avant Pull Request Azure DevOps** → Lancer en parallèle 3 sous-agents (Architecte API, Expert EF Code First, Auditeur RSSI & Prod) et synthétiser leurs retours.

---

## 6. Confidentialité, OPSEC & Anonymat Externe (Tolérance Zéro)
- **Zéro Divulgation d'Informations Personnelles (PII Développeur/Fondateur)** :
  - **Interdiction absolue** d'exposer le nom, prénom, adresse email personnelle (`@gmail.com`, etc.), numéro de téléphone ou chemin local (`C:\Users\<nom>`) de l'utilisateur dans le code source, les fichiers HTML/Markdown publics, les scripts ou les métadonnées des commits Git poussés sur GitHub ou tout service externe.
- **Identité Git & GitHub Anonymisée** :
  - Toujours utiliser une identité d'entreprise/studio neutre dans les fichiers publics (ex. `GarantIA Studio`, `contact@garantia-studio.fr`) et l'adresse GitHub `noreply` exacte du compte GitHub cible (ex. `rabahadel@users.noreply.github.com`) pour signer les commits Git, afin de ne jamais révéler l'email personnel ni lier le commit à un homonyme tiers.
