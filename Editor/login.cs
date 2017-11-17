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
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            string nombre = txt_user.Text;
            string contra = txt_pass.Text;

            if (master.getControlador().iniciar_sesion(nombre, contra))
            {
                MessageBox.Show("INICIO DE SESION EXITOSA");
            }else
                MessageBox.Show("USUARIO O CONTRASENIA INCORRECTA");
            this.Dispose();
        }
    }
}
