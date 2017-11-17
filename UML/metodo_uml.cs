using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_compi2_2sem_2017.UML
{
   public class metodo_uml
    {

        public string nombre;
        public string tipo;
        public string visibilidad;
        public string signo;
        public LinkedList<parametros> parametros;


        public metodo_uml(string nombre, string tipo, string visi)
        {
            switch (visi.ToLower())
            {
                case "publico":
                    signo = "+";
                    break;
                case "protegido":
                    signo = "#";
                    break;
                default:
                    signo = "-";
                    break;
            }
            this.nombre = nombre;
            this.tipo = tipo;
            this.visibilidad = visi;
            this.parametros = new LinkedList<UML.parametros>();
        }



        public override string ToString()
        {
            return nombre+ "(): "+tipo+" ==> "+ signo ;
        }
    }
}

