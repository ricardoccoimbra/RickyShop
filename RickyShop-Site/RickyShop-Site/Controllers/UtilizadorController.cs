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
        GestãoRickyShopEntities db = new GestãoRickyShopEntities();


        // GET: Utilizador
        public ActionResult Perfil()
        {
            int UserID = Convert.ToInt32(Session["UserID"]);
            var u = db.Utilizadores.Where(s => s.ID_Utilizador == UserID).FirstOrDefault();
            return View(u);
        }

        public ActionResult AtualizarPerfil(string morada, string codPostal, string contacto)
        {
            int UserID = Convert.ToInt32(Session["UserID"]);
            Regex regexPostal = new Regex(@"^\d{4}(-\d{3})?$");
            Regex regexTelem = new Regex(@"^[0-9]{9}$");

            Utilizadores dUser = db.Utilizadores.FirstOrDefault(s => s.ID_Utilizador == UserID);

            if (regexPostal.IsMatch(codPostal) == true && morada != "" && regexTelem.IsMatch(contacto) == true)
            {
                dUser.Contacto = contacto;
                dUser.CodigoPostal = codPostal;
                dUser.Morada = morada;

                db.SaveChangesAsync();

                return RedirectToAction("Perfil", "Utilizador", UserID);
            }
            return View();
        }
        public ActionResult AlterarPassword(string password, string NovaPassword, string ConfNovaPassword)
        {
            int UserID = Convert.ToInt32(Session["UserID"]);
            Utilizadores user = db.Utilizadores.FirstOrDefault(s => s.ID_Utilizador == UserID);
            if (password != "" && NovaPassword != "" && ConfNovaPassword != "")
            {
                if (Generic.CompararPassHash(password, user.PassWord) == true)
                {
                    if (NovaPassword == ConfNovaPassword)
                    {
                        NovaPassword = Generic.CriarPassHash(NovaPassword);
                        user.PassWord = NovaPassword;
                        db.SaveChangesAsync();
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
                mv.NumeroCartao = NumCartao;
                mv.Estado = "Pendente";
                mv.ID_Utilizador = UserID;
                mv.Tipo = "+";
                mv.Quantidade = Saldo;

                db.MovimentacaoSaldo.Add(mv);
                db.SaveChangesAsync();

                return RedirectToAction("Perfil", "Utilizador", UserID);
            }

            return RedirectToAction("Perfil", "Utilizador", UserID);

        }

    }
}