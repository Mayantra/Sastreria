using MySql.Data.MySqlClient;
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
        DataTable tabla =new DataTable();
        public static List<string> listacodigos;
        public static int IDCliente;

        ClientesVentas data = new ClientesVentas();
        public ProcesoVenta()
        {
            InitializeComponent();
            getData();
            printTable();
            getDatosCliente();
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
            tabla = data.getTabla();
            
            listacodigos = data.getLista();
            IDCliente = data.getIDCliente();
        }
        public void printTable()
        {
            DataDatos.DataContext = tabla;
        }

        public void getDatosCliente()
        {
            if (IDCliente>0)
            {
                ConexionDB cn = new ConexionDB();
                try
                {
                    
                    string query = "SELECT * FROM dbleonv2.cliente where idCliente='" + IDCliente + "';";
                    
                    MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                    MySqlDataReader dr = comando.ExecuteReader();

                    while (dr.Read())
                    {
                        txNombres.Text = dr.GetValue(1).ToString();
                        txApellidos.Text = dr.GetValue(2).ToString();
                        txTelefono.Text = dr.GetValue(3).ToString();
                        txNit.Text = dr.GetValue(5).ToString();
                        
                    }
                }
                catch (MySqlException e)
                {
                    MessageBox.Show(e.ToString());
                }
                
                cn.cerrarCN();
                
            }
            else
            {
                MessageBox.Show("Recomendamos llenar los datos del cliente");
            }
        }


        
    }
}
