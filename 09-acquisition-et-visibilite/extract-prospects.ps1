$ErrorActionPreference = "Stop"
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

Write-Host "Interrogation de l'API Open Data de l'Etat francais (recherche-entreprises.api.gouv.fr)..."
$urlCourtiers = "https://recherche-entreprises.api.gouv.fr/search?activite_principale=66.22Z&tranche_effectif_salarie=01,02,03&etat_administratif=A&per_page=25"
$response = Invoke-RestMethod -Method Get -Uri $urlCourtiers

$lines = @()
$lines += "# Liste de Prospects Reels (Open Data Etat Francais - Courtiers NAF 66.22Z)"
$lines += ""
$lines += "> **Source officielle** : API `recherche-entreprises.api.gouv.fr` (Cabinets de courtage d'assurance actifs en France)."
$lines += "> **Lien de ta Demo en ligne** : https://rabahadel.github.io/garantia-studio/"
$lines += ""
$lines += "| # | Nom du Cabinet | SIREN | Ville | Dirigeant(s) Identifie(s) | Action Rapide (LinkedIn 1-Clic) |"
$lines += "| :- | :--- | :--- | :--- | :--- | :--- |"

$index = 1
foreach ($ent in $response.results) {
    $nom = $ent.nom_complet
    $siren = $ent.siren
    $ville = if ($ent.siege.libelle_commune) { "$($ent.siege.code_postal) $($ent.siege.libelle_commune)" } else { "France" }
    
    $dirigeantsList = @()
    if ($ent.dirigeants) {
        foreach ($d in $ent.dirigeants | Select-Object -First 2) {
            if ($d.prenoms -and $d.nom) {
                $dirigeantsList += "$($d.prenoms) $($d.nom) ($($d.qualite))"
            } elseif ($d.denomination) {
                $dirigeantsList += "$($d.denomination)"
            }
        }
    }
    $dirigeantsStr = if ($dirigeantsList.Count -gt 0) { $dirigeantsList -join ", " } else { "Dirigeant Cabinet" }
    
    $firstDir = if ($dirigeantsList.Count -gt 0) { $dirigeantsList[0] } else { "" }
    $searchQuery = [uri]::EscapeDataString("$nom $firstDir courtier")
    $linkedinUrl = "https://www.linkedin.com/search/results/all/?keywords=$searchQuery"
    
    $lines += "| $index | **$nom** | ``$siren`` | $ville | $dirigeantsStr | [Contacter sur LinkedIn]($linkedinUrl) |"
    $index++
}

$outputPath = "c:\antigravity\comprendreia\09-acquisition-et-visibilite\PROSPECTS-REELS-FRANCE.md"
New-Item -ItemType Directory -Force -Path (Split-Path $outputPath) | Out-Null
[System.IO.File]::WriteAllLines($outputPath, $lines, [System.Text.Encoding]::UTF8)
Write-Host "Fichier genere avec succes : $outputPath"
