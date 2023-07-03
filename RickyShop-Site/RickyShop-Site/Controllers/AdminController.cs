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

namespace RickyShop_Site.Controllers
{
    public class AdminController : Controller
    {
        //GestãoRickyShopEntities Entities.db = new GestãoRickyShopEntities();

        // GET: Admin
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult Produtos(int? pagina)
        {
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
        public ActionResult ViewProdutosDetails(int id)
        {
            // Retorne a exibição do modal
            var p = Entities.db.Produto.Where(s => s.ID_Produto == id).ToList();
            return PartialView("ProdutosDetails", p);
        }
        public ActionResult CriarProduto()
        {
            return PartialView("CriarProduto");
        }
        public ActionResult AddImagemProduto(HttpPostedFileBase file)
        {
            string fileName = Path.GetFileName(file.FileName);
            string path = Path.Combine(Server.MapPath("~/Produtos/"), fileName);
            FileInfo fileInfo = new FileInfo(path);

            if (fileInfo.Exists != true)
            {
                string extension = Path.GetExtension(fileName);

                switch (extension.ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                        file.SaveAs(path);
                        return null;
                    default://Meter mensagens de erro e returnar certo
                        ModelState.AddModelError("file", "Apenas arquivos .jpg, .jpeg e .png são permitidos.");
                        return null;
                }
            }
            else
            {
                Response.Write($"<script>alert('Não existe nenhum produto pesquisado!')</script>");
                return null;
            }

        }
        public ActionResult ExportarExcel()
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
            worksheet.Cells["J1"].Value = "Genero";
            worksheet.Cells["K1"].Value = "Publicado";
            worksheet.Cells["L1"].Value = "Destaque";


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
                worksheet.Cells[linha, c + 10].Value = item.Genero;
                worksheet.Cells[linha, c + 11].Value = item.Descontinuado;
                worksheet.Cells[linha, c + 12].Value = item.Destaque;
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
            return null;
        }
        public ActionResult FiltroProdutos(int marca)
        {
            Session["FiltroProd"] = marca;
            return RedirectToAction("Produtos", "Admin");
        }
        public ActionResult ChartProdutoMaisVendido()
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
        public ActionResult ChartMarcaMaisVendida()
        {
            // Obtenha os dados do gráfico do seu modelo ou de qualquer outra fonte de dados

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
        public ActionResult EstadoMarca(int idM, string tipo)
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
        public ActionResult CriarProduto(string nomeProduto, int categoria, int preco, int qtdStock, string path, int desconto, int marca, string descricao)
        {
            string caminhoCompleto = Path.Combine(Server.MapPath("~/Produtos/"), path);
            FileInfo f = new FileInfo(caminhoCompleto);
            if (f.Exists == true)
            {
                if (nomeProduto != "" && descricao != "")
                {
                    if (Entities.db.Produto.Count(s => s.ImagemPath == "/Produtos/" + path || s.Nome == nomeProduto) == 0)
                    {
                        Produto produto = new Produto();
                        produto.Nome = nomeProduto;
                        produto.ID_Categoria = categoria;
                        produto.PreçoPorQuantidade = preco;
                        produto.QuantidadeStock = qtdStock;
                        produto.ImagemPath = "/Produto/" + path;
                        produto.ID_Marca = marca;
                        produto.Descrição = descricao;

                        if (desconto == 0) { produto.Desconto = null; }
                        else { produto.Desconto = desconto; }

                        produto.Descontinuado = 1;
                        produto.Destaque = 0;

                        Entities.db.Produto.Add(produto);
                        Entities.db.SaveChanges();
                    }
                    else
                    {
                        //existe produtis iguais

                    }
                }
            }
            else
            {
                Console.WriteLine("O arquivo existe na pasta.");
            }

            return View("Details");
        }
        public ActionResult AlterarProduto(int id, string nomeProduto, int nomeCategoria, int preco, int qtdStock, string file, int desconto, bool? publicado, int marca, string descricao, bool? destaque)
        {
            //Validar se a marca ou categoria estão ativas, se não tiverem dá erro
            bool valid = Generic.AtualizarProd(id, nomeProduto, nomeCategoria, preco, qtdStock, file, desconto, publicado, marca, descricao, destaque, Server.MapPath("~/FicheiroJson/SettingsRickyShop.json"));

            if (valid == true)
                return RedirectToAction("Produtos", "Admin");
            else
                return RedirectToAction("Produtos", "Admin");
        }
        public ActionResult CriarMarca(string marca)
        {
            if (Entities.db.MarcaProduto.Count(s => s.Marca.ToLower() == marca.ToLower()) == 0)
            {
                MarcaProduto m = new MarcaProduto();
                m.Marca = marca;
                Entities.db.MarcaProduto.Add(m);
                Entities.db.SaveChanges();
            }
            else
            {
                Response.Write($"<script>alert('Já existe uma marca com esse nome!')</script>");
            }
            return RedirectToAction("Produtos");
        }
        public ActionResult ViewAddProduto()
        {
            return PartialView("CriarProduto");
        }
        public ActionResult ViewAddMarca()
        {
            return PartialView("CriarMarca");
        }
        public ActionResult Utilizadores(int? pagina)
        {
            int tamanhoPagina = 10;
            int numeroPagina = pagina ?? 1;

            var u = Entities.db.Utilizadores.ToList().ToPagedList(numeroPagina, tamanhoPagina);

            return View(u);
        }
        public ActionResult ViewDescontoUserDetails(int id)
        {
            // Retorne a exibição do modal
            var u = Entities.db.Utilizadores.Where(s => s.ID_Utilizador == id).ToList();
            return PartialView("DescontoUserDetails", u);
        }
        public ActionResult AtribuirDesconto(int id, int valDesconto)
        {
            var u = Entities.db.Utilizadores.Where(s => s.ID_Utilizador == id).FirstOrDefault();

            u.Desconto = valDesconto;
            Entities.db.SaveChanges();

            return RedirectToAction("Utilizadores");
        }
        public ActionResult ViewMovConta(int id)
        {
            // Retorne a exibição do modal
            var u = Entities.db.MovimentacaoSaldo.Where(s => s.ID_Utilizador == id).ToList();
            return PartialView("MovimentacoesContaDetails", u);
        }
        public ActionResult ChartNewUser()
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
        public ActionResult ReportarAcesso(int id)
        {
            var l = Entities.db.Logs.Where(s => s.ID_Log == id).FirstOrDefault();
            var u = Entities.db.Utilizadores.Where(s => s.ID_Utilizador == l.ID_Utilizador).FirstOrDefault();

            string subject = "Reportar tentativa de Acesso";


            //string body = "Olá, a seu token de recuperação é: " + tokenAleatorio +
            //    "\r De seguida altere a sua password no site. \n Cumprimentos, ";

            string path = Server.MapPath(@"~\Views\Admin\TemplateReporte.cshtml");

            var conteudo = System.IO.File.ReadAllText(path);

            conteudo = conteudo.Replace("[NomeCliente]", u.PrimeiroNome + " " + u.SegundoNome);
            conteudo = conteudo.Replace("[IP]", l.IP_TentativaLogin.ToString());
            conteudo = conteudo.Replace("[Data]", l.Erro_Login.ToString());

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
            return RedirectToAction("Produtos");
        }
        public ActionResult Pedidos(int? pagina)
        {
            IPagedList<Pedidos> p;
            int tamanhoPagina = 10;
            int numeroPagina = pagina ?? 1;
            p = Entities.db.Pedidos.ToList().ToPagedList(numeroPagina, tamanhoPagina);

            return View(p);
        }
        public ActionResult ChartUserMaisGastador()
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
        public ActionResult ViewPedidosDetails(int id)
        {
            var p = Entities.db.PedidosDetalhes.Where(s => s.ID_Pedido == id).ToList();
            return PartialView("PedidosDetails", p);
        }
        public ActionResult ChartUserMaisComprador()
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
        public ActionResult ViewAddCategoria()
        {
            return PartialView("CriarCategoria");
        }
        public ActionResult CriarCategoria(string categoria)
        {
            if (Entities.db.Categoria.Count(s => s.NomeCategoria.ToLower() == categoria.ToLower()) == 0)
            {
                Categoria c = new Categoria();
                c.NomeCategoria = categoria;
                Entities.db.Categoria.Add(c);
                Entities.db.SaveChanges();
            }
            else
            {
                Response.Write($"<script>alert('Já existe uma categoria com esse nome!')</script>");
            }
            return RedirectToAction("Produtos");
        }
        public ActionResult EstadoCategoria(int id, string tipo)
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

                foreach (var item in Entities.db.Produto.Where(s => s.ID_Marca == c.ID_Categoria))
                {
                    Entities.db.Produto.Where(s => s.ID_Marca == c.ID_Categoria).FirstOrDefault().Descontinuado = 1;
                    item.Descontinuado = 1;
                }
            }

            Entities.db.SaveChanges();
            return RedirectToAction("Produtos");
        }
        public ActionResult RemoverMarca(int id)
        {
            var m = Entities.db.MarcaProduto.Where(s => s.ID_Marca == id).FirstOrDefault();
            Entities.db.MarcaProduto.Remove(m);
            Entities.db.SaveChanges();
            return RedirectToAction("Produtos");
        }
        public ActionResult RemoverCategoria(int id)
        {
            var c = Entities.db.Categoria.Where(s => s.ID_Categoria == id).FirstOrDefault();
            Entities.db.Categoria.Remove(c);
            Entities.db.SaveChanges();
            return RedirectToAction("Produtos");
        }
        public ActionResult ChartEstatisticasMesesCompras()
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
        public ActionResult ChartProdutosMenosStock()
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
        public ActionResult EscreverJson()
        {
            // Objeto que você deseja serializar em JSON
            var objeto = new DadosSettingsSite();

            // Caminho e nome do arquivo onde será salvo o JSON
            var caminhoArquivo = Server.MapPath("~/FicheiroJson/SettingsRickyShop.json");

            // Serializa o objeto em JSON
            var json = JsonConvert.SerializeObject(objeto);

            // Grava o JSON no arquivo
            System.IO.File.WriteAllText(caminhoArquivo, json);

            return View();
        }
        public ActionResult ConfigSite()
        {
            return View(Generic.ValSettings(Server.MapPath("~/FicheiroJson/SettingsRickyShop.json")));
        }

        [HttpPost]
        public ActionResult ConfigSite(DadosSettingsSite s)
        {
            // Objeto que você deseja serializar em JSON

            var objeto = s;

            // Caminho e nome do arquivo onde será salvo o JSON
            var caminhoArquivo = Server.MapPath("~/FicheiroJson/SettingsRickyShop.json");

            // Serializa o objeto em JSON
            var json = JsonConvert.SerializeObject(objeto);

            // Grava o JSON no arquivo
            System.IO.File.WriteAllText(caminhoArquivo, json);

            return View();
        }
        public ActionResult ViewReporteDetails(int id)
        {
            // Retorne a exibição do modal
            var p = Entities.db.Reporte.Where(s => s.ID_Reporte == id).FirstOrDefault();
            return PartialView("ReporteDetails", p);
        }
        public ActionResult FecharReporte(int id)
        {
            var r = Entities.db.Reporte.FirstOrDefault(s => s.ID_Reporte == id);

            r.Estado = 1;
            Entities.db.SaveChanges();

            return RedirectToAction("ConfigSite");
        }
        public ActionResult AddImagemPromo(HttpPostedFileBase file)
        {
            string fileName = Path.GetFileName(file.FileName);
            string path = Path.Combine(Server.MapPath("~/ImgPromo/"), fileName);
            FileInfo fileInfo = new FileInfo(path);

            if (fileInfo.Exists != true)
            {
                string extension = Path.GetExtension(fileName);

                switch (extension.ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                        file.SaveAs(path);
                        return null;
                    default://Meter mensagens de erro e avisar que só aquelas extesnsoes são validas
                        ModelState.AddModelError("file", "Apenas arquivos .jpg, .jpeg e .png são permitidos.");
                        return null;
                }
            }
            else
            {
                //Meter aviso que precisa de imagem
                Response.Write($"<script>alert('Não existe nenhum produto pesquisado!')</script>");
                return null;
            }

        }
        public ActionResult CriarPromo(int? categoria, int? marca, string descricao, DateTime dataFim, HttpPostedFileBase file, string nome, int desconto, string opcao)
        {
            //1 == categoria : 2 == marca

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
            }
            //return RedirectToAction("TemplateNovaPromo");
            return RedirectToAction("Home");
        }
        public ActionResult TemplateNovaPromo() 
        {
            foreach (var item in Entities.db.Utilizadores)
            {
                var p = Entities.db.Promo.FirstOrDefault(s => s.InicioData_Promo == DateTime.Today);

                string subject = "Promoções? Só na RickyShop!";

                //string body = "Olá, a seu token de recuperação é: " + tokenAleatorio +
                //    "\r De seguida altere a sua password no site. \n Cumprimentos, ";

                string path = Server.MapPath(@"~\Views\Admin\TemplateNovaPromo.cshtml");

                var conteudo = System.IO.File.ReadAllText(path);

                conteudo = conteudo.Replace("[NomeCliente]", item.PrimeiroNome + " " + item.SegundoNome);
                conteudo = conteudo.Replace("[Descricao]", p.Descrição);
                conteudo = conteudo.Replace("[DataDeInicio]", p.InicioData_Promo.ToString("dd/MM/yyyy"));
                conteudo = conteudo.Replace("[DataFim]", p.FimData_Promo.ToString("dd/MM/yyyy"));

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

                //WebMail.EnableSsl = true;
                //WebMail.SmtpServer = "smtp.mail.yahoo.com";
                //WebMail.SmtpPort = 465;
                //WebMail.UserName = "Ricardo Coimbra";
                //WebMail.Password = "Pitolni08";
                //WebMail.From = "cruzcoimbra08@yahoo.com";

            }


            TempData["MensagemAviso"] = "true";
            return RedirectToAction("Home");
        }
        public ActionResult ViewPromo(int id)
        {

            return PartialView("PromoDetails", Entities.db.Promo.FirstOrDefault(s => s.ID_Promo == id));
        }
    }
}
