﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_compi2_2sem_2017.UML
{
   public class variable_uml
    {
        public  string nombre;
        public string tipo;
        public string visiblidad;
        public bool estado;

        public variable_uml(string nombre,string tipo,string visi)
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.visiblidad = visi;
            estado = false;
        }


        public override string ToString()
        {
            return nombre;
        }
    }
}
