using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Expression.Interactivity.Media;
using MySql.Data.MySqlClient;
using Mysqlx.Connection;
namespace LoginSasteria
{
    internal class ConexionDB
    {
        //MySqlConnection con = new MySqlConnection();
        public static string servidor = "94.130.216.164";
        public static string db = "hismanreina_PruebasDBLeonV2";
        public static string username = "hismanreina_isa";
        public static string password = "Isaac@17Isaac@17";
        public static string port = "3306";
        public static string conexion = $"server={servidor};port={port};user id={username};password={password};database={db};";

        private static MySqlConnection con = new MySqlConnection(conexion);


        //Prueba de conexion Hisman
        // Crea y devuelve una NUEVA conexión ya abierta
        public MySqlConnection nuevaConexion()
        {
            MySqlConnection nueva = new MySqlConnection(conexion);
            try
            {
                nueva.Open();
            }
            catch (MySqlException)
            {
                MessageBox.Show("Error, no se puede conectar. Revise su conexión a WIFI", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            return nueva;
        }

        // Cierra y desecha una conexión (si se requiere)
        public void cerrarConexion(MySqlConnection con)
        {
            if (con != null && con.State != ConnectionState.Closed)
            {
                con.Close();
                con.Dispose();
            }
        }

        //Conexion de Isaac
        public MySqlConnection establecerCN()
        {
            // Crea una nueva instancia de conexión cada vez.

            try
            {

                // 🛡️ Si está abierta, la cerramos primero
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }

                con.Open(); // Ahora sí la abrimos

                return con;
            }
            catch (MySqlException)
            {
                MessageBox.Show("Error, no se puede conectar. Revise su conexión a WIFI", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
        public MySqlConnection cerrarCN()
        {
            con.Close();
            return con;
        }

        public string namedb()
        {
            return db;
        }

        public bool IsConnectionOpen()
        {
            return con != null && con.State == ConnectionState.Open;
        }

    }
}