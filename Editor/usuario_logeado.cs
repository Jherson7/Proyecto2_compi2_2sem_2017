using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_compi2_2sem_2017.Editor
{
    class usuario_logeado
    {
        public string nombre;
        public string pass;
        public int id;
        

        public usuario_logeado  (string name, string pass, int id)
        {
            this.nombre = name;
            this.pass = pass;
            this.id = id;
        }
    }
}
