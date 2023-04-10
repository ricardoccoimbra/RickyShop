using RickyShop_Site.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace RickyShop_Site.Controllers
{
    public class HomeController : Controller
    {
        GestãoRickyShopEntities db = new GestãoRickyShopEntities();

        public ActionResult Index()
        {
            return View();
        }

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
                var user = u;

                var verif = Generic.VerifRegisto(u.NIF, u.Contacto, u.Email);

                if (verif == "certo")
                {
                    u.Email = u.Email.ToLower();
                    u.DataDeAdesao = DateTime.Now;
                    u.DataDeNascimento = DateTime.Now;
                    db.Utilizadores.Add(u);
                   
                    db.SaveChanges();
                    Response.Write($"<script>alert('Conta Criada!')</script>");
                    return RedirectToAction("Login");
                }

                if (verif == "nif")
                {
                    Response.Write($"<script>alert('NIF já existente!')</script>");

                    return View();
                }

                if (verif == "email")
                {
                    Response.Write($"<script>alert('Email já existente!')</script>");
                    u.Email = "";
                    return View();
                }

                if (verif == "num")
                {
                    Response.Write($"<script>alert('Contacto já existente!')</script>");
                    return View();
                }
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return View();
            }
            return View();
        }

        [HandleError]
        [HttpPost]
        public ActionResult Login(Utilizadores u)
        {
            try
            {

                var user = db.Utilizadores.FirstOrDefault(us => us.Email == u.Email && us.PassWord == u.PassWord);

                if (user != null)
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
    }
}