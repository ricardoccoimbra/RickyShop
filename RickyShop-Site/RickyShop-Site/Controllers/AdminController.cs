using Microsoft.Ajax.Utilities;
using OfficeOpenXml;
using PagedList;
using RickyShop_Site.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RickyShop_Site.Controllers
{
    public class AdminController : Controller
    {
        GestãoRickyShopEntities db = new GestãoRickyShopEntities();

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
            { p = db.Produto.Where(s => s.MarcaProduto.Estado == 1).ToList().ToPagedList(numeroPagina, tamanhoPagina); }
            else
            {
                int marca = Convert.ToInt32(Session["FiltroProd"]);
                if (marca != -1)
                    p = db.Produto.Where(s => s.ID_Marca == marca).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                else
                    p = db.Produto.Where(s => s.MarcaProduto.Estado == 1).ToList().ToPagedList(numeroPagina, tamanhoPagina);
            }
            return View(p);
        }
        public ActionResult ViewProdutosDetails(int id)
        {
            // Retorne a exibição do modal
            var p = db.Produto.Where(s => s.ID_Produto == id).ToList();
            return PartialView("ProdutosDetails", p);
        }
        public ActionResult CriarProduto()
        {
            return PartialView("CriarProduto");
        }
        public ActionResult AddImagem(HttpPostedFileBase file)
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


            foreach (var item in db.Produto)
            {
                //Verificar o porque de não mostrar a ultima alinha
                worksheet.Cells[linha, c + 1].Value = item.ID_Produto;
                worksheet.Cells[linha, c + 2].Value = item.Nome;
                worksheet.Cells[linha, c + 3].Value = item.PreçoPorQuantidade;
                worksheet.Cells[linha, c + 4].Value = item.Desconto;
                worksheet.Cells[linha, c + 5].Value = item.QuantidadeStock;
                worksheet.Cells[linha, c + 6].Value = item.ImagemPath;
                worksheet.Cells[linha, c + 7].Value = item.Descrição;
                worksheet.Cells[linha, c + 8].Value = db.Categoria.FirstOrDefault(s => s.ID_Categoria == item.ID_Categoria).NomeCategoria;
                worksheet.Cells[linha, c + 9].Value = db.MarcaProduto.FirstOrDefault(s => s.ID_Marca == item.ID_Marca).Marca;
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
            foreach (var item in db.TOP5_ProdMaisVendidos())
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
            foreach (var item in db.TOP5_MarcasMaisVendidas())
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
            var m = db.MarcaProduto.Where(s => s.ID_Marca == idM).FirstOrDefault();

            if (tipo == "des")
            {
                m.Estado = 0;
                Entities.db.MarcaProduto.Where(s => s.ID_Marca == idM).FirstOrDefault().Estado = 0;

                foreach (var item in db.Produto.Where(s => s.ID_Marca == m.ID_Marca))
                {
                    Entities.db.Produto.Where(s => s.ID_Marca == m.ID_Marca).FirstOrDefault().Descontinuado = 0;
                    item.Descontinuado = 0;
                }

            }
            else
            {
                m.Estado = 1;
                Entities.db.MarcaProduto.Where(s => s.ID_Marca == idM).FirstOrDefault().Estado = 1;

                foreach (var item in db.Produto.Where(s => s.ID_Marca == m.ID_Marca))
                {
                    Entities.db.Produto.Where(s => s.ID_Marca == m.ID_Marca).FirstOrDefault().Descontinuado = 1;
                    item.Descontinuado = 1;
                }
            }

            db.SaveChanges();
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
                    if (db.Produto.Count(s => s.ImagemPath == "/Produtos/" + path || s.Nome == nomeProduto) == 0)
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

                        db.Produto.Add(produto);
                        db.SaveChangesAsync();
                    }
                    else
                    {


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
            bool valid = Generic.AtualizarProd(id, nomeProduto, nomeCategoria, preco, qtdStock, file, desconto, publicado, marca, descricao, destaque);

            if (valid == true)
                return RedirectToAction("Produtos", "Admin");
            else
                return RedirectToAction("Produtos", "Admin");
        }
        public ActionResult CriarMarca(string marca)
        {
            if (db.MarcaProduto.Count(s => s.Marca.ToLower() == marca.ToLower()) == 0)
            {
                MarcaProduto m = new MarcaProduto();
                m.Marca = marca;
                db.MarcaProduto.Add(m);
                db.SaveChanges();
            }
            else
            {
                Response.Write($"<script>alert('Já existe uma marca com esse nome!')</script>");
            }
            return null;
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

            var u = db.Utilizadores.ToList().ToPagedList(numeroPagina, tamanhoPagina);

            return View(u);
        }
    }
}