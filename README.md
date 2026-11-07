# 🧰 Kit de Mission — Développeur .NET & Harness IA
**Mission : Modernisation SI, Développement d'API (.NET 8) & Maintenance (.NET 4 / MVC / EF Code First)**

Ce dossier contient votre **Harness IA (harnais de pilotage)** prêt à l'emploi pour votre nouvelle mission. Il est conçu pour transformer n'importe quelle IA en un binôme senior qui connaît vos contraintes techniques (.NET 4 vs .NET 8, EF Code First) et organisationnelles (Scrum, Architecte, Urbaniste, RSSI, Production).

---

## 📂 Contenu du Kit

1. **[01-harnais-projets/](./01-harnais-projets/)** — *Les fichiers de cadrage permanent à glisser dans vos projets Git* :
   * [`HARNESS-DOTNET8-API.md`](./01-harnais-projets/HARNESS-DOTNET8-API.md) : Règles strictes pour les nouvelles Web API en **.NET 8 + EF Core Code First**.
   * [`HARNESS-DOTNET4-LEGACY.md`](./01-harnais-projets/HARNESS-DOTNET4-LEGACY.md) : Garde-fous pour maintenir les applications **.NET Framework 4.x / ASP.NET MVC / EF6** sans casser l'existant.
2. **[02-onboarding-et-cartographie/](./02-onboarding-et-cartographie/)** — *Pour comprendre vite les applications internes* :
   * [`PLAYBOOK-SEMAINE-1.md`](./02-onboarding-et-cartographie/PLAYBOOK-SEMAINE-1.md) : La méthode pas-à-pas pour faire analyser une application inconnue par l'IA et générer sa carte mémoire.
3. **[03-workflows-quotidiens/](./03-workflows-quotidiens/)** — *Vos prompts prêts à copier-coller au quotidien* :
   * [`PROMPTS-MISSIONS.md`](./03-workflows-quotidiens/PROMPTS-MISSIONS.md) : Prompts pour créer une API, sécuriser EF Code First, passer l'audit RSSI/Architecte/Prod et gérer Azure DevOps / Scrum.

---

## 🚀 Comment l'utiliser et DEPUIS OÙ ?

Chez votre client, vous serez dans l'un de ces **3 cas de figure** selon les outils autorisés sur votre poste de travail. Voici comment brancher ce kit dans chaque cas :

### Cas 1 : Vous utilisez Antigravity (ou Cursor / Windsurf / Claude Code) sur le code
C'est le mode le plus puissant (mode **Agentique**) car l'IA lit directement les fichiers de la solution `.sln` et lance les builds.
1. Ouvrez le dossier de l'application sur laquelle vous travaillez.
2. Créez un dossier `.agent/rules/` à la racine du projet (ou un fichier `GEMINI.md` / `CLAUDE.md` à la racine).
3. Copiez-y le contenu de [`HARNESS-DOTNET8-API.md`](./01-harnais-projets/HARNESS-DOTNET8-API.md) (si c'est une nouvelle API .NET 8) **ou** de [`HARNESS-DOTNET4-LEGACY.md`](./01-harnais-projets/HARNESS-DOTNET4-LEGACY.md) (si c'est une appli .NET 4).
4. **Astuce Git (Important !)** : Pour que ces fichiers restent votre « arme secrète » personnelle sans polluer le dépôt Git Azure DevOps de votre équipe, ajoutez leur nom dans le fichier `.git/info/exclude` de votre dépôt local (c'est comme un `.gitignore`, mais uniquement sur votre PC, invisible pour les autres !).

### Cas 2 : Vous utilisez uniquement Visual Studio 2022 + GitHub Copilot
Votre contrat mentionne **Visual Studio 2022**. Si vous avez l'extension GitHub Copilot dans Visual Studio :
1. Créez un fichier `.github/copilot-instructions.md` à la racine de la solution Visual Studio.
2. Collez-y le contenu de [`HARNESS-DOTNET8-API.md`](./01-harnais-projets/HARNESS-DOTNET8-API.md) ou [`HARNESS-DOTNET4-LEGACY.md`](./01-harnais-projets/HARNESS-DOTNET4-LEGACY.md).
3. Visual Studio 2022 injectera automatiquement ces règles dans **toutes** vos discussions Copilot Chat sans que vous ayez à les retaper !
4. Là aussi, ajoutez `.github/copilot-instructions.md` dans `.git/info/exclude` si vous voulez le garder privé sur votre poste.

### Cas 3 : Poste client verrouillé (Vous utilisez ChatGPT / Claude / Gemini dans le navigateur)
Si le poste client n'a pas d'IA intégrée à Visual Studio et que vous utilisez une interface Web :
1. Créez **2 espaces dédiés** (appelés *Projets* dans Claude/ChatGPT ou *Gems* dans Gemini) :
   * Un espace nommé **« Mission - Maintenance .NET 4 »** (collez [`HARNESS-DOTNET4-LEGACY.md`](./01-harnais-projets/HARNESS-DOTNET4-LEGACY.md) dans ses instructions personnalisées).
   * Un espace nommé **« Mission - Nouvelles API .NET 8 »** (collez [`HARNESS-DOTNET8-API.md`](./01-harnais-projets/HARNESS-DOTNET8-API.md) dans ses instructions personnalisées).
2. **Attention RSSI** : Dans un navigateur web grand public, **anonymisez toujours** les noms propres du client, les chaînes de connexion, les clés d'API et les URL internes avant de coller un bout de code !

---

## ⚡ La routine quotidienne en 3 étapes (Votre méthode de travail)

À chaque nouveau ticket Azure DevOps dans votre Sprint Scrum :

1. **Avant de coder (5 min)** : Ouvrez [`PROMPTS-MISSIONS.md`](./03-workflows-quotidiens/PROMPTS-MISSIONS.md) → Utilisez le **Prompt 1 (Cadrage de User Story)** ou tapez **`/plan`** / **`/grill-me`** pour faire le plan avec l'IA.
2. **Pendant le code** : Laissez le Harness (.NET 4 ou .NET 8) guider la génération du code et des configurations **EF Code First**.
3. **Avant de pousser sur Git / Azure DevOps (5 min)** : Utilisez le **Prompt 4 (Audit RSSI / Architecte / Prod)** sur votre code modifié. Vous arriverez en revue de code avec un coup d'avance sur toute l'équipe.

---

## ⌨️ Aide-Mémoire des Commandes `/` (Slash Commands Antigravity)

> **Astuce** : Tapez simplement **`/`** au début de la zone de saisie du chat d'Antigravity Desktop pour afficher la liste déroulante automatique de toutes les commandes disponibles.

| Commande | À quoi elle sert dans votre mission .NET |
| :--- | :--- |
| **`/plan`** | Demander un plan d'action étape par étape avant de toucher au code (idéal au début d'une User Story). |
| **`/grill-me`** | L'IA vous pose des questions ciblées pour lever toutes les zones d'ombre d'un ticket Azure DevOps flou. |
| **`/learn`** | Enregistrer une règle ou une correction que vous venez de faire pour que l'IA s'en souvienne à l'avenir. |
| **`/compact`** | Résumer et alléger la conversation en cours quand elle devient longue afin de garder l'IA rapide et précise. |
| **`/browser`** | Connecter l'IA au navigateur pour tester une application MVC locale, une page ou un Swagger en direct. |
| **`/teamwork`** | Mobiliser une équipe d'agents autonomes en parallèle sur un gros chantier (ex. migration .NET 4 → .NET 8). |
| **`/boost`** | Activer le mode de réflexion approfondie multi-perspectives (idéal sur un bug critique ou un choix d'architecture). |
| **`/goal`** | Lancer une tâche longue en autonomie jusqu'à ce que l'objectif (ex. tous les tests compilent et passent) soit atteint. |
| **`/schedule`** | Programmer une instruction récurrente ou un minuteur. |

---

## 🗂️ Comment gérer PLUSIEURS Repos et Applications chez le client ?

Dans votre mission, vous allez jongler entre plusieurs dépôts Git (.NET 4 MVC, nouvelles API .NET 8, etc.).
**Il ne faut jamais mélanger les spécificités d'une application dans la configuration globale de votre PC.**

```
💻 Votre PC Client
 ├── 🌐 Config Globale (C:\Users\<Vous>\.gemini\config\)
 │    ├── Règle globale : Ignorer bin/ obj/, 1 classe = 1 fichier, SOLID/GRASP/KISS
 │    └── Skills GitHub : dotnet-webapi, optimizing-ef-core-queries, systematic-debugging...
 │
 ├── 📁 C:\Repos\AppliInterne-RH-Net4\          <-- Repo 1 (Legacy .NET 4.7.2 + EF6)
 │    ├── GEMINI.md                             <-- Copie de HARNESS-DOTNET4-LEGACY.md (propre à ce repo)
 │    └── CARTE-APPLI.md                        <-- Schémas Mermaid & tables de l'appli RH uniquement
 │
 ├── 📁 C:\Repos\ApiFacturation-Net8\           <-- Repo 2 (Nouvelle API .NET 8 + EF Core 8)
 │    ├── GEMINI.md                             <-- Copie de HARNESS-DOTNET8-API.md (propre à ce repo)
 │    └── CARTE-APPLI.md                        <-- Schémas Mermaid & flux de l'API Facturation uniquement
 │
 └── 📁 C:\Repos\AppliStock-MVC-Net4\           <-- Repo 3 (Autre appli Legacy)
      ├── GEMINI.md                             <-- Copie de HARNESS-DOTNET4-LEGACY.md
      └── CARTE-APPLI.md                        <-- Schémas Mermaid & tables de l'appli Stock uniquement
```

