using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace LoginSasteria
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            /*ConexionDB objConection = new ConexionDB();
            objConection.establecerCN();*/
        }

        private void btnSalir(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ConsultaGeneral(object sender, RoutedEventArgs e)
        {
            ConexionDB objConection = new ConexionDB();
            
            string query = "SELECT * FROM dbleonv2.inventario;";
            
            try
            {
                MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataAdapter data = new MySqlDataAdapter(comando);
                DataTable tabla = new DataTable();
                
                data.Fill(tabla);

                DataConsulta.DataContext = tabla;
            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
        }
    }
}
