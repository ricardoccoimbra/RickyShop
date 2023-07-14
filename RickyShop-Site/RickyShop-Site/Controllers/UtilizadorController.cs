using RickyShop_Site.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
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
            if (Session["UserID"] != null)
            {
                if (TempData["AddSaldo"] != null)
                {
                    if (TempData["AddSaldo"].ToString() == "true")
                    {
                        Response.Write($"<script>alert('Saldo adicionado.')</script>");
                    }
                    else
                    {
                        Response.Write($"<script>alert('Dados inválidos!')</script>");
                    }
                }

                if (TempData["PassInvalida"] != null)
                {
                    Response.Write($"<script>alert('Password inválida!')</script>");
                }


                if (TempData["AlterarPass"] != null)
                {
                    if (TempData["AlterarPass"].ToString() == "true")
                    {
                        Response.Write($"<script>alert('Password alterada!')</script>");
                    }
                    else
                    {
                        if (TempData["AlterarPass"].ToString() == "NaoIguais")
                        {
                            Response.Write($"<script>alert('Passwords não iguais!')</script>");
                        }
                        else
                            Response.Write($"<script>alert('Password de User inválida!')</script>");
                    }
                }

                if (TempData["AlterarDados"] != null)
                {
                    if (TempData["AlterarDados"].ToString() == "true")
                    {
                        Response.Write($"<script>alert('Dados alterados com sucesso!')</script>");
                    }
                    else
                    {
                        Response.Write($"<script>alert('Dados inválidos!')</script>");
                    }
                }

                int UserID = Convert.ToInt32(Session["UserID"]);
                var u = Entities.db.Utilizadores.Where(s => s.ID_Utilizador == UserID).FirstOrDefault();
                return View(u);
            }
            else
            {
                return RedirectToAction("Inicio", "Home");
            }

        }
        public ActionResult AtualizarPerfil(string morada, string codPostal, string contacto)
        {
            try
            {
                if (Session["UserID"] != null)
                {
                    int UserID = Convert.ToInt32(Session["UserID"]);
                    Regex regexPostal = new Regex(@"^\d{4}-\d{3}$");
                    Regex regexTelem = new Regex(@"^[0-9]{9}$");

                    Utilizadores dUser = Entities.db.Utilizadores.FirstOrDefault(s => s.ID_Utilizador == UserID);

                    if (regexPostal.IsMatch(codPostal) == true && regexTelem.IsMatch(contacto) == true && Entities.db.Utilizadores.Count(s => s.Contacto == contacto && s.ID_Utilizador != UserID && s.Estado == 1) == 0)
                    {
                        dUser.Contacto = contacto;
                        dUser.CodigoPostal = codPostal;
                        dUser.Morada = morada;
                        Entities.db.SaveChanges();
                        TempData["AlterarDados"] = "true";

                    }
                    else
                    {
                        TempData["AlterarDados"] = "false";
                    }

                    return RedirectToAction("Perfil");
                }
                else
                {
                    return RedirectToAction("Inicio", "Home");
                }
            }
            catch (SqlException ex)
            {
                Response.Write($"<script>alert('Data error: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (FormatException ex)
            {
                Response.Write($"<script>alert('Wrong Format: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return RedirectToAction("NotFound", "Error");
            }
        }
        public ActionResult AlterarPassword(string password, string NovaPassword, string ConfNovaPassword)
        {
            try
            {
                if (Session["UserID"] != null)
                {

                    int UserID = Convert.ToInt32(Session["UserID"]);
                    Utilizadores user = Entities.db.Utilizadores.FirstOrDefault(s => s.ID_Utilizador == UserID);

                    if (Generic.CompararPassHash(password, user.PassWord) == true)
                    {
                        if (NovaPassword == ConfNovaPassword)
                        {
                            NovaPassword = Generic.CriarPassHash(NovaPassword);
                            user.PassWord = NovaPassword;
                            Entities.db.SaveChanges();
                            TempData["AlterarPass"] = "true";
                        }
                        else
                            TempData["AlterarPass"] = "NaoIguais";
                    }
                    else
                        TempData["AlterarPass"] = "PassUser";



                    return RedirectToAction("Perfil", "Utilizador", user.ID_Utilizador);
                }
                else
                {
                    return RedirectToAction("Inicio", "Home");
                }
            }
            catch (SqlException ex)
            {
                Response.Write($"<script>alert('Data error: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (FormatException ex)
            {
                Response.Write($"<script>alert('Wrong Format: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return RedirectToAction("NotFound", "Error");
            }

        }
        public ActionResult DepositarSaldo(string nomeTitular, string NumCartao, string CVV, string Validade, int Saldo)
        {
            try
            {
                if (Session["UserID"] != null)
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
                        Entities.db.SaveChanges();

                        TempData["AddSaldo"] = "true";
                        return RedirectToAction("Perfil", "Utilizador", UserID);
                    }
                    else
                    { Session["AddSaldo"] = "false"; }

                    return RedirectToAction("Perfil", "Utilizador", UserID);
                }
                else
                {
                    return RedirectToAction("Inicio", "Home");
                }
            }
            catch (SqlException ex)
            {
                Response.Write($"<script>alert('Data error: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (FormatException ex)
            {
                Response.Write($"<script>alert('Wrong Format: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return RedirectToAction("NotFound", "Error");
            }
        }
        public ActionResult ApagarConta(string password)
        {
            try
            {
                if (Session["UserID"] != null)
                {
                    int id = Convert.ToInt32(Session["UserID"]);
                    var user = Entities.db.Utilizadores.Where(s => s.ID_Utilizador == id).FirstOrDefault();

                    if (true == Generic.CompararPassHash(password, user.PassWord))
                    {
                        user.Estado = 0;

                        Entities.db.SaveChanges();
                        Session.Remove("UserID");
                        return RedirectToAction("Login", "Home");
                    }
                    else
                    {
                        TempData["PassInvalida"] = "true";
                        return RedirectToAction("Perfil");
                    }
                }
                else
                {
                    return RedirectToAction("Inicio", "Home");
                }
            }
            catch (SqlException ex)
            {
                Response.Write($"<script>alert('Data error: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (FormatException ex)
            {
                Response.Write($"<script>alert('Wrong Format: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return RedirectToAction("NotFound", "Error");
            }
        }
        public ActionResult HistoricoCompras(int id)
        {
            try
            {
                if (Session["UserID"] != null)
                {
                    Generic.AtualizarEstadoPedidos();
                    var prod = Entities.db.Pedidos.Where(s => s.ID_Utilizador == id).ToList();

                    return View(prod);
                }
                else
                {
                    return RedirectToAction("Inicio", "Home");
                }
            }
            catch (SqlException ex)
            {
                Response.Write($"<script>alert('Data error: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return RedirectToAction("NotFound", "Error");
            }
        }
        public ActionResult Details(int id)
        {
            try
            {
                if (Session["UserID"] != null)
                {
                    var p = Entities.db.PedidosDetalhes.Where(s => s.ID_Pedido == id).ToList();
                    return PartialView("Details", p);
                }
                else
                {
                    return RedirectToAction("Inicio", "Home");
                }
            }
            catch (SqlException ex)
            {
                Response.Write($"<script>alert('Data error: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return RedirectToAction("NotFound", "Error");
            }
        }
        public ActionResult AddProdFavoritos(int idP, int idC)
        {
            try
            {
                if (Session["UserID"] != null)
                {
                    ProdutosFavoritos f = new ProdutosFavoritos();
                    int userID = Convert.ToInt32(Session["UserID"]);
                    if (Entities.db.ProdutosFavoritos.Count(s => s.ID_Produto == idP && s.ID_Utilizador == userID) == 0)
                    {
                        var p = Entities.db.Produto.Where(s => s.ID_Produto == idP).FirstOrDefault();

                        f.ID_Produto = idP;
                        f.ID_Utilizador = userID;
                        Entities.db.ProdutosFavoritos.Add(f);
                        Entities.db.SaveChanges();
                    }
                    Session["AddFavorito"] = "true";
                    return RedirectToAction("ListaProdutos", "Produtos", new { id = idC });
                }
                else
                {
                    return RedirectToAction("Inicio", "Home");
                }
            }
            catch (SqlException ex)
            {
                Response.Write($"<script>alert('Data error: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (FormatException ex)
            {
                Response.Write($"<script>alert('Wrong Format: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return RedirectToAction("NotFound", "Error");
            }
        }
        public ActionResult ApagarProdFavorito(int idProd)
        {
            try
            {
                if (Session["UserID"] != null)
                {
                    var p = Entities.db.ProdutosFavoritos.Where(s => s.ID_Produto == idProd && s.ID_Utilizador == 2).FirstOrDefault();
                    Entities.db.ProdutosFavoritos.Remove(p);
                    Entities.db.SaveChanges();
                    return RedirectToAction("ProdutosFavoritos", "Utilizador");
                }
                else
                {
                    return RedirectToAction("Inicio", "Home");
                }
            }
            catch (SqlException ex)
            {
                Response.Write($"<script>alert('Data error: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (FormatException ex)
            {
                Response.Write($"<script>alert('Wrong Format: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return RedirectToAction("NotFound", "Error");
            }
        }
        public ActionResult ProdutosFavoritos(int id)
        {
            try
            {
                if (Session["UserID"] != null)
                {
                    var p = Entities.db.DadosProdutosFavoritos(id).ToList();
                    return View(p);
                }
                else
                {
                    return RedirectToAction("Inicio", "Home");
                }
            }
            catch (SqlException ex)
            {
                Response.Write($"<script>alert('Data error: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return RedirectToAction("NotFound", "Error");
            }
        }


        #region Carrinho
        public ActionResult CarrinhoProdutos(int id)
        {
            try
            {
                if (Session["UserID"] != null)
                {
                    var prod = Entities.db.DadosCarrinhoProduto(id).ToList();

                    return View(prod);
                }
                else
                {
                    return RedirectToAction("Inicio", "Home");
                }
            }
            catch (SqlException ex)
            {
                Response.Write($"<script>alert('Data error: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (FormatException ex)
            {
                Response.Write($"<script>alert('Wrong Format: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return RedirectToAction("NotFound", "Error");
            }
        }
        public ActionResult ProdCarrinho(Carrinho c, int idP, int idC)
        {
            try
            {
                if (Session["UserID"] != null)
                {

                    int userID = Convert.ToInt32(Session["UserID"]);
                    if (Entities.db.Carrinho.Count(s => s.ID_Produto == idP && s.ID_Utilizador == userID) == 0)
                    {
                        var p = Entities.db.Produto.Where(s => s.ID_Produto == idP).FirstOrDefault();

                        c.ID_Produto = idP;
                        c.PrecoProduto = p.PreçoPorQuantidade;
                        c.ID_Utilizador = userID;
                        c.Quantidade = 1;
                        Entities.db.Carrinho.Add(c);
                        Entities.db.SaveChanges();
                    }
                    Session["AddCarrinho"] = "true";
                    return RedirectToAction("ListaProdutos", "Produtos", new { id = idC });
                }
                else
                {
                    return RedirectToAction("Inicio", "Home");
                }
            }
            catch (SqlException ex)
            {
                Response.Write($"<script>alert('Data error: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (FormatException ex)
            {
                Response.Write($"<script>alert('Wrong Format: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return RedirectToAction("NotFound", "Error");
            }
        }
        public ActionResult EliminarProdCarrinho(int id)
        {
            try
            {
                if (Session["UserID"] != null)
                {
                    int UserID = Convert.ToInt32(Session["UserID"]);
                    var p = Entities.db.Carrinho.Where(s => s.ID_Utilizador == UserID && s.ID_Produto == id).FirstOrDefault();
                    Entities.db.Carrinho.Remove(p);
                    Entities.db.SaveChanges();

                    return RedirectToAction("CarrinhoProdutos", new { id = UserID });
                }
                else
                {
                    return RedirectToAction("Inicio", "Home");
                }
            }
            catch (SqlException ex)
            {
                Response.Write($"<script>alert('Data error: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (FormatException ex)
            {
                Response.Write($"<script>alert('Wrong Format: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return RedirectToAction("NotFound", "Error");
            }
        }
        public ActionResult SomaProd(int id)
        {
            try
            {
                if (Session["UserID"] != null)
                {

                    var p = Entities.db.Produto.FirstOrDefault(s => s.ID_Produto == id);
                    int UserID = Convert.ToInt32(Session["UserID"]);
                    var pc = Entities.db.Carrinho.Where(s => s.ID_Produto == id && s.ID_Utilizador == UserID).FirstOrDefault();

                    if (p.QuantidadeStock >= pc.Quantidade)
                    {
                        pc.Quantidade++;
                    }
                    Entities.db.SaveChanges();
                    return RedirectToAction("CarrinhoProdutos", new { id = UserID });
                }
                else
                {
                    return RedirectToAction("Inicio", "Home");
                }
            }
            catch (SqlException ex)
            {
                Response.Write($"<script>alert('Data error: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return RedirectToAction("NotFound", "Error");
            }

        }
        public ActionResult DiminuiProd(int id)
        {
            try
            {
                if (Session["UserID"] != null)
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
                else
                {
                    return RedirectToAction("Inicio", "Home");
                }

            }
            catch (SqlException ex)
            {
                Response.Write($"<script>alert('Data error: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (FormatException ex)
            {
                Response.Write($"<script>alert('Wrong Format: {ex.Message}');</script>");
                return RedirectToAction("NotFound", "Error");
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return RedirectToAction("NotFound", "Error");
            }
        }
        #endregion
    }
}