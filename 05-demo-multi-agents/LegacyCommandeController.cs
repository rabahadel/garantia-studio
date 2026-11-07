using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace LegacyApp.Controllers
{
    // Exemple typique d'un contrôleur Legacy .NET Framework 4.x / ASP.NET MVC / EF6 Code First
    // à moderniser vers une Web API .NET 8 selon le Schéma Directeur
    public class CommandeController : Controller
    {
        private AppDbContext _db = new AppDbContext();

        [HttpGet]
        public ActionResult GetCommandesClient(int clientId)
        {
            // 1. Aucun contrôle que le clientId demandé appartient à l'utilisateur connecté (IDOR)
            var commandes = _db.Commandes
                .Where(c => c.ClientId == clientId)
                .ToList();

            // 2. Problème N+1 EF6 dans une boucle + calcul métier dans le contrôleur
            foreach (var cmd in commandes)
            {
                var lignes = _db.LignesCommande.Where(l => l.CommandeId == cmd.Id).ToList();
                cmd.MontantTotal = lignes.Sum(l => l.PrixUnitaire * l.Quantite);
            }

            // 3. Exposition directe de l'entité EF Code First en JSON
            return Json(commandes, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ValiderCommande(Commande input)
        {
            // 4. Mass Assignment sur l'entité EF + appel HTTP externe synchrone sans timeout
            _db.Entry(input).State = EntityState.Modified;
            _db.SaveChanges();

            return new HttpStatusCodeResult(200);
        }
    }

    public class Commande
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string CodePromo { get; set; } // Pas de MaxLength -> nvarchar(max)
        public decimal MontantTotal { get; set; }
        public bool EstValidee { get; set; }
        public string CommentaireInterne { get; set; }
    }
}
