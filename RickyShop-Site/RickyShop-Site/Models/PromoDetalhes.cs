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
    
    public partial class PromoDetalhes
    {
        public int ID { get; set; }
        public int ID_Promo { get; set; }
        public int ID_Produto { get; set; }
    
        public virtual Produto Produto { get; set; }
        public virtual Promo Promo { get; set; }
    }
}
