using PAP_RickyShop.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PAP_RickyShop.Controllers
{
    public class HomeController : Controller
    {
        GestãoRickyShopEntities db = new GestãoRickyShopEntities();

        public ActionResult Sobre()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Contacto()
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

                        SHA256 sha256Hash = SHA256.Create();

                        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(u.PassWord));
                        StringBuilder builder = new StringBuilder();
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            builder.Append(bytes[i].ToString("x2"));
                        }
                        u.PassWord = builder.ToString();

                        db.Utilizadores.Add(u);

                        db.SaveChanges();
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
        public ActionResult Login(Utilizadores u)
        {
            try
            {
                var user = db.Utilizadores.FirstOrDefault(us => us.Email == u.Email);

                if (user != null)
                {
                    SHA256 sha256Hash = SHA256.Create();

                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(u.PassWord));
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }
                    u.PassWord = builder.ToString();


                    // Compara as senhas encriptadas
                    if (user.PassWord == u.PassWord)
                    {

                        Session["UserID"] = user.ID_Utilizador;
                        Session["UserNome"] = user.PrimeiroNome;
                        return RedirectToAction("About", "Home");
                    }
                    else
                    {
                        Response.Write("<script>alert('Wrong Information');</script>");
                        return View();
                    }

                }
                else
                {
                    if (db.Utilizadores.Count(s => s.Email == u.Email) != 0)
                    {
                        var d = Dns.GetHostAddresses(Dns.GetHostName());
                        Logs logs = new Logs();
                        logs.IP_TentativaLogin = d[1].ToString();
                        logs.ID_Utilizador = db.Utilizadores.FirstOrDefault(s => s.Email == u.Email).ID_Utilizador;
                        logs.Erro_Login = DateTime.Now;
                        db.Logs.Add(logs);
                        db.SaveChangesAsync();
                    }

                    Response.Write("<script>alert('Wrong Information');</script>");
                    return View();
                }
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
            var prodProm = db.Produto.ToList();

            return View(prodProm);
        }
        public ActionResult LogOff()
        {
            try
            {
                Session.Remove("UserID");

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
        public ActionResult Teste()
        {
            return View();
        }
    }
}