using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Proyecto2_compi2_2sem_2017.Editor;
using Proyecto2_compi2_2sem_2017.Compilador;
using Proyecto2_compi2_2sem_2017.Control3D;

namespace Proyecto2_compi2_2sem_2017
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
         
        }

        private void btnCompilar_Click(object sender, EventArgs e)
        {
            //try
            //{
                pagina nuevo = (pagina)tabControl1.TabPages[tabControl1.SelectedIndex];
                string contenido = nuevo.contenido.Text;
                Interprete inter = new Interprete();
                inter.analizar(contenido);
                RichTextBox aux = (RichTextBox) control_salida.TabPages[1].Controls[0];
                aux.Text = Convert.ToString(inter.errores);
                foreach(errores err in Control3d.getErrores())
            {
                aux.AppendText(err.tipo + "  |  " + err.descripcion + "  |  " + err.linea + "  |  " + err.columna+"\n");
            }
            RichTextBox c3d = (RichTextBox)control_salida.TabPages[3].Controls[0];
            c3d.Text = Convert.ToString(Control3d.retornarC3D().ToString());

        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            pagina pag = new pagina();
            tabControl1.TabPages.Add(pag);
            tabControl1.SelectTab(pag);

        }
    }
}
