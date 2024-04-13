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
                            "te.nombreTela AS Tela, a.nombre AS Almacen, e.Nombre AS Empleado, pr.Nombre AS Proveedor " +
                            "FROM dbleonv2.producto AS p " +
                            "JOIN dbleonv2.nombreproducto AS np ON p.nombreProducto_idnombreProducto = np.idnombreProducto " +
                            "JOIN dbleonv2.color AS c ON p.color_idcolor = c.idcolor " +
                            "JOIN dbleonv2.talla AS t ON p.talla_idtalla = t.idtalla " +
                            "JOIN dbleonv2.tela AS te ON p.tela_idtela = te.idtela " +
                            "JOIN dbleonv2.inventario AS i ON i.producto_idproducto = p.idproducto " +
                            "JOIN dbleonv2.almacen AS a ON i.almacen_idalmacen = a.idalmacen " +
                            "JOIN dbleonv2.empleado AS e ON i.Empleado_idEmpleado = e.idEmpleado " +
                            "JOIN dbleonv2.proveedor AS pr ON i.Proveedor_idProveedor = pr.idProveedor";

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
