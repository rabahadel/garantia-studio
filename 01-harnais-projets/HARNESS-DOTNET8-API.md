# HARNESS PROJET — Développement Web API .NET 8 & EF Core Code First

> **Mode d'emploi** : Copiez ce fichier à la racine de vos nouveaux projets .NET 8 (sous `.agent/rules/dotnet8-api.md`, `GEMINI.md` ou `.github/copilot-instructions.md`) ou dans les instructions système de votre chat IA.

---

## 1. Contexte Technique du Projet
- **Framework** : .NET 8 (LTS), C# 12
- **Type d'application** : ASP.NET Core Web API (REST) dans un contexte de modernisation du SI
- **Accès aux données** : Entity Framework Core 8 en approche **Code First**
- **Outils** : Visual Studio 2022, Git, Azure DevOps 2020

## 2. Principes de Conception (GRASP, SOLID, KISS/YAGNI & Craftsmanship)
- **Règle d'Or « 1 Type = 1 Fichier » & SRP** : Chaque `class`, `record`, `interface` ou `enum` public DOIT avoir son propre fichier `.cs` nommé à l'identique.
- **Anti-Over-Engineering (KISS, YAGNI & Rule of Three)** :
  - **Pas d'abstraction spéculative** : Ne crée jamais d'interface générique (`IRepository<T>`), de `Factory` ou de `Strategy` s'il n'y a qu'un seul cas d'usage concret et aucun besoin de mock.
  - Privilégie le code simple, direct et lisible (*KISS*) avec des *Guard Clauses* (retours anticipés, `ArgumentNullException.ThrowIfNull`).
- **GRASP & « Tell, Don't Ask » (Éviter le Modèle Anémique)** :
  - **Information Expert (GRASP)** : Place la règle de calcul ou de validation dans l'entité ou le Value Object qui possède déjà les données (ex. `commande.Valider(codePromo)` au lieu de manipuler ses propriétés depuis un service externe).
  - **Encapsulation EF Core Code First** : Utilise des `private set;` sur les propriétés d'entités et expose les collections via `IReadOnlyCollection<T>` (EF Core 8 mappe nativement les champs privés `_lignes`).
  - **Make Illegal States Unrepresentable** : Empêche la création d'objets dans un état invalide (constructeur métier ou méthode de fabrique, `<Nullable>enable</Nullable>`, Value Objects en `readonly record struct`).
- **CQS (Command-Query Separation), Loi de Déméter & Idempotence** :
  - Sépare strictement les lectures (sans effet de bord) des commandes (modification d'état).
  - Pas de chaînage profond (`a.B.C.D` — Loi de Déméter).
  - Prévois la concurrence optimiste (`IsRowVersion()`) et l'idempotence sur les endpoints critiques (`POST`/`PUT`).
- **Design Patterns ciblés (.NET 8)** :
  - **Result Pattern** (ou `ProblemDetails`) pour les erreurs métier attendues (zéro exception pour le flux métier normal).
  - **Options Pattern** (`IOptions<T>` avec `.ValidateOnStart()`) pour la configuration.
- **Modélisation Graphique Systématique (Mermaid)** :
  - **Avant de coder une fonctionnalité non triviale**, génère un diagramme Mermaid concis (`sequenceDiagram` pour un flux d'API, `erDiagram` pour EF Code First, `classDiagram` pour le domaine) afin de valider la conception avant l'implémentation.
- **Séparation des responsabilités (`dotnet-architecture-review`)** :
  - `Presentation / Controllers` : Endpoints HTTP uniquement. Pas de logique métier ni de `DbContext` direct.
  - `Contracts / DTOs` : Un fichier par `sealed record` avec XML `<summary>` et `DateTimeOffset` (cf. `dotnet-webapi`).
  - `Domain / Entities` : Entités riches, sans dépendance à ASP.NET Core.
  - `Infrastructure / Persistence / Configurations` : Un fichier `XConfiguration.cs` par entité (`IEntityTypeConfiguration<T>`).

## 3. Règles d'Architecture & Conception d'API (Exigences Urbaniste & Architecte)
- **Séparation stricte** : Ne **jamais** exposer les entités EF Core directement dans les contrôleurs ou endpoints. Toujours utiliser des DTOs (`sealed record` immuables) distincts pour les requêtes (Request) et les réponses (Response).
- **Sémantique HTTP & REST** :
  - Utiliser les verbes et codes HTTP exacts (`200 OK`, `201 Created` avec `CreatedAtAction`, `204 NoContent`, `400 BadRequest`, `404 NotFound`, `409 Conflict`).
  - Toujours documenter les types de retour avec `[ProducesResponseType]` ou `.Produces<T>()` pour que le contrat OpenAPI / Swagger soit complet pour les autres équipes de la DSI.
- **Gestion des erreurs** : Utiliser le standard **ProblemDetails (RFC 9457)** et un middleware global (`IExceptionHandler`) plutôt que des blocs `try/catch` répétitifs dans chaque contrôleur.
- **Asynchronisme** : Toutes les opérations d'I/O (base de données, appels HTTP) doivent être `async` et propager obligatoirement le `CancellationToken` de la requête HTTP jusqu'à EF Core (`ToListAsync(cancellationToken)`, `SaveChangesAsync(cancellationToken)`).

## 4. Règles Entity Framework Core — Code First (Performance & Fiabilité)
- **Configuration explicite (Fluent API)** :
  - Privilégier `IEntityTypeConfiguration<T>` dans des classes dédiées (1 fichier par entité) plutôt que de surcharger les entités avec des Data Annotations.
  - **Interdiction des `nvarchar(max)` implicites** : Toujours définir `.HasMaxLength(x)` sur toutes les propriétés `string`.
  - Toujours préciser la précision des décimaux (`.HasPrecision(18, 2)`).
  - Définir explicitement le comportement de suppression (`.OnDelete(DeleteBehavior.Restrict)` par défaut pour éviter les suppressions en cascade accidentelles).
  - Définir les index (`.HasIndex(...)`) sur les clés étrangères et les colonnes utilisées dans les clauses `Where` ou `OrderBy`.
- **Requêtes performantes** :
  - Toujours utiliser `.AsNoTracking()` sur les requêtes en lecture seule (GET).
  - Projeter directement vers le DTO avec `.Select(...)` avant d'appeler `.ToListAsync()` pour ne récupérer que les colonnes utiles en SQL.
  - Ne jamais exécuter de requête EF Core à l'intérieur d'une boucle `foreach` (prévenir systématiquement les problèmes N+1).
- **Migrations Code First** :
  - Avant de valider une nouvelle migration, toujours vérifier si elle contient des opérations destructrices (`DropColumn`, `DropTable`) et alerter le développeur.

## 5. Règles Sécurité & Production (Exigences RSSI & Prod)
- **Sécurité (RSSI)** :
  - Valider toutes les entrées (DataAnnotations ou FluentValidation).
  - Vérifier systématiquement les autorisations (`[Authorize]`) et prévenir les failles IDOR (vérifier que l'utilisateur ou le système appelant a le droit d'accéder à la ressource demandée).
  - Ne jamais logger de données sensibles (mots de passe, tokens, données personnelles critiques).
- **Observabilité & Résilience (Production)** :
  - Utiliser le logging structuré avec `ILogger<T>` (ex. `logger.LogInformation("Commande {OrderId} créée", orderId)` et jamais d'interpolation `$""` dans les logs).
  - Utiliser `IHttpClientFactory` pour tout appel vers une autre API du SI (jamais de `new HttpClient()`).

## 6. Standards de Tests Unitaires (xUnit, Given/When/Then & Combinatoire `TheoryData`)
- **Pattern Given / When / Then** : Structurer chaque méthode de test avec les blocs explicites `// Given`, `// When`, `// Then` et un nommage expressif (`Methode_GivenContexte_WhenAction_ThenResultatAttendu` ou équivalent métier).
- **Exploration Combinatoire avec xUnit `[Theory]`** :
  - Privilégier `[Theory]` couplé à `[ClassData(typeof(...))]` (héritant de `TheoryData<...>`) pour couvrir un maximum de combinatoires métier pertinentes, valeurs frontières (boundary values) et cas innovants sans dupliquer le corps du test.
- **Séparation Stricte « Code du Test vs Données de la Theory » (1 Type = 1 Fichier `.cs`)** :
  - Le fichier de test (`*Tests.cs`) ne contient **que** la logique d'orchestration des tests (`Given / When / Then`).
  - Les jeux de données combinatoires sont obligatoirement extraits dans des classes dédiées (`*TheoryData.cs` héritant de `TheoryData<...>`) dans leurs propres fichiers `.cs` (ex: `ConformiteAniCombinatoireTheoryData.cs`, `RgpdAnonymisationTheoryData.cs`).

## 7. Routage Automatique des Skills & Sous-Agents (L'utilisateur n'a pas à choisir)
Dès que le développeur ouvre une conversation et fournit un ticket Azure DevOps ou un besoin, **tu dois choisir toi-même les Skills et Sous-agents adaptés sans qu'il ait à les nommer** :
1. **Si c'est un BUG / Maintenance** :
   - Active obligatoirement le skill **`systematic-debugging`** : (1) Collecter symptômes & chemin d'exécution, (2) Formuler 3 hypothèses, (3) Écrire un test unitaire qui **reproduit** le bug (`run-tests` / `writing-mstest-tests`), (4) Corriger la cause racine, (5) Vérifier que le test passe au vert.
2. **Si c'est une Nouvelle API / Évolution (.NET 8)** :
   - Active obligatoirement **`dotnet-architecture-review`**, **`dotnet-webapi`** et **`optimizing-ef-core-queries`**.
   - Vérifie les conventions existantes de la `.sln` avant de créer les fichiers (1 fichier par classe/record/interface).
3. **Si la tâche touche plusieurs domaines ou nécessite une revue PR** :
   - Lance de ta propre initiative les sous-agents en parallèle (Architecte, EF Core, RSSI/Prod) puis synthétise.

## 7. Hygiène du Harness (Zéro gonflement — Principe DRY & Modulaire)
Un bon fichier `GEMINI.md` / `AGENTS.md` doit rester **court et dense (< 100 lignes)** pour ne pas diluer l'attention du modèle :
1. **Raffiner plutôt qu'empiler** : Lors d'un apprentissage (`/learn` ou correction), modifie ou précise la règle existante au lieu d'ajouter un historique chronologique qui grossit indéfiniment.
2. **Déléguer les détails aux Skills et sous-dossiers** :
   - Les règles propres à une couche vont dans le sous-dossier concerné (ex. `src/Infrastructure/GEMINI.md`).
   - Les procédures longues vont dans `.agents/skills/<nom>/SKILL.md` (chargées uniquement à la demande, 0 token au repos).

### 📌 Conventions Spécifiques du Client (Max 10 règles actives, synthétisées)
- *(Ajouter ici uniquement les conventions propres au SI du client : nommage SQL, format d'URL Urbaniste, règles RSSI locales)*
