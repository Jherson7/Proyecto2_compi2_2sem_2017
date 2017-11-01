using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto2_compi2_2sem_2017.Editor
{
    class pagina:TabPage
    {
        public RichTextBox contenido;
        string ruta;
        //aqui le voy a poner mas

        public pagina()
        {
            this.contenido = new RichTextBox();
            this.ruta = "";
            this.contenido.SetBounds(-1, 0, 650, 322);
            this.Controls.Add(contenido);
            this.Text = "NUEVO";
        }
    }
}
