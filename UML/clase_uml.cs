using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_compi2_2sem_2017.UML
{
    public class clase_uml

    {

        public string nombre;
        public LinkedList<metodo_uml> metodos;
        public LinkedList<variable_uml> variables;


        public clase_uml(string nombre)
        {
            this.nombre = nombre;
            this.metodos = new LinkedList<metodo_uml>();
            this.variables = new LinkedList<variable_uml>();
        }

        public override string ToString()
        {
            return nombre +", Clase";
        }

    }

   
}
