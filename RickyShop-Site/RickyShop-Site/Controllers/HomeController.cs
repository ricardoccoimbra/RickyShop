using RickyShop_Site.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace RickyShop_Site.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            if (TempData["EnvioReporte"] != null)
            {
                Response.Write($"<script>alert('Reporte enviado')</script>");
            }

            return View();
        }

        public ActionResult Login()
        {
            if (TempData["MensagemAviso"] != null)
            {
                if (TempData["MensagemAviso"].ToString() == "false")
                    Response.Write($"<script>alert('Ups! Email inválido!')</script>");
                else
                    Response.Write($"<script>alert('Conta criada com suceso!')</script>");


                TempData.Remove("MensageAviso");
            }

            if (TempData["MensagemResetPass"] != null)
            {
                Response.Write($"<script>alert('PassWord alterada com sucesso!')</script>");
                TempData.Remove("MensagemResetPass");
            }
            return View();
        }

        public ActionResult Registar()
        {
            return View();
        }

        [HandleError]
        [HttpPost]
        public ActionResult Registar(Utilizadores u)
        {
            try
            {
                DateTime dataNascimento = DateTime.Parse(Request.Form["DataNascimento"]);
                var verif = Generic.VerifRegisto(u.NIF, u.Contacto, u.Email, dataNascimento);

                switch (verif)
                {
                    case "certo":
                        var user = u;
                        u.Email.ToLower();
                        u.DataDeAdesao = DateTime.Now;
                        u.DataDeNascimento = dataNascimento;
                        u.Desconto = 10;
                        u.Saldo = 0;
                        u.PassWord = Generic.CriarPassHash(u.PassWord);
                        u.Estado = 1;

                        Entities.db.Utilizadores.Add(u);

                        Entities.db.SaveChanges();
                        TempData["MensagemAviso"] = "true";
                        return RedirectToAction("Login");

                    case "nif":
                        Response.Write($"<script>alert('NIF já existente!')</script>");
                        return View();

                    case "num":
                        Response.Write($"<script>alert('Contacto já existente!')</script>");
                        return View();

                    case "email":
                        Response.Write($"<script>alert('Email já existente!')</script>");
                        return View();

                    default:
                        return View();

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

        [HandleError]
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            try
            {
                var user = Entities.db.Utilizadores.FirstOrDefault(us => us.Email == email && us.Estado == 1);

                if (user != null)
                {
                    // Compara as senhas encriptadas
                    if (true == Generic.CompararPassHash(password, user.PassWord))
                    {

                        Session["UserID"] = user.ID_Utilizador;
                        return RedirectToAction("Inicio", "Home");
                    }
                    else
                    {
                        var d = Dns.GetHostAddresses(Dns.GetHostName());
                        Logs logs = new Logs();
                        logs.IP_TentativaLogin = d[3].ToString();
                        logs.ID_Utilizador = Entities.db.Utilizadores.FirstOrDefault(s => s.Email == email).ID_Utilizador;
                        logs.Erro_Login = DateTime.Now;
                        Entities.db.Logs.Add(logs);
                        Entities.db.SaveChangesAsync();

                        Response.Write("<script>alert('Credicen');</script>");
                        return View();
                    }

                }

                if (email == "admin" && password == "admin123")
                {
                    Session["Admin"] = "on";
                    return RedirectToAction("Home", "Admin");
                }
                Response.Write("<script>alert('Credenciais Inválidas!');</script>");
                return View();

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

        public ActionResult Inicio()
        {
            //se não tiver a null é porque há promo ativa, se tiver, a promo acabou
            try
            {

                if (Entities.db.CountPromoAtiva().FirstOrDefault() == 0)
                {
                    //codigo para retirar os descontos depois da promo
                    var dados = Generic.ValSettings(Server.MapPath("~/FicheiroJson/SettingsRickyShop.json"));
                    if (dados.ValidPosPromo == 1)
                    {
                        //Rescrever Json
                        DadosSettingsSite d = new DadosSettingsSite();
                        d.ValidPosPromo = 0;
                        d.QtdProdutosPagina = dados.QtdProdutosPagina;
                        d.QtdProdutosDestaque = dados.QtdProdutosDestaque;
                        Generic.EscreverJson(Server.MapPath("~/FicheiroJson/SettingsRickyShop.json"), d);


                        //var p = Entities.db.Promo.OrderByDescending(s => s.ID_Promo).FirstOrDefault();
                        var p = Entities.db.Promo.ToList().Last();

                        if (p.ID_Categoria == null)
                        {
                            foreach (var item in Entities.db.Produto.Where(s => s.ID_Marca == p.ID_Marca))
                            {
                                item.Desconto = null;

                            }

                        }
                        else
                        {
                            foreach (var item in Entities.db.Produto.Where(s => s.ID_Categoria == p.ID_Categoria))
                            {
                                item.Desconto = null;
                            }

                        }
                    }
                }

                //Meter primeiro a lista depois salvar porque deu erro numa FK-CategoriaID, não sei pq :(
                var prodProm = Entities.db.Produto.ToList();
                Entities.db.SaveChanges();

                return View(prodProm);
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

        public ActionResult LogOff()
        {
            try
            {
                Session.Remove("UserID");
                Session.Remove("Admin");

                return RedirectToAction("Inicio");
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

        public ActionResult EnviarReporte(Reporte r)
        {
            try
            {
                Entities.db.Reporte.Add(r);
                Entities.db.SaveChanges();
                TempData["EnvioReporte"] = "true";
                return RedirectToAction("Contact");
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
    }
}