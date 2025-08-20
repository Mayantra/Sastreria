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
    /// Lógica de interacción para EditarProdInventario.xaml
    /// </summary>
    public partial class EditarProdInventario : Window
    {
        ConexionDB objConection = new ConexionDB();
        public EditarProdInventario()
        {
            InitializeComponent();
            DesactivarTextBox();
        }

        void activarTextBox()
        {
            txtNombreProducto.IsReadOnly = false;
            txtPrecio.IsReadOnly = false;
        }
        void DesactivarTextBox()
        {
            txtNombreProducto.IsReadOnly = true;
            txtPrecio.IsReadOnly = true;
        }

        public class ComboItem
        {
            public int Id { get; set; }
            public string Nombre { get; set; }

            public override string ToString()
            {
                return Nombre;
            }
        }

        private void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnCancelar_Click_1(object sender, RoutedEventArgs e)
        {
            objConection.cerrarCN();
            InventarioInventario MenuInventario = new InventarioInventario();
            MenuInventario.Show();
            this.Close();
        }

        public void limpiar()
        {
            dgEdiInventario.ItemsSource = null;
            txtNombreProducto.Clear();
            txtPrecio.Clear();
            cbTalla.Items.Clear();
            cbTipoProducto.Items.Clear();
            cbAlmacen.Items.Clear();
        }

        void CargarDatos()
        {
            try
            {
                using (MySqlConnection conexion = objConection.nuevaConexion())
                {
                    //Vamos a traer todo de la tabla tipoproducto
                    string query = "SELECT * FROM " + objConection.namedb() + ".tipoProducto WHERE nombreTipo <> 'Encargo' ORDER BY nombreTipo ASC;";
                    using (MySqlCommand comando = new MySqlCommand(query, conexion))
                    using (MySqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbTipoProducto.Items.Add(new ComboItem
                            {
                                Id = reader.GetInt32("idtipoProducto"),
                                Nombre = reader.GetString("nombreTipo")
                            });
                        }
                    }
                    //Vamos a traer todo de la tabla almacen
                    string query2 = "SELECT * FROM " + objConection.namedb() + ".almacen;";
                    using (MySqlCommand comando2 = new MySqlCommand(query2, conexion))
                    using (MySqlDataReader reader2 = comando2.ExecuteReader())
                    {
                        while (reader2.Read())
                        {
                            cbAlmacen.Items.Add(new ComboItem
                            {
                                Id = reader2.GetInt32("idalmacen"),
                                Nombre = reader2.GetString("Nombre")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los datos necesarios. Verifique su conexión a internet o intente más tarde.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BuscarCodBarras(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    string codigo = txtLeerBarras.Text.Trim().Replace("\r", "").Replace("\n", "");

                    if (string.IsNullOrEmpty(codigo))
                    {
                        MessageBox.Show("Por favor, ingrese un código de barras.");
                        return;
                    }

                    using (MySqlConnection conexion = objConection.nuevaConexion())
                    {
                        if (conexion == null)
                        {
                            MessageBox.Show("Error de conexión a la base de datos.");
                            return;
                        }

                        // PRIMERO: Verificar si el código existe en INVENTARIO
                        string queryCheckInventario = "SELECT COUNT(*) FROM " + objConection.namedb() + ".inventario WHERE producto_idproducto = @codigo";

                        using (MySqlCommand comandoCheck = new MySqlCommand(queryCheckInventario, conexion))
                        {
                            comandoCheck.Parameters.AddWithValue("@codigo", codigo);
                            int count = Convert.ToInt32(comandoCheck.ExecuteScalar());

                            if (count == 0)
                            {
                                MessageBox.Show("El código no existe en el inventario. Solo se pueden editar productos inventariados.");
                                limpiar();
                                txtLeerBarras.Clear();
                                txtLeerBarras.Focus();
                                return;
                            }
                        }

                        // SEGUNDO: Si existe en inventario, buscar los datos en PRODUCTO
                        string queryProducto = @"SELECT 
                                                    p.idproducto AS codigo, 
                                                    p.precio, 
                                                    np.idnombreProducto, 
                                                    np.Nombre AS Producto, 
                                                    c.nombre AS Color, 
                                                    t.nombreTalla AS Talla,  
                                                    a.nombre AS Almacen, 
                                                    e.Nombre AS Empleado, 
                                                    pr.Nombre AS Proveedor
                                                FROM " + objConection.namedb() + @".producto AS p 
                                                    LEFT JOIN " + objConection.namedb() + @".nombreProducto AS np 
                                                        ON p.nombreProducto_idnombreProducto = np.idnombreProducto 
                                                    LEFT JOIN " + objConection.namedb() + @".color AS c 
                                                        ON p.color_idcolor = c.idcolor 
                                                    LEFT JOIN " + objConection.namedb() + @".tipoTall AS tt 
                                                        ON p.talla_idtalla = tt.idtalla 
                                                    LEFT JOIN " + objConection.namedb() + @".talla AS t 
                                                        ON tt.talla_idtalla = t.idtalla 
                                                    LEFT JOIN " + objConection.namedb() + @".inventario AS i 
                                                        ON i.producto_idproducto = p.idproducto 
                                                    LEFT JOIN " + objConection.namedb() + @".almacen AS a 
                                                        ON i.almacen_idalmacen = a.idalmacen 
                                                    LEFT JOIN " + objConection.namedb() + @".Empleado AS e 
                                                        ON i.Empleado_idEmpleado = e.idEmpleado 
                                                    LEFT JOIN " + objConection.namedb() + @".Proveedor AS pr 
                                                        ON i.Proveedor_idProveedor = pr.idProveedor 
                                                WHERE p.idproducto = @codigo";

                        using (MySqlCommand comandoProducto = new MySqlCommand(queryProducto, conexion))
                        {
                            comandoProducto.Parameters.AddWithValue("@codigo", codigo);

                            using (MySqlDataAdapter adapter = new MySqlDataAdapter(comandoProducto))
                            {
                                DataTable dataTable = new DataTable();
                                adapter.Fill(dataTable);

                                // Mostrar en DataGrid
                                dgEdiInventario.ItemsSource = dataTable.DefaultView;

                                if (dataTable.Rows.Count > 0)
                                {
                                    // Llenar TextBoxes con datos del PRODUCTO
                                    txtNombreProducto.Text = dataTable.Rows[0]["Producto"].ToString();
                                    txtPrecio.Text = dataTable.Rows[0]["precio"].ToString();
                                    txtIdProducto.Text = dataTable.Rows[0]["idnombreProducto"].ToString();

                                    txtLeerBarras.IsReadOnly = true;
                                    activarTextBox();
                                    CargarDatos();

                                    MessageBox.Show("Producto encontrado en inventario. Puede editar los datos.");
                                }
                                else
                                {
                                    // Este caso sería raro: existe en inventario pero no en producto
                                    MessageBox.Show("Error: El código existe en inventario pero no se encuentra en la tabla de productos. Contacte al administrador.");
                                    limpiar();
                                    txtLeerBarras.Clear();
                                    txtLeerBarras.Focus();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al buscar producto: {ex.Message}\n\nDetalle: {ex.StackTrace}");
                }
            }
        }

        private void CargarDatosTalla(object sender, SelectionChangedEventArgs e)
        {
            cbTalla.Items.Clear();
            objConection.cerrarCN();

            if (cbTipoProducto.SelectedItem != null)
            {
                int idTipoProducto = ((ComboItem)cbTipoProducto.SelectedItem).Id;

                // Consulta genérica para obtener las tallas según el tipo de producto
                string query = "SELECT tt.idtalla, t.nombreTalla FROM " + objConection.namedb() + ".tipoTall AS tt " +
                    "JOIN " + objConection.namedb() + ".talla AS t ON tt.talla_idtalla = t.idtalla " +
                    "WHERE tt.tipoProducto_idtipoProducto = @idTipoProducto";

                using (MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN()))
                {
                    comando.Parameters.AddWithValue("@idTipoProducto", idTipoProducto);

                    try
                    {
                        MySqlDataReader myReader = comando.ExecuteReader();
                        while (myReader.Read())
                        {
                            int id = myReader.GetInt32("idtalla"); // Asumiendo que el campo se llama idTalla
                            string nombre = myReader.GetString("nombreTalla");
                            ComboItem item = new ComboItem() { Id = id, Nombre = nombre };
                            cbTalla.Items.Add(item);
                        }
                        myReader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al cargar las tallas: " + ex.Message);
                    }
                    finally
                    {
                        objConection.cerrarCN();
                    }
                }
            }
        }

        private void btnActualizarDatos_Click(object sender, RoutedEventArgs e)
        {
            // Validar que todos los campos necesarios estén llenos
            if (string.IsNullOrWhiteSpace(txtPrecio.Text) ||
                 cbAlmacen.SelectedItem == null ||
                 cbTalla.SelectedItem == null ||
                 cbTipoProducto.SelectedItem == null ||
                 string.IsNullOrWhiteSpace(txtNombreProducto.Text))
            {
                MessageBox.Show("Por favor, completa todos los campos antes de registrar.", "Campos incompletos", MessageBoxButton.OK, MessageBoxImage.Information);
                return; // Salir del método para evitar ejecutar el resto del código
            }

            // Validar que el precio sea un número válido
            if (!decimal.TryParse(txtPrecio.Text, out decimal precio))
            {
                MessageBox.Show("Por favor, ingrese un precio válido.");
                return;
            }

            using (MySqlConnection conexion = objConection.nuevaConexion())
            {
                if (conexion == null)
                    return;

                string actualizar = "UPDATE " + objConection.namedb() + ".nombreProducto SET Nombre = @NombreProducto WHERE idnombreProducto = @IdProducto; " +
                    "UPDATE " + objConection.namedb() + ".producto SET precio = @precio, talla_idtalla = @talla WHERE idproducto = @codigo; " +
                    "UPDATE " + objConection.namedb() + ".inventario SET almacen_idalmacen = @Almacen WHERE producto_idproducto = @codigo;";

                using (MySqlCommand comando = new MySqlCommand(actualizar, conexion))
                {
                    comando.Parameters.AddWithValue("@NombreProducto", txtNombreProducto.Text);
                    comando.Parameters.AddWithValue("@IdProducto", txtIdProducto.Text);
                    comando.Parameters.AddWithValue("@precio", txtPrecio.Text);
                    comando.Parameters.AddWithValue("@Almacen", ((ComboItem)cbAlmacen.SelectedItem).Id);
                    comando.Parameters.AddWithValue("@talla", ((ComboItem)cbTalla.SelectedItem).Id);
                    comando.Parameters.AddWithValue("@codigo", txtLeerBarras.Text);

                    try
                    {
                        comando.ExecuteNonQuery();
                        MessageBox.Show("Producto actualizado correctamente.", "Actualización exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                        DesactivarTextBox();
                        limpiar();
                        txtLeerBarras.Clear();
                        txtLeerBarras.IsReadOnly = false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("No se pudo actualizar el producto. Verifique los datos e intente nuevamente.", "Error de actualización", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }
    }
}
