﻿using RickyShop_Site.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;

namespace RickyShop_Site.Controllers
{
    public class ResetPassController : Controller
    {
        //GestãoRickyShopEntities Entities.db = new GestãoRickyShopEntities();

        // GET: ResetPass
        public ActionResult EnvioToken()
        {
            return View();
        }

        public ActionResult ValidacaoToken()
        {
            if (TempData["MensagemAviso"] != null)
            {
                if (TempData["MensagemAviso"].ToString() == "true")
                {
                    Response.Write($"<script>alert('Token enviado. Consulte o email!')</script>");
                    TempData.Remove("MensageAviso");
                }
                else
                {
                    Response.Write($"<script>alert('Tem um token ativo. Consulte o email.')</script>");
                    TempData.Remove("MensageAviso");
                }
            }
            return View();
        }

        public ActionResult ResetPassword()
        {
            return View();
        }

        public ActionResult TemplateEmail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ValidacaoToken(Token token)
        {
            int UserID = Convert.ToInt32(Session["UserID"]);
            if (Entities.db.Token.Count(t => t.ID_Utilizador == UserID && t.TokenAleatorio == token.TokenAleatorio && t.Estado == 1) != 0)
            {
                var t = Entities.db.Token.FirstOrDefault(s => s.ID_Utilizador == UserID && s.Estado == 1);
                t.Estado = 0;
                Entities.db.SaveChanges();
                return RedirectToAction("ResetPassword");
            }
            else
            {
                Response.Write($"<script>alert('Ups, token inválido.')</script>");
                return View();
            }

        }

        [HttpPost]
        public ActionResult EnvioToken(Utilizadores u)
        {
            bool valid = false;
            string tokenAleatorio;
            Utilizadores user = Entities.db.Utilizadores.Where(s => s.Email == u.Email).FirstOrDefault();
            do
            {
                tokenAleatorio = Generic.TokenAleatorio();
                if (Entities.db.Token.Count(t => t.TokenAleatorio == tokenAleatorio) == 0)
                {

                    Token token = new Token();
                    token.TokenAleatorio = tokenAleatorio;
                    token.ID_Utilizador = Entities.db.Utilizadores.Where(s => s.Email == u.Email).FirstOrDefault().ID_Utilizador;
                    token.Estado = 1;
                    Session["UserID"] = token.ID_Utilizador;

                    if (Entities.db.Token.Count(t => t.ID_Utilizador == token.ID_Utilizador && t.Estado == 1) != 0)
                    {
                        
                        TempData["MensagemAviso"] = "true";
                        return RedirectToAction("ValidacaoToken");
                    }
                    else
                    {
                        Entities.db.Token.Add(token);
                        valid = true;
                    }
                }
            } while (valid != true);


            string subject = "Redefinição de senha da RickyShop";


            //string body = "Olá, a seu token de recuperação é: " + tokenAleatorio +
            //    "\r De seguida altere a sua password no site. \n Cumprimentos, ";

            string path = Server.MapPath(@"~\Views\ResetPass\TemplateEmail.cshtml");

            var conteudo = System.IO.File.ReadAllText(path);

            conteudo = conteudo.Replace("[NomeCliente]", user.PrimeiroNome + " " + user.SegundoNome);
            conteudo = conteudo.Replace("[TokenCliente]", tokenAleatorio);
            conteudo = conteudo.Replace("[EmailCliente]", user.Email);

            string body = conteudo;

            //string body = "Olá, a seu token de recuperação é: " + tokenAleatorio +
            //   "\r De seguida altere a sua password no site. \n Cumprimentos, ";

            MailMessage objEmail = new MailMessage();

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, new ContentType("text/html"));

            objEmail.AlternateViews.Add(htmlView);

            //rementente do email
            //objEmail.From = new MailAddress("i200059@inete.net");
            objEmail.From = new MailAddress("suporterickyshop@hotmail.com");

            //email para resposta(quando o destinatário receber e clicar em responder, vai para:)
            //objEmail.ReplyTo = new MailAddress("email@seusite.com.br");

            //destinatário(s) do email(s). Obs. pode ser mais de um, pra isso basta repetir a linha
            //abaixo com outro endereço
            objEmail.To.Add(u.Email);

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
            objEmail.Body = subject + "\r\r\r" + htmlView;
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
            objSmtp.Credentials = new NetworkCredential("suporterickyshop@hotmail.com", "papRickyShop");
            //objSmtp.Credentials = new NetworkCredential("i200059@inete.net", "Pitolini2005");

            objSmtp.EnableSsl = true;

            //envia o email
            objSmtp.Send(objEmail);

            //WebMail.EnableSsl = true;
            //WebMail.SmtpServer = "smtp.mail.yahoo.com";
            //WebMail.SmtpPort = 465;
            //WebMail.UserName = "Ricardo Coimbra";
            //WebMail.Password = "Pitolni08";
            //WebMail.From = "cruzcoimbra08@yahoo.com";

            Entities.db.SaveChanges();


            TempData["MensagemAviso"] = "true";
            return RedirectToAction("ValidacaoToken");
        }

        [HttpPost]
        public ActionResult ResetPassword(Utilizadores u)
        {
            int UserID = Convert.ToInt32(Session["UserID"]);
            var user = Entities.db.Utilizadores.FirstOrDefault(s => s.ID_Utilizador == UserID);
            if (true == Generic.CompararPassHash(u.PassWord, user.PassWord))
            {

                u.PassWord = Generic.CriarPassHash(u.PassWord);

                user.PassWord = u.PassWord;
                Entities.db.SaveChanges();
                TempData["MensagemResetPass"] = "true";
                return RedirectToAction("Login", "Home");
            }
            else
            {
                Response.Write($"<script>alert('Estas passwords não correspondem.')</script>");
                return View();
            }
        }
    }
}