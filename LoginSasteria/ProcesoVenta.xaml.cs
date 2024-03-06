using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LoginSasteria
{
    /// <summary>
    /// Lógica de interacción para ProcesoVenta.xaml
    /// </summary>
    public partial class ProcesoVenta : Window
    {
        DataTable tabla = new DataTable();
        public static List<string> listacodigos = new List<string>();
        public static int IDCliente = 0;
        public ProcesoVenta()
        {
            InitializeComponent();
            getData();
            printTable();
        }

        private void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        public void getData()
        {
            ClientesVentas data = new ClientesVentas();

            tabla = data.getTabla();
            listacodigos = data.getLista();
            IDCliente = data.getIDCliente();
        }
        public void printTable()
        {
            DataDatos.DataContext = tabla;
        }
        
    }
}
