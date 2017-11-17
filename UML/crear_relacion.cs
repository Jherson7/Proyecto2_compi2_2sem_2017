using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto2_compi2_2sem_2017.UML
{
    public partial class crear_relacion : Form
    {
        int tipo = 1;
        LinkedList<relacion> lista;
        LinkedList<clase_uml> lista_clases;
        public crear_relacion(int tipo,LinkedList<relacion> lista, LinkedList<clase_uml> clases)
        {
            InitializeComponent();
            this.tipo = tipo;
            this.lista = lista;
            this.lista_clases = clases;
            llenar_combos();
        }

        private void llenar_combos()
        {
            foreach(clase_uml a in lista_clases)
            {
                cmb_destino.Items.Add(a);
                cmb_origen.Items.Add(a);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                clase_uml uno = (clase_uml)cmb_origen.SelectedItem;
                clase_uml dos = (clase_uml)cmb_destino.SelectedItem;
                lista.AddLast(new relacion(uno.nombre, tipo, dos.nombre));
                this.Dispose();
            }
            catch
            {
                MessageBox.Show("Error al crear la relacion");
                this.Dispose();
            }

        }
    }
}
