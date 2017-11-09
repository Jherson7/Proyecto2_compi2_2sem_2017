using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Proyecto2_compi2_2sem_2017;

namespace Proyecto2_compi2_2sem_2017.Editor
{
    class pagina:TabPage
    {
        public RichTextBox contenido;
        string ruta;
        string tipo;
        //aqui le voy a poner mas

        public pagina(string type)
        {
            this.contenido = new RichTextBox();
            this.ruta = "";
            this.tipo = type;
            this.contenido.SetBounds(-1, 0, 650, 322);
            this.Controls.Add(contenido);
            if(type.ToUpper(). Equals("OLC"))
                this.Text = "nuevo"+Form1.cont_olc++ +".olc";
            else
                this.Text = "nuevo" + Form1.cont_tree++ + ".tree";
        }
    }
}
