using RickyShop_Site.Models;
using System;
using System.Collections.Generic;
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
        //GestãoRickyShopEntities Entities.db = new GestãoRickyShopEntities();

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            if (TempData["MensagemAviso"] != null)
            {
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
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return View();
            }
        }

        [HandleError]
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            try
            {
                var user = Entities.db.Utilizadores.FirstOrDefault(us => us.Email == email);

                if (user != null)
                {
                    // Compara as senhas encriptadas
                    if (true == Generic.CompararPassHash(password, user.PassWord))
                    {

                        Session["UserID"] = user.ID_Utilizador;
                        Session["UserNome"] = user.PrimeiroNome;
                        return RedirectToAction("About", "Home");
                    }
                    else
                    {
                        var d = Dns.GetHostAddresses(Dns.GetHostName());
                        Logs logs = new Logs();
                        logs.IP_TentativaLogin = d[1].ToString();
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
                return View();
            }
            catch (FormatException ex)
            {
                Response.Write($"<script>alert('Wrong Format: {ex.Message}');</script>");
                return View();
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return View();
            }
        }

        public ActionResult Inicio()
        {
            var prodProm = Entities.db.Produto.ToList();

            return View(prodProm);
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
                return View();
            }
            catch (FormatException ex)
            {
                Response.Write($"<script>alert('Wrong Format: {ex.Message}');</script>");
                return View();
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return View();
            }
        }

        public ActionResult EnviarReporte(string Nome, string Email, string Titulo, string descricao)
        {
            Reporte r = new Reporte();

            r.Nome_Utilizador = Nome;
            r.Email = Email;
            r.Titulo_Reporte = Titulo;
            r.Descrição = descricao;

            Entities.db.Reporte.Add(r);
            Entities.db.SaveChangesAsync();
            return RedirectToAction("Contact");
        }
    }
}