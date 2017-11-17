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
        public LinkedList<metodo_uml> constructores;
        public string hereda;

        public clase_uml(string nombre)
        {
            this.nombre = nombre;
            this.hereda = "error";
            this.metodos = new LinkedList<metodo_uml>();
            this.variables = new LinkedList<variable_uml>();
            this.constructores = new LinkedList<metodo_uml>();
        }

        public clase_uml(string nombre,string hereda)
        {
            
            this.nombre = nombre;
            this.hereda = hereda;
            this.metodos = new LinkedList<metodo_uml>();
            this.variables = new LinkedList<variable_uml>(); this.constructores = new LinkedList<metodo_uml>();

        }

        public override string ToString()
        {
            return nombre +", Clase";
        }

    }

   
}
