using System;
using System.Collections;
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
        ConexionDB objConection = new ConexionDB();
        
        public MainWindow()
        {
            InitializeComponent();
            /*ConexionDB objConection = new ConexionDB();
            objConection.establecerCN();*/

            primeraCon();
            tipoCon();

        }
        void primeraCon()
        {
            string query = "SELECT idproducto AS ID, \r\nalmacen.nombre AS Sucursal, \r\nnombreproducto.Nombre AS Producto,\r\ncolor.nombre As Color,\r\nproducto.precio AS Precio, \r\ntipoproducto.nombreTipo AS 'Tipo Producto', \r\ntalla.nombreTalla As Talla\r\nFROM dbleonv2.inventario \r\nINNER JOIN dbleonv2.producto \r\nON inventario.producto_idproducto = producto.idproducto\r\nINNER JOIN dbleonv2.nombreproducto \r\nON producto.idproducto = nombreproducto.idnombreProducto \r\nINNER JOIN dbleonv2.almacen\r\nON inventario.almacen_idalmacen = almacen.idalmacen\r\nINNER JOIN dbleonv2.color\r\nON producto.color_idcolor = color.idcolor\r\nINNER JOIN dbleonv2.tipotall\r\nON producto.talla_idtalla = tipotall.idtalla\r\nINNER JOIN dbleonv2.tipoproducto\r\nON tipotall.tipoProducto_idtipoProducto = tipoproducto.idtipoProducto\r\nINNER JOIN  dbleonv2.talla\r\nON tipotall.talla_idtalla = talla.idtalla\r\n;";

            try
            {
                MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataAdapter data = new MySqlDataAdapter(comando);
                DataTable tabla = new DataTable();

                data.Fill(tabla);

                DataConsulta.DataContext = tabla;
                objConection.cerrarCN();
            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
        }

        void tipoCon()
        {
            string query = "SELECT * FROM dbleonv2.tipoproducto;";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataAdapter data = new MySqlDataAdapter(comando);
                MySqlDataReader myread;

                myread = comando.ExecuteReader();

                while (myread.Read())
                {
                    
                    string nombres = myread.GetString("nombreTipo");
                    cbTipo.Items.Add(nombres);

                }

               
                objConection.cerrarCN();
            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
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
            primeraCon();
        }

        private void Obtener(object sender, RoutedEventArgs e)
        {
            string query = "SELECT * FROM dbleonv2.tipoproducto;";
            //MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
            object idTipo = cbTipo.SelectedValue;

            idTipo = idTipo.ToString();
            Console.WriteLine(idTipo);
        }
    }
}
