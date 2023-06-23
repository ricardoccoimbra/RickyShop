using RickyShop_Site.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace RickyShop_Site.Controllers
{
    public class UtilizadorController : Controller
    {
        //GestãoRickyShopEntities Entities.db = new GestãoRickyShopEntities();


        // GET: Utilizador
        public ActionResult Perfil()
        {
            int UserID = Convert.ToInt32(Session["UserID"]);
            var u = Entities.db.Utilizadores.Where(s => s.ID_Utilizador == UserID).FirstOrDefault();
            return View(u);
        }

        public ActionResult AtualizarPerfil(string morada, string codPostal, string contacto)
        {
            int UserID = Convert.ToInt32(Session["UserID"]);
            Regex regexPostal = new Regex(@"^\d{4}(-\d{3})?$");
            Regex regexTelem = new Regex(@"^[0-9]{9}$");

            Utilizadores dUser = Entities.db.Utilizadores.FirstOrDefault(s => s.ID_Utilizador == UserID);

            if (regexPostal.IsMatch(codPostal) == true && morada != "" && regexTelem.IsMatch(contacto) == true)
            {
                dUser.Contacto = contacto;
                dUser.CodigoPostal = codPostal;
                dUser.Morada = morada;

                Entities.db.SaveChangesAsync();

                return RedirectToAction("Perfil", "Utilizador", UserID);
            }
            return View();
        }
        public ActionResult AlterarPassword(string password, string NovaPassword, string ConfNovaPassword)
        {
            int UserID = Convert.ToInt32(Session["UserID"]);
            Utilizadores user = Entities.db.Utilizadores.FirstOrDefault(s => s.ID_Utilizador == UserID);
            if (password != "" && NovaPassword != "" && ConfNovaPassword != "")
            {
                if (Generic.CompararPassHash(password, user.PassWord) == true)
                {
                    if (NovaPassword == ConfNovaPassword)
                    {
                        NovaPassword = Generic.CriarPassHash(NovaPassword);
                        user.PassWord = NovaPassword;
                        Entities.db.SaveChangesAsync();
                        return RedirectToAction("Perfil", "Utilizador", user.ID_Utilizador);
                    }
                    else
                        Response.Write($"<script>alert('Password não identicas!')</script>");
                }
                else
                    Response.Write($"<script>alert('Password de user inválida!')</script>");

            }
            else
                Response.Write($"<script>alert('Preencha os campos!')</script>");

            return RedirectToAction("Perfil", "Utilizador", user.ID_Utilizador);

        }
        public ActionResult DepositarSaldo(string nomeTitular, string NumCartao, string CVV, string Validade, int Saldo)
        {
            Regex regexCartao = new Regex(@"^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14})$");
            Regex regexValidade = new Regex(@"^(0[1-9]|1[0-2])\/(2[3-9]|[3-9][0-9])$");
            Regex regexCVV = new Regex(@"^[0-9]{3}$");

            int UserID = Convert.ToInt32(Session["UserID"]);

            if (regexCartao.IsMatch(NumCartao) && regexCVV.IsMatch(CVV) && regexValidade.IsMatch(Validade) && nomeTitular != "")
            {
                MovimentacaoSaldo mv = new MovimentacaoSaldo();
                var u = Entities.db.Utilizadores.Where(s => s.ID_Utilizador == UserID).FirstOrDefault();
                mv.NumeroCartao = NumCartao;
                mv.ID_Utilizador = UserID;
                mv.Tipo = "+";
                mv.Quantidade = Saldo;

                u.Saldo += Saldo; 


                Entities.db.MovimentacaoSaldo.Add(mv);
                Entities.db.SaveChangesAsync();

                return RedirectToAction("Perfil", "Utilizador", UserID);
            }

            return RedirectToAction("Perfil", "Utilizador", UserID);

        }

        public ActionResult HistoricoCompras(int id)
        {
            var prod = Entities.db.Pedidos.Where(s => s.ID_Utilizador == id).ToList();

            return View(prod);
        }
        public ActionResult Details(int id)
        {
            // Faça qualquer processamento necessário com o ID

            // Retorne a exibição do modal
            var p = Entities.db.PedidosDetalhes.Where(s => s.ID_Pedido == id).ToList();
            return PartialView("Details", p);
        }

        public ActionResult AddProdFavoritos(int idP, int idC)
        {
            ProdutosFavoritos f = new ProdutosFavoritos();
            int userID = Convert.ToInt32(Session["UserID"]);
            if (Entities.db.ProdutosFavoritos.Count(s => s.ID_Produto == idP && s.ID_Utilizador == userID) == 0)
            {
                var p = Entities.db.Produto.Where(s => s.ID_Produto == idP).FirstOrDefault();

                f.ID_Produto = idP;
                f.ID_Utilizador = userID;
                Entities.db.ProdutosFavoritos.Add(f);
                Entities.db.SaveChangesAsync();
            }
            return RedirectToAction("ListaProdutos", "Produtos", new { id = idC });
        }
        public ActionResult ApagarProdFavorito(int idProd)
        {
            var p = Entities.db.ProdutosFavoritos.Where(s => s.ID_Produto == idProd && s.ID_Utilizador == 2).FirstOrDefault();
            Entities.db.ProdutosFavoritos.Remove(p);
            Entities.db.SaveChangesAsync();
            return RedirectToAction("ProdutosFavoritos", "Utilizador");
        }
        public ActionResult ProdutosFavoritos()
        {
            var p = Entities.db.DadosProdutosFavoritos(2);
            return View(p.ToList());
        }


        #region Carrinho
        public ActionResult CarrinhoProdutos(int id)
        {
            var prod = Entities.db.DadosCarrinhoProduto(id).ToList();

            return View(prod);
        }
        public ActionResult ProdCarrinho(Carrinho c, int idP, int idC)
        {
            int userID = Convert.ToInt32(Session["UserID"]);
            if (Entities.db.Carrinho.Count(s => s.ID_Produto == idP && s.ID_Utilizador == userID) == 0)
            {
                var p = Entities.db.Produto.Where(s => s.ID_Produto == idP).FirstOrDefault();

                c.ID_Produto = idP;
                c.PrecoProduto = p.PreçoPorQuantidade;
                c.ID_Utilizador = userID;
                c.Quantidade = 1;
                c.Tamanho = "M";
                Entities.db.Carrinho.Add(c);
                Entities.db.SaveChangesAsync();
            }
            return RedirectToAction("ListaProdutos", "Produtos", new { id = idC });
        }
        public ActionResult EliminarProdCarrinho(int id)
        {
            int UserID = Convert.ToInt32(Session["UserID"]);
            var p = Entities.db.Carrinho.Where(s => s.ID_Utilizador == UserID && s.ID_Produto == id).FirstOrDefault();
            Entities.db.Carrinho.Remove(p);
            Entities.db.SaveChangesAsync();

            return RedirectToAction("CarrinhoProdutos", new { id = UserID });
        }
        public ActionResult SomaProd(int id)
        {
            var p = Entities.db.Produto.FirstOrDefault(s => s.ID_Produto == id);
            int UserID = Convert.ToInt32(Session["UserID"]);
            var pc = Entities.db.Carrinho.Where(s => s.ID_Produto == id && s.ID_Utilizador == UserID).FirstOrDefault();

            if (p.QuantidadeStock >= pc.Quantidade)
            {
                pc.Quantidade++;
            }
            else
            { /*deu erro*/ }
            Entities.db.SaveChanges();
            return RedirectToAction("CarrinhoProdutos", new { id = UserID });

        }
        public ActionResult DiminuiProd(int id)
        {
            int UserID = Convert.ToInt32(Session["UserID"]);
            var p = Entities.db.Carrinho.Where(s => s.ID_Produto == id && s.ID_Utilizador == UserID).FirstOrDefault();
            if (p.Quantidade == 1)
            {
                Entities.db.Carrinho.Remove(p);
                Entities.db.Carrinho.Remove(p);
            }
            else
            {
                p.Quantidade--;
            }
            //Entities.db.SaveChangesAsync();
            Entities.db.SaveChanges();
            return RedirectToAction("CarrinhoProdutos", new { id = UserID });
        }
        #endregion
    }
}