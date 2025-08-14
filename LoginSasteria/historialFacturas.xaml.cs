using MySql.Data.MySqlClient;
using System;
using System.Collections;
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
    /// Lógica de interacción para historialFacturas.xaml
    /// </summary>
    public partial class historialFacturas : Window
    {
        ConexionDB cn = new ConexionDB();
        public historialFacturas()
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
            string query = @"
    SELECT 
        dv.idDetallesVenta AS Codigo,
        dv.FechaHora AS Fecha,
        c.Nombres AS Cliente,
        c.telefono AS 'Telefono Cliente',
        c.NIT AS NIT,
        em.Nombre AS Empleado,
        SUM(p.precio) AS 'Total Venta',
        COUNT(p.idproducto) AS 'Total Productos',
        CASE 
            WHEN dv.regalo = 1 THEN 'Regalo'
            ELSE '0'
        END AS Regalo
    FROM " + cn.namedb() + @".DetallesVenta dv
    INNER JOIN " + cn.namedb() + @".Cliente c
        ON dv.Cliente_idCliente = c.idCliente
    INNER JOIN " + cn.namedb() + @".RegistroVenta rv 
        ON dv.idDetallesVenta = rv.DetallesVenta_idDetallesVenta
    INNER JOIN " + cn.namedb() + @".producto p 
        ON rv.producto_idproducto = p.idproducto
    INNER JOIN " + cn.namedb() + @".Empleado em
        ON dv.Empleado_idEmpleado = em.idEmpleado
    GROUP BY dv.idDetallesVenta, dv.FechaHora, c.Nombres, c.telefono, c.NIT, em.Nombre
    ORDER BY dv.idDetallesVenta DESC;";

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
                MessageBox.Show("Seleccione una venta para imprimir la factura.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MySqlConnection conn = null;
            try
            {
                // Obtener datos de la fila seleccionada
                var row = (DataRowView)DataConsulta.SelectedItem;

                string codigoVenta = row["Codigo"].ToString();
                string fecha = row["Fecha"].ToString();
                string nombreCliente = row["Cliente"].ToString();
                string telefono = row["Telefono Cliente"].ToString();
                string nit = row["NIT"].ToString();
                string nombreEmpleado = row["Empleado"].ToString();
                string totalVenta = row["Total Venta"].ToString();

                int regalo = row["Regalo"].ToString().Trim().Equals("Regalo", StringComparison.OrdinalIgnoreCase) ? 1 : 0;

                string dataEmpleado = $"{nombreEmpleado} - {fecha}";
                string dataCliente = $"{nombreCliente}\nTel: {telefono}\nNIT: {nit}";
                string dataDetalles = $"Factura N° {codigoVenta}";

                // Consulta para obtener detalles
                string queryDetalle = @"
        SELECT 
            p.idproducto AS Producto,
            p.precio AS PrecioUnitario,
            COUNT(*) AS Cantidad,
            (p.precio * COUNT(*)) AS Subtotal
        FROM " + cn.namedb() + @".RegistroVenta rv
        INNER JOIN " + cn.namedb() + @".producto p 
            ON rv.producto_idproducto = p.idproducto
        WHERE rv.DetallesVenta_idDetallesVenta = " + codigoVenta +
                        " GROUP BY p.idproducto, p.precio" +
                        " ORDER BY p.idproducto;";

                // Crear y abrir conexión manualmente
                conn = cn.establecerCN();
                if (conn == null || conn.State != ConnectionState.Open)
                {
                    MessageBox.Show("No se pudo establecer conexión con la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Ejecutar consulta
                MySqlCommand cmd = new MySqlCommand(queryDetalle, conn);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable tablaDetalle = new DataTable();
                da.Fill(tablaDetalle);

                // Verificar datos
                if (tablaDetalle.Rows.Count == 0)
                {
                    MessageBox.Show("No se encontraron detalles para esta venta.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Generar factura
                GenerarFactura factura = new GenerarFactura();
                factura.CrearFactura(
                    codigoVenta,
                    dataEmpleado,
                    dataCliente,
                    dataDetalles,
                    totalVenta,
                    tablaDetalle,
                    regalo
                );

                MessageBox.Show("Factura generada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
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
                // Cerrar conexión manualmente en el bloque finally
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        private void regresarInicio(object sender, RoutedEventArgs e)
        {
            
            consultaInventario abrir = new consultaInventario();
            abrir.Show();
            this.Close();
        }
    }
}
