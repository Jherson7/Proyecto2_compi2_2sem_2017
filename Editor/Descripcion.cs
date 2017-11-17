using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto2_compi2_2sem_2017.Editor
{
    public partial class Descripcion : Form
    {

        private static Descripcion des;
        private static string cont;
        public Descripcion()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cont = txt_descripcion.Text;
            this.Dispose();
        }

        public static string get_des()
        {
            des = new Descripcion();
            des.ShowDialog();
            return cont;;
        }
    }
}
