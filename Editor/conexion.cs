using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace Proyecto2_compi2_2sem_2017.Editor
{
    public class Conexion
    {
        private static SqlConnection conexion;

        public Conexion()
        {

        }

        public static SqlConnection getConection()
        {
            if (conexion == null)
            {
                try
                {
                    conexion = new SqlConnection("Data Source=JHERSON\\SQLEXPRESS;Initial Catalog=proyecto_compi2;Integrated Security=True");

                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Error al conectar a la base de datos");
                }
            }
            return conexion;
        }
    }
}