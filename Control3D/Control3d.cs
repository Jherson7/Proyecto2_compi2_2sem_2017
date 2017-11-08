using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proyecto2_compi2_2sem_2017.TablaSimbolos;
using Proyecto2_compi2_2sem_2017.Compilador;
using Irony.Parsing;

namespace Proyecto2_compi2_2sem_2017.Control3D
{
    class Control3d
    {
        private static tablaSimbolos tabla;
        private static LinkedList<errores> lista_errores;
        private static string ruta_predeterminada = "C:\\Users\\Jherson Sazo\\Documents\\COMPI2\\Archivos Proyecto2 2sem 2017\\";
        private static StringBuilder c3d;
        private static LinkedList<metodo> lista_metodos;
        private static Dictionary<string,objeto_clase> lista_clases;
        private static int contador_temp=-1;
        private static int contador_eti= -1;
        private static objeto_clase clase_actual;

        public static tablaSimbolos getTabla()
        {
            if (tabla == null)
                tabla = new tablaSimbolos();
            return tabla;
        }

        public static void iniciar_controlador()
        {
            if (tabla == null)
                tabla = new tablaSimbolos();
            if (lista_errores == null)
                lista_errores = new LinkedList<errores>();
            if(c3d==null)
                c3d= new StringBuilder();
            if (contador_eti == -1)
                contador_eti = 0;
            if (contador_temp == -1)
                contador_temp = 0;
        }

        public static void limpiar()
        {
            tabla.Clear();
            //lista_clases.Clear();
            //lista_metodos.Clear();
            lista_errores.Clear();
            c3d.Clear();
            contador_eti = 0;
            contador_temp = 0;
        }

        public static void agregarError(errores err)
        {
            if (lista_errores == null)
                lista_errores = new LinkedList<errores>();
            lista_errores.AddLast(err);
        }

        public static string getRuta()
        {
            return ruta_predeterminada;
        }

        public static LinkedList<errores> getErrores()
        {
            if(lista_errores == null)
                lista_errores = new LinkedList<errores>();
            return lista_errores;
        }

        public static StringBuilder retornarC3D()
        {
            return c3d;
        }

        public static void setListaMetodos(LinkedList<metodo> lista)
        {
            lista_metodos = lista;
        }

        public static void setListaClases(Dictionary<string,objeto_clase> lista)
        {
            lista_clases = lista;
        }


        public static LinkedList<metodo> getListaMetodo()
        {
            return lista_metodos;
        }

        public  static Dictionary<string,objeto_clase> getListaClase()
        {
            return lista_clases;
        }

        public static string getTemp()
        {
            return "t" + contador_temp++;
        }

        public static string getEti()
        {
            return "L" + contador_eti++;
        }

        public static void set_clase_actual(objeto_clase clase)
        {
            clase_actual = clase;
        }

        public static objeto_clase get_clase_actual()
        {
            return clase_actual;
        }

        public static void addError(string tipo,string descripcion,ParseTreeNode raiz)
        {
            if (lista_errores == null)
                lista_errores = new LinkedList<errores>();
            lista_errores.AddLast(new errores(tipo,raiz.Span.Location.Line,raiz.Span.Location.Column,descripcion));
        }

    }
}
