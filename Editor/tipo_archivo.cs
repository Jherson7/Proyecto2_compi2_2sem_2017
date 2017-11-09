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
    public partial class tipo_archivo : Form
    {
        public static string tipo = "";
        static tipo_archivo msjBox;

        public tipo_archivo()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btn_olc_Click(object sender, EventArgs e)
        {
            tipo = "olc";
            this.Dispose();

        }

        private void btn_tree_Click(object sender, EventArgs e)
        {
            tipo = "tree";
            this.Dispose();
        }

        public static string getType()
        {
            msjBox = new tipo_archivo();
            msjBox.ShowDialog();
            return tipo;
        }
    }
}
