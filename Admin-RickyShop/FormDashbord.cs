using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Admin_RickyShop
{
    public partial class FormDashbord : Form
    {
        public FormDashbord()
        {
            InitializeComponent();
        }

        private void FormDashbord_Load(object sender, EventArgs e)
        {
            DAL.GetProdutos();
            CriarGrafico();
        }

        private void CriarGrafico()
        {

            var lista = Generic.produtosVendidos;

            var nomeProdutos = Generic.GetNomeProduto(lista);
            var qntProdutos = Generic.GetQtdProduto(lista);

            //Titulo Principal
            var titulo = new Title();
            titulo.Font = new Font("Arial", 15, FontStyle.Bold);
            titulo.ForeColor = Color.DarkGreen;
            titulo.Text = "Estatística de Produtos Vendidos";
            grafProdutos.Titles.Add(titulo);

            grafProdutos.Series.Add("Produtos");
            grafProdutos.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;

            // Mostrar os valores em cima das fatias
            grafProdutos.Series[0].IsValueShownAsLabel = true;

            //Estilo do gráfico
            grafProdutos.ChartAreas[0].Area3DStyle.Enable3D = true;
            grafProdutos.ChartAreas[0].Area3DStyle.Inclination = 30;
            grafProdutos.ChartAreas[0].Area3DStyle.Rotation = 60;

            // Mostrar Legenda
            grafProdutos.Legends.Add("Legenda");
            grafProdutos.Legends[0].Title = "Produtos";

            grafProdutos.Series[0].Points.DataBindXY(nomeProdutos, qntProdutos);
        }
    }
}
