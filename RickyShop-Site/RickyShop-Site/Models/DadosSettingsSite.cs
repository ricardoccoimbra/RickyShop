using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RickyShop_Site.Models
{
    public class DadosSettingsSite
    {
        //{"QtdProdutosDestaque":3,"QtdProdutosPagina":6}
        public int QtdProdutosDestaque { get; set; }
        public int QtdProdutosPagina { get; set; }
        public int ValidPosPromo { get; set; }

    }

     
}