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

        public AgregarProdIventario()
        {
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
                //Vamos a traer todo de la tabla almacen
                string query = "SELECT * FROM " + objConection.namedb() + ".almacen";
                MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataReader myReader = comando.ExecuteReader();
                while (myReader.Read())
                {
                    int id = myReader.GetInt32("idalmacen");
                    string nombre = myReader.GetString("nombre");
                    ComboItem item = new ComboItem() { Id = id, Nombre = nombre };
                    cbAlmacen.Items.Add(item);
                }
                objConection.cerrarCN();

                //Vamos a traer todo de la tabla proveedor
                string query2 = "SELECT * FROM " + objConection.namedb() + ".Proveedor";
                MySqlCommand comando2 = new MySqlCommand(query2, objConection.establecerCN());
                MySqlDataReader myReader2 = comando2.ExecuteReader();
                while (myReader2.Read())
                {
                    int id = myReader2.GetInt32("idProveedor");
                    string nombre = myReader2.GetString("Nombre");
                    ComboItem item = new ComboItem() { Id = id, Nombre = nombre };
                    cbProveedor.Items.Add(item);
                }
                objConection.cerrarCN();
            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error al cargar los datos: " + x);
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
            if (int.TryParse(txbContadorCod.Text, out int contador))
            {
                // Verifica si el contador llegó a cero
                if (contador == 0)
                {
                    txtCodBarras.IsEnabled = false;
                    btnRegistrar.IsEnabled = true;
                }
            }
        }

        private bool VerificarProductoEnRegistroVenta(string codigoProducto)
        {
            string queryVerificacion = "SELECT COUNT(*) FROM " + objConection.namedb() + ".RegistroVenta WHERE `producto_idproducto` = @productoId";
            using (MySqlCommand comandoVerificacion = new MySqlCommand(queryVerificacion, objConection.establecerCN()))
            {
                comandoVerificacion.Parameters.AddWithValue("@productoId", codigoProducto);
                int count = Convert.ToInt32(comandoVerificacion.ExecuteScalar());
                objConection.cerrarCN();

                return count > 0;
            }
        }

        private bool VerificarProductoExistente(string codigoProducto)
        {
            string queryVerificacion = "SELECT COUNT(*) FROM " + objConection.namedb() + ".inventario WHERE `producto_idproducto` = @productoId";
            using (MySqlCommand comandoVerificacion = new MySqlCommand(queryVerificacion, objConection.establecerCN()))
            {
                comandoVerificacion.Parameters.AddWithValue("@productoId", codigoProducto);
                //objConection.establecerCN().Open();
                int count = Convert.ToInt32(comandoVerificacion.ExecuteScalar());
                objConection.cerrarCN();

                return count > 0;
            }
        }

        private void EscanerEinsertarProductos(object sender, KeyEventArgs e)
        {
            // Preparar la interfaz para el primer escaneo
            txtCodBarras.Focus();

            if (e.Key == Key.Enter)
            {
                string codigoBarras = txtCodBarras.Text.Trim();
                if (!string.IsNullOrEmpty(codigoBarras))
                {
                    try
                    {
                        objConection.cerrarCN();
                        // Verificar si el producto ya existe en el RegistroVenta
                        if (VerificarProductoEnRegistroVenta(codigoBarras))
                        {
                            MessageBox.Show("El producto ya fue vendido, por favor ingrese un código valido");
                            txbContadorCod.Text = "0";
                            txtCodBarras.Clear();
                            txtCodBarras.Focus();
                            return;
                        }

                        // Verificar si el producto ya existe en el inventario
                        if (VerificarProductoExistente(codigoBarras))
                        {
                            MessageBox.Show("El producto ya está registrado en el inventario. Por favor, ingrese un código válido.");
                            txbContadorCod.Text = "0";
                            txtCodBarras.Clear();
                            txtCodBarras.Focus();
                            return;
                        }

                        if (!productoEstadoObtenido)
                        {
                            // Obtener ProductoEstado_idProductoEstado
                            string queryEstado = "SELECT ProductoEstado_idProductoEstado " +
                                                 "FROM " + objConection.namedb() + ".producto_has_ProductoEstado " +
                                                 "WHERE producto_idproducto = @codigoBarras";
                            objConection.cerrarCN();
                            using (MySqlCommand comandoEstado = new MySqlCommand(queryEstado, objConection.establecerCN()))
                            {
                                comandoEstado.Parameters.AddWithValue("@codigoBarras", codigoBarras);
                                using (MySqlDataReader reader = comandoEstado.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        productoEstadoId = reader.GetInt32("ProductoEstado_idProductoEstado");
                                        productoEstadoObtenido = true;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Código de barras no encontrado");
                                        return;
                                    }
                                }
                            }
                            objConection.cerrarCN();
                            // Contar repeticiones de ProductoEstado_idProductoEstado
                            string queryContar = "SELECT COUNT(*) AS Repeticiones " +
                                                 "FROM " + objConection.namedb() + ".producto_has_ProductoEstado " +
                                                 "WHERE ProductoEstado_idProductoEstado = @productoEstadoId";
                            objConection.cerrarCN();
                            using (MySqlCommand comandoContar = new MySqlCommand(queryContar, objConection.establecerCN()))
                            {
                                comandoContar.Parameters.AddWithValue("@productoEstadoId", productoEstadoId);
                                estadoRepeticiones = Convert.ToInt32(comandoContar.ExecuteScalar());

                                txbContadorCod.Text = estadoRepeticiones.ToString();
                            }
                            objConection.cerrarCN();
                        }

                        // Verificar si el código de barras pertenece al mismo producto_idproducto
                        string queryVerificar = "SELECT producto_idproducto " +
                                                "FROM " + objConection.namedb() + ".producto_has_ProductoEstado " +
                                                "WHERE producto_idproducto = @codigoBarras AND ProductoEstado_idProductoEstado = @productoEstadoId";
                        objConection.cerrarCN();
                        using (MySqlCommand comandoVerificar = new MySqlCommand(queryVerificar, objConection.establecerCN()))
                        {
                            comandoVerificar.Parameters.AddWithValue("@codigoBarras", codigoBarras);
                            comandoVerificar.Parameters.AddWithValue("@productoEstadoId", productoEstadoId);
                            using (MySqlDataReader reader = comandoVerificar.ExecuteReader())
                            {
                                if (!reader.Read())
                                {
                                    MessageBox.Show("El código de barras no pertenece al mismo producto");
                                    txtCodBarras.Clear();
                                    return;
                                }
                            }
                        }

                        // Consulta a la base de datos para obtener datos del producto
                        string queryProducto = "SELECT p.idproducto AS codigo, p.precio, np.Nombre AS Producto, c.nombre AS Color, t.nombreTalla AS Talla, p.detalles " +
                                               "FROM " + objConection.namedb() + ".producto AS p " +
                                               "JOIN " + objConection.namedb() + ".nombreProducto AS np ON p.nombreProducto_idnombreProducto = np.idnombreProducto " +
                                               "JOIN " + objConection.namedb() + ".color AS c ON p.color_idcolor = c.idcolor " +
                                               "JOIN " + objConection.namedb() + ".talla AS t ON p.talla_idtalla = t.idtalla " +
                                               "WHERE p.idproducto = @codigoBarras";
                        objConection.cerrarCN();
                        using (MySqlCommand comandoProducto = new MySqlCommand(queryProducto, objConection.establecerCN()))
                        {
                            comandoProducto.Parameters.AddWithValue("@codigoBarras", codigoBarras);

                            using (MySqlDataReader reader = comandoProducto.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // Crear un objeto con los datos obtenidos
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
                                    objConection.cerrarCN();
                                    var productos = (List<dynamic>)dgProductos.ItemsSource;
                                    objConection.cerrarCN();
                                    if (productos == null)
                                    {
                                        objConection.cerrarCN();
                                        productos = new List<dynamic>();
                                        objConection.cerrarCN();
                                        dgProductos.ItemsSource = productos;
                                    }

                                    if (!productos.Any(p => p.codigo == producto.codigo))
                                    {
                                        productos.Add(producto);
                                        dgProductos.Items.Refresh();

                                        // Agregar el código a la lista de códigos válidos
                                        listaCodigosValidos.Add(codigoBarras);

                                        if (int.TryParse(txbContadorCod.Text, out int contador))
                                        {
                                            // Resta uno al valor y actualiza el TextBlock con el nuevo valor
                                            contador--;
                                            txbContadorCod.Text = contador.ToString();
                                            ActualizarEstadoContador();
                                        }
                                        objConection.cerrarCN();
                                    }
                                    else
                                    {
                                        MessageBox.Show("El código de barras ya ha sido escaneado.");
                                    }

                                }
                                else
                                {
                                    MessageBox.Show("Código de barras no encontrado en la tabla producto.");
                                }
                            }
                            objConection.cerrarCN();
                        }
                        objConection.cerrarCN();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Error al consultar la base de datos: " + ex.Message);
                    }
                    finally
                    {
                        objConection.cerrarCN();
                    }
                }
            }
            objConection.cerrarCN();
            txtCodBarras.Clear();
            objConection.cerrarCN();
        }

        private void InsertarInventario(string codigoProducto)
        {
            string query = "INSERT INTO " + objConection.namedb() + ".inventario " +
                           "(`fechaIngreso`, `almacen_idalmacen`, `Empleado_idEmpleado`, `Proveedor_idProveedor`, `producto_idproducto`) " +
                           "VALUES (@fechaIngreso, @almacen, @empleado, @proveedor, @producto)";
            using (MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN()))
            {
                // Obtenemos el ID seleccionado de cada ComboBox
                int idAlmacen = ((ComboItem)cbAlmacen.SelectedItem).Id;
                int idProveedor = ((ComboItem)cbProveedor.SelectedItem).Id;

                // Obtiene la fecha y hora actual
                DateTime now = DateTime.Now;
                string fechahora = now.ToString("yyyy-MM-dd HH:mm:ss");

                // Agregar los parámetros al comando
                comando.Parameters.AddWithValue("@fechaIngreso", fechahora);
                comando.Parameters.AddWithValue("@almacen", idAlmacen);
                comando.Parameters.AddWithValue("@empleado", _idUsuario);
                comando.Parameters.AddWithValue("@proveedor", idProveedor);
                comando.Parameters.AddWithValue("@producto", codigoProducto);

                comando.ExecuteNonQuery();
            }
        }

        private void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            objConection.cerrarCN();
            List<string> errores = new List<string>();
            objConection.cerrarCN();

            foreach (var codigoProducto in listaCodigosValidos)
            {
                objConection.cerrarCN();
                try
                {
                    InsertarInventario(codigoProducto);
                }
                catch (Exception ex)
                {
                    errores.Add($"Error con el código {codigoProducto}: {ex.Message}");
                }
            }

            if (errores.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errores));
            }
            else
            {
                MessageBox.Show("Todos los productos fueron registrados correctamente.");
            }
            Limpiar();
        }

        private void btnCancelar_Click_1(object sender, RoutedEventArgs e)
        {
            InventarioInventario abrirMenuInventario = new InventarioInventario();
            abrirMenuInventario.Show();
            this.Close();
        }
    }
}
