//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RickyShop_Site.Models
{
    using System;
    
    public partial class TOP5_ProdutosMenosStock_Result
    {
        public int ID_Produto { get; set; }
        public string Nome { get; set; }
        public decimal PreçoPorQuantidade { get; set; }
        public int ID_Categoria { get; set; }
        public int QuantidadeStock { get; set; }
        public string ImagemPath { get; set; }
        public string Descrição { get; set; }
        public Nullable<int> Desconto { get; set; }
        public int ID_Marca { get; set; }
        public string Genero { get; set; }
        public int Descontinuado { get; set; }
        public int Destaque { get; set; }
    }
}
