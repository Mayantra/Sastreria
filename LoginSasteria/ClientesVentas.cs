using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginSasteria
{
    internal class ClientesVentas
    {
        public static DataTable tabla = new DataTable();
        public static List<string> listacodigos;
        public static int IDCliente;
        public static int IDUsuario;

        public void setDatos(DataTable table, List<string> listCodes, int IDsclient)
        {
            tabla = table;
            foreach (DataRow dataRow in tabla.Rows)
            {
                foreach (var item in dataRow.ItemArray)
                {
                    Console.WriteLine(item);
                }
            }
            listacodigos = listCodes;
            IDCliente = IDsclient;
            ProcesoVenta abrir = new ProcesoVenta();
            abrir.Show();
        }

        
        public DataTable getTabla()
        {
            DataTable aux = new DataTable();
            aux = tabla;
            return aux;
        }
        public List<string> getLista()
        {
            return listacodigos;
        }
        public int getIDCliente()
        {
            return IDCliente;
        }

    }
}
