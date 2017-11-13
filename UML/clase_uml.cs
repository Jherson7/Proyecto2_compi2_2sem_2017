using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_compi2_2sem_2017.UML
{
    class clase_uml

    {

        public string nombre;
        public string tipo;
        public string visilidad;
        public LinkedList<metodo_uml> metodos;
        public LinkedList<variable_uml> variables;

        public clase_uml(string nombre, string tipo,string visiblidad)
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.visilidad = visiblidad;
            this.metodos = new LinkedList<metodo_uml>();
            this.variables = new LinkedList<variable_uml>();
        }

        public override string ToString()
        {
            return nombre +"," + tipo;
        }

    }
}
