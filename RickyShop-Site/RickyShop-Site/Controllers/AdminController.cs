using PagedList;
using RickyShop_Site.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RickyShop_Site.Controllers
{
    public class AdminController : Controller
    {
        GestãoRickyShopEntities db = new GestãoRickyShopEntities();

        // GET: Admin
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Produtos(int? pagina)
        {
            IPagedList<Produto> p;
            int tamanhoPagina = 10;
            int numeroPagina = pagina ?? 1;

            if (Session["FiltroProd"] == null)
            { p = db.Produto.ToList().ToPagedList(numeroPagina, tamanhoPagina); }
            else
            {
                int marca = Convert.ToInt32(Session["FiltroProd"]);
                if (marca != -1)
                    p = db.Produto.Where(s => s.ID_Marca == marca).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                else
                    p = db.Produto.ToList().ToPagedList(numeroPagina, tamanhoPagina);
            }
            return View(p);
        }

        public ActionResult ProdutosDetails(int id)
        {
            // Retorne a exibição do modal
            var p = db.Produto.Where(s => s.ID_Produto == id).ToList();
            return PartialView("ProdutosDetails", p);
        }

        public ActionResult CriarProduto()
        {
            return PartialView("CriarProduto");
        }

        [HttpPost]
        public ActionResult CriarProduto(Produto p)
        {
            return PartialView("CriarProduto");
        }

        public ActionResult FiltroProdutos(int marca)
        {
            Session["FiltroProd"] = marca;
            return RedirectToAction("Produtos", "Admin");
        }
    }
}