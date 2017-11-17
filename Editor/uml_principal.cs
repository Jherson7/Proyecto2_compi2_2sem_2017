using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Proyecto2_compi2_2sem_2017.UML;
using System.Threading;

namespace Proyecto2_compi2_2sem_2017.Editor
{
    public partial class uml_principal : Form
    {

        public static LinkedList<clase_uml> lista_clases;
        public static LinkedList<relacion> lista_relaciones;
        public uml_principal()
        {
            InitializeComponent();
            lista_clases = new LinkedList<clase_uml>();
            lista_relaciones = new LinkedList<relacion>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            crear_clase clase = new crear_clase();
            clase.ShowDialog();
            Thread.Sleep(1000);
            this.pictureBox1.Image = new System.Drawing.Bitmap(@"C:\compiladores\imagenes_uml\diagrama.png");
            //this.pictureBox1.Image = new System.Drawing.Bitmap(@"C:\compiladores\imagenes_uml\diagrama.png");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //agregacion 1 
            crear_relacion rel = new crear_relacion(1,lista_relaciones,lista_clases);
            rel.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //composicion 2
            crear_relacion rel = new crear_relacion(2, lista_relaciones, lista_clases);
            rel.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //dependencia 3
            crear_relacion rel = new crear_relacion(3, lista_relaciones, lista_clases);
            rel.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //herencia 4
            crear_relacion rel = new crear_relacion(4, lista_relaciones, lista_clases);
            rel.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //asociasion 5

            //solo se crean objetos de la clase referenciada
            crear_relacion rel = new crear_relacion(5, lista_relaciones, lista_clases);
            rel.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if(cmb_clase.SelectedItem==null)
            {
                MessageBox.Show("Seleccione tipo de salida");
                return;
            }
            string clase = cmb_clase.SelectedItem.ToString();
            if (clase.Equals("Tree"))
                traducir_clase_tree();
            else
                traducir_clase_olc();
        }

        private void traducir_clase_olc()
        {
            LinkedList<clase_uml> lista = uml_principal.lista_clases;
            StringBuilder clases = new StringBuilder();
            
            foreach(clase_uml clase in lista)
            {
                StringBuilder cad = new StringBuilder();

                //escribimos el nombre de la clase
                cad.Append("publico " + clase.nombre + " ");
                //buscar de que clase hereda
                string hereda = clase_heredada(clase.nombre);
                if (hereda != "")
                    cad.Append("hereda_de " + hereda);
                cad.Append(" {\n");
                //retonar las clases de que tienen que ver con variables

                foreach (variable_uml var in clase.variables)
                    cad.Append("\t\t"+var.visiblidad + " " + var.tipo + " " + var.nombre + "; \n");

                foreach (string a in get_asociaciones(clase.nombre))
                    cad.Append("\t\t"+"publico " + a + " objeto_" + a + "; \n");

                LinkedList<string> agregaciones = new LinkedList<string>();
                LinkedList<string> composicion  = new LinkedList<string>();
                LinkedList<string> dependencias = new LinkedList<string>();
                //todas las variables que tiene que ver con metodos
                llenar_agregaciones(clase.nombre,agregaciones);
                llenar_composiciones(clase.nombre,composicion);
                llenar_dependencias(clase.nombre, dependencias);

                foreach(var item in agregaciones)//lleno las variables de las agregaciones
                    cad.Append("\t\tpublico " + item + " objeto_" + item + "; \n");

                foreach (var item in composicion)//lleno las variables de las composiciones
                    cad.Append("\t\tpublico " + item + " objeto_" + item + "; \n");


                
                foreach (var item in agregaciones)//creo los metodos de las agregaciones
                {
                    cad.Append("publico vacio " +clase.nombre+"_metodo_"+ item + "("+item+ "nuevo_objeto_" + item + "){ \n");
                    cad.Append("\t\tthis.objeto_" + item + "  = nuevo_objeto_" + item + ";\n}");
                } 

                foreach (var item in composicion)//creo los metodos de la composicion
                {
                    cad.Append("publico " + clase.nombre + "(" + item + "nuevo_objeto_" + item + "){ \n");
                    cad.Append("\t\tthis.objeto_" + item + "  = nuevo_objeto_" + item + ";\n}");
                }

                foreach (var item in dependencias)//creo los metodos de la composicion
                {
                    cad.Append("publico " + clase.nombre + "_dependencia_"+item+"(" + item + "nuevo_objeto" + item + "){ \n }");
                }

                foreach(var item in clase.metodos)
                {
                    cad.Append("\n\n" + item.visibilidad + "  " + item.tipo + "  " + item.nombre + " (");
                    bool a = false;
                    foreach (var par in item.parametros) {
                        if(a)
                            cad.Append(","+par.tipo + "  " + par.nombre);
                        else
                            cad.Append(par.tipo + "  " + par.nombre);
                        a = true;
                    }
                    cad.Append(") {\n}");
                        
                }


                cad.Append("\n\n}\n\n");
                clases.Append(cad+"\n");
            }

            this.txt_salida.Text = clases.ToString();

        }



        private string clase_heredada(string nombre)
        {
            LinkedList<relacion> lista = uml_principal.lista_relaciones;
            foreach (var item in lista)
            {
                if (item.origen.Equals(nombre) && item.tipo_rel == 4)
                    return item.destino;
            }
            return "";
        }

        private LinkedList<string> get_asociaciones(string nombre)
        {
            LinkedList<relacion> lista = uml_principal.lista_relaciones;
            LinkedList<string> asociaciones = new LinkedList<string>();
            foreach (var item in lista)
            {
                if (item.origen.Equals(nombre) && item.tipo_rel == 5)
                    asociaciones.AddLast(item.destino);
            }
            return asociaciones;
        }

        private void llenar_agregaciones(string nombre, LinkedList<string> lista)
        {
            LinkedList<relacion> relaciones = uml_principal.lista_relaciones;
            foreach (var item in relaciones)
            {
                if (item.origen.Equals(nombre) && item.tipo_rel == 1)
                    lista.AddLast(item.destino);
            }
        }


        private void llenar_composiciones(string nombre, LinkedList<string> lista)
        {
            LinkedList<relacion> relaciones = uml_principal.lista_relaciones;
            foreach (var item in relaciones)
            {
                if (item.origen.Equals(nombre) && item.tipo_rel == 2)
                    lista.AddLast(item.destino);
            }
        }

        private void llenar_dependencias(string nombre, LinkedList<string> lista)
        {
            LinkedList<relacion> relaciones = uml_principal.lista_relaciones;
            foreach (var item in relaciones)
            {
                if (item.origen.Equals(nombre) && item.tipo_rel == 3)
                    lista.AddLast(item.destino);
            }
        }

        private void traducir_clase_tree()
        {
            LinkedList<clase_uml> lista = uml_principal.lista_clases;
            StringBuilder clases = new StringBuilder();

            foreach (clase_uml clase in lista)
            {
                StringBuilder cad = new StringBuilder();

                //escribimos el nombre de la clase
                cad.Append(clase.nombre + " [");
                //buscar de que clase hereda
                string hereda = clase_heredada(clase.nombre);
                if (hereda != "")
                    cad.Append(hereda);
                cad.Append(" ]:\n");
                //retonar las clases de que tienen que ver con variables


                LinkedList<string> agregaciones = new LinkedList<string>();
                LinkedList<string> composicion = new LinkedList<string>();
                LinkedList<string> dependencias = new LinkedList<string>();
                //todas las variables que tiene que ver con metodos
                llenar_agregaciones(clase.nombre, agregaciones);
                llenar_composiciones(clase.nombre, composicion);
                llenar_dependencias(clase.nombre, dependencias);


                foreach (variable_uml var in clase.variables)
                    cad.Append("\t" + var.visiblidad + " " + var.tipo + " " + var.nombre + "\n");

                foreach (string a in get_asociaciones(clase.nombre))
                    cad.Append("\t" + "publico " + a + " objeto_" + a + "\n");

                foreach (var item in agregaciones)//lleno las variables de las agregaciones
                    cad.Append("\tpublico " + item + " objeto_" + item + "\n");

                foreach (var item in composicion)//lleno las variables de las composiciones
                    cad.Append("\tpublico " + item + " objeto_" + item + "\n");


                foreach (var item in agregaciones)//creo los metodos de las agregaciones
                {
                    cad.Append("\tpublico vacio " + clase.nombre + "_metodo_" + item + "[" + item + "  nuevo_objeto_" + item + "]: \n");
                    cad.Append("\\ttself.objeto_" + item + "  = nuevo_objeto_" + item + "\n");
                }

                foreach (var item in composicion)//creo los metodos de la composicion
                {
                    cad.Append("\tpublico " + clase.nombre + " [" + item + "  nuevo_objeto_" + item + "]:\n");
                    cad.Append("\t\tself.objeto_" + item + "  = nuevo_objeto_" + item);
                }

                foreach (var item in dependencias)//creo los metodos de la composicion
                {
                    cad.Append("\tpublico  " + clase.nombre + "_dependencia_" + item + " [" + item + "  nuevo_objeto" + item + "]: \n");
                }

                foreach (var item in clase.metodos)
                {
                    cad.Append("\n\t" + item.visibilidad + "  " + item.tipo + "  " + item.nombre + " [");
                    bool a = false;
                    foreach (var par in item.parametros)
                    {
                        if (a)
                            cad.Append("," + par.tipo + "  " + par.nombre);
                        else
                            cad.Append(par.tipo + "  " + par.nombre);
                        a = true;
                    }
                    cad.Append("]:\n");

                }


                cad.Append("\n\n}\n\n");
                clases.Append(cad + "\n");
            }

            this.txt_salida.Text = clases.ToString();
        }
    }
}

public class relacion
{

    public string origen;
    public string destino;
    public int tipo_rel;

    public relacion(string clase, int tip, string claes2)
    {
        this.origen = clase;
        this.tipo_rel = tip;
        this.destino = claes2;
    }
}