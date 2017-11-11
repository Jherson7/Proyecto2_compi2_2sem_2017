using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto2_compi2_2sem_2017.Editor
{
    class nodo_tree_view: TreeNode
    {
        public string ruta;
        public int tipo_nodo;

        public nodo_tree_view(string ruta,int tipo)
        {
            this.ruta = ruta;
            tipo_nodo = tipo;
        }

    }
}
