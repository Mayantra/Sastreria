using iTextSharp.text.xml;
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
using ZXing;

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
            ctxPrecio.Focus();
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
            string query = "SELECT idproducto as 'Código'," +
                "\r\nnombreProducto.nombre as Nombre," +
                "\r\nprecio as Precio," +
                "\r\ntalla.nombreTalla as Talla" +
                ",\r\ntipoProducto.nombreTipo as 'Tipo Producto'," +
                "\r\ncolor.nombre as 'Color'" +
                "\r\nFROM " + cn.namedb() + ".producto " +
                "\r\ninner join nombreProducto on producto.nombreProducto_idnombreProducto = nombreProducto.idnombreProducto" +
                "\r\ninner join tipoTall on producto.talla_idtalla = tipoTall.idtalla" +
                "\r\ninner join talla on tipoTall.talla_idtalla = talla.idtalla" +
                "\r\ninner join tipoProducto on tipoTall.tipoProducto_idtipoProducto = tipoProducto.idtipoProducto" +
                "\r\ninner join color on producto.color_idcolor = color.idcolor" +
                "\r\nwhere idproducto='" + codigo + "';";

            
            try
            {
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader reader = comando.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string resultado = "Código: " + reader["Código"].ToString() + "\r\n" +
                                           "Nombre: " + reader["Nombre"].ToString() + "\r\n" +
                                           "Precio: Q " + reader["Precio"].ToString() + "\r\n" +
                                           "Talla: " + reader["Talla"].ToString() + "\r\n" +
                                           "Tipo Producto: " + reader["Tipo Producto"].ToString() + "\r\n" +
                                           "Color: " + reader["Color"].ToString();

                        txPrecio.Text = resultado;
                    }
                }
                else
                {
                    MessageBox.Show("El código ingresado no existe.");
                }

                reader.Close();
                cn.cerrarCN();
                
            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
        }

        private void LeerCodigoBarras(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                consultaPrecio(null,null);
                ctxPrecio.Clear();
            }
        }
    }
}
