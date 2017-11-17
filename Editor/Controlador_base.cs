using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Sql;
using System.Data.SqlClient;


namespace Proyecto2_compi2_2sem_2017.Editor
{
    public class Controlador_base
    {
        SqlConnection con;
        SqlCommand consulta;
        SqlDataReader resultSet;

        public Controlador_base()
        {
            con = Conexion.getConection();
        }

        public bool iniciar_sesion(string user, string pass)
        {
            String sql = "select * from usuario where usuario = '" + user + "' and contrasenia = '" + pass + "'";
            bool ret = false;
            try
            {
                con.Open();
                consulta = new SqlCommand(sql, con);
                resultSet = consulta.ExecuteReader();
                if (resultSet.Read())
                {
                    usuario_logeado nuevo = new usuario_logeado(resultSet.GetString(0), resultSet.GetString(2), resultSet.GetInt32(3));
                    master.set_usuario(nuevo);
                    ret = true;

                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Error al hacer la consulta a la base: " + e.GetBaseException());
            }
            finally
            {
                con.Close();
            }
            return ret;
        }



        public int insertar_clase(string nombre, int usuario, string ruta, string descripcion, string fecha,string contenido)
        {
            String sql = "insert into proyecto (nombre_clase, ruta, fecha_creacion, descripcion, usuario,contenido)  values " +
                "('" + nombre + "', '" + ruta + "','" + fecha + "','" + descripcion + "'," + usuario + ",'"+contenido+"')";

            try
            {
                con.Open();
                consulta = new SqlCommand(sql, con);
                int a = consulta.ExecuteNonQuery();
                return a;

            }
            catch (SqlException e)
            {
                Console.WriteLine("ERROR al INSERTAR El proyecto: " + e.GetBaseException());
            }
            finally
            {
                con.Close();
            }
            return 0;
        }

        public int eliminarProducto(int id)
        {
            String sql = "exec eliminarProducto  " + id;
            try
            {
                con.Open();
                consulta = new SqlCommand(sql, con);
                int a = consulta.ExecuteNonQuery();
                return a;

            }
            catch (SqlException e)
            {
                Console.WriteLine("ERROR al eliminar el producto: " + e.GetBaseException());
            }
            finally
            {
                con.Close();
            }
            return 0;

        }

    }
}