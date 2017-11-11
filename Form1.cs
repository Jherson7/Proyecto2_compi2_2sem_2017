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
using Proyecto2_compi2_2sem_2017.Ejecucion3D;

namespace Proyecto2_compi2_2sem_2017
{
    public partial class Form1 : Form
    {

        public static  int cont_tree=0;
        public static int cont_olc= 0;

        public Form1()
        {
            InitializeComponent();
         
        }

        private void btnCompilar_Click(object sender, EventArgs e)
        {
            //try
            //{
                Control3d.iniciar_controlador();
                Control3d.limpiar();
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
            string tipo =tipo_archivo.getType();
            

            pagina pag = new pagina(tipo);
            tabControl1.TabPages.Add(pag);
            tabControl1.SelectTab(pag);

        }

        private void btnEjecutar3d_Click(object sender, EventArgs e)
        {
            Interprete3d inter = new Interprete3d();
            RichTextBox c3d = (RichTextBox)control_salida.TabPages[3].Controls[0];
            string cont = c3d.Text;
            inter.analizar(cont);

            RichTextBox salida = (RichTextBox)control_salida.TabPages[0].Controls[0];
            salida.Text = inter.salida.ToString();
            RichTextBox aux = (RichTextBox)control_salida.TabPages[1].Controls[0];
            foreach (errores err in Control3d.getErrores())
            {
                aux.AppendText(err.tipo + "  |  " + err.descripcion + "  |  " + err.linea + "  |  " + err.columna + "\n");
            }

        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            for (int i=0; i< 10; i++)
            {
                Console.WriteLine("fibonacci de " + i + ", es: " + fibonacci(i));
            }
        }


        private int fibonacci(int n)
        {
            if (n <= 1)
                return 1;
            else
                return fibonacci(n - 1) + fibonacci(n - 2);
        }
    }
}
