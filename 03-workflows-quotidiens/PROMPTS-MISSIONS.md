# ⚡ Workflows & Prompts Quotidiens — Mission Développeur .NET

Voici vos **5 prompts « Harnais »** prêts à copier-coller pour chaque moment clé de votre journée dans une équipe Scrum (.NET 4 / .NET 8 / EF Code First / Azure DevOps).

---

## 1. Démarrer une User Story (Cadrage en 2 temps)
*À utiliser juste après avoir pris un ticket dans Azure DevOps, AVANT de coder.*

```markdown
Voici la User Story / le ticket Azure DevOps sur lequel je dois travailler :
"""
[COLLER LE TICKET ICI]
"""

Contexte : Application [.NET 4 MVC / API .NET 8] utilisant Entity Framework Code First.
Voici les fichiers concernés : [indiquer ou coller les classes/entités proches].

NE GÉNÈRE AUCUN CODE POUR L'INSTANT.
1. Liste les cas limites (edge cases) ou règles métier implicites qu'il faut vérifier (valeurs nulles, concurrence, droits d'accès, volumétrie).
2. Indique s'il y a un impact sur le modèle EF Code First (nouvelle colonne, index, migration nécessaire).
3. Propose un plan d'implémentation étape par étape très court (3 à 5 étapes max).
Attends ma validation avant d'écrire le code.
```

---

## 2. Extraire une logique Legacy (.NET 4) vers une nouvelle API (.NET 8)
*Pour répondre au « Schéma Directeur de modernisation du SI » de votre contrat.*

```markdown
Dans le cadre de la modernisation du SI, je dois transformer ce traitement existant (issu d'une application .NET Framework 4 / ASP.NET MVC) en un endpoint propre dans une nouvelle Web API .NET 8.

Voici le code existant :
"""
[COLLER L'ACTION DU CONTRÔLEUR OU SERVICE .NET 4]
"""

Procède en 3 étapes structurées :
1. **Extraction des règles métier** : Liste toutes les règles de gestion exécutées par ce code (y compris les règles cachées et les appels base de données), afin d'être certain de n'oublier aucun comportement fonctionnel.
2. **Proposition du contrat d'API REST (.NET 8)** : Propose la route, le verbe HTTP, le Request DTO, le Response DTO, et les codes de retour HTTP (200, 400, 404, 409) conformes aux bonnes pratiques d'urbanisation.
3. **Requête EF Core 8 optimisée** : Réécris l'accès aux données en EF Core 8 (async, CancellationToken, AsNoTracking si lecture, projection Select sans N+1).
```

---

## 3. Sécuriser une modification Entity Framework Code First
*À utiliser chaque fois que vous créez ou modifiez une entité ou une requête EF.*

```markdown
Agis comme un expert Entity Framework Code First et SQL Server.
Examine mes entités, ma configuration Fluent API et mes requêtes LINQ ci-dessous :
"""
[COLLER ENTITÉ + CONFIGURATION + REQUÊTE LINQ]
"""

Vérifie les 5 points suivants :
1. **Schéma SQL généré** : Y a-t-il des colonnes `nvarchar(max)` oubliées, des décimaux sans précision, ou des suppressions en cascade (`Cascade Delete`) dangereuses ?
2. **Performance & N+1** : Cette requête LINQ risque-t-elle de déclencher des requêtes N+1 ou de charger trop de données en mémoire (évaluation client) ?
3. **Indexation** : Quels index (`HasIndex`) dois-je déclarer en Code First pour que les filtres (`Where`), tris (`OrderBy`) et jointures soient rapides ?
4. **Sécurité de la Migration** : Si je génère une migration sur une base de production existante contenant déjà des données, est-ce que cette modification risque d'échouer (ex: colonne non-nullable sans valeur par défaut) ?
5. Donne le code corrigé si nécessaire.
```

---

## 4. Le « Conseil des 4 » avant une Pull Request (Architecte, Urbaniste, RSSI, Prod)
*Votre arme secrète avant de pousser votre branche sur Azure DevOps 2020.*

```markdown
Je m'apprête à créer une Pull Request sur Azure DevOps pour ce code.
Audite mes modifications selon les 4 points de vue de notre DSI :

1. **Point de vue RSSI (Sécurité)** : Y a-t-il une faille de sécurité (IDOR, sur-exposition d'entité EF sans DTO, défaut de validation des entrées, fuite d'information technique dans les erreurs ou les logs) ?
2. **Point de vue Urbaniste / Architecte** : Le contrat d'API est-il propre (codes HTTP, nommage, séparation des responsabilités, respect du modèle) ?
3. **Point de vue Production (Exploitation & Perf)** : Que se passe-t-il si la base de données ou l'API distante est lente ? Les `CancellationToken`, les timeouts, les `AsNoTracking()` et les logs structurés (`ILogger`) sont-ils bien en place ?
4. **Point de vue Équipe Dev (Maintenabilité)** : Le code est-il lisible et facile à tester ?

Ne signale que les vrais problèmes classés par criticité (Critique / Moyen / Mineur) avec le correctif exact.
```

---

## 5. Rédiger une Pull Request impeccable pour Azure DevOps 2020
*Pour faire gagner du temps à vos 3 collègues développeurs lors des revues de code.*

```markdown
À partir de mes modifications Git (ou du résumé de ce qu'on vient de coder), génère une description de Pull Request claire et concise pour Azure DevOps :
- 🎯 **Objectif fonctionnel** (1 à 2 phrases)
- 🛠️ **Modifications techniques principales** (API, Services, etc.)
- 🗄️ **Impact Base de données / EF Code First** (Aucune migration / Nouvelle migration à appliquer : préciser laquelle)
- ✅ **Vérifications effectuées** (Tests unitaires, cas limites testés, contrat Swagger)
```
