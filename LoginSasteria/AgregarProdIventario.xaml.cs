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
using static LoginSasteria.CrearBarras;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock.Themes;

namespace LoginSasteria
{
    /// <summary>
    /// Lógica de interacción para AgregarProdIventario.xaml
    /// </summary>
    public partial class AgregarProdIventario : Window
    {
        ConexionDB objConection = new ConexionDB();
        private int _idUsuario; // Campo para almacenar el ID del usuario.

        private int productoEstadoId = 0;
        private int estadoRepeticiones = 0;
        private bool productoEstadoObtenido = false;
        private List<string> listaCodigosValidos = new List<string>();

        // 🔐 Control para bloquear ComboBox tras primer escaneo
        private bool comboBloqueado = false;

        public AgregarProdIventario()
        {
            objConection.cerrarCN();
            InitializeComponent();
            CargarDatos();
            cargarUser();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        public void Limpiar()
        {
            CargarDatos();
            cbAlmacen.Items.Clear();
            cbProveedor.Items.Clear();
            txtCodBarras.Clear();

            // Limpiar la lista y el DataGrid
            listaCodigosValidos.Clear();
            dgProductos.ItemsSource = null;
            objConection.cerrarCN();

            // Reiniciar campos y estados
            txbContadorCod.Text = "0"; // o el valor inicial que corresponda
            txtCodBarras.IsEnabled = true;
            btnRegistrar.IsEnabled = false;

            // Resetear la variable productoEstadoId y productoEstadoObtenido
            productoEstadoId = 0;
            productoEstadoObtenido = false;

            //Volvemos a habilitar los comboBox
            comboBloqueado = false;
            cbAlmacen.IsEnabled = true;
            cbProveedor.IsEnabled = true;
        }

        void cargarUser()
        {
            leerPass user = new leerPass();
            string usuario = user.getUser();
            txtEmpleado.Text = usuario;

            leerPass iduser = new leerPass();
            _idUsuario = iduser.getIDuser(); // Almacena el ID del usuario en el campo.
        }

        void CargarDatos()
        {
            try
            {
                using (var conn = objConection.nuevaConexion())
                {
                    //Vamos a traer todo de la tabla almacen
                    string query = "SELECT * FROM " + objConection.namedb() + ".almacen";
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbAlmacen.Items.Add(new ComboItem { Id = reader.GetInt32("idalmacen"), Nombre = reader.GetString("nombre") });
                        }
                    }

                    //Vamos a traer todo de la tabla proveedor
                    string query2 = "SELECT * FROM " + objConection.namedb() + ".Proveedor";
                    using (var cmd2 = new MySqlCommand(query2, conn))
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        while (reader2.Read())
                        {
                            cbProveedor.Items.Add(new ComboItem { Id = reader2.GetInt32("idProveedor"), Nombre = reader2.GetString("Nombre") });
                        }
                    }
                }
            }
            catch (MySqlException)
            {
                MessageBox.Show("Error al cargar los datos. Verifique la conexión a la base de datos.", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            DateTime fechaActual = DateTime.Now;
            string hora = fechaActual.ToString("hh");
            string min = fechaActual.ToString("mm");
            string hora24 = fechaActual.ToString("HH");
            if (int.Parse(hora24) >= 12)
            {
                txblockHora.Text = hora + "\n" + min + " pm";
            }
            else
            {
                txblockHora.Text = hora + "\n" + min + " am";
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

        private void ActualizarEstadoContador()
        {
            if (int.TryParse(txbContadorCod.Text, out int contador) && contador == 0)
            {
                txtCodBarras.IsEnabled = false;
                btnRegistrar.IsEnabled = true;
            }
        }

        private bool VerificarProductoEnRegistroVenta(string codigoProducto)
        {
            string query = $"SELECT COUNT(*) FROM {objConection.namedb()}.RegistroVenta WHERE producto_idproducto = @productoId";
            using (var conn = objConection.nuevaConexion())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@productoId", codigoProducto);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private bool VerificarProductoExistente(string codigoProducto)
        {
            string query = $"SELECT COUNT(*) FROM {objConection.namedb()}.inventario WHERE producto_idproducto = @productoId";
            using (var conn = objConection.nuevaConexion())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@productoId", codigoProducto);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private bool VerificarBarrasUserAlmacen(string codigoProducto, string empleadoNombre)
        {
            try
            {
                // Separar el número inicial (almacén_idalmacen) del código de barras
                string numeroInicial = new string(codigoProducto.TakeWhile(char.IsDigit).ToArray());

                if (string.IsNullOrEmpty(numeroInicial))
                    return false;

                // Comparar el número inicial con almacen_idalmacen en la tabla Empleado
                string query = $"SELECT COUNT(*) FROM {objConection.namedb()}.Empleado " +
                               "WHERE almacen_idalmacen = @numeroInicial AND Nombre = @empleadoNombre";

                using (var conn = objConection.nuevaConexion())
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@numeroInicial", numeroInicial);
                    cmd.Parameters.AddWithValue("@empleadoNombre", empleadoNombre.Trim());

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count > 0;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al verificar si el código de barras pertenece al almacén del empleado.\n\n" +
                                "Detalles: " + ex.Message, "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void EscanerEinsertarProductos(object sender, KeyEventArgs e)
        {
            // Preparar la interfaz para el primer escaneo
            txtCodBarras.Focus();

            if (e.Key != Key.Enter) return;

            string codigoBarras = txtCodBarras.Text.Trim();
            if (string.IsNullOrEmpty(codigoBarras)) return;

            if (cbAlmacen.SelectedItem == null || cbProveedor.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un almacén y un proveedor antes de escanear productos.", "Campos requeridos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Verificar si el código de barras pertenece al idalmacen del empleado
                if (!VerificarBarrasUserAlmacen(codigoBarras, txtEmpleado.Text))
                {
                    MessageBox.Show("El código de barras no pertenece a este almacén.", "Código inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtCodBarras.Clear(); txtCodBarras.Focus(); return;
                }

                // Verificar si el producto ya existe en el RegistroVenta
                if (VerificarProductoEnRegistroVenta(codigoBarras))
                {
                    MessageBox.Show("El producto ya fue vendido.", "Producto inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txbContadorCod.Text = "0"; txtCodBarras.Clear(); txtCodBarras.Focus(); return;
                }

                // Verificar si el producto ya existe en el inventario
                if (VerificarProductoExistente(codigoBarras))
                {
                    MessageBox.Show("El producto ya está en el inventario.", "Producto duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txbContadorCod.Text = "0"; txtCodBarras.Clear(); txtCodBarras.Focus(); return;
                }

                if (!productoEstadoObtenido)
                {
                    // Obtener ProductoEstado_idProductoEstado
                    string queryEstado = $"SELECT ProductoEstado_idProductoEstado FROM {objConection.namedb()}.producto_has_ProductoEstado WHERE producto_idproducto = @codigo";
                    using (var conn = objConection.nuevaConexion())
                    using (var cmd = new MySqlCommand(queryEstado, conn))
                    {
                        cmd.Parameters.AddWithValue("@codigo", codigoBarras);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                productoEstadoId = reader.GetInt32(0);
                                productoEstadoObtenido = true;
                            }
                            else
                            {
                                MessageBox.Show("Código no encontrado."); return;
                            }
                        }
                    }

                    // Contar repeticiones de ProductoEstado_idProductoEstado
                    string queryCount = $"SELECT COUNT(*) FROM {objConection.namedb()}.producto_has_ProductoEstado WHERE ProductoEstado_idProductoEstado = @id";
                    using (var conn = objConection.nuevaConexion())
                    using (var cmd = new MySqlCommand(queryCount, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", productoEstadoId);
                        estadoRepeticiones = Convert.ToInt32(cmd.ExecuteScalar());
                        txbContadorCod.Text = estadoRepeticiones.ToString();
                    }
                }

                // Verificar si el código de barras pertenece al mismo producto_idproducto
                string queryVerificar = "SELECT producto_idproducto " +
                                        $"FROM {objConection.namedb()}.producto_has_ProductoEstado " +
                                        "WHERE producto_idproducto = @codigoBarras AND ProductoEstado_idProductoEstado = @productoEstadoId";

                using (var conn = objConection.nuevaConexion())
                using (var comandoVerificar = new MySqlCommand(queryVerificar, conn))
                {
                    comandoVerificar.Parameters.AddWithValue("@codigoBarras", codigoBarras);
                    comandoVerificar.Parameters.AddWithValue("@productoEstadoId", productoEstadoId);

                    using (var reader = comandoVerificar.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            MessageBox.Show("El código de barras no pertenece al mismo producto.", "Código inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                            txtCodBarras.Clear();
                            return;
                        }
                    }
                }

                // Consulta a la base de datos para obtener datos del producto
                string queryProducto = $"SELECT p.idproducto AS codigo, p.precio, np.Nombre AS Producto, c.nombre AS Color,ta.nombreTalla AS Talla, p.detalles " +
                                        $"FROM {objConection.namedb()}.producto p " +
                                        $"JOIN {objConection.namedb()}.nombreProducto np ON p.nombreProducto_idnombreProducto = np.idnombreProducto " +
                                        $"JOIN {objConection.namedb()}.color c ON p.color_idcolor = c.idcolor " +
                                        $"JOIN {objConection.namedb()}.tipoTall tt ON p.talla_idtalla = tt.idtalla " +
                                        $"JOIN {objConection.namedb()}.talla ta ON tt.talla_idtalla = ta.idtalla " +
                                        $"WHERE p.idproducto = @codigo";

                using (var conn = objConection.nuevaConexion())
                using (var cmd = new MySqlCommand(queryProducto, conn))
                {
                    cmd.Parameters.AddWithValue("@codigo", codigoBarras);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var producto = new
                            {
                                codigo = reader["codigo"].ToString(),
                                precio = reader["precio"].ToString(),
                                Producto = reader["Producto"].ToString(),
                                Color = reader["Color"].ToString(),
                                Talla = reader["Talla"].ToString(),
                                detalles = reader["detalles"].ToString()
                            };

                            // Verificar si el código ya ha sido agregado
                            var productos = (List<dynamic>)dgProductos.ItemsSource ?? new List<dynamic>();
                            if (!productos.Any(p => p.codigo == producto.codigo))
                            {
                                productos.Add(producto);
                                dgProductos.ItemsSource = productos;
                                dgProductos.Items.Refresh();
                                // Agregar el código a la lista de códigos válidos
                                listaCodigosValidos.Add(codigoBarras);

                                // Bloquear los ComboBox tras el primer escaneo
                                if (!comboBloqueado && cbAlmacen.Items.Count > 0 && cbProveedor.Items.Count > 0)
                                {
                                    cbAlmacen.IsEnabled = false;
                                    cbProveedor.IsEnabled = false;
                                    comboBloqueado = true;
                                }

                                if (int.TryParse(txbContadorCod.Text, out int contador))
                                {
                                    // Resta uno al valor y actualiza el TextBlock con el nuevo valor
                                    contador--;
                                    txbContadorCod.Text = contador.ToString();
                                    ActualizarEstadoContador();
                                }
                            }
                            else
                            {
                                MessageBox.Show("El producto ya fue escaneado.", "Duplicado", MessageBoxButton.OK, MessageBoxImage.Information);
                            }

                            txtCodBarras.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Producto no encontrado en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error al consultar los datos del producto. Intente nuevamente.", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InsertarInventario(string codigoProducto)
        {
            string query = $"INSERT INTO {objConection.namedb()}.inventario (fechaIngreso, almacen_idalmacen, Empleado_idEmpleado, Proveedor_idProveedor, producto_idproducto) " +
                           $"VALUES (@fecha, @almacen, @empleado, @proveedor, @producto)";
            using (var conn = objConection.nuevaConexion())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@almacen", ((ComboItem)cbAlmacen.SelectedItem).Id);
                cmd.Parameters.AddWithValue("@empleado", _idUsuario);
                cmd.Parameters.AddWithValue("@proveedor", ((ComboItem)cbProveedor.SelectedItem).Id);
                cmd.Parameters.AddWithValue("@producto", codigoProducto);
                cmd.ExecuteNonQuery();
            }
        }

        private void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            // Verificar que los ComboBox no estén vacíos
            if (cbAlmacen.SelectedItem == null || cbProveedor.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un almacén y un proveedor antes de registrar.", "Campos requeridos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            List<string> errores = new List<string>();
            foreach (var codigo in listaCodigosValidos)
            {
                try
                {
                    InsertarInventario(codigo);
                }
                catch
                {
                    errores.Add($"Error al registrar el código: {codigo}");
                }
            }

            if (errores.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errores), "Errores encontrados", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                MessageBox.Show("Todos los productos fueron registrados exitosamente.", "Registro completo", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            Limpiar();
            CargarDatos();
        }


        private void btnCancelar_Click_1(object sender, RoutedEventArgs e)
        {
            InventarioInventario abrirMenuInventario = new InventarioInventario();
            abrirMenuInventario.Show();
            this.Close();
        }
    }
}
