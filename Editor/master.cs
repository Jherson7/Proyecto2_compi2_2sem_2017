using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using proyecto_de_clase_2.Modelo;


namespace Proyecto2_compi2_2sem_2017.Editor
{
    class master
    {
        private static Controlador_base cr;
        private static usuario_logeado usuario;



        public static usuario_logeado get_usuario()
        {
            return usuario;
        }
        public static Controlador_base getControlador()
        {
            if(cr == null)
            {
                cr = new Controlador_base();
            }
            return cr;
        }

        public static void set_usuario(usuario_logeado u)
        {
            usuario  = u;
        }
       

        
    }
}