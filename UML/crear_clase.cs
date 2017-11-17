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
using Proyecto2_compi2_2sem_2017.Editor;


namespace Proyecto2_compi2_2sem_2017.UML
{
    public partial class crear_clase : Form
    {


        static clase_uml actual;
        private static crear_clase holis;
        public crear_clase()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string nombre = txt_nombre_var.Text;
            string tipo = cmb_tipo_variable.SelectedItem.ToString();
            string visiblidad = cmb_visiblidad_variable.SelectedItem.ToString();

            Boolean fl = true;
            foreach(var item in combo_variables.Items)
            {
                variable_uml ac = (variable_uml)item;
                if (ac.nombre.Equals(nombre))
                {
                    ac.tipo = tipo;
                    ac.visiblidad = visiblidad;
                    fl = false;
                    break;
                }
            }

            if (fl)
            {
                actual.variables.AddLast(new variable_uml(nombre, tipo, visiblidad));
                cargar_cmb_variable();
            }
            
        }

        private void cargar_cmb_variable()
        {
            combo_variables.Items.Clear();
            foreach (var item in actual.variables)
            {
                combo_variables.Items.Add(item);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string nombre = txt_nombre_clase.Text; ;
            actual = new clase_uml(nombre);
        }

        private void combo_variables_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
        }

        private void combo_variables_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                variable_uml aux = (variable_uml)combo_variables.SelectedItem;
                txt_nombre_var.Text = aux.nombre;
                cmb_tipo_variable.SelectedItem = aux.tipo;
                cmb_visiblidad_variable.SelectedItem = aux.visiblidad;
            }
            catch
            {

            }
        }

        private void btn_crear_metodo_Click(object sender, EventArgs e)
        {
            string nombre = txt_nombre_metodo.Text;
            string tipo = cmb_tipo_met.SelectedItem.ToString();
            string visiblidad = cmb_visi_met.SelectedItem.ToString();

            Boolean fl = true;
            foreach (var item in combo_metodos.Items)
            {
                metodo_uml ac = (metodo_uml)item;
                if (ac.nombre.Equals(nombre))
                {
                    ac.tipo = tipo;
                    ac.visibilidad = visiblidad;
                    fl = false;
                    break;
                }
            }

            if (actual == null)
                MessageBox.Show("Debe crear una clase primero");
            else
            {
                if (fl)
                {
                    actual.metodos.AddLast(new metodo_uml(nombre, tipo, visiblidad));
                    cargar_combo_metodos();
                    txt_nombre_metodo.Text = "";
                    
                }
            }

           
        }

        private void cargar_combo_metodos()
        {
            combo_metodos.Items.Clear();
            foreach (var item in actual.metodos)
            {
                combo_metodos.Items.Add(item);
            }
        }

        private void btn_crear_parametro_Click(object sender, EventArgs e)
        {
            string nombre = txt_nom_param.Text;
            string tipo = txt_tipo_param.Text;

            Boolean fl = true;
            foreach (var item in cmb_parametros.Items)
            {
                parametros ac = (parametros)item;
                if (ac.nombre.Equals(nombre))
                {
                    ac.tipo = tipo;
                    fl = false;
                    break;
                }
            }
            if (fl)
            {
                metodo_uml met_actual = (metodo_uml)combo_metodos.SelectedItem;
                if (met_actual == null)
                    MessageBox.Show("seleccione metodo");
                else
                {
                    met_actual.parametros.AddLast(new parametros(nombre, tipo));
                    cargar_parametros_metodo(met_actual);
                    txt_nom_param.Text = "";
                    txt_tipo_param.Text = "";
                }
                
            }
        }

        private void cargar_parametros_metodo(metodo_uml met)
        {
            cmb_parametros.Items.Clear();
            foreach (var item in met.parametros)
            {
                cmb_parametros.Items.Add(item);
            }
        }

        private void cmb_parametros_SelectedIndexChanged(object sender, EventArgs e)
        {
            parametros aux = (parametros)cmb_parametros.SelectedItem;
            txt_nom_param.Text = aux.nombre;
            txt_tipo_param.Text = aux.tipo;
        }

        private void combo_metodos_SelectedIndexChanged(object sender, EventArgs e)
        {
            metodo_uml aux = (metodo_uml)combo_metodos.SelectedItem;
            txt_nombre_metodo .Text = aux.nombre;
            cmb_tipo_met.SelectedItem = aux.tipo;
            cmb_visi_met.SelectedItem = aux.visibilidad;

            cargar_parametros_metodo(aux);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            uml_principal.lista_clases.AddLast(actual);
            this.Dispose();

            generar_dot_clases();


        }

        private void generar_dot_clases()
        {
            StringBuilder dot = new StringBuilder();

            LinkedList<clase_uml> lista = uml_principal.lista_clases;


            dot.Append("digraph{  node[shape = record color = \"blue\"];\n");
            foreach(clase_uml a in lista)
            {
                dot.Append(a.nombre+"[ label= \"{"+a.nombre+"|ATRIBUTOS| ");
                foreach(variable_uml var in a.variables)
                {
                    string signo = "(+)";
                    if (var.visiblidad.Equals("privado"))
                        signo = "(-)";
                    else if (var.visiblidad.Equals("protegido"))
                        signo = "(#)";
                    dot.Append(signo + " " + var.nombre + ": " + var.tipo + "\\n");
                }

                dot.Append("|METODOS|");
                foreach (metodo_uml met in a.metodos)
                {
                    string signo = "(+)";
                    if (met.visibilidad.Equals("privado"))
                        signo = "(-)";
                    else if (met.visibilidad.Equals("protegido"))
                        signo = "(#)";
                    dot.Append(signo + " " + met.nombre + "():  " + met.tipo + "\\n");
                }

                dot.Append("}\"];\n");

            }
            

            LinkedList<relacion> relaciones = uml_principal.lista_relaciones;


            foreach(relacion r in relaciones)
            {
                switch (r.tipo_rel)
                {
                    case 1:
                        dot.Append( r.origen + "->" + r.destino + " [arrowhead = odiamond];\n");
                        break;
                    case 2:
                        dot.Append(r.origen + "->" + r.destino + "  ;\n");
                        break;
                    case 3:
                        dot.Append(r.origen + "->" + r.destino + " [arrowhead = diamond];\n");
                        break;
                    case 4:
                        dot.Append(r.origen + "->" + r.destino + " [arrowhead = vee style = dashed];\n");
                        break;
                    default:
                        dot.Append(r.origen + "->" + r.destino + " [arrowhead = o];\n");
                        break;
                }
                
            }
            dot.Append("}");

            generarDOT_PNG(dot.ToString(), "C:\\compiladores\\imagenes_uml\\uml.dot", "C:\\compiladores\\imagenes_uml\\diagrama.png");
            /*digraph{
               

                struct3[shape = record, label = "{ Nombre clase|atributos|c| d|e|| metodos|uno|dos|tres}"];
                struct4[shape = record, label = "{ Nombre clase|atributos|c| d|e|| metodos|uno|dos|tres}"];

                struct3->struct4;

            }*/

        }

        private void generarDOT_PNG(string graphviz, String rdot, String rpng)
        {
            System.IO.File.WriteAllText(rdot, graphviz);
            String comandodot = "C:\\Graphviz\\bin\\dot.exe -Tpng " + rdot + " -o " + rpng + " ";
            var command = string.Format(comandodot);
            var procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/C" + command);
            var proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            proc.WaitForExit();
        }

    }
}
