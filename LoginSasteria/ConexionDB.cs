using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;
namespace LoginSasteria
{
    internal class ConexionDB
    {
        MySqlConnection con = new MySqlConnection();
        public static string servidor = "localhost";
        public static string db = "dbLeonV2";
        public static string username = "root";
        public static string password = "Admin";
        public static string port="3306";

        string conexion = "server=" + servidor + ";" + "port=" + port+";" + "user id=" + username + ";"
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
        }

    }
}
