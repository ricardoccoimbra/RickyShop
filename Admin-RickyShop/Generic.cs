using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin_RickyShop
{
    public static class Generic
    {
        public static List<EstatisticaProdutos> produtosVendidos = new List<EstatisticaProdutos>();

        public static string[] GetNomeProduto(List<EstatisticaProdutos> produtos)
        {
            string[] nome = new string[produtos.Count];

            for (int i = 0; i < produtos.Count; i++)
            { nome[i] = produtos[i]._nomeProduto; }

            return nome;
        }

        public static int[] GetQtdProduto(List<EstatisticaProdutos> produtos)
        {
            int[] qnt = new int[produtos.Count];

            for (int i = 0; i < produtos.Count; i++)
            { qnt[i] = produtos[i]._QtdProduto; }

            return qnt;
        }

    }
}
