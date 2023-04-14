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

 
        public ActionResult ListaProdutos(int? pagina)
        {
                int tamanhoPagina = 6;
                 
                // valor não pode ser nulo, caso seja ele fica a 1, como se fosse um if
                int numeroPagina = pagina ?? 1;

                //Para fazer do mais caro => barato com orderby
                var prodPage = db.Produto.OrderBy(a => a.ID_Produto).ToPagedList(numeroPagina, tamanhoPagina);
                return View(prodPage);
        }

        [HttpPost]
        public ActionResult ListaProdutos(int? pagina, Produto P)
        {
            int tamanhoPagina = 6;

            // valor não pode ser nulo, caso seja ele fica a 1, como se fosse um if
            int numeroPagina = pagina ?? 1;

            //Para fazer do mais caro => barato com orderby
            var prodPage = db.Produto.OrderBy(a => a.ID_Produto).ToPagedList(numeroPagina, tamanhoPagina);
            return View(prodPage);
        }

        public ActionResult FiltrarProdutos(int? pagina, int id)
        {
            int tamanhoPagina = 6;

            // valor não pode ser nulo, caso seja ele fica a 1, como se fosse um if
            int numeroPagina = pagina ?? 1;

            //Para fazer do mais caro => barato com orderby
            var prodPage = db.Produto.Where(a => a.ID_Marca == id).ToList().ToPagedList(numeroPagina, tamanhoPagina);
            return View("ListaProdutos", prodPage);
        }


    }
}