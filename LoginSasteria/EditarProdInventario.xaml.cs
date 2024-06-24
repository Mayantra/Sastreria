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
            objConection.cerrarCN();

            try
            {
                //Vamos a traer todo de la tabla tipoproducto
                string query = "SELECT * FROM " + objConection.namedb() + ".tipoProducto;";
                MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataReader myReader = comando.ExecuteReader();
                while (myReader.Read())
                {
                    int id = myReader.GetInt32("idtipoProducto");
                    string nombre = myReader.GetString("nombreTipo");
                    ComboItem item = new ComboItem() { Id = id, Nombre = nombre };
                    cbTipoProducto.Items.Add(item);
                }
                objConection.cerrarCN();

                //Vamos a traer todo de la tabla almacen
                string query5 = "SELECT * FROM " + objConection.namedb() + ".almacen;";
                MySqlCommand comando5 = new MySqlCommand(query5, objConection.establecerCN());
                MySqlDataReader myReader5 = comando5.ExecuteReader();
                while (myReader5.Read())
                {
                    int id = myReader5.GetInt32("idalmacen"); // Asumiendo que el campo se llama idNombreProducto
                    string nombre = myReader5.GetString("Nombre");
                    ComboItem item = new ComboItem() { Id = id, Nombre = nombre };
                    cbAlmacen.Items.Add(item);
                }
                objConection.cerrarCN();

            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
        }

        private void BuscarCodBarras(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //Antes de llenar el DataGrid con los datos del codigo a buscar, se verifica que el codigo exista en el inventario
                string Query = "SELECT COUNT(*) FROM " + objConection.namedb() + ".inventario WHERE producto_idproducto = @codigo";

                using (MySqlCommand comando2 = new MySqlCommand(Query, objConection.establecerCN()))
                {
                    comando2.Parameters.AddWithValue("@codigo", txtLeerBarras.Text);
                    int count = Convert.ToInt32(comando2.ExecuteScalar());
                    objConection.cerrarCN();

                    // Si el conteo es 0, el código no existe
                    if (count == 0)
                    {
                        MessageBox.Show("El código no existe en el inventario, por favor ingrese un código valido");
                        //Limpia el DataGrid y los TextBox si anteriormente tenían datos
                        limpiar();
                        txtLeerBarras.Clear();
                        txtLeerBarras.Focus();

                    }
                    else
                    {
                        // Si el código existe, ejecuta la consulta principal
                        string query = "SELECT p.idproducto AS codigo, p.precio, np.idnombreProducto, np.Nombre AS Producto, c.nombre AS Color, t.nombreTalla AS Talla,  " +
                            "a.nombre AS Almacen, e.Nombre AS Empleado, pr.Nombre AS Proveedor " +
                            "FROM " + objConection.namedb() + ".producto AS p " +
                            "JOIN " + objConection.namedb() + ".nombreProducto AS np ON p.nombreProducto_idnombreProducto = np.idnombreProducto " +
                            "JOIN " + objConection.namedb() + ".color AS c ON p.color_idcolor = c.idcolor " +
                            "JOIN " + objConection.namedb() + ".talla AS t ON p.talla_idtalla = t.idtalla " +
                            "JOIN " + objConection.namedb() + ".inventario AS i ON i.producto_idproducto = p.idproducto " +
                            "JOIN " + objConection.namedb() + ".almacen AS a ON i.almacen_idalmacen = a.idalmacen " +
                            "JOIN " + objConection.namedb() + ".Empleado AS e ON i.Empleado_idEmpleado = e.idEmpleado " +
                            "JOIN " + objConection.namedb() + ".Proveedor AS pr ON i.Proveedor_idProveedor = pr.idProveedor WHERE p.idproducto = @codigo";

                        using (MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN()))
                        {
                            comando.Parameters.AddWithValue("@codigo", txtLeerBarras.Text);

                            // Ejecuta la consulta principal
                            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(comando);
                            DataTable dataTable = new DataTable();
                            dataAdapter.Fill(dataTable);

                            // Asigna el DataTable como la fuente de datos del DataGrid
                            dgEdiInventario.ItemsSource = dataTable.DefaultView;

                            // Comprueba si hay datos para evitar errores
                            if (dataTable.Rows.Count > 0)
                            {
                                // Asigna los valores a los TextBox
                                txtNombreProducto.Text = dataTable.Rows[0]["Producto"].ToString();
                                txtPrecio.Text = dataTable.Rows[0]["precio"].ToString();
                                txtIdProducto.Text = dataTable.Rows[0]["idnombreProducto"].ToString();
                            }

                            // Bloqueamos el txtLeerBarras para que no se pueda modificar durante el uso del formulario
                            txtLeerBarras.IsReadOnly = true;

                            CargarDatos();
                            activarTextBox();
                        }
                        objConection.cerrarCN();
                    }
                }
            }

        }

        private void CargarDatosTalla(object sender, SelectionChangedEventArgs e)
        {
            objConection.cerrarCN();

            cbTalla.Items.Clear();

            int idTipoProducto = 0; // Inicializa con un valor por defecto

            if (cbTipoProducto.SelectedItem != null)
            {
                // Asegúrate de que hay un ítem seleccionado antes de intentar acceder a su propiedad
                idTipoProducto = ((ComboItem)cbTipoProducto.SelectedItem).Id;

                //Vamos a generar el Codigo de barras segun el TipoProducto
                if (idTipoProducto == 1)
                {
                    //Vamos a traer las tallas pertenecientes a las camisas
                    string query1 = "SELECT tt.idtalla, t.nombreTalla FROM " + objConection.namedb() + ".tipoTall AS tt " +
                        "JOIN " + objConection.namedb() + ".talla AS t ON tt.talla_idtalla = t.idtalla WHERE tt.tipoProducto_idtipoProducto = '1'";
                    MySqlCommand comando1 = new MySqlCommand(query1, objConection.establecerCN());
                    MySqlDataReader myReader1 = comando1.ExecuteReader();
                    while (myReader1.Read())
                    {
                        int id = myReader1.GetInt32("idTalla"); // Asumiendo que el campo se llama idTalla
                        string nombre = myReader1.GetString("nombreTalla");
                        ComboItem item = new ComboItem() { Id = id, Nombre = nombre };
                        cbTalla.Items.Add(item);
                    }
                    objConection.cerrarCN();
                }
                else if (idTipoProducto == 2)
                {
                    //Vamos a traer las tallas pertenecientes a los pantalones
                    string query2 = "SELECT tt.idtalla, t.nombreTalla FROM " + objConection.namedb() + ".tipoTall AS tt " +
                        "JOIN " + objConection.namedb() + ".talla AS t ON tt.talla_idtalla = t.idtalla WHERE tt.tipoProducto_idtipoProducto = '2'";
                    MySqlCommand comando2 = new MySqlCommand(query2, objConection.establecerCN());
                    MySqlDataReader myReader2 = comando2.ExecuteReader();
                    while (myReader2.Read())
                    {
                        int id = myReader2.GetInt32("idTalla"); // Asumiendo que el campo se llama idTalla
                        string nombre = myReader2.GetString("nombreTalla");
                        ComboItem item = new ComboItem() { Id = id, Nombre = nombre };
                        cbTalla.Items.Add(item);
                    }
                    objConection.cerrarCN();
                }
                else if (idTipoProducto == 3)
                {
                    //Vamos a traer las tallas pertenecientes a los sacos
                    string query3 = "SELECT tt.idtalla, t.nombreTalla FROM " + objConection.namedb() + ".tipoTall AS tt " +
                         "JOIN " + objConection.namedb() + ".talla AS t ON tt.talla_idtalla = t.idtalla WHERE tt.tipoProducto_idtipoProducto = '3'";
                    MySqlCommand comando3 = new MySqlCommand(query3, objConection.establecerCN());
                    MySqlDataReader myReader3 = comando3.ExecuteReader();
                    while (myReader3.Read())
                    {
                        int id = myReader3.GetInt32("idTalla"); // Asumiendo que el campo se llama idTalla
                        string nombre = myReader3.GetString("nombreTalla");
                        ComboItem item = new ComboItem() { Id = id, Nombre = nombre };
                        cbTalla.Items.Add(item);
                    }
                    objConection.cerrarCN();
                }
                else if (idTipoProducto == 5)
                {
                    //Vamos a traer las tallas pertenecientes a los chalecos
                    string query4 = "SELECT tt.idtalla, t.nombreTalla FROM " + objConection.namedb() + ".tipoTall AS tt " +
                        "JOIN " + objConection.namedb() + ".talla AS t ON tt.talla_idtalla = t.idtalla WHERE tt.tipoProducto_idtipoProducto = '5'";
                    MySqlCommand comando4 = new MySqlCommand(query4, objConection.establecerCN());
                    MySqlDataReader myReader4 = comando4.ExecuteReader();
                    while (myReader4.Read())
                    {
                        int id = myReader4.GetInt32("idTalla"); // Asumiendo que el campo se llama idTalla
                        string nombre = myReader4.GetString("nombreTalla");
                        ComboItem item = new ComboItem() { Id = id, Nombre = nombre };
                        cbTalla.Items.Add(item);
                    }
                    objConection.cerrarCN();
                }
            }
            else
            {

            }
        }

        private void btnActualizarDatos_Click(object sender, RoutedEventArgs e)
        {
            // Validar que los campos de texto y los ComboBox no estén vacíos
            if (string.IsNullOrWhiteSpace(txtPrecio.Text) ||
                cbAlmacen.SelectedItem == null ||
                cbTalla.SelectedItem == null ||
                cbTipoProducto.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtNombreProducto.Text))
            {
                MessageBox.Show("Por favor, completa todos los campos antes de registrar.");
                return; // Salir del método para evitar ejecutar el resto del código
            }

            try
            {
                string ActualizarDatos = "UPDATE " + objConection.namedb() + ".nombreProducto SET Nombre = @NombreProducto WHERE idnombreProducto = @IdProducto; " +
                    "UPDATE " + objConection.namedb() + ".producto SET precio = @precio, talla_idtalla = @talla WHERE idproducto = @codigo; " +
                    "UPDATE " + objConection.namedb() + ".inventario SET almacen_idalmacen = @Almacen WHERE producto_idproducto = @codigo;";

                using (MySqlCommand comando = new MySqlCommand(ActualizarDatos, objConection.establecerCN()))
                {
                    int idAlmacen = ((ComboItem)cbAlmacen.SelectedItem).Id;
                    int idTalla = ((ComboItem)cbTalla.SelectedItem).Id;


                    comando.Parameters.AddWithValue("@NombreProducto", txtNombreProducto.Text);
                    comando.Parameters.AddWithValue("@IdProducto", txtIdProducto.Text);
                    comando.Parameters.AddWithValue("@precio", txtPrecio.Text);
                    comando.Parameters.AddWithValue("@Almacen", idAlmacen);
                    comando.Parameters.AddWithValue("@talla", idTalla);
                    comando.Parameters.AddWithValue("@codigo", txtLeerBarras.Text);

                    try
                    {
                        comando.ExecuteNonQuery();
                        MessageBox.Show("Producto actualizado correctamente");
                    }
                    catch (Exception ex)
                    {
                        // Manejar cualquier error aquí
                        MessageBox.Show("Error al actualizar el producto" + ex);
                    }
                    finally
                    {
                        // Cerrar la conexión
                        objConection.cerrarCN();
                        DesactivarTextBox();
                        limpiar();
                        txtLeerBarras.Clear();
                        txtLeerBarras.IsReadOnly = false;
                    }

                }

            }
            catch (Exception Er)
            {
                MessageBox.Show("Error" + Er);
            }
        }
    }
}
