# 💻 Mémo Rapide : Réinstaller vos Skills & Harnais sur la Machine Client

Quand vous serez sur votre poste client (avec Antigravity Desktop ou Visual Studio 2022), voici comment tout remettre en place en 2 minutes.

---

## 1. Installer les Skills officiels Microsoft (`dotnet/skills`) depuis GitHub

Les Skills Microsoft que vous avez sur votre machine personnelle viennent du dépôt officiel **`https://github.com/dotnet/skills`**.

### Option A : Via un outil compatible Plugins / Skills CLI
```powershell
# Ajouter le marketplace officiel Microsoft .NET
/plugin marketplace add dotnet/skills

# Installer les briques utiles pour votre mission (.NET 4 -> .NET 8, API, EF Core, Tests)
/plugin install dotnet@dotnet-agent-skills
/plugin install dotnet-aspnetcore@dotnet-agent-skills
/plugin install dotnet-data@dotnet-agent-skills
/plugin install dotnet-upgrade@dotnet-agent-skills
/plugin install dotnet-test@dotnet-agent-skills
```

### Option B : Dans Antigravity Desktop (Dossier Global)
Sur Antigravity Desktop, vos Skills globaux sont de simples dossiers placés dans :
`C:\Users\<VotreNomUtilisateur>\.gemini\config\skills\`

Pour les transférer ou les installer sur le poste client :
1. Soit vous copiez votre dossier `.gemini\config\skills\` (et votre règle globale) sur le poste client.
2. Soit vous clonez `https://github.com/dotnet/skills` et copiez les sous-dossiers `skills/<nom-du-skill>` qui vous intéressent dans `C:\Users\<UserClient>\.gemini\config\skills\`.

---

## 2. Les 7 Skills à garder sous la main pour votre mission (et comment les déclencher)

Dans Antigravity, **vous n'avez rien à configurer pour les activer** : l'IA lit votre demande et charge le bon Skill toute seule. Vous pouvez aussi citer leur nom pour forcer leur utilisation :

| Besoin dans votre mission | Skill qui s'active | Mot-clé à mettre dans votre prompt |
| :--- | :--- | :--- |
| Créer/modifier un endpoint API .NET 8, Swagger, `ProblemDetails` | **`dotnet-webapi`** | *"Avec `dotnet-webapi`, crée l'endpoint..."* |
| Optimiser du EF Core, éviter les N+1, `AsNoTracking`, migrations | **`optimizing-ef-core-queries`** | *"Audite cet accès EF Core avec `optimizing-ef-core-queries`"* |
| Nettoyer du vieux code .NET 4 sans casser le comportement | **`csharp-refactoring`** | *"Refactorise sans changer le comportement (`csharp-refactoring`)"* |
| Vérifier l'architecture, le couplage, Clean Archi / CQRS | **`dotnet-architecture-review`** | *"Fais une revue d'architecture de ce module"* |
| Analyser une requête SQL Server lente ou un index manquant | **`sqlserver-analysis`** | *"Analyse l'impact SQL Server et les index"* |
| Diagnostiquer un bug complexe étape par étape | **`systematic-debugging`** | *"Aide-moi à diagnostiquer ce bug méthodiquement"* |
| Lancer ou corriger les tests unitaires | **`run-tests`** / **`writing-mstest-tests`** | *"Lance les tests"* / *"Corrige ce test unitaire"* |
