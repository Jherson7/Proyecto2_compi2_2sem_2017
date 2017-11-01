using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_compi2_2sem_2017.Control3D
{
    class errores
    {
        public string tipo;
        public int linea;
        public int columna;
        public string descripcion;

        public errores(string type,int line,int colum,string description)
        {
            this.tipo = type;
            this.linea = line;
            this.columna = colum;
            this.descripcion = description;
        }
    }
}
