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
        public DataTable tabla = new DataTable();
        public List<string> listacodigos = new List<string>();
        public int IDCliente = 0;

        public void agregarDatos(DataTable table, List<string> listCodes, int IDsclient)
        {
            tabla = table;
            listacodigos = listCodes;
            IDCliente = IDsclient;
        }
        public DataTable getTabla()
        {
            return tabla;
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
