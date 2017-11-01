using System;
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
        public int tipo;
        public string tipo_valor;

        public nodo3d()
        {
            this.etf = "";
            this.etv = "";
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
        }


    }
}
