﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
namespace Proyecto2_compi2_2sem_2017.TablaSimbolos
{
    class nodoTabla
    {

        public string nombre;
        public string ambito;
        public string tipo;
        public string rol;
        public string visibilidad;
        public int pos;//de las cosas mas importantes
        public int tam;
        private ParseTreeNode exp;
        public int noMetodo;
        public LinkedList<int> dimArray;
      
        public nodoTabla(string visibilidad,string type,string name,string rol, int pos,int tam,string ambito)
        {
            this.visibilidad = visibilidad;
            this.tipo = type;
            this.nombre = name;
            this.rol = rol;
            this.pos = pos;
            this.tam = tam;
            this.ambito = ambito;
            this.exp = null;
            this.dimArray = null;
        }

        public nodoTabla(string visibilidad, string type, string name, string rol, int pos, int tam, string ambito,ParseTreeNode expr)
        {
            this.visibilidad = visibilidad;
            this.tipo = type;
            this.nombre = name;
            this.rol = rol;
            this.pos = pos;
            this.tam = tam;
            this.ambito = ambito;
            this.exp = expr;
            this.dimArray = null;
        }

        public nodoTabla(string visibilidad, string type, string name, string rol, int pos, int tam, string ambito, LinkedList<int> casilla, ParseTreeNode expr)
        {
            this.visibilidad = visibilidad;
            this.tipo = type;
            this.nombre = name;
            this.rol = rol;
            this.pos = pos;
            this.tam = tam;
            this.ambito = ambito;
            this.exp = expr;
            this.dimArray = casilla;
        }

        public void setNoMetodo(int numero)
        {
            this.noMetodo = numero;
        }


    }
}
