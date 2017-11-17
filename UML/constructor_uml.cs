using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_compi2_2sem_2017.UML
{
    class constructor_uml
    {

        public string nombre;
        public string visilidad;
        public LinkedList<parametros> parametros;
        public LinkedList<variable_uml> variables;

        public constructor_uml(string nombre)
        {
            this.nombre = nombre;
            visilidad = "";
            parametros = new LinkedList<UML.parametros>();
            variables = new LinkedList<variable_uml>();
        }


        public constructor_uml(string nombre,string visis)
        {
            this.nombre = nombre;
            visilidad = visis;
            parametros = new LinkedList<UML.parametros>();
            variables = new LinkedList<variable_uml>();
        }
    }
}
