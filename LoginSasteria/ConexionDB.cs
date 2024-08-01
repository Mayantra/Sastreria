using System;
using System.Collections.Generic;
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
        
        

        public MySqlConnection establecerCN()
        {
            // Crea una nueva instancia de conexión cada vez.
            
            try
            {
                con.Open();
                // MessageBox.Show("Data Base Connection Already");
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Error al conectarse a la base de datos" + e.ToString());
                return null;
            }
            return con;
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

    }
}





/*string conexion = "server=" + servidor + ";" + "port=" + port+";" + "user id=" + username + ";"
            + "password="+password+";"+"database="+db+";";
        
        public MySqlConnection establecerCN()
        {
            try
            {
                con.ConnectionString = conexion;
                con.Open();
                //MessageBox.Show("Data Base Conection Already");
            }
            catch (MySqlException e){
                MessageBox.Show("Error al conectarse a la base de datos"+e.ToString());
            }
            return con;
        }
        public MySqlConnection cerrarCN()
        {
            con.Close();
            return con;
        }*/
