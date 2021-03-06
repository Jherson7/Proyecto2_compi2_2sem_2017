﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_compi2_2sem_2017.Control3D
{
    class nodo3d
    {
        public string etv;
        public string etf;
        public string val;
        public string refe;
        public int tipo;
        public string tipo_valor;
        public int categoria;
        private string nombre;
        //public Boolean destino;

        public nodo3d()
        {
            this.etf = "";
            this.etv = "";
            this.tipo = -1;
            this.refe = "";
        }


        //para el tipo:
        /// <summary>
        ///  el tipo ==1 son de tipo etiquetas (cond)
        ///  el tipo ==2 son de tipo temporales (tienen un valor)
        ///  el tipo ==3 son de tipo num, dec etc, individuales (tienen un tipo_string y valor)
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="valor"></param>
        public nodo3d(string tipo,string valor)
        {
            this.tipo = 3;
            this.tipo_valor = tipo;
            this.val = valor;
            //this.destino = destino;
            switch (tipo)
            {
                case ("cad"):
                    this.categoria = 1;
                    break;
                case ("num"):
                case "entero":
                    this.categoria = 2;
                    break;
                case ("char"):
                    this.categoria = 3;
                    break;
                case ("bool"):
                    this.categoria = 4;
                    break;
                default:
                    this.categoria = 5;
                    break;
            }
        }

        
        public nodo3d(string etv,string etf,int tipo)
        {
            this.etv = etv;
            this.etf = etf;
            this.tipo = 1; 
        }

        public nodo3d(string tipo, string valor,string referencia)
        {
            this.tipo = 3;
            this.tipo_valor = tipo;
            this.val = valor;
            this.refe = referencia;
            //this.destino = destino;
            switch (tipo)
            {
                case ("cad"):
                    this.categoria = 1;
                    break;
                case ("num"):
                case "entero":
                    this.categoria = 2;
                    break;
                case ("char"):
                    this.categoria = 3;
                    break;
                case ("bool"):
                    this.categoria = 4;
                    break;
                default:
                    this.categoria = 5;
                    break;
            }
        }



        public void setNombreUltimo(string nombre)
        {
            this.nombre = nombre;
        }

        public string get_nombre_ultimo()
        {
            return this.nombre;
        }

    }
}
