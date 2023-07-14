using Microsoft.Ajax.Utilities;
using OfficeOpenXml;
using PagedList;
using RickyShop_Site.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System.Data.SqlClient;

namespace RickyShop_Site.Controllers
{
    public class AdminController : Controller
    {
        //GestãoRickyShopEntities Entities.db = new GestãoRickyShopEntities();

        // GET: Admin
        public ActionResult Home()
        {
            if (Session["Admin"] != null)
            {
                if (TempData["NovaPromo"] != null)
                {
                    if (TempData["NovaPromo"].ToString() == "true")
                        Response.Write($"<script>alert('Promoção criada!')</script>");
                    else
                        Response.Write($"<script>alert('Imagem não encontrada na pasta! Selecionar uma imagem da pasta ~/ImgPromo/')</script>");

                }

                if (TempData["AddImagem"] != null)
                {
                    if (TempData["AddImagem"].ToString() == "true")
                    {
                        Response.Write($"<script>alert('Imagem guardada!')</script>");
                    }
                    else
                    {
                        Response.Write($"<script>alert('Apenas arquivos .jpg, .jpeg e .png são permitidos.')</script>");
                    }
                }

                return View();
            }
            else
            {
                return RedirectToAction("Inicio", "Home");
            }
        }
        public ActionResult Produtos(int? pagina)
        {
            try
            {
                if (Session["Admin"] != null)
                {
                    if (TempData["EnvioReporte"] != null)
                    {
                        Response.Write($"<script>alert('Reporte enviado!')</script>");
                    }

                    if (TempData["AddImagem"] != null)
                    {
                        if (TempData["AddImagem"].ToString() == "true")
                        {
                            Response.Write($"<script>alert('Imagem guardada!')</script>");
                        }
                        else
                        {
                            Response.Write($"<script>alert('Apenas arquivos .jpg, .jpeg e .png são permitidos.')</script>");
                        }
                    }

                    if (TempData["AddMarca"] != null)
                    {
                        if (TempData["AddMarca"].ToString() == "true")
                        {
                            Response.Write($"<script>alert('Marca adicionada!')</script>");
                        }
                        else
                        {
                            Response.Write($"<script>alert('Marca inválida! Nome existente!')</script>");
                        }
                    }

                    if (TempData["AddCategoria"] != null)
                    {
                        if (TempData["AddCategoria"].ToString() == "true")
                        {
                            Response.Write($"<script>alert('Categoria adicionada!')</script>");
                        }
                        else
                        {
                            Response.Write($"<script>alert('Categoria inválida! Nome existente!')</script>");
                        }
                    }

                    if (TempData["NovoProduto"] != null)
                    {
                        if (TempData["NovoProduto"].ToString() == "true")
                        {
                            Response.Write($"<script>alert('Produto criado!')</script>");
                        }
                        else
                        {
                            if (TempData["NovoProduto"].ToString() == "ProdIguais")
                            {
                                Response.Write($"<script>alert('Nome do produto ou Path de imagem repetidos!')</script>");
                            }
                            else
                                Response.Write($"<script>alert('Foto inválida! Selecione uma da pasta '~/Produtos' ')</script>");
                        }
                    }

                    IPagedList<Produto> p;
                    int tamanhoPagina = 10;
                    int numeroPagina = pagina ?? 1;

                    if (Session["FiltroProd"] == null)
                    { p = Entities.db.Produto.Where(s => s.MarcaProduto.Estado == 1).ToList().ToPagedList(numeroPagina, tamanhoPagina); }
                    else
                    {
                        int marca = Convert.ToInt32(Session["FiltroProd"]);
                        if (marca != -1)
                            p = Entities.db.Produto.Where(s => s.ID_Marca == marca).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                        else
                            p = Entities.db.Produto.Where(s => s.MarcaProduto.Estado == 1).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                    }
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
        public ActionResult ViewProdutosDetails(int id)
        {
            try
            {
                if (Session["Admin"] != null)
                {
                    // Retorne a exibição do modal
                    var p = Entities.db.Produto.Where(s => s.ID_Produto == id).ToList();
                    return PartialView("ProdutosDetails", p);
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
        public ActionResult AddImagemProduto(HttpPostedFileBase file)
        {
            try
            {
                if (Session["Admin"] != null)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string path = Path.Combine(Server.MapPath("~/Produtos/"), fileName);
                    FileInfo fileInfo = new FileInfo(path);


                    string extension = Path.GetExtension(fileName);

                    switch (extension.ToLower())
                    {
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                            file.SaveAs(path);
                            TempData["AddImagem"] = "true";
                            return RedirectToAction("Produtos");
                        default://Meter mensagens de erro e avisar que só aquelas extesnsoes são validas
                            TempData["AddImagem"] = "false";
                            return RedirectToAction("Produtos");
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
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return RedirectToAction("NotFound", "Error");
            }

        }
        public ActionResult ExportarExcel()
        {
            try
            {
                if (Session["Admin"] != null)
                {
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                    int c = 0;
                    int linha = 2;
                    var package = new ExcelPackage();
                    var worksheet = package.Workbook.Worksheets.Add("Stock_Produtos");

                    worksheet.Cells["A1"].Value = "ID";
                    worksheet.Cells["B1"].Value = "Produto";
                    worksheet.Cells["C1"].Value = "Preço";
                    worksheet.Cells["D1"].Value = "Desconto";
                    worksheet.Cells["E1"].Value = "Qtd. Stock";
                    worksheet.Cells["F1"].Value = "Path Imagem";
                    worksheet.Cells["G1"].Value = "Descrição";
                    worksheet.Cells["H1"].Value = "Categoria";
                    worksheet.Cells["I1"].Value = "Marca";
                    worksheet.Cells["J1"].Value = "Publicado";
                    worksheet.Cells["K1"].Value = "Destaque";


                    foreach (var item in Entities.db.Produto)
                    {
                        //Verificar o porque de não mostrar a ultima alinha
                        worksheet.Cells[linha, c + 1].Value = item.ID_Produto;
                        worksheet.Cells[linha, c + 2].Value = item.Nome;
                        worksheet.Cells[linha, c + 3].Value = item.PreçoPorQuantidade;
                        worksheet.Cells[linha, c + 4].Value = item.Desconto;
                        worksheet.Cells[linha, c + 5].Value = item.QuantidadeStock;
                        worksheet.Cells[linha, c + 6].Value = item.ImagemPath;
                        worksheet.Cells[linha, c + 7].Value = item.Descrição;
                        worksheet.Cells[linha, c + 8].Value = Entities.db.Categoria.FirstOrDefault(s => s.ID_Categoria == item.ID_Categoria).NomeCategoria;
                        worksheet.Cells[linha, c + 9].Value = Entities.db.MarcaProduto.FirstOrDefault(s => s.ID_Marca == item.ID_Marca).Marca;
                        worksheet.Cells[linha, c + 10].Value = item.Descontinuado;
                        worksheet.Cells[linha, c + 11].Value = item.Destaque;
                        linha++;
                    }

                    //Salve o arquivo Excel em uma pasta temporária
                    string fileName = Path.GetTempFileName();
                    package.SaveAs(fileName);

                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.Headers.Add("Content-Disposition", "attachment; filename=ProdutosExcel.xlsx");

                    // Escreva o conteúdo do arquivo Excel na resposta HTTP
                    Response.BinaryWrite(package.GetAsByteArray());

                    //A resolver Retornar certo
                    return RedirectToAction("Produtos");
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
        public ActionResult FiltroProdutos(int marca)
        {
            if (Session["Admin"] != null)
            {
                Session["FiltroProd"] = marca;
                return RedirectToAction("Produtos", "Admin");
            }
            else
            {
                return RedirectToAction("Inicio", "Home");
            }
        }
        public ActionResult ChartProdutoMaisVendido()
        {
            try
            {
                // Obtenha os dados do gráfico do seu modelo ou de qualquer outra fonte de dados

                string[] nomes = new string[5];
                int[] quantidade = new int[5];

                int cnt = 0;
                foreach (var item in Entities.db.TOP5_ProdMaisVendidos())
                {
                    nomes[cnt] = item.Nome;
                    quantidade[cnt] = Convert.ToInt32(item.total_vendido);
                    cnt++;
                }

                var chartData = new
                {
                    labels = nomes,
                    datasets = new[]
                    {
            new
            {
                data = quantidade,
                backgroundColor = "rgba(65, 255, 30, 0.5)",
                borderColor = "rgba(65, 255, 30, 1)",
                borderWidth = 1,
            }
        }
                };

                return Json(chartData, JsonRequestBehavior.AllowGet);
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
        public ActionResult ChartMarcaMaisVendida()
        {
            // Obtenha os dados do gráfico do seu modelo ou de qualquer outra fonte de dados
            try
            {
                string[] marcas = new string[5];
                int[] quantidade = new int[5];

                int cnt = 0;
                foreach (var item in Entities.db.TOP5_MarcasMaisVendidas())
                {
                    marcas[cnt] = item.Marca;
                    quantidade[cnt] = Convert.ToInt32(item.total_vendido);
                    cnt++;
                }

                var chartData = new
                {
                    labels = marcas,
                    datasets = new[]
                    {
            new
            {
                data = quantidade,
                backgroundColor = "rgba(65, 255, 30, 0.5)",
                borderColor = "rgba(65, 255, 30, 1)",
                borderWidth = 1,
            }
        }
                };

                return Json(chartData, JsonRequestBehavior.AllowGet);
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
        public ActionResult EstadoMarca(int idM, string tipo)
        {
            try
            {
                if (Session["Admin"] != null)
                {

                    var m = Entities.db.MarcaProduto.Where(s => s.ID_Marca == idM).FirstOrDefault();

                    if (tipo == "des")
                    {
                        m.Estado = 0;
                        Entities.db.MarcaProduto.Where(s => s.ID_Marca == idM).FirstOrDefault().Estado = 0;

                        foreach (var item in Entities.db.Produto.Where(s => s.ID_Marca == m.ID_Marca))
                        {
                            Entities.db.Produto.Where(s => s.ID_Marca == m.ID_Marca).FirstOrDefault().Descontinuado = 0;
                            item.Descontinuado = 0;
                        }

                    }
                    else
                    {
                        m.Estado = 1;
                        Entities.db.MarcaProduto.Where(s => s.ID_Marca == idM).FirstOrDefault().Estado = 1;

                        foreach (var item in Entities.db.Produto.Where(s => s.ID_Marca == m.ID_Marca))
                        {
                            Entities.db.Produto.Where(s => s.ID_Marca == m.ID_Marca).FirstOrDefault().Descontinuado = 1;
                            item.Descontinuado = 1;
                        }
                    }

                    Entities.db.SaveChanges();
                    return RedirectToAction("Produtos", "Admin");
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
        public ActionResult CriarProduto(string nomeProduto, int categoria, int preco, int qtdStock, string path, int desconto, int marca, string descricao)
        {
            try
            {
                if (Session["Admin"] != null)
                {

                    string caminhoCompleto = Path.Combine(Server.MapPath("~/Produtos/"), path);
                    FileInfo f = new FileInfo(caminhoCompleto);
                    if (f.Exists == true)
                    {
                        if (nomeProduto != "" && descricao != "")
                        {
                            if (Entities.db.Produto.Count(s => s.Nome == nomeProduto) == 0 && Entities.db.Produto.Count(s => s.ImagemPath == "/Produtos/" + path) == 0)
                            {
                                Produto produto = new Produto();
                                produto.Nome = nomeProduto;
                                produto.ID_Categoria = categoria;
                                produto.PreçoPorQuantidade = preco;
                                produto.QuantidadeStock = qtdStock;
                                produto.ImagemPath = "/Produtos/" + path;
                                produto.ID_Marca = marca;
                                produto.Descrição = descricao;

                                if (desconto == 0) { produto.Desconto = null; }
                                else { produto.Desconto = desconto; }

                                produto.Descontinuado = 1;
                                produto.Destaque = 0;

                                Entities.db.Produto.Add(produto);
                                Entities.db.SaveChanges();
                                TempData["NovoProduto"] = "true";
                            }
                            else
                            {
                                TempData["NovoProduto"] = "ProdIguais";
                            }
                        }
                    }
                    else
                    {
                        TempData["NovoProduto"] = "FotoInvalida";
                    }

                    return RedirectToAction("Produtos");
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
        public ActionResult AlterarProduto(int id, string nomeProduto, int nomeCategoria, int preco, int qtdStock, string file, int desconto, bool? publicado, int marca, string descricao, bool? destaque)
        {
            try
            {
                if (Session["Admin"] != null)
                {
                    //Validar se a marca ou categoria estão ativas, se não tiverem dá erro
                    bool valid = Generic.AtualizarProd(id, nomeProduto, nomeCategoria, preco, qtdStock, file, desconto, publicado, marca, descricao, destaque, Server.MapPath("~/FicheiroJson/SettingsRickyShop.json"));

                    if (valid == true)
                        return RedirectToAction("Produtos", "Admin");
                    else
                        return RedirectToAction("Produtos", "Admin");
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
        public ActionResult CriarMarca(string marca)
        {
            try
            {
                if (Session["Admin"] != null)
                {
                    if (Entities.db.MarcaProduto.Count(s => s.Marca.ToLower() == marca.ToLower()) == 0)
                    {
                        MarcaProduto m = new MarcaProduto();
                        m.Marca = marca;
                        Entities.db.MarcaProduto.Add(m);
                        Entities.db.SaveChanges();
                        TempData["AddMarca"] = "true";
                    }
                    else
                    {
                        TempData["AddMarca"] = "false";
                    }
                    return RedirectToAction("Produtos");
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
        public ActionResult ViewAddProduto()
        {
            if (Session["Admin"] != null)
            {
                return PartialView("CriarProduto");
            }
            else
            {
                return RedirectToAction("Inicio", "Home");
            }
        }
        public ActionResult ViewAddMarca()
        {
            if (Session["Admin"] != null)
            {
                return PartialView("CriarMarca");
            }
            else
            {
                return RedirectToAction("Inicio", "Home");
            }
        }
        public ActionResult Utilizadores(int? pagina)
        {
            try
            {
                if (Session["Admin"] != null)
                {
                    int tamanhoPagina = 10;
                    int numeroPagina = pagina ?? 1;

                    var u = Entities.db.Utilizadores.Where(s => s.Estado == 1).ToList().ToPagedList(numeroPagina, tamanhoPagina);

                    return View(u);
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
        public ActionResult ViewDescontoUserDetails(int id)
        {
            try
            {
                if (Session["Admin"] != null)
                {

                    // Retorne a exibição do modal
                    var u = Entities.db.Utilizadores.Where(s => s.ID_Utilizador == id).ToList();
                    return PartialView("DescontoUserDetails", u);
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
        public ActionResult AtribuirDesconto(int id, int valDesconto)
        {
            try
            {
                if (Session["Admin"] != null)
                {

                    var u = Entities.db.Utilizadores.Where(s => s.ID_Utilizador == id).FirstOrDefault();

                    u.Desconto = valDesconto;
                    Entities.db.SaveChanges();

                    return RedirectToAction("TemplateNotificacaoDesconto", new { id });
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
        public ActionResult ViewMovConta(int id)
        {
            try
            {
                if (Session["Admin"] != null)
                {

                    // Retorne a exibição do modal
                    var u = Entities.db.MovimentacaoSaldo.Where(s => s.ID_Utilizador == id).ToList();
                    return PartialView("MovimentacoesContaDetails", u);
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
        public ActionResult ChartNewUser()
        {
            try
            {
                // Obtenha os dados do gráfico do seu modelo ou de qualquer outra fonte de dados

                string[] meses = new string[] { "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" };
                int[] auxCont = new int[11];
                int mes = 0;
                foreach (var item in Entities.db.Utilizadores)
                {
                    mes = item.DataDeAdesao.Month - 1;
                    auxCont[mes] += 1;
                }

                var chartData = new
                {
                    labels = meses,
                    datasets = new[]
                    {
            new
            {
                label = "Qtd. Novos Users",
                data = auxCont,
                backgroundColor = "rgba(65, 255, 30, 0.5)",
                borderColor = "rgba(65, 255, 30, 1)",
                borderWidth = 1,
            }
        }
                };

                return Json(chartData, JsonRequestBehavior.AllowGet);
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
        public ActionResult ReportarAcesso(int id)
        {
            try
            {
                if (Session["Admin"] != null)
                {
                    var l = Entities.db.Logs.Where(s => s.ID_Log == id).FirstOrDefault();
                    var u = Entities.db.Utilizadores.Where(s => s.ID_Utilizador == l.ID_Utilizador).FirstOrDefault();

                    string subject = "Reportar tentativa de Acesso";

                    string path = Server.MapPath(@"~\Views\Admin\TemplateReporte.cshtml");

                    var conteudo = System.IO.File.ReadAllText(path);

                    conteudo = conteudo.Replace("[NomeCliente]", u.PrimeiroNome + " " + u.SegundoNome);
                    conteudo = conteudo.Replace("[IP]", l.IP_TentativaLogin.ToString());
                    conteudo = conteudo.Replace("[Data]", l.Erro_Login.ToString());

                    string body = conteudo;

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

                    Entities.db.SaveChanges();

                    TempData["EnvioReporte"] = "true";
                    return RedirectToAction("Produtos");
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
        public ActionResult Pedidos(int? pagina)
        {
            try
            {
                if (Session["Admin"] != null)
                {
                    IPagedList<Pedidos> p;
                    int tamanhoPagina = 10;
                    int numeroPagina = pagina ?? 1;

                    Generic.AtualizarEstadoPedidos();
                    p = Entities.db.Pedidos.ToList().ToPagedList(numeroPagina, tamanhoPagina);

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
        public ActionResult ChartUserMaisGastador()
        {

            try
            {
                // Obtenha os dados do gráfico do seu modelo ou de qualquer outra fonte de dados

                string[] nomes = new string[5];
                double[] val = new double[5];

                int cnt = 0;
                foreach (var item in Entities.db.TOP5_UserMaisGastador())
                {
                    nomes[cnt] = Entities.db.Utilizadores.FirstOrDefault(s => s.ID_Utilizador == item.ID_Utilizador).PrimeiroNome + " " + Entities.db.Utilizadores.FirstOrDefault(s => s.ID_Utilizador == item.ID_Utilizador).SegundoNome;
                    val[cnt] = Convert.ToDouble(item.preco_total);
                    cnt++;
                }

                var chartData = new
                {
                    labels = nomes,
                    datasets = new[]
                    {
            new
            {
                data = val,
                backgroundColor = "rgba(65, 255, 30, 0.5)",
                borderColor = "rgba(65, 255, 30, 1)",
                borderWidth = 1,
            }
        }
                };

                return Json(chartData, JsonRequestBehavior.AllowGet);
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
        public ActionResult ViewPedidosDetails(int id)
        {
            try
            {
                if (Session["Admin"] != null)
                {
                    var p = Entities.db.PedidosDetalhes.Where(s => s.ID_Pedido == id).ToList();
                    return PartialView("PedidosDetails", p);
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
        public ActionResult ChartUserMaisComprador()
        {
            try
            {
                // Obtenha os dados do gráfico do seu modelo ou de qualquer outra fonte de dados

                string[] nomes = new string[5];
                double[] qnt = new double[5];

                int cnt = 0;
                foreach (var item in Entities.db.TOP5_UserMaisComprador())
                {
                    nomes[cnt] = Entities.db.Utilizadores.FirstOrDefault(s => s.ID_Utilizador == item.ID_Utilizador).PrimeiroNome + " " + Entities.db.Utilizadores.FirstOrDefault(s => s.ID_Utilizador == item.ID_Utilizador).SegundoNome;
                    qnt[cnt] = Convert.ToDouble(item.total_vendido);
                    cnt++;
                }

                var chartData = new
                {
                    labels = nomes,
                    datasets = new[]
                    {
            new
            {
                data = qnt,
                backgroundColor = "rgba(65, 255, 30, 0.5)",
                borderColor = "rgba(65, 255, 30, 1)",
                borderWidth = 1,
            }
        }
                };

                return Json(chartData, JsonRequestBehavior.AllowGet);
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
        public ActionResult ViewAddCategoria()
        {
            if (Session["Admin"] != null)
            {
                return PartialView("CriarCategoria");
            }
            else
            {
                return RedirectToAction("Inicio", "Home");
            }
        }
        public ActionResult CriarCategoria(string categoria)
        {
            try
            {
                if (Session["Admin"] != null)
                {
                    if (Entities.db.Categoria.Count(s => s.NomeCategoria.ToLower() == categoria.ToLower()) == 0)
                    {
                        Categoria c = new Categoria();
                        c.NomeCategoria = categoria;
                        Entities.db.Categoria.Add(c);
                        Entities.db.SaveChanges();
                        TempData["AddCategoria"] = "true";
                    }
                    else
                    {
                        TempData["AddCategoria"] = "false";
                    }
                    return RedirectToAction("Produtos");
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
        public ActionResult EstadoCategoria(int id, string tipo)
        {
            try
            {
                if (Session["Admin"] != null)
                {
                    var c = Entities.db.Categoria.Where(s => s.ID_Categoria == id).FirstOrDefault();

                    if (tipo == "des")
                    {
                        c.Estado = 0;
                        Entities.db.Categoria.Where(s => s.ID_Categoria == id).FirstOrDefault().Estado = 0;

                        foreach (var item in Entities.db.Produto.Where(s => s.ID_Categoria == c.ID_Categoria))
                        {
                            Entities.db.Produto.Where(s => s.ID_Marca == c.ID_Categoria).FirstOrDefault().Descontinuado = 0;
                            item.Descontinuado = 0;
                        }

                    }
                    else
                    {
                        c.Estado = 1;
                        Entities.db.Categoria.Where(s => s.ID_Categoria == id).FirstOrDefault().Estado = 1;

                        foreach (var item in Entities.db.Produto.Where(s => s.ID_Categoria == c.ID_Categoria))
                        {
                            Entities.db.Produto.Where(s => s.ID_Marca == c.ID_Categoria).FirstOrDefault().Descontinuado = 1;
                            item.Descontinuado = 1;
                        }
                    }

                    Entities.db.SaveChanges();
                    return RedirectToAction("Produtos");
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
        public ActionResult RemoverMarca(int id)
        {
            try
            {
                if (Session["Admin"] != null)
                {
                    var m = Entities.db.MarcaProduto.Where(s => s.ID_Marca == id).FirstOrDefault();
                    Entities.db.MarcaProduto.Remove(m);
                    Entities.db.SaveChanges();
                    return RedirectToAction("Produtos");
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
        public ActionResult RemoverCategoria(int id)
        {
            try
            {
                if (Session["Admin"] != null)
                {

                    var c = Entities.db.Categoria.Where(s => s.ID_Categoria == id).FirstOrDefault();
                    Entities.db.Categoria.Remove(c);
                    Entities.db.SaveChanges();
                    return RedirectToAction("Produtos");
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
        public ActionResult ChartEstatisticasMesesCompras()
        {
            try
            {
                // Obtenha os dados do gráfico do seu modelo ou de qualquer outra fonte de dados

                string[] meses = new string[] { "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" };
                int[] auxCont = new int[11];
                int mes = 0;
                foreach (var item in Entities.db.Pedidos)
                {
                    mes = item.DataDoPedido.Month - 1;
                    auxCont[mes] += 1;
                }

                var chartData = new
                {
                    labels = meses,
                    datasets = new[]
                    {
            new
            {
                label = "Qtd. Pedidos",
                data = auxCont,
                backgroundColor = "rgba(65, 255, 30, 0.5)",
                borderColor = "rgba(65, 255, 30, 1)",
                borderWidth = 1,
            }
        }
                };

                return Json(chartData, JsonRequestBehavior.AllowGet);
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
        public ActionResult ChartProdutosMenosStock()
        {
            try
            {
                // Obtenha os dados do gráfico do seu modelo ou de qualquer outra fonte de dados

                string[] nomes = new string[5];
                double[] qnt = new double[5];

                int cnt = 0;
                foreach (var item in Entities.db.TOP5_ProdutosMenosStock())
                {
                    nomes[cnt] = item.Nome;
                    qnt[cnt] = Convert.ToDouble(item.QuantidadeStock);
                    cnt++;
                }

                var chartData = new
                {
                    labels = nomes,
                    datasets = new[]
                    {
            new
            {
                data = qnt,
                backgroundColor = "rgba(65, 255, 30, 0.5)",
                borderColor = "rgba(65, 255, 30, 1)",
                borderWidth = 1,
            }
        }
                };

                return Json(chartData, JsonRequestBehavior.AllowGet);
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
        public ActionResult ConfigSite()
        {
            try
            {
                if (Session["Admin"] != null)
                {
                    if (TempData["FecharReporte"] != null)
                    {
                        Response.Write($"<script>alert('Reporte concluido!')</script>");
                    }

                    return View(Generic.ValSettings(Server.MapPath("~/FicheiroJson/SettingsRickyShop.json")));
                }
                else
                {
                    return RedirectToAction("Inicio", "Home");
                }
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return RedirectToAction("NotFound", "Error");
            }
        }

        [HttpPost]
        public ActionResult ConfigSite(DadosSettingsSite s)
        {
            try
            {
                if (Session["Admin"] != null)
                {

                    // Objeto que você deseja serializar em JSON
                    var objeto = s;

                    // Caminho e nome do arquivo onde será salvo o JSON
                    var caminhoArquivo = Server.MapPath("~/FicheiroJson/SettingsRickyShop.json");

                    // Serializa o objeto em JSON
                    var json = JsonConvert.SerializeObject(objeto);

                    // Grava o JSON no arquivo
                    System.IO.File.WriteAllText(caminhoArquivo, json);
                    Response.Write($"<script>alert('Configuração salva.')</script>");
                    return View();
                }
                else
                {
                    return RedirectToAction("Inicio", "Home");
                }
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert({ex.Message});</script>");
                return RedirectToAction("NotFound", "Error");
            }
        }
        public ActionResult ViewReporteDetails(int id)
        {
            try
            {
                if (Session["Admin"] != null)
                {
                    // Retorne a exibição do modal
                    var p = Entities.db.Reporte.Where(s => s.ID_Reporte == id).FirstOrDefault();
                    return PartialView("ReporteDetails", p);
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
        public ActionResult FecharReporte(int id)
        {
            try
            {
                if (Session["Admin"] != null)
                {

                    var r = Entities.db.Reporte.FirstOrDefault(s => s.ID_Reporte == id);

                    r.Estado_Reporte = 1;
                    Entities.db.SaveChanges();

                    TempData["FecharReporte"] = "true";
                    return RedirectToAction("ConfigSite");
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
        public ActionResult AddImagemPromo(HttpPostedFileBase file)
        {
            try
            {
                if (Session["Admin"] != null)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string path = Path.Combine(Server.MapPath("~/ImgPromo/"), fileName);
                    FileInfo fileInfo = new FileInfo(path);


                    string extension = Path.GetExtension(fileName);

                    switch (extension.ToLower())
                    {
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                            file.SaveAs(path);
                            TempData["AddImagem"] = "true";
                            return RedirectToAction("Home");
                        default://Meter mensagens de erro e avisar que só aquelas extesnsoes são validas
                            TempData["AddImagem"] = "false";
                            return RedirectToAction("Home");
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
        public ActionResult CriarPromo(int? categoria, int? marca, string descricao, DateTime dataFim, HttpPostedFileBase file, string nome, int desconto, string opcao)
        {
            //1 == categoria : 2 == marca

            try
            {
                if (Session["Admin"] != null)
                {

                    //Validar erros, categoria null etc
                    string caminhoCompleto = Path.Combine(Server.MapPath("~/ImgPromo/"), file.FileName);
                    FileInfo f = new FileInfo(caminhoCompleto);

                    if (f.Exists == true)
                    {
                        if (opcao == "1")
                        {
                            Promo p = new Promo();

                            p.Nome = nome;
                            p.Descrição = descricao;
                            p.Path_Logo = "/ImgPromo/" + file.FileName;
                            p.InicioData_Promo = DateTime.Now;
                            p.FimData_Promo = dataFim;
                            p.Desconto = desconto;
                            p.ID_Categoria = categoria;

                            foreach (var item in Entities.db.Produto.Where(s => s.ID_Categoria == categoria))
                            {
                                item.Desconto = desconto;
                            }

                            Entities.db.Promo.Add(p);
                            Entities.db.SaveChanges();
                        }
                        else
                        {
                            Promo p = new Promo();

                            p.Nome = nome;
                            p.Descrição = descricao;
                            p.Path_Logo = "/ImgPromo/" + file.FileName;
                            p.InicioData_Promo = DateTime.Now;
                            p.FimData_Promo = dataFim;
                            p.Desconto = desconto;
                            p.ID_Marca = marca;

                            foreach (var item in Entities.db.Produto.Where(s => s.ID_Marca == marca))
                            {
                                item.Desconto = desconto;
                            }

                            Entities.db.Promo.Add(p);
                            Entities.db.SaveChanges();

                        }

                        var dados = Generic.ValSettings(Server.MapPath("~/FicheiroJson/SettingsRickyShop.json"));
                        DadosSettingsSite d = new DadosSettingsSite();
                        d.ValidPosPromo = 1;
                        d.QtdProdutosPagina = dados.QtdProdutosPagina;
                        d.QtdProdutosDestaque = dados.QtdProdutosDestaque;
                        Generic.EscreverJson(Server.MapPath("~/FicheiroJson/SettingsRickyShop.json"), d);
                        TempData["NovaPromo"] = "true";
                        return RedirectToAction("TemplateNovaPromo");
                    }
                    else
                    {
                        TempData["NovaPromo"] = "ImgForaDaPasta";
                        return RedirectToAction("Home");
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
        public ActionResult TemplateNovaPromo()
        {
            try
            {
                if (Session["Admin"] != null)
                {
                    foreach (var item in Entities.db.Utilizadores)
                    {
                        var p = Entities.db.Promo.FirstOrDefault(s => s.InicioData_Promo == DateTime.Today);

                        string subject = "Promoções? Só na RickyShop!";

                        string path = Server.MapPath(@"~\Views\Admin\TemplateNovaPromo.cshtml");

                        var conteudo = System.IO.File.ReadAllText(path);

                        conteudo = conteudo.Replace("[NomeCliente]", item.PrimeiroNome + " " + item.SegundoNome);
                        conteudo = conteudo.Replace("[Descricao]", p.Descrição);
                        conteudo = conteudo.Replace("[DataDeInicio]", p.InicioData_Promo.ToString("dd/MM/yyyy"));
                        conteudo = conteudo.Replace("[DataFim]", p.FimData_Promo.ToString("dd/MM/yyyy"));

                        string body = conteudo;

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
                        objEmail.To.Add(item.Email);

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


                    }


                    TempData["MensagemAviso"] = "true";
                    return RedirectToAction("Home");
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
        public ActionResult ViewPromo(int id)
        {
            if (Session["Admin"] != null)
            {
                return PartialView("PromoDetails", Entities.db.Promo.FirstOrDefault(s => s.ID_Promo == id));
            }
            else
            {
                return RedirectToAction("Inicio", "Home");
            }
        }

        public ActionResult TemplateNotificacaoDesconto(int id)
        {
            try
            {
                if (Session["Admin"] != null)
                {
                    var u = Entities.db.Utilizadores.FirstOrDefault(s => s.ID_Utilizador == id);

                    string subject = "Promoções? Só na RickyShop!";

                    string path = Server.MapPath(@"~\Views\Admin\TemplateNotificacaoDesconto.cshtml");

                    var conteudo = System.IO.File.ReadAllText(path);

                    conteudo = conteudo.Replace("[NomeCliente]", u.PrimeiroNome + " " + u.SegundoNome);
                    conteudo = conteudo.Replace("[Desconto]", u.Desconto.ToString());

                    string body = conteudo;

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

                    TempData["MensagemAviso"] = "true";
                    return RedirectToAction("Utilizadores");
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
    }
}
