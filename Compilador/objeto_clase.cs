using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proyecto2_compi2_2sem_2017.TablaSimbolos;

namespace Proyecto2_compi2_2sem_2017.Compilador
{
    class objeto_clase
    {
        public LinkedList<metodo> metodos;
        public ambitos variables;
        public LinkedList<metodo> constructores;
        public string nombre;
        public string visibilidad;

        public objeto_clase(string name)//debe llevar tambien la visibilidad
        {
            this.nombre = name;
            this.variables = new ambitos("Global");
            this.constructores = new LinkedList<metodo>();
            this.metodos = new LinkedList<metodo>();
        }

    }
}
