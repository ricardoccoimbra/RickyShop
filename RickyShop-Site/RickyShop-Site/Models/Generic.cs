using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;


namespace RickyShop_Site.Models
{
    public static class Generic
    {
        public static GestãoRickyShopEntities db = new GestãoRickyShopEntities();

        public static string VerifRegisto(string _NIF, string _Contacto, string _Email, DateTime _date)
        {
            string i = "certo";

            var nif = Entities.db.Utilizadores.Count(s => s.NIF == _NIF);
            var num = Entities.db.Utilizadores.Count(s => s.Contacto == _Contacto);
            var mail = Entities.db.Utilizadores.Count(s => s.Email == _Email);

            if (nif == 0)
            {
                if (num == 0)
                {
                    if (mail == 0)
                    {
                        return i;
                    }
                    else
                    {
                        i = "email";
                        return i;
                    }
                }
                else
                {
                    i = "num";
                    return i;
                }
            }
            else
            {
                i = "nif";
                return i;
            }
        }

        public static string TokenAleatorio()
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();

            for (int i = 0; i < 7; i++)
            {
                int index = rnd.Next(chars.Length);
                sb.Append(chars[index]);
            }

            return sb.ToString();
        }

        public static void MarcaID()
        {

            List<string> ls = new List<string>();

            foreach (var item in db.MarcaProduto)
            {
                ls.Add(item.Marca);
            }
        }

        public static IPagedList Filtros(string filtros, int id, IQueryable<Produto> produtos, int? pagina, int numeroPagina, int tamanhoPagina)
        {
            IPagedList<Produto> prodPage;

            var filtro = filtros.ToString().Split('-');
            if (filtro[0].ToString() != "nada")
            {
                var filtro2 = filtro[1].ToString();
                int n = Convert.ToInt32(filtro[0]);


                if (id == 0)
                {
                    if (filtro2 == "PrecoCres")
                    {
                        prodPage = produtos.Where(s => s.ID_Marca == n && s.Desconto != null && s.Descontinuado == 1).OrderBy(s => s.PreçoPorQuantidade).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                        prodPage.FirstOrDefault().ID_Categoria = id;
                        return prodPage;
                    }
                    else
                    {
                        prodPage = produtos.Where(s => s.ID_Marca == n && s.Desconto != null && s.Descontinuado == 1).OrderByDescending(s => s.PreçoPorQuantidade).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                        prodPage.FirstOrDefault().ID_Categoria = id;
                        return prodPage;
                    }
                }
                else
                {
                    if (filtro2 == "PrecoCres")
                    {
                        prodPage = produtos.Where(s => s.ID_Marca == n && s.ID_Categoria == id && s.Descontinuado == 1).OrderBy(s => s.PreçoPorQuantidade).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                        return prodPage;
                    }
                    else
                    {
                        prodPage = produtos.Where(s => s.ID_Marca == n && s.ID_Categoria == id && s.Descontinuado == 1).OrderByDescending(s => s.PreçoPorQuantidade).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                        return prodPage;
                    }
                }

            }
            else
            {
                if (id == 0)
                {
                    if (filtro[1].ToString() == "PrecoCres")
                    {
                        prodPage = produtos.Where(s => s.Desconto != null && s.Descontinuado == 1).OrderBy(s => s.PreçoPorQuantidade).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                        prodPage.FirstOrDefault().ID_Categoria = id;
                        return prodPage;
                    }
                    else
                    {
                        prodPage = produtos.Where(s => s.Desconto != null && s.Descontinuado == 1).OrderByDescending(s => s.PreçoPorQuantidade).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                        prodPage.FirstOrDefault().ID_Categoria = id;
                        return prodPage;
                    }
                }

                else
                {
                    if (id != 4)
                    {
                        if (filtro[1].ToString() == "PrecoCres")
                        {
                            prodPage = produtos.Where(s => s.ID_Categoria == id && s.Descontinuado == 1).OrderBy(s => s.PreçoPorQuantidade).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                            return prodPage;
                        }
                        else
                        {
                            prodPage = produtos.Where(s => s.ID_Categoria == id && s.Descontinuado == 1).OrderByDescending(s => s.PreçoPorQuantidade).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                            return prodPage;
                        }
                    }
                    else
                    {
                        if (filtro[1].ToString() == "PrecoCres")
                        {
                            prodPage = produtos.Where(s => s.Descontinuado == 1 && s.Descontinuado == 1).OrderBy(s => s.PreçoPorQuantidade).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                            prodPage.FirstOrDefault().ID_Categoria = 4;
                            return prodPage;
                        }
                        else
                        {
                            prodPage = produtos.Where(s => s.Descontinuado == 1 && s.Descontinuado == 1).OrderByDescending(s => s.PreçoPorQuantidade).ToList().ToPagedList(numeroPagina, tamanhoPagina);
                            prodPage.FirstOrDefault().ID_Categoria = 4;
                            return prodPage;
                        }
                    }
                }
            }
        }

        public static decimal PrecoTotal(int id)
        {
            var u = Entities.db.Utilizadores.FirstOrDefault(s => s.ID_Utilizador == id);
            decimal total = 0;

            foreach (var item in Entities.db.DadosCarrinhoProduto(id))
            {
                if (item.Desconto == null)
                {
                    total += item.PreçoPorQuantidade * item.Quantidade;
                }
                else
                {
                    double valDesconto = Convert.ToDouble(item.PreçoPorQuantidade * item.Desconto) / 100;
                    total += (item.PreçoPorQuantidade - Convert.ToDecimal(valDesconto)) * item.Quantidade;
                }
            }

            if (u.Desconto != null)
            {
                decimal valDesconto = Convert.ToDecimal(total * u.Desconto) / 100;
                total -= valDesconto;
            }
            return total;
        }

        public static bool CompararPassHash(string pass1, string pass2)
        {
            SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(pass1));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            pass1 = builder.ToString();


            if (pass1 == pass2)
                return true;
            else
                return false;
        }

        public static string CriarPassHash(string pass)
        {
            SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(pass));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            pass = builder.ToString();

            return pass;
        }

        public static bool AtualizarProd(int id, string nomeProduto, int nomeCategoria, int preco, int qtdStock, string file, int desconto, bool? publicado, int marca, string descricao, bool? destaque, string caminhoPath)
        {
            Produto p = Entities.db.Produto.Where(s => s.ID_Produto == id).FirstOrDefault();
            if (file != "")
            {
                return true;
            }
            else
            {
                string pathImagem = "/Produtos/" + id + ".jpg";
                p.ImagemPath = pathImagem;
                p.ID_Marca = marca;
                p.ID_Categoria = nomeCategoria;
                p.QuantidadeStock = qtdStock;

                if (Entities.db.Produto.ToList().Where(s => s.ID_Produto != id).Count(s => s.Nome == nomeProduto) == 0)
                    p.Nome = nomeProduto; // Ver se o nome do produto ja existe
                else
                    return false;

                if (desconto == 0)
                    p.Desconto = null;
                else
                    p.Desconto = desconto;

                p.PreçoPorQuantidade = preco;
                p.Descrição = descricao;

                if (destaque == true)
                {
                    if (Entities.db.Produto.ToList().Where(s => s.ID_Produto != id).Count(s => s.Destaque == 1) != ValSettings(caminhoPath).QtdProdutosDestaque)
                    {
                        p.Destaque = 1;
                        p.Descontinuado = 1;
                    }
                }
                else
                    p.Destaque = 0;

                if (publicado == null && destaque == null)
                {
                    p.Descontinuado = 0;
                }
                else
                    p.Descontinuado = 1;


                Entities.db.SaveChanges();
                return true;
            }
        }

        public static DadosSettingsSite ValSettings(string caminhoArquivo /* ~/FicheiroJson/SettingsRickyShop.json */)
        {

            // Lê o conteúdo do arquivo JSON
            var json = System.IO.File.ReadAllText(caminhoArquivo);

            // Desserializa o JSON em um objeto ou estrutura de dados adequada
            var objeto = JsonConvert.DeserializeObject<DadosSettingsSite>(json);

            return objeto;
        }
    }
}