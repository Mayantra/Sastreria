using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Mysqlx.Crud;

namespace LoginSasteria
{
    /// <summary>
    /// Lógica de interacción para CrearBarras.xaml
    /// </summary>
    public partial class CrearBarras : Window
    {
        ConexionDB objConection = new ConexionDB();

        public static int IdnombreProducto = 0;

        public CrearBarras()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static long GenerarNumeroAleatorio()
        {
            Random rng = new Random();
            byte[] buf = new byte[8];
            rng.NextBytes(buf); // Llenamos el buffer con bytes aleatorios
            long longRand = BitConverter.ToInt64(buf, 0);

            return Math.Abs(longRand % 99999999999999999) + 1;
        }

        public void Limpiar()
        {
            cbTalla.Items.Clear();
            cbColor.Items.Clear();
            cbColor.IsEnabled = true;
            cbNombre.Items.Clear();
            cbNombre.IsEnabled = true;
            //cbTipoProducto.SelectedValue = null;
            cbTipoProducto.Items.Clear();
            txtOtroNombre.Clear();
            txtPrecio.Clear();
            txtCantidad.Text = "1";
            txtOtroNombre.Clear();
            txtNuevColor.Clear();
            txtOtroNombre.Visibility = Visibility.Collapsed;
            txtNuevColor.Visibility = Visibility.Collapsed;
            lblNArticulo.Visibility = Visibility.Collapsed;
            lblNcolor.Visibility = Visibility.Collapsed;
        }

        //Permite localizar el ultimo ID de la tabla nombreProducto para poder insertar un nuevo dato
        public int MaxID()
        {
            objConection.cerrarCN();
            int id = 0;

            try
            {
                string query = "SELECT max(idnombreProducto) FROM " + objConection.namedb() + ".nombreProducto";

                using (MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN()))
                {
                    try
                    {
                        MySqlDataReader reader = comando.ExecuteReader();
                        while (reader.Read())
                        {
                            id = reader.GetInt32(0);
                        }
                        reader.Close();
                        //MessageBox.Show("NombreProducto insertado correctamente.");
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Error al insertar el producto: " + ex.Message);
                    }
                    finally
                    {
                        // Cerrar la conexión
                        objConection.cerrarCN();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return id;
        }

        //Permite localizar el ultimo ID de la tabla color para poder insertar un nuevo dato
        public int MaxColorID()
        {
            objConection.cerrarCN();
            int id = 0;

            try
            {
                string query = "SELECT max(idcolor) FROM " + objConection.namedb() + ".color";

                using (MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN()))
                {
                    try
                    {
                        MySqlDataReader reader = comando.ExecuteReader();
                        while (reader.Read())
                        {
                            id = reader.GetInt32(0);
                        }
                        reader.Close();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Error al obtener el máximo ID de color: " + ex.Message);
                    }
                    finally
                    {
                        // Cerrar la conexión
                        objConection.cerrarCN();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return id;
        }


        private void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            objConection.cerrarCN();

            // Validar que los campos de texto y los ComboBox no estén vacíos
            if (string.IsNullOrWhiteSpace(txtPrecio.Text) ||
                cbTalla.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtDetalles.Text) ||
                (string.IsNullOrWhiteSpace(cbNombre.Text) && string.IsNullOrWhiteSpace(txtOtroNombre.Text)) ||
                (string.IsNullOrWhiteSpace(cbColor.Text) && string.IsNullOrWhiteSpace(txtNuevColor.Text)))
            {
                MessageBox.Show("Por favor, completa todos los campos antes de registrar.");
                return; // Salir del método para evitar ejecutar el resto del código
            }

            // Si cbNombre y txtOtroNombre están vacíos, no permitir insertar
            if (string.IsNullOrWhiteSpace(cbNombre.Text) && string.IsNullOrWhiteSpace(txtOtroNombre.Text))
            {
                MessageBox.Show("Por favor, selecciona un nombre o ingresa el nombre del nuevo artículo.");
                return; // Salir del método para evitar ejecutar el resto del código
            }

            // Si cbColor y txtNuevoColor están vacíos, no permitir insertar
            if (string.IsNullOrWhiteSpace(cbColor.Text) && string.IsNullOrWhiteSpace(txtNuevColor.Text))
            {
                MessageBox.Show("Por favor, selecciona un color o ingresa el nuevo color.");
                return; // Salir del método para evitar ejecutar el resto del código
            }

            //Permite verificar que el txtCantidad contenga un dato mayor a 0
            int cantidad = 0;
            if (!int.TryParse(txtCantidad.Text, out cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Por favor, ingresa una cantidad válida.");
                return;
            }

            try
            {
                // Verificar si necesitamos insertar un nuevo nombre de producto
                if (string.IsNullOrWhiteSpace(cbNombre.Text) && !string.IsNullOrWhiteSpace(txtOtroNombre.Text))
                {
                    objConection.cerrarCN();

                    // Ingresamos primero el nuevo nombre del producto dentro de la tabla 'nombreProducto'
                    int maxid = MaxID() + 1;
                    IdnombreProducto = maxid;
                    string nombreProducto = txtOtroNombre.Text;
                    string query = "INSERT INTO " + objConection.namedb() + ".nombreProducto (`idnombreProducto`, `Nombre`) VALUES (@idnombreProducto, @nombreProducto);";

                    using (MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN()))
                    {
                        comando.Parameters.AddWithValue("@idnombreProducto", maxid);
                        comando.Parameters.AddWithValue("@nombreProducto", nombreProducto);

                        try
                        {
                            comando.ExecuteNonQuery();
                            //MessageBox.Show("NombreProducto insertado correctamente.");
                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show("Error al insertar el producto: " + ex.Message);
                            return; // Salir del método para evitar ejecutar el resto del código
                        }
                        finally
                        {
                            // Cerrar la conexión
                            objConection.cerrarCN();
                        }
                    }
                }

                // Verificar si necesitamos insertar un nuevo color
                int idColor;
                if (string.IsNullOrWhiteSpace(cbColor.Text) && !string.IsNullOrWhiteSpace(txtNuevColor.Text))
                {
                    objConection.cerrarCN();

                    // Ingresamos primero el nuevo color dentro de la tabla 'color'
                    int maxColorId = MaxColorID() + 1;
                    idColor = maxColorId;
                    string nuevoColor = txtNuevColor.Text;
                    string queryColor = "INSERT INTO " + objConection.namedb() + ".color (`idcolor`, `Nombre`) VALUES (@idcolor, @nuevoColor);";

                    using (MySqlCommand comandoColor = new MySqlCommand(queryColor, objConection.establecerCN()))
                    {
                        comandoColor.Parameters.AddWithValue("@idcolor", maxColorId);
                        comandoColor.Parameters.AddWithValue("@nuevoColor", nuevoColor);

                        try
                        {
                            comandoColor.ExecuteNonQuery();
                            //MessageBox.Show("Color insertado correctamente.");
                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show("Error al insertar el color: " + ex.Message);
                            return; // Salir del método para evitar ejecutar el resto del código
                        }
                        finally
                        {
                            // Cerrar la conexión
                            objConection.cerrarCN();
                        }
                    }
                }
                else
                {
                    idColor = ((ComboItem)cbColor.SelectedItem).Id;
                }

                // Insertamos los datos dentro de la BD
                for (int i = 0; i < cantidad; i++)
                {
                    objConection.cerrarCN();

                    string CodBarras = CrearCodigoBarras();

                    string query2 = "INSERT INTO " + objConection.namedb() + ".producto (idproducto, abono, precio, nombreProducto_idnombreProducto, color_idcolor, talla_idtalla, detalles, imagen, fechaCodigo) " +
                        "VALUES (@codigo, @abono, @precio, @idNombreProducto, @idColor, @idTalla, @detalles, @imagen, @fecha)";
                    using (MySqlCommand comando = new MySqlCommand(query2, objConection.establecerCN()))
                    {
                        // Obtenemos el ID seleccionado de cada ComboBox
                        int idTalla = ((ComboItem)cbTalla.SelectedItem).Id;
                        int idNombreProducto = string.IsNullOrWhiteSpace(cbNombre.Text) ? IdnombreProducto : ((ComboItem)cbNombre.SelectedItem).Id;

                        // Obtiene la fecha y hora actual
                        DateTime now = DateTime.Now;
                        string fechahora = now.ToString("yyyy-MM-dd HH:mm:ss");

                        // Agregar los parámetros al comando
                        comando.Parameters.AddWithValue("@codigo", CodBarras);
                        comando.Parameters.AddWithValue("@abono", txtPrecio.Text);
                        comando.Parameters.AddWithValue("@precio", txtPrecio.Text);
                        comando.Parameters.AddWithValue("@idNombreProducto", idNombreProducto);
                        comando.Parameters.AddWithValue("@idColor", idColor);
                        comando.Parameters.AddWithValue("@idTalla", idTalla);
                        comando.Parameters.AddWithValue("@detalles", txtDetalles.Text);
                        comando.Parameters.AddWithValue("@imagen", DBNull.Value);
                        comando.Parameters.AddWithValue("@fecha", fechahora);

                        try
                        {
                            comando.ExecuteNonQuery();
                            //MessageBox.Show("Producto ingresado correctamente");
                        }
                        catch (Exception ex)
                        {
                            // Manejar cualquier error aquí
                            MessageBox.Show("Error al ingresar los datos" + ex);
                        }
                        finally
                        {
                            // Cerrar la conexión
                            objConection.cerrarCN();
                        }
                    }
                }
                MessageBox.Show("Se han creado exitosamente " + txtCantidad.Text + " códigos de barras únicos para el producto ingresado");
                objConection.cerrarCN();
            }
            catch (Exception Er)
            {
                MessageBox.Show("Error" + Er);
            }

            Limpiar();
            CargarDatos();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            //No sirve jajaja
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

                //Vamos a traer todo de la tabla color
                string query2 = "SELECT * FROM " + objConection.namedb() + ".color;";
                MySqlCommand comando2 = new MySqlCommand(query2, objConection.establecerCN());
                MySqlDataReader myReader2 = comando2.ExecuteReader();
                while (myReader2.Read())
                {
                    int id = myReader2.GetInt32("idcolor");
                    string nombre = myReader2.GetString("nombre");
                    ComboItem item = new ComboItem() { Id = id, Nombre = nombre };
                    cbColor.Items.Add(item);
                }
                objConection.cerrarCN();

                //Vamos a traer todo de la tabla nombreproducto
                string query5 = "SELECT * FROM " + objConection.namedb() + ".nombreProducto;";
                MySqlCommand comando5 = new MySqlCommand(query5, objConection.establecerCN());
                MySqlDataReader myReader5 = comando5.ExecuteReader();
                while (myReader5.Read())
                {
                    int id = myReader5.GetInt32("idnombreProducto"); // Asumiendo que el campo se llama idNombreProducto
                    string nombre = myReader5.GetString("Nombre");
                    ComboItem item = new ComboItem() { Id = id, Nombre = nombre };
                    cbNombre.Items.Add(item);
                }
                objConection.cerrarCN();

            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
        }

        // Crea los códigos de barras según el producto seleccionado en el objeto cbTipoProducto
        public string CrearCodigoBarras()
        {
            if (cbTipoProducto.SelectedItem == null)
            {
                throw new InvalidOperationException("Debe seleccionar un tipo de producto.");
            }

            string tipoProductoTexto = ((ComboItem)cbTipoProducto.SelectedItem).Nombre;
            string prefijo = tipoProductoTexto.Substring(0, Math.Min(3, tipoProductoTexto.Length)).ToUpper();

            bool codigoExiste = true;
            string codigo = "";

            while (codigoExiste)
            {
                long numeroAleatorio = GenerarNumeroAleatorio();
                codigo = prefijo + numeroAleatorio.ToString();
                codigoExiste = ExisteCodigoBarras(codigo);
            }

            return codigo;
        }


        //Verificar si el codigo de barras ya existen o no en la BD
        bool ExisteCodigoBarras(string codigoBarras)
        {
            objConection.cerrarCN();

            string query = "SELECT COUNT(*) FROM " + objConection.namedb() + ".producto WHERE idproducto = @codigo";

            using (MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN()))
            {
                comando.Parameters.AddWithValue("@codigo", codigoBarras);

                // Ejecuta la consulta
                int count = Convert.ToInt32(comando.ExecuteScalar());

                // Retorna true si se encontró al menos un registro con ese código de barras
                return count > 0;
            }
        }

        //Se cargan las tallas al objeto cbTalla segun el producto seleccionado
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
                        int id = myReader1.GetInt32("idtalla"); // Asumiendo que el campo se llama idTalla
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
                        int id = myReader2.GetInt32("idtalla"); // Asumiendo que el campo se llama idTalla
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
                        int id = myReader3.GetInt32("idtalla"); // Asumiendo que el campo se llama idTalla
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

        private void btnCancelar_Click_1(object sender, RoutedEventArgs e)
        {
            Limpiar();
            objConection.cerrarCN();
            crearBarrasMenu abrirMenu = new crearBarrasMenu();
            abrirMenu.Show();
            this.Close();
        }

        private void btnNuevArticulo_Click(object sender, RoutedEventArgs e)
        {
            cbNombre.IsEnabled = false;
            cbNombre.Items.Clear();
            lblNArticulo.Visibility = Visibility.Visible;
            txtOtroNombre.Visibility = Visibility.Visible;
        }

        private void btnNuevColor_Click(object sender, RoutedEventArgs e)
        {
            cbColor.IsEnabled = false;
            cbColor.Items.Clear();
            lblNcolor.Visibility = Visibility.Visible;
            txtNuevColor.Visibility = Visibility.Visible;
        }
    }
}