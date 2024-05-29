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

        List <string> codigosProductos = new List <string> ();//Alamecenar todos los codigos
        public CrearEncargoFormulario()
        {
            InitializeComponent();
            tipoCon();
        }
        private static readonly Random rng = new Random();
        private void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        public static long GenerarNumeroAleatorio()
        {
            byte[] buf = new byte[8];
            rng.NextBytes(buf); // Llenamos el buffer con bytes aleatorios
            long longRand = BitConverter.ToInt64(buf, 0);

            return Math.Abs(longRand % 999999999999999999) + 1;
        }
        void tipoCon()
        {
            string query = "SELECT * FROM "+objConection.namedb()+".tipoProducto;";
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
                if (valorSeleccionadoCompleto.Length >= 2)
                {
                    string firstTwoLetters = valorSeleccionadoCompleto.Substring(0, 2);
                    return firstTwoLetters;
                }
                else
                {
                    return "";
                }
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
            AgregarCodigosLista(tipo, cantidad);
            for (int i = 0; i < cantidad; i++)
            {
                Console.WriteLine(codigosProductos[i]);
            }


        }
        public void AgregarCodigosLista(string tipo, int cantidad)
        {
            for (int i = 0; i < cantidad; i++)
            {
                long numeroAleatorio = GenerarNumeroAleatorio();
                codigosProductos.Add(tipo + numeroAleatorio);
            }

        }

        private void CancelarFormulario(object sender, RoutedEventArgs e)
        {
            mainInventario abrir = new mainInventario();
            abrir.Show();
            this.Close();
        }

        //Generar Encargo,
        public void GenerarEncargo()
        {
            string CodigoEncargo = "";

            CodigoEncargo= "EN"+GenerarNumeroAleatorio();

            MessageBox.Show(CodigoEncargo);
        }

        private void CrearEncargo(object sender, RoutedEventArgs e)
        {
            GenerarEncargo();
        }
    }
}
