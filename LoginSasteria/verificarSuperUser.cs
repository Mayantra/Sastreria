using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LoginSasteria
{
    internal class verificarSuperUser
    {
        ConexionDB cn = new ConexionDB();
        int iduser=0;

        public Boolean superUser()
        {
            Boolean estado = false;
            cn.cerrarCN();
            leerPass leerUser = new leerPass();
            iduser =leerUser.getIDuser();
            try
            {
                string query = "SELECT superUser FROM "+cn.namedb()+".Empleado where idEmpleado ='"+iduser+"';";

                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();

                while (dr.Read())
                {
                    if (dr.GetUInt32(0)==1)
                    {
                        estado = true;
                        return estado;
                    }
                    else
                    {
                        return estado;
                    }
                }
                cn.cerrarCN();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
                return estado;
            }
            return estado;
        }
    }
}
