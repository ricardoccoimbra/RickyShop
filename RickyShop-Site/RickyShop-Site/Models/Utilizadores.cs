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
    using System.ComponentModel.DataAnnotations;

    public partial class Utilizadores
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Utilizadores()
        {
            this.Carrinho = new HashSet<Carrinho>();
            this.Logs = new HashSet<Logs>();
            this.Pedidos = new HashSet<Pedidos>();
            this.ProdutosFavoritos = new HashSet<ProdutosFavoritos>();
            this.Token = new HashSet<Token>();
        }

        public int ID_Utilizador { get; set; }

        [Display(Name = "Primeiro Nome")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo Obrigatório")]
        [DataType(DataType.Text)]
        public string PrimeiroNome { get; set; }

        [Display(Name = "Segundo Nome")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo Obrigatório")]
        [DataType(DataType.Text)]
        public string SegundoNome { get; set; }

        [Display(Name = "Data de Nascimento")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo Obrigatório")]
        [DataType(DataType.DateTime)]
        public System.DateTime DataDeNascimento { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo Obrigatório")]
        [MinLength(9)]
        [MaxLength(9)]
        public string NIF { get; set; }

        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo Obrigatório")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo Obrigatório")]
        [DataType(DataType.Password)]
        public string PassWord { get; set; }

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo Obrigatório")]
        [DataType(DataType.Password)]
        public string ConfirmarPassWord { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "É necessário o Contacto")]
        [DataType(DataType.PhoneNumber)]
        [MinLength(9)]
        [MaxLength(9)]
        public string Contacto { get; set; }
        public System.DateTime DataDeAdesao { get; set; }
        public Nullable<int> Desconto { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Carrinho> Carrinho { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Logs> Logs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pedidos> Pedidos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProdutosFavoritos> ProdutosFavoritos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Token> Token { get; set; }
    }
}
