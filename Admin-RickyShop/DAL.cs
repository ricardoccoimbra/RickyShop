using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin_RickyShop
{
    public class DAL
    {

        private static string stringConnect = @"Data Source=.\SQLEXPRESS;Initial Catalog =GestãoRickyShop; Trusted_Connection=True;";
        public static SqlConnection GetConexao()
        {
            SqlConnection con = new SqlConnection(stringConnect);
            con.Open();
            return con;
        }

        public static void FecharConexao(SqlConnection con)
        {
            con.Close();
        }

        public static void GetProdutos()
        {
            var con = GetConexao();
            SqlCommand cmd = new SqlCommand("select ID_Produto, Nome, QuantidadeStock from Produto", con);
            SqlDataReader rdr = cmd.ExecuteReader();
            Generic.produtosVendidos.Clear();
            while (rdr.Read())
            {
                var x = rdr.GetValue(0).ToString();
                var a = rdr.GetValue(1).ToString();
                var d = rdr.GetValue(2).ToString();
                Generic.produtosVendidos.Add(new EstatisticaProdutos()
                { _IdProduto = Convert.ToInt32(rdr.GetValue(0)), _nomeProduto = rdr.GetValue(1).ToString(), _QtdProduto = Convert.ToInt32(rdr.GetValue(2)) });
            }


            rdr.Close();
            FecharConexao(con);
        }

    }
}
