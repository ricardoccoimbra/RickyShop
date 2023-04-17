using RickyShop_Site.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace RickyShop_Site.Controllers
{
    public class ProdutosController : Controller
    {
        GestãoRickyShopEntities db = new GestãoRickyShopEntities();

        // GET: Produtos
        public ActionResult ProdutoDetalhado(int id)
        {
            var a = db.Produto.Where(p => p.ID_Produto == id).ToList();

            return View(a);
        }


        public ActionResult ListaProdutos(int? pagina, string searchString)
        {
            int tamanhoPagina = 6;
            IPagedList<Produto> prodPage;
            var produtos = from s in db.Produto select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                produtos = produtos.Where(s => s.Nome.ToUpper().Contains(searchString.ToUpper())
                                            || s.MarcaProduto.Marca.ToUpper().Contains(searchString.ToUpper()));
            }

            // valor não pode ser nulo, caso seja ele fica a 1, como se fosse um if
            int numeroPagina = pagina ?? 1;

            //Para fazer do mais caro => barato com orderby
            if (Session["Filtro"] != null)
            {
                int id = Convert.ToInt32(Session["Filtro"]);

                //Para fazer do mais caro => barato com orderby
                 prodPage = db.Produto.Where(s => s.ID_Marca == id).ToList().ToPagedList(numeroPagina, tamanhoPagina);

                return View(prodPage);
            }
            else
            {
                 prodPage = produtos.ToList().ToPagedList(numeroPagina, tamanhoPagina);

                return View(prodPage);
            }
        }

        [HttpPost]
        public ActionResult ListaProdutos(int? pagina, Produto p)
        {
            int id = 0;
            Session["Filtro"] = p.ID_Marca.ToString();
            id = p.ID_Marca;

            int tamanhoPagina = 6;

            // valor não pode ser nulo, caso seja ele fica a 1, como se fosse um if
            int numeroPagina = pagina ?? 1;

            //Para fazer do mais caro => barato com orderby
            var prodPage = db.Produto.Where(s => s.ID_Marca == id).ToList().ToPagedList(numeroPagina, tamanhoPagina);

            return View(prodPage);
        }


        public ActionResult LimparFiltro()
        {
            Session.Remove("Filtro");
            return RedirectToAction("ListaProdutos");
        }


    }
}