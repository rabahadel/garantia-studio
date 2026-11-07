# HARNESS PROJET — Maintenance .NET Framework 4.x / ASP.NET MVC / EF6 Code First

> **Mode d'emploi** : Copiez ce fichier à la racine des projets existants en .NET Framework 4.x (sous `.agent/rules/dotnet4-legacy.md`, `GEMINI.md` ou `.github/copilot-instructions.md`) pour empêcher l'IA d'inventer du code .NET 8 incompatible.

---

## 1. Contexte Technique du Projet (Legacy / Existant)
- **Framework** : .NET Framework 4.x (4.6.2 / 4.7.2 / 4.8)
- **Architecture Web** : ASP.NET MVC 5 et/ou ASP.NET Web API 2 (`System.Web`, `ApiController`, `Global.asax`, `Web.config`)
- **Accès aux données** : Entity Framework 6 (EF6) en approche **Code First** (`System.Data.Entity`, `DbModelBuilder`)
- **Outils** : Visual Studio 2022, Git, Azure DevOps 2020

## 2. Garde-fous de Compatibilité C# & .NET Framework 4.x (CRITIQUE)
- **Interdictions strictes de syntaxe moderne** (sauf si le `.csproj` définit explicitement une LangVersion récente) :
  - Ne **pas** utiliser de syntaxe C# 9/10/11/12 incompatible avec le compilateur classique de .NET Framework (pas de `file-scoped namespaces`, pas de `global using`, pas de `record`, pas de `required`, pas de `primary constructors`, pas de collection expressions `[]`).
  - Ne **pas** utiliser les API de `Microsoft.AspNetCore.*` ou `Microsoft.EntityFrameworkCore.*`.
  - Utiliser les namespaces historiques : `System.Web.Mvc`, `System.Web.Http`, `System.Data.Entity`.
- **Configuration** : Utiliser `ConfigurationManager` (`Web.config` / `App.config`) et non `appsettings.json` (sauf si déjà mis en place dans le projet).

## 3. Règles de Maintenance Prudente, Clean Code & Modélisation (« Chirurgie sans régression »)
- **1 Type = 1 Fichier `.cs` & Principes SOLID** : Tout nouveau type créé (classe de service, DTO, ViewModel, Strategy/Factory, configuration EF6) doit être dans son propre fichier `.cs` dédié.
- **Modélisation Graphique avant modification complexe (Mermaid)** :
  - Pour tout bug complexe ou refactoring Legacy, génère d'abord un `sequenceDiagram` (flux d'appel actuel vs corrigé) ou un `erDiagram` (impact tables EF6) avant de toucher au code.
- **Principe du changement minimal** : Sur une application existante, modifier uniquement ce qui est strictement nécessaire au ticket. Ne pas reformater tout un fichier (cela pollue les Pull Requests dans Azure DevOps).
- **Analyse d'impact avant modification** :
  - Avant de modifier une méthode partagée ou une entité EF6, toujours lister tous les contrôleurs, vues Razor (`.cshtml`) ou services qui l'appellent.
  - Respecter le style et les conventions de nommage déjà présents dans la solution, même s'ils sont anciens.

## 4. Protocole Automatique de Résolution de Bug (Reproduction → Fix définitif)
Dès que le développeur soumet un bug ou un ticket d'anomalie Azure DevOps, applique automatiquement les skills **`systematic-debugging`** et **`csharp-refactoring`** sans qu'il ait besoin de les demander :
1. **Traçage du flux** : Identifie le chemin complet (`Controller` → `Service` → `EF6 DbContext` → `SQL`).
2. **Hypothèses classées** : Formule les 2 ou 3 causes racines les plus probables avec preuves dans le code.
3. **Reproduction avant correction** : Propose un test unitaire (ou un cas de reproduction exact) qui échoue à cause du bug.
4. **Correction minimale & non-régression** : Corrige la cause racine sans effet de bord, puis vérifie avec `run-tests`.

## 7. Hygiène du Harness (Zéro gonflement — Principe DRY & Modulaire)
Ce fichier `GEMINI.md` / `AGENTS.md` doit rester **court (< 100 lignes)** :
1. **Fusionner et synthétiser** : Lors d'un apprentissage (`/learn`), mets à jour la règle concernée plutôt que d'empiler un journal infini.
2. **Déléguer aux sous-dossiers ou Skills** : Si un module Legacy a une règle très longue, place-la dans un `GEMINI.md` local à ce sous-dossier ou dans `.agents/skills/`.

### 📌 Spécificités & Pièges Connus du Projet (Max 10 points clés)
- *(Documenter ici de façon ultra-concise les conventions propres à cette application Legacy)*

## 4. Règles spécifiques Entity Framework 6 (EF6 Code First)
- **Attention au Lazy Loading** : Dans EF6, le Lazy Loading est souvent activé par défaut via les propriétés `virtual`. Faire très attention aux boucles N+1 dans les vues Razor (`.cshtml`) ou lors de la sérialisation JSON (boucles infinies de sérialisation).
- **Include en EF6** : Utiliser `using System.Data.Entity;` pour avoir accès à `.Include(x => x.Propriete)` fortement typé (attention : `.ThenInclude()` n'existe pas en EF6, on utilise `.Include(x => x.Prop.Select(y => y.SousProp))`).
- **Lecture seule** : Utiliser `.AsNoTracking()` sur les lectures lourdes.
- **Cycle de vie du `DbContext`** : Vérifier que le `DbContext` est bien instancié par requête HTTP (ou dans un bloc `using`) et jamais en `static` ou Singleton.

## 5. Préparation à la Modernisation (Lien vers .NET 8)
- Lors de l'ajout d'une nouvelle règle métier dans une application .NET 4, isoler la logique dans une classe de service pure (POCO, sans dépendance directe à `HttpContext.Current`) afin qu'elle puisse être facilement extraite ou appelée par une future API .NET 8.
