using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;

namespace LoginSasteria
{
    internal class leerPass
    {
        public static string usuario;
        public static string contra;
        
        public int setPass(string user, string pass)
        {
            int estado = 0;
            string contrasena="";
            string query = "SELECT pin FROM dbleonv2.empleado where Usuario ='"+user+"';";
            ConexionDB cn = new ConexionDB();
            MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
            MySqlDataReader dr = comando.ExecuteReader();
            while (dr.Read()) {
                contrasena = dr.GetString("pin");
            }

            if(pass == contrasena)
            {
                estado = 1;
                usuario = user;
                contra = pass;
                MessageBox.Show("Contraseña Correcta");

            }
            else
            {
                estado=0;
                MessageBox.Show("Contraseña Incorrecta");
            }

            return estado;
        }
        string getUser()
        {
            return usuario;
        }
        string getPass()
        {
            return contra;
        }
        
    }
}
