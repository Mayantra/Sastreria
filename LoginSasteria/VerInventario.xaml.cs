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
    /// Lógica de interacción para VerInventario.xaml
    /// </summary>
    public partial class VerInventario : Window
    {
        ConexionDB objConection = new ConexionDB();

        public VerInventario()
        {
            InitializeComponent();
            CargarInventario();
        }
        private void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        void CargarInventario()
        {
            string query = "SELECT p.idproducto AS codigo, p.precio, np.idnombreProducto, np.Nombre AS Producto, c.nombre AS Color, t.nombreTalla AS Talla,  " +
                            "a.nombre AS Almacen, e.Nombre AS Empleado, pr.Nombre AS Proveedor " +
                            "FROM " + objConection.namedb() + ".producto AS p " +
                            "JOIN " + objConection.namedb() + ".nombreProducto AS np ON p.nombreProducto_idnombreProducto = np.idnombreProducto " +
                            "JOIN " + objConection.namedb() + ".color AS c ON p.color_idcolor = c.idcolor " +
                            "JOIN " + objConection.namedb() + ".talla AS t ON p.talla_idtalla = t.idtalla " +
                            "JOIN " + objConection.namedb() + ".inventario AS i ON i.producto_idproducto = p.idproducto " +
                            "JOIN " + objConection.namedb() + ".almacen AS a ON i.almacen_idalmacen = a.idalmacen " +
                            "JOIN " + objConection.namedb() + ".Empleado AS e ON i.Empleado_idEmpleado = e.idEmpleado " +
                            "JOIN " + objConection.namedb() + ".Proveedor AS pr ON i.Proveedor_idProveedor = pr.idProveedor";

            using (MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN()))
            {
                // Ejecuta la consulta principal
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(comando);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                // Asigna el DataTable como la fuente de datos del DataGrid
                dgEdiInventario.ItemsSource = dataTable.DefaultView;
            }
            objConection.cerrarCN();
        }

        private void btnCancelar_Click_1(object sender, RoutedEventArgs e)
        {
            InventarioInventario MenuInventario = new InventarioInventario();
            MenuInventario.Show();
            this.Close();
        }
    }
}
