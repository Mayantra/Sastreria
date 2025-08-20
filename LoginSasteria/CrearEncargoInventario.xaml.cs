using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
using ZXing.Rendering;

namespace LoginSasteria
{
    /// <summary>
    /// Lógica de interacción para CrearEncargoInventario.xaml
    /// </summary>
    public partial class CrearEncargoInventario : Window
    {
        ConexionDB cn = new ConexionDB();
        List<string> productoIds = new List<string>();
        public CrearEncargoInventario()
        {
            InitializeComponent();
        }
        private void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void LeerCodigoBarras(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buscarEncargo(txVerificar.Text);
                txVerificar.Clear();
            }
        }

        private void buscarEncargo(string codigo)
        {


            string query = "SELECT producto_idproducto FROM " + cn.namedb() + ".EncargoProducto " +
                "where Encargo_idEncargo='" + codigo + "' and estadoEncargo = '0';";
            string queryCheck = "SELECT * FROM " + cn.namedb() + ".EncargoProducto " +
                "WHERE Encargo_idEncargo='" + codigo + "';";

            try
            {
                cn.cerrarCN();
                MySqlCommand comandoCheck = new MySqlCommand(queryCheck, cn.establecerCN());
                MySqlDataReader readerCheak = comandoCheck.ExecuteReader();
                if (!readerCheak.HasRows)
                {
                    MessageBox.Show("El código no existe o es incorrecto");
                }
                else
                {
                    cn.cerrarCN();
                    MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                    MySqlDataReader reader = comando.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string productoId = reader.GetString("producto_idproducto");
                            productoIds.Add(productoId);


                        }
                        string products = "";

                        foreach (string id in productoIds)
                        {
                            products += "\n" + id;
                        }
                        RichEstados.Document.Blocks.Clear();
                        RichEstados.AppendText("El encargo aún no está listo, faltan " + productoIds.Count + " códigos:" +
                            products);


                    }
                    else
                    {
                        RichEstados.Document.Blocks.Clear();
                        RichEstados.Background = new SolidColorBrush(Colors.LightGreen);
                        RichEstados.Foreground = new SolidColorBrush(Colors.Black);
                        RichEstados.FontSize = 45;
                        RichEstados.AppendText("EL ENCARGO ESTÁ LISTO");
                    }




                }




            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);


            }
            finally
            {
                cn.cerrarCN();

            }



        }

        private void AbrirCrearEncargo(object sender, RoutedEventArgs e)
        {
            CrearEncargoFormulario abrir = new CrearEncargoFormulario();
            abrir.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainInventario abrir = new mainInventario();
            abrir.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ventaInventario abrir = new ventaInventario();
            abrir.Show();
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            consultaInventario abrir = new consultaInventario();
            abrir.Show();
            this.Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            InventarioInventario abrir = new InventarioInventario();
            abrir.Show();
            this.Close();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            crearBarrasMenu abrir = new crearBarrasMenu();
            abrir.Show();
            this.Close();
        }

        private void btnCancelar_Click_1(object sender, RoutedEventArgs e)
        {
            mainInventario abrir = new mainInventario();
            abrir.Show();
            this.Close();
        }

        private void abrirEncargoAbonos(object sender, RoutedEventArgs e)
        {
            CrearEncargoAbonos abrir = new CrearEncargoAbonos();
            abrir.Show();
            this.Close();
        }

        private void AbrirRecepcion(object sender, RoutedEventArgs e)
        {
            rebibirEncargoInv abrir = new rebibirEncargoInv();
            abrir.Show();
            this.Close();
        }

        private void historialEncargo(object sender, RoutedEventArgs e)
        {
            historialEncargo abrir = new historialEncargo();
            abrir.Show();
            this.Close();
        }
    }
}
