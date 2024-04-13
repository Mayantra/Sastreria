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
            cbAlmacen.Items.Clear();
            cbProveedor.Items.Clear();
            txtCodBarras.Clear();
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
                string query = "SELECT * FROM dbleonv2.almacen";
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
                string query2 = "SELECT * FROM dbleonv2.proveedor";
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

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            InventarioInventario abrirMenuInventario = new InventarioInventario();
            abrirMenuInventario.Show();
            this.Close();
        }

        private bool VerificarProductoExistente(string codigoProducto)
        {
            string queryVerificacion = "SELECT COUNT(*) FROM `dbleonv2`.`inventario` WHERE `producto_idproducto` = @productoId";
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
                // Realizar la inserción
                bool insercionExitosa = InsertarInventario();

                txtCodBarras.Clear(); // Limpiamos el campo independientemente del resultado
                txtCodBarras.Focus(); // Reenfocar el campo para el siguiente escaneo
            }
        }

        private bool InsertarInventario()
        {
            string codigoProducto = txtCodBarras.Text;

            // Verificar si el producto ya existe en el inventario
            if (VerificarProductoExistente(codigoProducto))
            {
                MessageBox.Show("El producto ya está registrado en el inventario. Por favor, ingrese un código válido.");
                txtCodBarras.Clear();
                txtCodBarras.Focus();
                return false;
            }

            // Procede a insertar el producto si no existe
            string query = "INSERT INTO `dbleonv2`.`inventario` (`fechaIngreso`, `almacen_idalmacen`, `Empleado_idEmpleado`, `Proveedor_idProveedor`, `producto_idproducto`) " +
                "VALUES (@fechaIngreso, @almacen, @empleado, @proveedor, @producto)";
            using (MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN()))
            {
                // Obtenemos el ID seleccionado de cada ComboBox
                int idAlmacen = ((ComboItem)cbAlmacen.SelectedItem).Id;
                int idProveedor = ((ComboItem)cbProveedor.SelectedItem).Id;

                //Obtiene la fecha y hora actual
                DateTime now = DateTime.Now;
                string fechahora = now.ToString("yyyy-MM-dd HH:mm:ss");

                // Agregar los parámetros al comando
                comando.Parameters.AddWithValue("@fechaIngreso", fechahora);
                comando.Parameters.AddWithValue("@almacen", idAlmacen);
                comando.Parameters.AddWithValue("@empleado", _idUsuario);
                comando.Parameters.AddWithValue("@proveedor", idProveedor);
                comando.Parameters.AddWithValue("@producto", txtCodBarras.Text);

                try
                {
                    comando.ExecuteNonQuery();
                    MessageBox.Show("Producto agregado al inventario");
                }
                catch (Exception ex)
                {
                    // Manejar cualquier error aquí
                    MessageBox.Show("Error al ingresar los productos al inventario" + ex);
                }
                finally
                {
                    // Cerrar la conexión
                    objConection.cerrarCN();
                }
            }
            return true;
        }
    }
}
