using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_compi2_2sem_2017.Compilador
{
    class ambitos:LinkedList<Variable>
    {
        public string nombre;
        public int noIf;
        public int noFor;
        public int noWhile;
        public int doWhile;
        public int noUntil;
        public int noX;
        //le puedo agregar mas cosas

        public ambitos(string nombre)
        {
            this.nombre = nombre;
            noIf = noFor = noWhile = noWhile = noUntil = noX = 0;
        }
    }
}
