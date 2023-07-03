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
    using System.Collections.Generic;

    public partial class Produto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Produto()
        {
            this.Carrinho = new HashSet<Carrinho>();
            this.ProdutosFavoritos = new HashSet<ProdutosFavoritos>();
            this.PedidosDetalhes = new HashSet<PedidosDetalhes>();
        }

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
        public bool EstadoProm { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Carrinho> Carrinho { get; set; }
        public virtual Categoria Categoria { get; set; }
        public virtual MarcaProduto MarcaProduto { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProdutosFavoritos> ProdutosFavoritos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PedidosDetalhes> PedidosDetalhes { get; set; }
    }
}