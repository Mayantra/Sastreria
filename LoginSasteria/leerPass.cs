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
        
        
        public int setPass(string user, string pass)
        {
            ConexionDB cn = new ConexionDB();
            int estado = 0;
            string contrasena="";
            string query = "SELECT pin FROM "+cn.namedb()+".Empleado where Usuario ='"+user+"';";
            
            MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
            MySqlDataReader dr = comando.ExecuteReader();
            while (dr.Read()) {
                contrasena = dr.GetString("pin");
            }

            if(pass == contrasena)
            {
                estado = 1;
                usuario = user;
                //MessageBox.Show("Contraseña Correcta");

            }
            else
            {
                estado=0;
                MessageBox.Show("Contraseña Incorrecta");
            }
            cn.cerrarCN();
            return estado;
        }
        public string getUser()
        {
            string nombreUser ="";
            ConexionDB cn = new ConexionDB();
            string query = "SELECT Nombre FROM "+cn.namedb()+".Empleado where Usuario ='" + usuario + "';";
            
            MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
            MySqlDataReader dr = comando.ExecuteReader();
            
            while (dr.Read())
            {
                nombreUser = dr.GetString("Nombre");
            }
            cn.cerrarCN();
            return nombreUser;
        }
        public int getIDuser()
        {
            int IDUser = 0;
            ConexionDB cn = new ConexionDB();
            string query = "SELECT idEmpleado FROM "+cn.namedb()+".Empleado where Usuario ='" + usuario + "';";
            
            MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
            MySqlDataReader dr = comando.ExecuteReader();

            while (dr.Read())
            {
                IDUser = dr.GetInt32("idEmpleado");
            }
            cn.cerrarCN();
            return IDUser;
        }

        public Boolean getAcceso(string user, string pass)
        {
            Boolean estado= false;
            string contrasena = "";
            ConexionDB cn = new ConexionDB();
            string query = "SELECT pin FROM "+cn.namedb()+".Empleado where Usuario ='" + user + "';";
            
            MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
            MySqlDataReader dr = comando.ExecuteReader();
            while (dr.Read())
            {
                contrasena = dr.GetString("pin");
            }

            if (pass == contrasena)
            {
                estado = true;                

            }
            else
            {
                estado = false;
                MessageBox.Show("Contraseña Incorrecta");
            }
            cn.cerrarCN();
            return estado;
        }
        public int getIdLogUser(string users)
        {
            int IDUser = 0;
            ConexionDB cn = new ConexionDB();
            string query = "SELECT idEmpleado FROM "+cn.namedb()+".Empleado where Usuario ='" + users + "';";
            
            MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
            MySqlDataReader dr = comando.ExecuteReader();

            while (dr.Read())
            {
                IDUser = dr.GetInt32("idEmpleado");
            }
            cn.cerrarCN();
            return IDUser;
        }
        
    }
}
