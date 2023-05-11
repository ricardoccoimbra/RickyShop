﻿using RickyShop_Site.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Security.Permissions;
using Microsoft.Ajax.Utilities;

namespace RickyShop_Site.Controllers
{
    public class ProdutosController : Controller
    {
        GestãoRickyShopEntities db = new GestãoRickyShopEntities();

        // GET: Produtos
        #region Produtos
        public ActionResult ProdutoDetalhado(int id)
        {
            var a = db.Produto.Where(p => p.ID_Produto == id).ToList();

            return View(a);
        }
        public ActionResult ListaProdutos(int? pagina, string searchString, int id)
        {
            IPagedList<Produto> prodPage;
            int tamanhoPagina = 6;
            int numeroPagina = pagina ?? 1;  // valor não pode ser nulo, caso seja ele fica a 1, como se fosse um if


            if (Session["ID"] == null)
            {
                Session["ID"] = id;
            }
            else
            {
                if (Convert.ToInt32(Session["ID"]) != id)
                {
                    Session["ID"] = id;
                    Session.Remove("Filtro");
                    Session.Remove("DetalhesPesquisa");
                    Session.Remove("SoProm");

                }
            }


            if (Session["SoProm"] == null)
            {
                var produtos = from s in db.Produto select s;

                if (Session["DetalhesPesquisa"] != null)
                {
                    if (searchString == null)
                        searchString = Session["DetalhesPesquisa"].ToString();
                }

                if (!String.IsNullOrEmpty(searchString))
                {
                    Session["DetalhesPesquisa"] = searchString;
                    if (id == 0)
                    {
                        produtos = produtos.Where(s => s.Nome.ToUpper().Contains(searchString.ToUpper())
                                || s.MarcaProduto.Marca.ToUpper().Contains(searchString.ToUpper())).Where(c => c.Desconto != null);
                    }
                    else
                    {
                        if (id == 4)
                        {
                            produtos = produtos.Where(s => s.Nome.ToUpper().Contains(searchString.ToUpper())
                                    || s.MarcaProduto.Marca.ToUpper().Contains(searchString.ToUpper()));
                            produtos.ToList().ToPagedList(numeroPagina, tamanhoPagina).FirstOrDefault().ID_Categoria = 4;

                            prodPage = produtos.ToList().ToPagedList(numeroPagina, tamanhoPagina);

                        }
                        else
                        {
                            produtos = produtos.Where(s => s.Nome.ToUpper().Contains(searchString.ToUpper())
                                     || s.MarcaProduto.Marca.ToUpper().Contains(searchString.ToUpper())).Where(c => c.ID_Categoria == id);
                        }
                        if (produtos.Count() == 0)
                        {
                            Response.Write($"<script>alert('Não existe nenhum produto pesquisado!')</script>");
                            return View(db.Produto.Where(s => s.ID_Categoria == id).ToList().ToPagedList(numeroPagina, tamanhoPagina));
                        }
                    }
                }//Procurar produtos consoante a pesquisa do user

                //Filtro:
                if (Session["Filtro"] != null)
                {
                    //Função que verifica os filtros ativos
                    var p = Generic.Filtros(Session["Filtro"].ToString(), id, produtos, pagina, numeroPagina, tamanhoPagina);
                    return View(p);
                }//Verificar Filtros

                if (id == 0)
                {
                    prodPage = db.Produto.Where(s => s.Desconto != null).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                    prodPage.FirstOrDefault().ID_Categoria = 0;

                }//Categoria produtos com promoção
                else
                {
                    if (id != 4)
                        prodPage = produtos.Where(s => s.ID_Categoria == id).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                    else
                    {
                        produtos.ToList().ToPagedList(numeroPagina, tamanhoPagina).FirstOrDefault().ID_Categoria = 4;
                        prodPage = produtos.ToList().ToPagedList(numeroPagina, tamanhoPagina);
                    }
                }
                return View(prodPage);
            }
            else
            {
                var produtos = from s in db.Produto select s;

                if (Session["DetalhesPesquisa"] != null)
                {
                    if (searchString == null)
                        searchString = Session["DetalhesPesquisa"].ToString();
                }

                if (!String.IsNullOrEmpty(searchString))
                {
                    Session["DetalhesPesquisa"] = searchString;

                    if (id == 0)
                    {
                        produtos = produtos.Where(s => s.Nome.ToUpper().Contains(searchString.ToUpper())
                                || s.MarcaProduto.Marca.ToUpper().Contains(searchString.ToUpper())).Where(c => c.Desconto != null);
                    }
                    else
                    {
                        if (id != 4)
                        {
                            produtos = produtos.Where(s => s.Nome.ToUpper().Contains(searchString.ToUpper())
                            || s.MarcaProduto.Marca.ToUpper().Contains(searchString.ToUpper())).Where(s => s.Desconto != null && s.ID_Categoria == id);

                            prodPage = produtos.ToList().ToPagedList(numeroPagina, tamanhoPagina);
                        }
                        else
                        {
                            produtos = produtos.Where(s => s.Nome.ToUpper().Contains(searchString.ToUpper())
                            || s.MarcaProduto.Marca.ToUpper().Contains(searchString.ToUpper())).Where(s => s.Desconto != null);
                            produtos.ToList().ToPagedList(numeroPagina, tamanhoPagina).FirstOrDefault().ID_Categoria = 4;

                            prodPage = produtos.ToList().ToPagedList(numeroPagina, tamanhoPagina);
                        }
                    }
                }

                if (Session["Filtro"] != null)
                {
                    //Função que verifica os filtros ativos
                    var p = Generic.Filtros(Session["Filtro"].ToString(), id, produtos, pagina, numeroPagina, tamanhoPagina);
                    return View(p);
                }//Verificar Filtros

                if (id == 0)
                {
                    prodPage = db.Produto.Where(s => s.Desconto != null).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                    prodPage.FirstOrDefault().ID_Categoria = 0;

                }//Categoria produtos com promoção
                else
                {
                    if (id != 4)
                        prodPage = produtos.Where(s => s.ID_Categoria == id).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                    else
                    {
                        prodPage = produtos.Where(s => s.Desconto != null).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                        prodPage.FirstOrDefault().ID_Categoria = 4;
                    }
                }

                return View(prodPage);
            }

        }

        [HttpPost]
        public ActionResult ListaProdutos(Produto p, int id)
        {
            IPagedList<Produto> prodPage;
            int tamanhoPagina = 6;
            // valor não pode ser nulo, caso seja ele fica a 1, como se fosse um if
            int numeroPagina = 1;

            if (p.ID_Marca == 0)
            {
                if (p.EstadoProm == true)
                {
                    Session["SoProm"] = "true";
                    if (Session["DetalhesPesquisa"] != null)
                    {
                        var produtos = from s in db.Produto select s;
                        string searchString = Session["DetalhesPesquisa"].ToString();


                        if (id != 4)
                        {
                            produtos = produtos.Where(s => s.Nome.ToUpper().Contains(searchString.ToUpper())
                            || s.MarcaProduto.Marca.ToUpper().Contains(searchString.ToUpper())).Where(s => s.Desconto != null && s.ID_Categoria == id);

                            prodPage = produtos.ToList().ToPagedList(numeroPagina, tamanhoPagina);
                        }
                        else
                        {
                            produtos = produtos.Where(s => s.Nome.ToUpper().Contains(searchString.ToUpper())
                            || s.MarcaProduto.Marca.ToUpper().Contains(searchString.ToUpper())).Where(s => s.Desconto != null);
                            produtos.ToList().ToPagedList(numeroPagina, tamanhoPagina).FirstOrDefault().ID_Categoria = 4;
                            prodPage = produtos.ToList().ToPagedList(numeroPagina, tamanhoPagina);
                        }
                    }
                    else
                    {
                        if (id != 4)
                        {
                            prodPage = db.Produto.Where(s => s.Desconto != null && s.ID_Categoria == id).ToList().ToPagedList(numeroPagina, tamanhoPagina);

                        }
                        else
                        {

                            prodPage = db.Produto.Where(s => s.Desconto != null).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                            prodPage.FirstOrDefault().ID_Categoria = 4;
                        }
                    }

                }
                else
                {
                    Response.Write($"<script>alert('Selecione uma Marca!')</script>");
                    prodPage = db.Produto.ToList().ToPagedList(numeroPagina, tamanhoPagina);

                }
                return View(prodPage);
            }
            else
            {
                if (Session["Filtro"] == null)
                {
                    Session["Filtro"] = p.ID_Marca + "-nada";
                }
                else
                {
                    var aux1 = p.ID_Marca;
                    var aux2 = Session["Filtro"].ToString().Split('-')[1];
                    Session.Remove("Filtro");
                    Session["Filtro"] = aux1 + "-" + aux2;
                }
            }




            if (id == 0)
            {
                prodPage = db.Produto.Where(s => s.Desconto != null && s.ID_Marca == p.ID_Marca).ToList().ToPagedList(numeroPagina, tamanhoPagina);
            }
            else
            {
                if (p.EstadoProm == true)
                {
                    if (db.Produto.Where(s => s.ID_Marca == p.ID_Marca && s.Desconto != null).Count() == 0)
                    {
                        Response.Write($"<script>alert('Não existe promoções nesta marca.')</script>");
                        prodPage = db.Produto.Where(s => s.ID_Marca == p.ID_Marca && s.ID_Categoria == id).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                    }
                    else
                        prodPage = db.Produto.Where(s => s.ID_Marca == p.ID_Marca && s.Desconto != null && s.ID_Categoria == id).ToList().ToPagedList(numeroPagina, tamanhoPagina);

                }
                else
                    prodPage = db.Produto.Where(s => s.ID_Marca == p.ID_Marca && s.ID_Categoria == id).ToList().ToPagedList(numeroPagina, tamanhoPagina);

            }


            prodPage.FirstOrDefault().ID_Categoria = id;
            return View(prodPage);
        }
        public ActionResult LimparFiltro(int id)
        {
            Session.Remove("Filtro");
            Session.Remove("DetalhesPesquisa");
            Session.Remove("SoProm");
            return RedirectToAction("ListaProdutos", new { id });
        }
        public ActionResult OrdemPrecos(int? pagina, string tipo, int id)
        {
            //variaveis auxiliares
            string aux1, aux2;

            if (tipo == "PrecoCres")
            {
                if (Session["Filtro"] == null)
                {
                    Session["Filtro"] = "nada-" + tipo;
                }
                else
                {
                    aux1 = Session["Filtro"].ToString().Split('-')[0];
                    aux2 = tipo;
                    Session.Remove("Filtro");
                    Session["Filtro"] = aux1 + "-" + aux2;
                }

            }
            else
            {
                if (Session["Filtro"] == null)
                {
                    Session["Filtro"] = "nada-" + tipo;
                }
                else
                {
                    aux1 = Session["Filtro"].ToString().Split('-')[0];
                    aux2 = tipo;
                    Session.Remove("Filtro");
                    Session["Filtro"] = aux1 + "-" + aux2;
                }
            }
            return RedirectToAction("ListaProdutos", new { id });
        }
        public ActionResult PesquisarProduto(string searchString)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                return RedirectToAction("ListaProdutos", "Produtos", new { id = db.Categoria.Where(s => s.NomeCategoria == "Todos").FirstOrDefault().ID_Categoria, searchString = searchString });
            }

            Response.Write($"<script>alert('Não existe nenhum produto pesquisado!')</script>");
            return RedirectToAction("Inicio", "Home");
        }
        #endregion

        public ActionResult ProdFavorito(Carrinho c, int idP, int idC)
        {
            int userID = Convert.ToInt32(Session["UserID"]);
            if (db.Carrinho.Count(s => s.ID_Produto == idP && s.ID_Utilizador == userID) == 0)
            {
                var p = db.Produto.Where(s => s.ID_Produto == idP).FirstOrDefault();

                c.ID_Produto = idP;
                c.PrecoProduto = p.PreçoPorQuantidade;
                c.ID_Utilizador = userID;
                c.Quantidade = 1;
                c.Tamanho = "M";
                db.Carrinho.Add(c);
                db.SaveChangesAsync();
            }
            return RedirectToAction("ListaProdutos", new { id = idC });
        }

        #region Carrinho
        public ActionResult CarrinhoProdutos(int id)
        {
            var prod = db.DadosCarrinhoProduto(id).ToList();

            return View(prod);
        }
        public ActionResult ProdCarrinho(Carrinho c, int idP, int idC)
        {
            int userID = Convert.ToInt32(Session["UserID"]);
            if (db.Carrinho.Count(s => s.ID_Produto == idP && s.ID_Utilizador == userID) == 0)
            {
                var p = db.Produto.Where(s => s.ID_Produto == idP).FirstOrDefault();

                c.ID_Produto = idP;
                c.PrecoProduto = p.PreçoPorQuantidade;
                c.ID_Utilizador = userID;
                c.Quantidade = 1;
                c.Tamanho = "M";
                db.Carrinho.Add(c);
                db.SaveChangesAsync();
            }
            return RedirectToAction("ListaProdutos", new { id = idC });
        }
        public ActionResult EliminarProdCarrinho(int id)
        {
            int UserID = Convert.ToInt32(Session["UserID"]);
            var p = db.Carrinho.Where(s =>  s.ID_Utilizador == UserID && s.ID_Produto == id).FirstOrDefault();
            db.Carrinho.Remove(p);
            db.SaveChangesAsync();

            return RedirectToAction("CarrinhoProdutos", new { id = UserID });
        }
        public ActionResult SomaProd(int id)
        {
            int UserID = Convert.ToInt32(Session["UserID"]);
            var p = db.Carrinho.Where(s => s.ID_Produto == id && s.ID_Utilizador == UserID).FirstOrDefault();
            p.Quantidade++;
            db.SaveChangesAsync();

            return RedirectToAction("CarrinhoProdutos", new { id = UserID });
        }
        public ActionResult DiminuiProd(int id)
        {
            int UserID = Convert.ToInt32(Session["UserID"]);
            var p = db.Carrinho.Where(s => s.ID_Produto == id && s.ID_Utilizador == UserID).FirstOrDefault();
            if (p.Quantidade == 1)
            {
                db.Carrinho.Remove(p);
            }
            else
            {
                p.Quantidade--;
            }
            db.SaveChangesAsync();

            return RedirectToAction("CarrinhoProdutos", new { id = UserID });
        }
        #endregion


        public ActionResult Teste()
        {
            return View();
        }

    }
}