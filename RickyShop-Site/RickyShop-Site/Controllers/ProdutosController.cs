using RickyShop_Site.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Security.Permissions;
using Microsoft.Ajax.Utilities;
using System.Text.RegularExpressions;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Threading.Tasks;
using System.IO;
using System.Web.UI.WebControls;

namespace RickyShop_Site.Controllers
{
    public class ProdutosController : Controller
    {
        //GestãoRickyShopEntities Entities.db = new GestãoRickyShopEntities();

        // GET: Produtos
        #region Produtos
        public ActionResult ProdutoDetalhado(int id)
        {
            var a = Entities.db.Produto.Where(p => p.ID_Produto == id).ToList();

            return View(a);
        }
        public ActionResult ListaProdutos(int? pagina, string searchString, int id)
        {
            IPagedList<Produto> prodPage;
            int tamanhoPagina = Generic.ValSettings(Server.MapPath("~/FicheiroJson/SettingsRickyShop.json")).QtdProdutosPagina;
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
                var produtos = from s in Entities.db.Produto select s;

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
                                || s.MarcaProduto.Marca.ToUpper().Contains(searchString.ToUpper())).Where(c => c.Desconto != null && c.Descontinuado == 1);
                    }
                    else
                    {
                        if (id == -1)
                        {
                            produtos = produtos.Where(s => s.Nome.ToUpper().Contains(searchString.ToUpper())
                                    || s.MarcaProduto.Marca.ToUpper().Contains(searchString.ToUpper()));
                            produtos.ToList().ToPagedList(numeroPagina, tamanhoPagina).FirstOrDefault().ID_Categoria = -1;

                            prodPage = produtos.Where(s => s.Descontinuado == 1).ToList().ToPagedList(numeroPagina, tamanhoPagina);

                        }
                        else
                        {
                            produtos = produtos.Where(s => s.Nome.ToUpper().Contains(searchString.ToUpper())
                                     || s.MarcaProduto.Marca.ToUpper().Contains(searchString.ToUpper())).Where(c => c.ID_Categoria == id && c.Descontinuado == 1);
                        }
                        if (produtos.Count() == 0)
                        {
                            Response.Write($"<script>alert('Não existe nenhum produto pesquisado!')</script>");
                            return View(Entities.db.Produto.Where(s => s.ID_Categoria == id && s.Descontinuado == 1).ToList().ToPagedList(numeroPagina, tamanhoPagina));
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
                    prodPage = Entities.db.Produto.Where(s => s.Desconto != null && s.Descontinuado == 1).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                    prodPage.FirstOrDefault().ID_Categoria = 0;

                }//Categoria produtos com promoção
                else
                {
                    if (id != -1)
                        prodPage = produtos.Where(s => s.ID_Categoria == id && s.Descontinuado == 1).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                    else
                    {
                        produtos.Where(s => s.Descontinuado == 1).ToList().ToPagedList(numeroPagina, tamanhoPagina).FirstOrDefault().ID_Categoria = -1;
                        prodPage = produtos.ToList().ToPagedList(numeroPagina, tamanhoPagina);
                    }
                }
                return View(prodPage);
            }
            else
            {
                var produtos = from s in Entities.db.Produto select s;

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
                                || s.MarcaProduto.Marca.ToUpper().Contains(searchString.ToUpper())).Where(c => c.Desconto != null && c.Descontinuado == 1);
                    }
                    else
                    {
                        if (id != -1)
                        {
                            produtos = produtos.Where(s => s.Nome.ToUpper().Contains(searchString.ToUpper())
                            || s.MarcaProduto.Marca.ToUpper().Contains(searchString.ToUpper())).Where(s => s.Desconto != null && s.ID_Categoria == id && s.Descontinuado == 1);

                            prodPage = produtos.ToList().ToPagedList(numeroPagina, tamanhoPagina);
                        }
                        else
                        {
                            produtos = produtos.Where(s => s.Nome.ToUpper().Contains(searchString.ToUpper())
                            || s.MarcaProduto.Marca.ToUpper().Contains(searchString.ToUpper())).Where(s => s.Desconto != null && s.Descontinuado == 1);
                            produtos.ToList().ToPagedList(numeroPagina, tamanhoPagina).FirstOrDefault().ID_Categoria = -1;

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
                    prodPage = Entities.db.Produto.Where(s => s.Desconto != null && s.Descontinuado == 1).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                    prodPage.FirstOrDefault().ID_Categoria = 0;

                }//Categoria produtos com promoção
                else
                {
                    if (id != -1)
                        prodPage = produtos.Where(s => s.ID_Categoria == id && s.Descontinuado == 1).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                    else
                    {
                        prodPage = produtos.Where(s => s.Desconto != null && s.Descontinuado == 1).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                        prodPage.FirstOrDefault().ID_Categoria = -1;
                    }
                }

                return View(prodPage);
            }

        }

        [HttpPost]
        public ActionResult ListaProdutos(Produto p, int id)
        {
            IPagedList<Produto> prodPage;
            int tamanhoPagina = Generic.ValSettings(Server.MapPath("tamanhoPagina")).QtdProdutosPagina;
            // valor não pode ser nulo, caso seja ele fica a 1, como se fosse um if
            int numeroPagina = 1;

            if (p.ID_Marca == 0)
            {
                if (p.EstadoProm == true)
                {
                    Session["SoProm"] = "true";
                    if (Session["DetalhesPesquisa"] != null)
                    {
                        var produtos = from s in Entities.db.Produto select s;
                        string searchString = Session["DetalhesPesquisa"].ToString();


                        if (id != -1)
                        {
                            produtos = produtos.Where(s => s.Nome.ToUpper().Contains(searchString.ToUpper())
                            || s.MarcaProduto.Marca.ToUpper().Contains(searchString.ToUpper())).Where(s => s.Desconto != null && s.ID_Categoria == id);

                            prodPage = produtos.ToList().ToPagedList(numeroPagina, tamanhoPagina);
                        }
                        else
                        {
                            produtos = produtos.Where(s => s.Nome.ToUpper().Contains(searchString.ToUpper())
                            || s.MarcaProduto.Marca.ToUpper().Contains(searchString.ToUpper())).Where(s => s.Desconto != null);
                            produtos.ToList().ToPagedList(numeroPagina, tamanhoPagina).FirstOrDefault().ID_Categoria = -1;
                            prodPage = produtos.ToList().ToPagedList(numeroPagina, tamanhoPagina);
                        }
                    }
                    else
                    {
                        if (id != -1)
                        {
                            prodPage = Entities.db.Produto.Where(s => s.Desconto != null && s.ID_Categoria == id).ToList().ToPagedList(numeroPagina, tamanhoPagina);

                        }
                        else
                        {

                            prodPage = Entities.db.Produto.Where(s => s.Desconto != null).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                            prodPage.FirstOrDefault().ID_Categoria = -1;
                        }
                    }

                }
                else
                {
                    Response.Write($"<script>alert('Selecione uma Marca!')</script>");
                    prodPage = Entities.db.Produto.ToList().ToPagedList(numeroPagina, tamanhoPagina);

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
                prodPage = Entities.db.Produto.Where(s => s.Desconto != null && s.ID_Marca == p.ID_Marca).ToList().ToPagedList(numeroPagina, tamanhoPagina);
            }
            else
            {
                if (p.EstadoProm == true)
                {
                    if (Entities.db.Produto.Where(s => s.ID_Marca == p.ID_Marca && s.Desconto != null).Count() == 0)
                    {
                        Response.Write($"<script>alert('Não existe promoções nesta marca.')</script>");
                        prodPage = Entities.db.Produto.Where(s => s.ID_Marca == p.ID_Marca && s.ID_Categoria == id).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                    }
                    else
                        prodPage = Entities.db.Produto.Where(s => s.ID_Marca == p.ID_Marca && s.Desconto != null && s.ID_Categoria == id).ToList().ToPagedList(numeroPagina, tamanhoPagina);

                }
                else
                    prodPage = Entities.db.Produto.Where(s => s.ID_Marca == p.ID_Marca && s.ID_Categoria == id).ToList().ToPagedList(numeroPagina, tamanhoPagina);

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
                return RedirectToAction("ListaProdutos", "Produtos", new { id = Entities.db.Categoria.Where(s => s.NomeCategoria == "Todos").FirstOrDefault().ID_Categoria, searchString = searchString });
            }

            Response.Write($"<script>alert('Não existe nenhum produto pesquisado!')</script>");
            return RedirectToAction("Inicio", "Home");
        }

        #endregion



        public ActionResult FecharCompra(string locEntrega, string codPostal)
        {
            List<DadosCarrinhoProduto_Result> prod = new List<DadosCarrinhoProduto_Result>();
            int UserID = Convert.ToInt32(Session["UserID"]);
            Regex regex = new Regex(@"^\d{4}(-\d{3})?$");

            if (regex.IsMatch(codPostal) == true && locEntrega != "")
            {
                var u = Entities.db.Utilizadores.Where(s => s.ID_Utilizador == UserID).FirstOrDefault();
                //Fechar compra, mas verificar se o user tem saldo, se não tiver saldo mostrar mensagem de aviso

                double saldo = Convert.ToDouble(u.Saldo);
                double total = Convert.ToDouble(Generic.PrecoTotal(UserID));

                if (saldo >= total)
                {
                    Pedidos p = new Pedidos();
                    p.ID_Utilizador = UserID;
                    p.DataDoPedido = DateTime.Now;
                    p.DataDeEntrega = DateTime.Now.AddDays(7);
                    p.MoradaEntrega = locEntrega;
                    p.CodigoPostal = codPostal;
                    p.PreçoTotal = Generic.PrecoTotal(UserID);

                    p.ID_Estado = 0;
                    p.Email = u.Email;
                    p.Contacto = u.Contacto;
                    Entities.db.Pedidos.Add(p);
                    u.Desconto = null;
                    Entities.db.SaveChanges();
                }
                else
                {
                    //Dá erro, saldo insoficiente
                    return null;
                }


            }
            else
            {
                //Ou o codigo postal está inválido ou não meteu nada no local de entrega
                prod = Entities.db.DadosCarrinhoProduto(UserID).ToList();
                return View(prod);
            }


            int ultmPed = Entities.db.Pedidos.ToList().Last(s => s.ID_Utilizador == UserID).ID_Pedido;
            //Criar função para devolver o total


            List<PedidosDetalhes> dLista = new List<PedidosDetalhes>();
            foreach (var item in Entities.db.DadosCarrinhoProduto(UserID))
            {
                PedidosDetalhes pd = new PedidosDetalhes();
                pd.ID_Pedido = ultmPed;
                pd.ID_Produto = item.ID_Produto;
                pd.Quantidade = item.Quantidade;
                if (item.Desconto == null)
                    pd.Preco = item.PreçoPorQuantidade;
                else
                {
                    decimal valDesconto = Convert.ToDecimal(item.PreçoPorQuantidade * item.Desconto) / 100;
                    decimal preco = item.PreçoPorQuantidade - valDesconto;
                    pd.Preco = preco;
                }

                dLista.Add(pd);
            }



            Entities.db.PedidosDetalhes.AddRange(dLista);
            prod = Entities.db.DadosCarrinhoProduto(UserID).ToList();

            var c = Entities.db.Carrinho.Where(s => s.ID_Utilizador == UserID).ToList();
            Entities.db.Carrinho.RemoveRange(c);
            Entities.db.SaveChanges();
            return View(prod);
        }

    }
}