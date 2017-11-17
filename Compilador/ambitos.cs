using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proyecto2_compi2_2sem_2017.Control3D;

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
        public int noLoop;
        public int no_else_if;
        public int no_switch;
        public string salida;
        public string continuar;
        //le puedo agregar mas cosas

        public ambitos(string nombre)
        {
            this.nombre = nombre;
            noIf = noFor = noWhile = noWhile = noUntil = noX = no_else_if= noLoop= no_switch= 0;
            this.salida = "";
            this.continuar = "";
        }
    }
}
