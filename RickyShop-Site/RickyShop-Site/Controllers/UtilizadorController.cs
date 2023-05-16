using RickyShop_Site.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RickyShop_Site.Controllers
{
    public class UtilizadorController : Controller
    {
        GestãoRickyShopEntities db = new GestãoRickyShopEntities();


        // GET: Utilizador
        public ActionResult Perfil()
        {
            var u = db.Utilizadores.Where(s => s.ID_Utilizador == 2).FirstOrDefault();
            return View(u);
        }
    }
}