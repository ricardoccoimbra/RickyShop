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
    
    public partial class Token
    {
        public int ID_Token { get; set; }
        public int ID_Utilizador { get; set; }
        public string TokenAleatorio { get; set; }
        public int Estado { get; set; }
    
        public virtual Utilizadores Utilizadores { get; set; }
    }
}
