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
            string query = @"
            SELECT 
                p.idproducto as 'Codigo',
                np.nombre as Nombre,
                p.precio as Precio,
                t.nombreTalla as Talla,
                tp.nombreTipo as 'Tipo Producto',
                c.nombre as 'Color'
            FROM " + cn.namedb() + @".producto p
            INNER JOIN " + cn.namedb() + @".nombreProducto np 
                ON p.nombreProducto_idnombreProducto = np.idnombreProducto
            INNER JOIN " + cn.namedb() + @".tipoTall tt 
                ON p.talla_idtalla = tt.idtalla
            INNER JOIN " + cn.namedb() + @".talla t 
                ON tt.talla_idtalla = t.idtalla
            INNER JOIN " + cn.namedb() + @".tipoProducto tp 
                ON tt.tipoProducto_idtipoProducto = tp.idtipoProducto
            INNER JOIN " + cn.namedb() + @".color c 
                ON p.color_idcolor = c.idcolor
            WHERE p.idproducto = '" + codigo + "';";


            try
            {
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader reader = comando.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string resultado = "Codigo: " + reader["Codigo"].ToString() + "\r\n" +
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
