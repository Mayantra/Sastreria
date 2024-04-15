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

namespace LoginSasteria
{
    /// <summary>
    /// Lógica de interacción para verPrecioProducto.xaml
    /// </summary>
    public partial class verPrecioProducto : Window
    {
        ConexionDB cn = new ConexionDB();
        public verPrecioProducto()
        {
            InitializeComponent();
        }
       
        private void abrirMain(object sender, RoutedEventArgs e)
        {
            MainWindow abrir = new MainWindow();
            abrir.Show();
            this.Close();
        }

        private void consultaPrecio(object sender, RoutedEventArgs e)
        {

            string codigo = ctxPrecio.Text;
            if (codigo == null | codigo == "")
            {
                MessageBox.Show("Ingrese un codigo de producto");
            }
            else
            {
                consultarPrecio(codigo);
            }
        }
        public void consultarPrecio(string codigo)
        {

            cn.cerrarCN();
            string query = "SELECT precio FROM dbleonv2.inventario" +
                "\r\ninner join dbleonv2.producto\r\non producto_idproducto = idproducto" +
                "\r\nwhere idproducto='"+codigo+"';";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader reader = comando.ExecuteReader();
                if (reader.HasRows || reader ==null)
                {
                    while (reader.Read())
                    {
                        txPrecio.Text = "Q "+reader.GetValue(0).ToString();
                    }
                }
                else
                {
                    MessageBox.Show("El codigo ingresado no existe");
                }
                reader.Close();
                cn.cerrarCN();
            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
        }
    }
}
