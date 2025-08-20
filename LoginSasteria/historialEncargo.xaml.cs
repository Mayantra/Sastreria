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
    /// Lógica de interacción para historialEncargo.xaml
    /// </summary>
    public partial class historialEncargo : Window
    {
        ConexionDB cn = new ConexionDB();
        public historialEncargo()
        {
            InitializeComponent();
            CargarDatos();
        }
        private void btnSalir(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CargarDatos()
        {
            string query = @"SELECT
    ep.Encargo_idEncargo as idEncargo,
    COUNT(ep.Producto_idproducto) as cantidad_productos,
    CASE 
        WHEN ep.estadoEncargo = 1 THEN 'Completo'
        ELSE '0'
    END AS Estado,
    c.telefono as 'TelefonoCliente',
    c.Nombres as 'NombreCliente',
    en.Detalles,
    e.Nombre as Empleado
FROM " + cn.namedb() + @".EncargoProducto ep
INNER JOIN " + cn.namedb() + @".Encargo en 
    ON ep.Encargo_idEncargo = en.idEncargo
INNER JOIN " + cn.namedb() + @".Cliente c
    ON en.Cliente_idCliente = c.idCliente
INNER JOIN " + cn.namedb() + @".Empleado e
    ON en.Empleado_idEmpleado = e.idEmpleado
GROUP BY ep.Encargo_idEncargo, en.idEncargo
ORDER BY ep.idEncargoProducto DESC;";

            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            MySqlDataAdapter da = null;

            try
            {
                // 1. Establecer conexión
                conn = cn.establecerCN();
                if (conn == null || conn.State != ConnectionState.Open)
                {
                    MessageBox.Show("Error de conexión", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 2. Configurar comando y adapter
                cmd = new MySqlCommand(query, conn);
                da = new MySqlDataAdapter(cmd);

                // 3. Llenar DataTable
                DataTable dt = new DataTable();
                da.Fill(dt);

                // 4. Asignar datos al control
                DataConsulta.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
            finally
            {
                // Liberar recursos en orden inverso a su creación
                if (da != null)
                {
                    da.Dispose();
                }

                if (cmd != null)
                {
                    cmd.Dispose();
                }

                if (conn != null)
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                    conn.Dispose();
                }
            }
        }

        private void imprimirFactura(object sender, RoutedEventArgs e)
        {
            if (DataConsulta.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un encargo para imprimir la factura.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MySqlConnection conn = null;
            try
            {
                // ================= OBTENER idEncargo DE LA FILA SELECCIONADA =================
                var row = (DataRowView)DataConsulta.SelectedItem;
                string idEncargo = row["idEncargo"].ToString();

                conn = cn.establecerCN();
                if (conn == null || conn.State != ConnectionState.Open)
                {
                    MessageBox.Show("No se pudo establecer conexión con la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // ================= CONSULTAR DATOS DEL ENCARGO =================
                string queryEncargo = @"
            SELECT 
                Empleado_idEmpleado, 
                Cliente_idCliente, 
                Detalles
            FROM " + cn.namedb() + @".Encargo
            WHERE idEncargo = @idEncargo";

                MySqlCommand cmdEncargo = new MySqlCommand(queryEncargo, conn);
                cmdEncargo.Parameters.AddWithValue("@idEncargo", idEncargo);

                string idEmpleado = "", idCliente = "", detallesPedido = "";
                MySqlDataReader readerEncargo = cmdEncargo.ExecuteReader();
                if (readerEncargo.Read())
                {
                    idEmpleado = readerEncargo["Empleado_idEmpleado"].ToString();
                    idCliente = readerEncargo["Cliente_idCliente"].ToString();
                    detallesPedido = readerEncargo["Detalles"].ToString();
                }
                readerEncargo.Close();

                // ================= DATOS DEL EMPLEADO =================
                string queryEmpleado = "SELECT Nombre FROM " + cn.namedb() + ".Empleado WHERE idEmpleado = @id";
                MySqlCommand cmdEmpleado = new MySqlCommand(queryEmpleado, conn);
                cmdEmpleado.Parameters.AddWithValue("@id", idEmpleado);
                string nombreEmpleado = Convert.ToString(cmdEmpleado.ExecuteScalar());

                // ================= DATOS DEL CLIENTE =================
                string queryCliente = "SELECT Nombres, telefono, NIT FROM " + cn.namedb() + ".Cliente WHERE idCliente = @id";
                MySqlCommand cmdCliente = new MySqlCommand(queryCliente, conn);
                cmdCliente.Parameters.AddWithValue("@id", idCliente);

                string nombreCliente = "", telefono = "", nit = "";
                MySqlDataReader readerCliente = cmdCliente.ExecuteReader();
                if (readerCliente.Read())
                {
                    nombreCliente = readerCliente["Nombres"].ToString();
                    telefono = readerCliente["telefono"].ToString();
                    nit = readerCliente["NIT"].ToString();
                }
                readerCliente.Close();

                // ================= OBTENER EL PRODUCTO-ENCARGO =================
                string queryProductoEncargo = "SELECT precio, abono FROM " + cn.namedb() + ".producto WHERE idproducto = @idEncargo";
                MySqlCommand cmdProductoEncargo = new MySqlCommand(queryProductoEncargo, conn);
                cmdProductoEncargo.Parameters.AddWithValue("@idEncargo", idEncargo);
                MySqlDataReader readerEncargoProducto = cmdProductoEncargo.ExecuteReader();

                decimal total = 0, abono = 0;
                if (readerEncargoProducto.Read())
                {
                    total = Convert.ToDecimal(readerEncargoProducto["precio"]);
                    abono = Convert.ToDecimal(readerEncargoProducto["abono"]);
                }
                readerEncargoProducto.Close();

                // ================= SOLO CODIGOS DE PRODUCTOS ASOCIADOS AL ENCARGO =================
                string queryProductos = @"
            SELECT 
                p.idproducto AS CodigoBarras
            FROM " + cn.namedb() + @".EncargoProducto ep
            INNER JOIN " + cn.namedb() + @".producto p 
                ON ep.producto_idproducto = p.idproducto
            WHERE ep.Encargo_idEncargo = @idEncargo;
        ";

                MySqlCommand cmdProductos = new MySqlCommand(queryProductos, conn);
                cmdProductos.Parameters.AddWithValue("@idEncargo", idEncargo);

                MySqlDataAdapter da = new MySqlDataAdapter(cmdProductos);
                DataTable tablaProductos = new DataTable();
                da.Fill(tablaProductos);

                if (tablaProductos.Rows.Count == 0)
                {
                    MessageBox.Show("No se encontraron productos asociados a este encargo.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Lista de códigos de barras
                List<string> codigosBarras = tablaProductos.AsEnumerable()
                    .Select(r => r["CodigoBarras"].ToString())
                    .ToList();

                // ================= ARMAR DATOS FACTURA =================
                string dataEmpleado = $"{nombreEmpleado} - {DateTime.Now:dd/MM/yyyy}";
                string dataCliente = $"{nombreCliente}\nTel: {telefono}\nNIT: {nit}";
                string dataDetalles = $"Encargo N° {idEncargo}";

                // ================= CREAR FACTURA =================
                FacturaEncargoProductos factura = new FacturaEncargoProductos();
                factura.CrearFactura(
                    idEncargo,
                    dataEmpleado,
                    dataCliente,
                    dataDetalles,
                    total.ToString("F2"),
                    tablaProductos,
                    codigosBarras,
                    abono.ToString("F2"),
                    detallesPedido
                );

                MessageBox.Show("Factura de encargo generada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error de base de datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        private void regresarInicio(object sender, RoutedEventArgs e)
        {

            CrearEncargoInventario abrir = new CrearEncargoInventario();
            abrir.Show();
            this.Close();
        }
    }

}
