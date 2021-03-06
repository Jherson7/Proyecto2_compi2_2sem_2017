﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
namespace Proyecto2_compi2_2sem_2017.Compilador
{
    class Variable
    {
        public String nombre;
        public string tipo;
        public string visibilidad;
        public ParseTreeNode valor;
        public LinkedList<int> casilla;
        public bool es_heredada;

        public Variable(String nom)
        {
            nombre = nom;
        }
        public Variable(string visibilidad,String tipo,String nom, ParseTreeNode val)
        {
            nombre = nom;
            valor = val;
            this.tipo = tipo;
            this.visibilidad = visibilidad;
            this.casilla = null;
            es_heredada = false;
        }
        public Variable(string visibilidad, String tipo, String nom, ParseTreeNode val, LinkedList<int> casillas)
        {
            nombre = nom;
            valor = val;
            this.tipo = tipo;
            this.visibilidad = visibilidad;
            this.casilla = casillas;
            es_heredada = false;
        }

        public void set_hereda(bool val)
        {
            this.es_heredada = val;
        }
    }
}
