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
            }
        }

        private Boolean buscarEncargo(string codigo)
        {
            
            string query = "SELECT count(producto_idproducto) FROM "+cn.namedb()+".inventario where producto_idproducto = '"+codigo+"';";

            try
            {
                cn.cerrarCN();
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader myread;

                myread = comando.ExecuteReader();
                int dato = 0;
                while (myread.Read())
                {

                    dato = myread.GetInt32(0);

                }
                myread.Close();
                cn.cerrarCN();
                if (dato > 0)
                {
                    txEstados.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#09594D"));
                    txEstados.Text = "El producto se encuentra en el inventario";
                    return true;             

                }
                else
                {
                    txEstados.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#590E09"));
                    txEstados.Text = "El producto aún NO está listo";
                    return false;
                }
                       

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;

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
    }
}
