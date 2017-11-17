using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Proyecto2_compi2_2sem_2017;
using Proyecto2_compi2_2sem_2017.Compilador;
using Proyecto2_compi2_2sem_2017.Ejecucion3D;

namespace Proyecto2_compi2_2sem_2017.Editor
{
    class pagina:TabPage
    {
        public FastColoredTextBoxNS.IronyFCTB contenido;
        
        public string ruta;
        public string tipo;
        //aqui le voy a poner mas

        public pagina(string type)
        {
            if (type.Equals("olc"))
            {
                this.contenido = new FastColoredTextBoxNS.IronyFCTB();
                this.contenido.SetParser(new Gramatica());
            }else
            {
                this.contenido = new FastColoredTextBoxNS.IronyFCTB();
                this.contenido.SetParser(new GramaticaTre());
            }
                
            this.ruta = "";
            this.tipo = type;
            this.contenido.SetBounds(-1, 0, 650, 322);
            this.Controls.Add(contenido);
            if(type.ToUpper(). Equals("OLC"))
                this.Text = "nuevo"+Form1.cont_olc++ +".olc";
            else
                this.Text = "nuevo" + Form1.cont_tree++ + ".tree";
        }

        public pagina(string tipo,string cont,string nombre,string ruta)
        {
            if (tipo.Equals("olc"))
            {
                this.contenido = new FastColoredTextBoxNS.IronyFCTB();
                this.contenido.SetParser(new Gramatica());
            }
            else
            {
                this.contenido = new FastColoredTextBoxNS.IronyFCTB();
                this.contenido.SetParser(new GramaticaTre());
            }
            this.ruta = "";
            this.tipo = tipo;
            this.contenido.SetBounds(-1, 0, 650, 322);
            this.Controls.Add(contenido);
            this.contenido.Text = cont;
            this.Text = nombre;
            this.ruta = ruta;

        }


        public pagina(int type)
        {
            this.contenido = new FastColoredTextBoxNS.IronyFCTB();
            this.contenido.SetParser(new Gramatica3d());
                
            this.ruta = "";
            this.tipo = "3d";
            this.contenido.SetBounds(-1, 0, 793,163);
            this.Controls.Add(contenido);
        }
    }
}
