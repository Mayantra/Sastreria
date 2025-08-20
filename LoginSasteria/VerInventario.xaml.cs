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

        //conexion Hisman

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
            string query = @"SELECT 
                                p.idproducto AS codigo, 
                                p.precio, 
                                np.idnombreProducto, 
                                np.Nombre AS Producto, 
                                c.nombre AS Color, 
                                t.nombreTalla AS Talla,  
                                a.nombre AS Almacen, 
                                e.Nombre AS Empleado, 
                                pr.Nombre AS Proveedor,
                                i.fechaIngreso
                            FROM " + objConection.namedb() + @".inventario AS i 
                            LEFT JOIN " + objConection.namedb() + @".producto AS p 
                                ON i.producto_idproducto = p.idproducto 
                            LEFT JOIN " + objConection.namedb() + @".nombreProducto AS np 
                                ON p.nombreProducto_idnombreProducto = np.idnombreProducto 
                            LEFT JOIN " + objConection.namedb() + @".color AS c 
                                ON p.color_idcolor = c.idcolor 
                            LEFT JOIN " + objConection.namedb() + @".tipoTall AS tt 
                                ON p.talla_idtalla = tt.idtalla 
                            LEFT JOIN " + objConection.namedb() + @".talla AS t 
                                ON tt.talla_idtalla = t.idtalla 
                            LEFT JOIN " + objConection.namedb() + @".almacen AS a 
                                ON i.almacen_idalmacen = a.idalmacen 
                            LEFT JOIN " + objConection.namedb() + @".Empleado AS e 
                                ON i.Empleado_idEmpleado = e.idEmpleado 
                            LEFT JOIN " + objConection.namedb() + @".Proveedor AS pr 
                                ON i.Proveedor_idProveedor = pr.idProveedor
                            ORDER BY i.fechaIngreso DESC";

            using (MySqlConnection conexion = objConection.nuevaConexion())
            {
                if (conexion == null)
                    return;

                try
                {
                    using (MySqlCommand comando = new MySqlCommand(query, conexion))
                    {
                        MySqlDataAdapter dataAdapter = new MySqlDataAdapter(comando);
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        dgEdiInventario.ItemsSource = dataTable.DefaultView;

                        // Opcional: Mostrar cuántos registros se cargaron
                        //MessageBox.Show($"Se cargaron {dataTable.Rows.Count} registros del inventario.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar inventario: {ex.Message}");
                }
            }
        }

        private void btnCancelar_Click_1(object sender, RoutedEventArgs e)
        {
            InventarioInventario MenuInventario = new InventarioInventario();
            MenuInventario.Show();
            this.Close();
        }
    }
}
