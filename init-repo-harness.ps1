<#
.SYNOPSIS
    Initialise le Harness IA (GEMINI.md + CARTE-APPLI.md) dans un dépôt Git client (.NET 4 ou .NET 8)
    et le masque automatiquement dans .git/info/exclude pour ne jamais polluer Azure DevOps.

.EXAMPLE
    .\init-repo-harness.ps1 -RepoPath "C:\Repos\MonApiNet8" -Stack Net8
    .\init-repo-harness.ps1 -RepoPath "C:\Repos\MonAppliLegacy" -Stack Net4
#>
param(
    [Parameter(Mandatory = $true, HelpMessage = "Chemin complet du dépôt Git cloné")]
    [string]$RepoPath,

    [Parameter(Mandatory = $true, HelpMessage = "Stack du projet : Net8 ou Net4")]
    [ValidateSet("Net8", "Net4")]
    [string]$Stack
)

$ErrorActionPreference = "Stop"
$KitRoot = $PSScriptRoot

if (-not (Test-Path -Path $RepoPath)) {
    Write-Error "Le dossier cible '$RepoPath' n'existe pas."
    exit 1
}

# 1. Sélection du bon modèle de Harness
$SourceHarness = if ($Stack -eq "Net8") {
    Join-Path $KitRoot "01-harnais-projets\HARNESS-DOTNET8-API.md"
} else {
    Join-Path $KitRoot "01-harnais-projets\HARNESS-DOTNET4-LEGACY.md"
}

$TargetGeminiMd = Join-Path $RepoPath "GEMINI.md"
Copy-Item -Path $SourceHarness -Destination $TargetGeminiMd -Force
Write-Host "[OK] GEMINI.md ($Stack) copie dans : $TargetGeminiMd" -ForegroundColor Green

# 2. Création de la trame CARTE-APPLI.md si elle n'existe pas encore
$TargetCarteMd = Join-Path $RepoPath "CARTE-APPLI.md"
if (-not (Test-Path -Path $TargetCarteMd)) {
    $CarteTemplate = @"
# 🗺️ Carte d'Architecture & Mémoire de l'Application

> Fichier généré et maintenu avec l'IA (ignoré par Git via `.git/info/exclude`).
> Demandez à Antigravity : *"Analyse cette solution selon PLAYBOOK-SEMAINE-1.md et remplis CARTE-APPLI.md avec les diagrammes Mermaid."*

## 1. Stack & Architecture Globale (`flowchart LR`)
*(À générer au premier scan)*

## 2. Modèle de Données EF Code First (`erDiagram`)
*(À générer au premier scan)*

## 3. Principaux Flux Métiers (`sequenceDiagram`)
*(À générer au premier scan)*

## 4. Règles Métier Clés & Zones à Risque
*(À compléter au fil des tickets Azure DevOps)*
"@
    Set-Content -Path $TargetCarteMd -Value $CarteTemplate -Encoding UTF8
    Write-Host "[OK] Trame CARTE-APPLI.md creee dans : $TargetCarteMd" -ForegroundColor Green
}

# 3. Masquage furtif dans .git/info/exclude (100% local, invisible dans git status et sur Azure DevOps)
$GitDir = Join-Path $RepoPath ".git"
if (Test-Path -Path $GitDir) {
    $ExcludeFile = Join-Path $GitDir "info\exclude"
    $InfoDir = Split-Path $ExcludeFile -Parent
    if (-not (Test-Path -Path $InfoDir)) {
        New-Item -ItemType Directory -Path $InfoDir -Force | Out-Null
    }
    $ExistingContent = if (Test-Path -Path $ExcludeFile) { Get-Content -Path $ExcludeFile -Raw } else { "" }
    $EntriesToExclude = @("GEMINI.md", "AGENTS.md", "CARTE-APPLI.md", ".agent/", ".agents/", ".github/copilot-instructions.md")

    $Added = 0
    foreach ($Entry in $EntriesToExclude) {
        if ($ExistingContent -notmatch [regex]::Escape($Entry)) {
            Add-Content -Path $ExcludeFile -Value $Entry -Encoding UTF8
            $Added++
        }
    }
    Write-Host "[OK] Fichiers IA masques dans .git/info/exclude ($Added nouvelles entrees ajoutees)." -ForegroundColor Cyan
} else {
    Write-Host "[INFO] Pas de dossier .git detecte dans '$RepoPath' (pensez a relancer le script apres git init / git clone)." -ForegroundColor Yellow
}

Write-Host "`nPret ! Ouvrez '$RepoPath' dans Antigravity Desktop et lancez votre analyse." -ForegroundColor Magenta
