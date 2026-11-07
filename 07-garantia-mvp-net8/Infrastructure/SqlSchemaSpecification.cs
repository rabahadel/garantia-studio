namespace GarantIA.Infrastructure;

/// <summary>
/// Spécification du schéma SQL Server Haute Performance pour GarantIA (Zéro NVARCHAR(MAX) implicite,
/// typage strict VARCHAR(64) sur les hashes SHA-256, DECIMAL(18,2) sur les montants et index couvrants).
/// </summary>
public static class SqlSchemaSpecification
{
    /// <summary>
    /// Retourne le script DDL T-SQL optimisé pour SQL Server / Azure SQL Database.
    /// </summary>
    public static string GetRecommendedSqlServerDdl() =>
        """
        -- =========================================================================================
        -- GARANTIA SAAS B2B - SCHEMA SQL SERVER HAUTE PERFORMANCE & FINOPS IA
        -- Règles : Zéro NVARCHAR(MAX) implicite, VARCHAR(64) ASCII sur SHA-256, DECIMAL(18,2),
        -- Isolation Multi-Tenant par UNIQUEIDENTIFIER (TenantCabinetId) et Index Couvrants (INCLUDE).
        -- =========================================================================================

        CREATE TABLE dbo.GrillesGarantiesCache
        (
            GrilleCacheId          UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_GrillesGarantiesCache PRIMARY KEY NONCLUSTERED,
            DocumentSha256         VARCHAR(64)      NOT NULL,
            NomAssureur            NVARCHAR(120)    NOT NULL,
            NomContrat             NVARCHAR(160)    NOT NULL,
            CotisationMensuelleHt  DECIMAL(18, 2)   NOT NULL,
            DateIndexation         DATETIMEOFFSET   NOT NULL
        );

        -- Index couvrant unique sur l'empreinte SHA-256 (Lookup < 1 ms en lecture seule sans Key Lookup)
        CREATE UNIQUE CLUSTERED INDEX IX_GrillesGarantiesCache_DocumentSha256
            ON dbo.GrillesGarantiesCache (DocumentSha256);

        CREATE NONCLUSTERED INDEX IX_GrillesGarantiesCache_Assureur_Contrat
            ON dbo.GrillesGarantiesCache (NomAssureur, NomContrat)
            INCLUDE (DocumentSha256, CotisationMensuelleHt, DateIndexation);

        CREATE TABLE dbo.AuditsComparatifsDda
        (
            AuditId                   UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_AuditsComparatifsDda PRIMARY KEY NONCLUSTERED,
            TenantCabinetId           UNIQUEIDENTIFIER NOT NULL,
            DocumentActuelSha256      VARCHAR(64)      NOT NULL,
            DocumentCibleSha256       VARCHAR(64)      NOT NULL,
            CotisationActuelleHt      DECIMAL(18, 2)   NOT NULL,
            CotisationCibleHt         DECIMAL(18, 2)   NOT NULL,
            EconomieMensuelleEstimee  DECIMAL(18, 2)   NOT NULL,
            CacheHitFinOps            BIT              NOT NULL,
            DateCreationUtc           DATETIMEOFFSET   NOT NULL
        );

        -- Index Clustered sur (TenantCabinetId, DateCreationUtc DESC) pour l'isolation Multi-Tenant et les quotas FinOps
        CREATE CLUSTERED INDEX IX_AuditsComparatifsDda_Tenant_DateCreation
            ON dbo.AuditsComparatifsDda (TenantCabinetId, DateCreationUtc DESC);

        -- Index Non-Clustered couvrant pour le calcul rapide des quotas mensuels et de l'historique par cabinet
        CREATE NONCLUSTERED INDEX IX_AuditsComparatifsDda_Tenant_Hashes_Covering
            ON dbo.AuditsComparatifsDda (TenantCabinetId, DocumentActuelSha256, DocumentCibleSha256)
            INCLUDE (CotisationActuelleHt, CotisationCibleHt, EconomieMensuelleEstimee, CacheHitFinOps, DateCreationUtc);
        """;
}
