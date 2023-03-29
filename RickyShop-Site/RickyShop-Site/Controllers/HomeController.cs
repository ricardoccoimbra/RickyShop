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
        private SmtpClient SmtpClient;
        private MailMessage MailMessage;

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

        public ActionResult ResetPassword()
        {
            return View();
        }

        public ActionResult Registar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(Utilizadores u)
        {
            Random random = new Random();

            var num = random.Next();

            string subject = "Redefinição de senha da RickyShop";
            string body = "Olá, a sua password passou a ser: " + num.ToString() +
                "\r De seguida altere a sua password no site. \n Cumprimentos, ";

            MailMessage objEmail = new MailMessage();
            //rementente do email
            objEmail.From = new MailAddress("i200059@inete.net");

            //email para resposta(quando o destinatário receber e clicar em responder, vai para:)
            //objEmail.ReplyTo = new MailAddress("email@seusite.com.br");

            //destinatário(s) do email(s). Obs. pode ser mais de um, pra isso basta repetir a linha
            //abaixo com outro endereço
            objEmail.To.Add("cruzcoimbra08@gmail.com");

            //se quiser enviar uma cópia oculta pra alguém, utilize a linha abaixo:
            // objEmail.Bcc.Add("oculto@provedor.com.br");

            //prioridade do email
            objEmail.Priority = MailPriority.High;

            //utilize true pra ativar html no conteúdo do email, ou false, para somente texto
            objEmail.IsBodyHtml = true;

            //Assunto do email
            objEmail.Subject = subject;

            //corpo do email a ser enviado
            //objEmail.Body = "Conteúdo do email. Se ativar html, pode utilizar cores, fontes, etc.";
            objEmail.Body = subject + "\r\r\r" + body;
            //codificação do assunto do email para que os caracteres acentuados serem reconhecidos.
            objEmail.SubjectEncoding = Encoding.GetEncoding("ISO-8859-1");

            //codificação do corpo do emailpara que os caracteres acentuados serem reconhecidos.
            objEmail.BodyEncoding = Encoding.GetEncoding("ISO-8859-1");

            //cria o objeto responsável pelo envio do email
            SmtpClient objSmtp = new SmtpClient();

            //endereço do servidor SMTP(para mais detalhes leia abaixo do código)
            objSmtp.Host = "SMTP.office365.com";
            objSmtp.Port = 587;

            //para envio de email autenticado, coloque login e senha de seu servidor de email
            //para detalhes leia abaixo do código
            objSmtp.Credentials = new NetworkCredential("i200059@inete.net", "Pitolini08");

            objSmtp.EnableSsl = true;

            //envia o email
            objSmtp.Send(objEmail);

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