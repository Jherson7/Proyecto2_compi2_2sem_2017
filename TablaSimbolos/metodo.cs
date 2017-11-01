using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;

namespace Proyecto2_compi2_2sem_2017.TablaSimbolos
{
    class metodo
    {
        public string nombre;
        public string tipo;
        public string visibilidad;
        public ParseTreeNode parametros;
        public ParseTreeNode sentencia;
        public int noMetodo;

        public metodo(string visi,string tipo,string nombre,ParseTreeNode sentencia,int noMethod)//falta la visibilidad y de que clase es
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.sentencia = sentencia;
            this.parametros = new ParseTreeNode(new Token(new Terminal("sin hijos"), new SourceLocation(), "sin hijos", null));
            this.visibilidad = visi;
            this.noMetodo = noMethod;
        }
        public metodo(string visibilidad,string tipo, string nombre, ParseTreeNode sentencia, ParseTreeNode parametros,int noMethod)//falta la visibilidad y de que clase es
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.sentencia = sentencia;
            this.parametros = parametros;
            this.visibilidad = visibilidad;
            this.noMetodo = noMethod;
        }

        public ParseTreeNode getSentencias()
        {
            return this.sentencia;
        }
    }
}
