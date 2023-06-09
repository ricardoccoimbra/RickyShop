﻿using PAP_RickyShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace PAP_RickyShop
{
    public static class Generic
    {
        public static GestãoRickyShopEntities db = new GestãoRickyShopEntities();

        public static string VerifRegisto(string _NIF, string _Contacto, string _Email, DateTime _date)
        {
            string i = "certo";

            var nif = db.Utilizadores.Count(s => s.NIF == _NIF);
            var num = db.Utilizadores.Count(s => s.Contacto == _Contacto);
            var mail = db.Utilizadores.Count(s => s.Email == _Email);

            if (nif == 0)
            {
                if (num == 0)
                {
                    if (mail == 0)
                    {
                        return i;
                    }
                    else
                    {
                        i = "email";
                        return i;
                    }
                }
                else
                {
                    i = "num";
                    return i;
                }
            }
            else
            {
                i = "nif";
                return i;
            }
        }

        public static string TokenAleatorio()
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();

            for (int i = 0; i < 7; i++)
            {
                int index = rnd.Next(chars.Length);
                sb.Append(chars[index]);
            }

            return sb.ToString();
        }

        public static void MarcaID()
        {
            
            List<string> ls = new List<string>();

            foreach (var item in db.MarcaProduto)
            {
                ls.Add(item.Marca);
            }
        }

    }
}

