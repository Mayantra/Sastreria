using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace LoginSasteria
{
    internal class ClientesVentas
    {
        public static DataTable tabla = new DataTable();
        public static List<string> listacodigos;
        public static int IDCliente;
        public static int IDUsuario;
        public static int regalo;
        public static Boolean existeCliente;

        public void setDatos(DataTable table, List<string> listCodes, int IDsclient, int regalo, Boolean Cliente)
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
            existeCliente = Cliente;
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
        public int getRegalo()
        {
            return regalo;
        }
        public Boolean getExistenciaCliente()
        {
            return existeCliente;
        }
    }
}
