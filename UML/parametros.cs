using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_compi2_2sem_2017.UML
{
    public class parametros
    {
        public string nombre;
        public string tipo;
        public LinkedList<int> dimensiones;

        public parametros(string nom, string tip)
        {
            nombre = nom;
            tipo = tip;
            dimensiones = new LinkedList<int>();

        }

        public override string ToString()
        {
            return nombre + ", " + tipo;
        }
    }
}
