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

namespace Proyecto2_compi2_2sem_2017.UML
{
    public partial class crear_clase : Form
    {


        static clase_uml actual;
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
            string nombre = txt_nombre_var.Text; ;
            string tipo = cbm_tipo_clase.SelectedItem.ToString();
            string visi = cmb_visibilidad_clase.SelectedItem.ToString();
            actual = new clase_uml(nombre, tipo,visi);
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
    }
}
