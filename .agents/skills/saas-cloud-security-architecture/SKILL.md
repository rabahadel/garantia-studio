---
name: saas-cloud-security-architecture
description: >-
  Architecture d'applications SaaS B2B modernes en .NET 8+, Web Services haute performance,
  Architecture Cloud, Security-by-Design (OWASP, Multi-Tenancy, RGPD/Zero-Data-Retention),
  FinOps IA (Cache SHA-256, Quotas par Tenant) et optimisation Base de Données SQL Server.
---

# Architecture SaaS, Cloud, Web Services & Security-by-Design (.NET 8+)

Ce Skill guide les Agents Architectes et Bâtisseurs pour concevoir et coder dès le départ (*Shift-Left*) des applications SaaS B2B robustes, performantes, sécurisées et prêtes pour le Cloud.

## 1. Architecture SaaS Multi-Tenant & Cloud-Native
- **Isolation Multi-Tenant stricte** : Chaque requête, commande ou entité persistée pour un cabinet/client DOIT porter un `TenantId` (`CabinetId`) vérifié dès l'entrée du pipeline HTTP (Claims JWT / En-tête sécurisé) et filtré via *Global Query Filters* dans EF Core.
- **Stateless Web Services** : Aucun état de session en mémoire locale d'un nœud (`IMemoryCache` uniquement pour des données publiques immuables ou doublé d'un cache distribué `IDistributedCache` / Redis / SQL).
- **Observabilité & HealthChecks** : Exposer `/healthz` et structurer les logs avec `ILogger<T>` (sans jamais logger de données personnelles PII).

## 2. Performance Base de Données, Cache FinOps & Web Services
- **Cache d'Empreinte Documentaire (FinOps IA)** : Avant tout appel coûteux (OCR / LLM), calculer le hash `SHA-256` du document entrant. Si le hash existe dans le référentiel des grilles connues, retourner le modèle structuré en $< 5\text{ ms}$ pour $0,00\text{ €}$.
- **SQL Server & EF Core Haute Performance** :
  - Séparation CQS stricte : `AsNoTracking()` + projection `.Select(...)` directe vers des `record` DTO sur toutes les lectures.
  - Configurations explicites `IEntityTypeConfiguration<T>` : interdiction des `nvarchar(max)` implicites, `varchar(N)` (`IsUnicode(false)`) sur les codes/hashes, `HasPrecision(18, 2)` sur les montants, et index couvrants (`HasIndex`) sur `TenantId` et clés de recherche.
- **Performance Runtime .NET** :
  - Zéro appel I/O synchrone bloquant, propagation obligatoire de `CancellationToken ct` de l'endpoint HTTP jusqu'au driver SQL/HTTP.
  - Utiliser `readonly record struct` pour les Value Objects légers afin de réduire la pression sur le Garbage Collector (Gen0).

## 3. Security-by-Design (OWASP, RGPD & Résilience Prod)
- **Privacy-by-Design & Évitement HDS** : Anonymiser/masquer à la volée (Regex/Span) les données nominatives (NIR / N° Sécurité Sociale, emails, téléphones, noms) avant tout traitement externe. Ne jamais persister les fichiers PDF sources des assurés sur disque (*Zero-Data Retention* en mémoire `Stream`).
- **Protection OWASP API Top 10** :
  - **Anti-IDOR (BOLA)** : Toute action sur une ressource `{id}` doit vérifier l'appartenance au `TenantId` de l'utilisateur authentifié.
  - **Anti-Mass Assignment** : Contrats d'entrée/sortie (`record` Request/Response) strictement séparés des entités du Domaine et d'EF Core.
  - **Quotas FinOps & Rate Limiting** : Vérifier le quota mensuel d'analyses du `TenantId` (Coupe-circuit budgétaire) pour garantir la marge brute > 90 % et bloquer les attaques par déni de portefeuille (*Denial of Wallet*).
