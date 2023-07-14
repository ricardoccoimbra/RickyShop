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
    
    public partial class Utilizadores
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Utilizadores()
        {
            this.Carrinho = new HashSet<Carrinho>();
            this.Logs = new HashSet<Logs>();
            this.MovimentacaoSaldo = new HashSet<MovimentacaoSaldo>();
            this.Pedidos = new HashSet<Pedidos>();
            this.ProdutosFavoritos = new HashSet<ProdutosFavoritos>();
            this.Token = new HashSet<Token>();
        }
    
        public int ID_Utilizador { get; set; }
        public string PrimeiroNome { get; set; }
        public string SegundoNome { get; set; }
        public System.DateTime DataDeNascimento { get; set; }
        public string NIF { get; set; }
        public string Email { get; set; }
        public string PassWord { get; set; }
        public string Contacto { get; set; }
        public System.DateTime DataDeAdesao { get; set; }
        public Nullable<int> Desconto { get; set; }
        public string Morada { get; set; }
        public string CodigoPostal { get; set; }
        public decimal Saldo { get; set; }
        public int Estado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Carrinho> Carrinho { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Logs> Logs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MovimentacaoSaldo> MovimentacaoSaldo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pedidos> Pedidos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProdutosFavoritos> ProdutosFavoritos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Token> Token { get; set; }
    }
}
