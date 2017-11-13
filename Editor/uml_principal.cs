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

namespace Proyecto2_compi2_2sem_2017.Editor
{
    public partial class uml_principal : Form
    {
        public uml_principal()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            crear_clase clase = new crear_clase();
            clase.ShowDialog();
        }
    }
}
