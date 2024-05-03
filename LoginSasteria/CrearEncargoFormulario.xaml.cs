using iText.IO.Util;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Lógica de interacción para CrearEncargoFormulario.xaml
    /// </summary>
    public partial class CrearEncargoFormulario : Window
    {
        ConexionDB objConection = new ConexionDB();
        public CrearEncargoFormulario()
        {
            InitializeComponent();
            tipoCon();
        }
        private void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        void tipoCon()
        {
            string query = "SELECT * FROM dbleonv2.tipoproducto;";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataReader myread;

                myread = comando.ExecuteReader();

                while (myread.Read())
                {

                    string nombres = myread.GetString("nombreTipo");
                    CbTipo.Items.Add(nombres);

                }
                objConection.cerrarCN();
                
            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
        }
        private int ObtenerCantidad()
        {
            // Accede a la propiedad Value del control IntegerUpDown
            int cantidad = NumberData.Value ?? 0; // Usa 0 como valor predeterminado si el valor es nulo
            return cantidad;
        }
        private string ObtenerValorComboBox()
        {
            // Accede al valor seleccionado del ComboBox
            string valorSeleccionadoCompleto = CbTipo.SelectedItem?.ToString(); // Obtén la cadena completa seleccionada
            if (valorSeleccionadoCompleto != null)
            {
                // Divide la cadena en palabras
                string[] palabras = Regex.Split(valorSeleccionadoCompleto, @"\W+");

                // Toma las primeras dos palabras
                string primerasDosPalabras = "";
               
                primerasDosPalabras = palabras[0] + palabras[1];
                
                return primerasDosPalabras;
            }
            else
            {
                MessageBox.Show("No se ha seleccionado ningún elemento en el ComboBox.");
                return "";
            }
        }

        private void AgregarProducto(object sender, RoutedEventArgs e)
        {
            int  cantidad = ObtenerCantidad();
            string tipo = ObtenerValorComboBox();
            MessageBox.Show("Elegiste "+tipo+" "+cantidad.ToString());

        }
    }
}
