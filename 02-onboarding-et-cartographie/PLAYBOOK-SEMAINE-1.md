# 🗺️ Playbook Semaine 1 — Cartographier une Application Interne avec l'IA

Dans votre mission, vous allez maintenir **plusieurs applications internes dans des domaines fonctionnels et techniques variés** et créer de **nouvelles API**. 
Le piège classique est de se noyer dans le code existant.

Voici comment utiliser votre IA dès l'ouverture d'un nouveau dépôt Git pour comprendre en **15 minutes** ce qui prend normalement **3 jours**.

---

## Étape 1 : Le Scan Rapide d'une Solution Visual Studio (Prompt d'Onboarding)

Dès que vous clonez une application depuis Azure DevOps 2020, ouvrez votre IA et envoyez ce prompt (en lui donnant accès au dossier ou en lui collant le `.sln`, les `.csproj` et `Program.cs` / `Global.asax` + `DbContext`) :

```markdown
Tu es un Architecte .NET Senior. Je viens d'arriver sur ce projet que je dois maintenir et faire évoluer.
Analyse la structure de la solution (sans lire les dossiers bin/ et obj/) en suivant cet ordre :
1. Les fichiers .sln et .csproj (versions .NET, packages NuGet, dépendances entre projets).
2. Le point d'entrée (Program.cs, Startup.cs ou Global.asax.cs / WebApiConfig.cs).
3. Le(s) DbContext Entity Framework Code First et les configurations de mapping.

Génère-moi une fiche synthétique "CARTE-APPLI.md" contenant obligatoirement une **modélisation graphique Mermaid** pour que je visualise tout en un coup d'œil :
1. **Stack & Architecture (`flowchart LR`)** : Version .NET, EF6/EF Core, authentification, et diagramme Mermaid des projets/couches de la `.sln` et des appels externes (autres API, BDD).
2. **Modèle de Données EF Code First (`erDiagram`)** : Diagramme entité-relation Mermaid des 5 à 10 tables/entités principales du `DbContext` avec leurs clés et relations.
3. **Principaux Flux Métiers (`sequenceDiagram`)** : 1 ou 2 diagrammes de séquence Mermaid illustrant les parcours métiers majeurs de l'application (du contrôleur/écran jusqu'à SQL).
4. **Règles métier clés & Zones à risque** : Dette technique visible, configurations particulières (`Web.config` / `appsettings.json`), et pièges à éviter avant de modifier le code.
```

---

## Étape 2 : Sauvegarder votre « Mémoire Externe »

1. Enregistrez la réponse de l'IA dans un fichier `CARTE-APPLI.md` à la racine du projet (et ajoutez `CARTE-APPLI.md` dans `.git/info/exclude` pour le garder pour vous).
2. **Pourquoi c'est magique ?** La semaine suivante, quand on vous donnera un bug urgent sur cette application, vous n'aurez qu'à dire à l'IA :
   > *« Lis `CARTE-APPLI.md` pour te remettre le contexte en tête, puis analyse le bug suivant… »*

---

## Étape 3 : Les questions stratégiques à poser dès la Semaine 1 (Équipe, Architecte, Urbaniste)

Votre contrat indique que vous travaillez avec **une équipe de 4 personnes, l'Architecte, l'Urbaniste, le RSSI et la Production**.
Voici la checklist des informations à récupérer dès vos premiers jours pour enrichir votre fichier [`HARNESS-DOTNET8-API.md`](../01-harnais-projets/HARNESS-DOTNET8-API.md) :

- [ ] **Standard d'API du client (Urbaniste / Architecte)** :
  - Quel est le format standard des URLs ? (ex: `/api/v1/clients/{id}`)
  - Y a-t-il un template de solution .NET 8 interne à l'entreprise ou un package NuGet maison obligatoire ?
- [ ] **Authentification & Sécurité (RSSI)** :
  - Comment les nouvelles API .NET 8 sont-elles sécurisées ? (JWT / OAuth2 / Active Directory / Windows Auth / API Key entre services ?)
- [ ] **Entity Framework Code First (Équipe Dev & DBA/Prod)** :
  - Comment les migrations Code First sont-elles appliquées en Recette et en Production sur Azure DevOps 2020 ? (Script SQL généré via `Script-Migration` / `dotnet ef migrations script --idempotent` validé par un DBA, ou exécution automatique au déploiement ?)
- [ ] **Exploitation (Production)** :
  - Où vont les logs des API ? (Fichiers plats, Serilog, Seq, ELK, Application Insights ?)
