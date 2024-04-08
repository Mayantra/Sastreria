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

            return Math.Abs(longRand % 999999999999999999) + 1;
        }

        public void Limpiar()
        {
            cbTalla.Items.Clear();
            cbColor.Items.Clear();
            cbNombre.Items.Clear();
            cbTela.Items.Clear();
            //cbTipoProducto.SelectedValue = null;
            cbTipoProducto.Items.Clear();
            txtOtroNombre.Clear();
            txtPrecio.Clear();
            txtCantidad.Text = "1";
        }

        public int MaxID()
        {
            objConection.cerrarCN();
            int id = 0;

            try
            {
                string query = "SELECT max(idnombreProducto) FROM dbleonv2.nombreproducto";

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

        private void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            objConection.cerrarCN();

            // Validar que los campos de texto y los ComboBox no estén vacíos
            if (string.IsNullOrWhiteSpace(txtPrecio.Text) ||
                cbNombre.SelectedItem == null ||
                cbTela.SelectedItem == null ||
                cbTalla.SelectedItem == null ||
                cbColor.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtDetalles.Text) ||
                (cbNombre.SelectedItem.ToString() == "OTRO" && string.IsNullOrWhiteSpace(txtOtroNombre.Text))) // Condición adicional si cbNombre es "OTRO"
            {
                MessageBox.Show("Por favor, completa todos los campos antes de registrar.");
                return; // Salir del método para evitar ejecutar el resto del código
            }

            // Si cbNombre es "OTRO", se debe validar también txtOtroNombre
            if (cbNombre.SelectedItem.ToString() == "OTRO" && string.IsNullOrWhiteSpace(txtOtroNombre.Text))
            {
                MessageBox.Show("Por favor, ingresa el nombre del nuevo articulo.");
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

                if (cbNombre.SelectedItem != null && cbNombre.SelectedItem.ToString() == "OTRO")//Obtengo lo que hay dentro del ComboBox cbNombre
                {
                    objConection.cerrarCN();

                    //Si lo que esta en el ComboBox cbNombre es iguala "OTRO" los datos a insertar en la BD seran los siguientes

                    //Ingresamos primero el nuevo nombre del producto dentro de la tabla 'nombreproducto'
                    int maxid = MaxID() + 1;
                    IdnombreProducto = maxid;
                    string nombreProducto = txtOtroNombre.Text;
                    string query = "INSERT INTO `dbleonv2`.`nombreproducto` (`idnombreProducto`, `Nombre`) VALUES ('" + maxid + "', '" + nombreProducto + "');";//maxid

                    using (MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN()))
                    {
                        try
                        {
                            MySqlDataReader reader = comando.ExecuteReader();
                            //MessageBox.Show("NombreProducto insertado correctamente.");
                            reader.Close();
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

                    //Insertamos los datos dentro de la BD
                    for (int i = 0; i < cantidad; i++)
                    {
                        objConection.cerrarCN();

                        string CodBarras = CrearCodigoBarras();

                        string query2 = "INSERT INTO `dbleonv2`.`producto` (idproducto, precio, nombreProducto_idnombreProducto, color_idcolor, talla_idtalla, tela_idtela, detalles, imagen, fechaCodigo) " +
                            "VALUES (@codigo, @precio, @idNombreProducto, @idColor, @idTalla, @idTela, @detalles, @imagen, @fecha)";
                        using (MySqlCommand comando = new MySqlCommand(query2, objConection.establecerCN()))
                        {
                            // Obtenemos el ID seleccionado de cada ComboBox
                            //int idTipoProducto = ((ComboItem)cbTipoProducto.SelectedItem).Id;
                            int idColor = ((ComboItem)cbColor.SelectedItem).Id;
                            int idTalla = ((ComboItem)cbTalla.SelectedItem).Id;
                            int idTela = ((ComboItem)cbTela.SelectedItem).Id;
                            //int idNombreProducto = ((ComboItem)cbNombre.SelectedItem).Id;

                            //Obtiene la fecha y hora actual
                            DateTime now = DateTime.Now;
                            string fechahora = now.ToString("yyyy-MM-dd HH:mm:ss");

                            // Agregar los parámetros al comando
                            comando.Parameters.AddWithValue("@codigo", CodBarras);
                            comando.Parameters.AddWithValue("@precio", txtPrecio.Text);
                            comando.Parameters.AddWithValue("@idNombreProducto", IdnombreProducto);
                            comando.Parameters.AddWithValue("@idColor", idColor);
                            comando.Parameters.AddWithValue("@idTalla", idTalla);
                            comando.Parameters.AddWithValue("@idTela", idTela);
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
                    MessageBox.Show("Se han creado exitosamente " + txtCantidad.Text + " codigos de barras unicos para el producto ingresado");
                    objConection.cerrarCN();

                }
                else
                {
                    //Si lo que esta en el ComboBox cbNombre son diferentes a "OTRO" los datos a insertar en la BD seran los siguientes
                    for (int i = 0; i < cantidad; i++)
                    {
                        objConection.cerrarCN();

                        string CodBarras = CrearCodigoBarras();

                        string query = "INSERT INTO `dbleonv2`.`producto` (idproducto, precio, nombreProducto_idnombreProducto, color_idcolor, talla_idtalla, tela_idtela, detalles, imagen, fechaCodigo) " +
                            "VALUES (@codigo, @precio, @idNombreProducto, @idColor, @idTalla, @idTela, @detalles, @imagen, @fecha)";
                        using (MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN()))
                        {
                            // Obtenemos el ID seleccionado de cada ComboBox
                            //int idTipoProducto = ((ComboItem)cbTipoProducto.SelectedItem).Id;
                            int idColor = ((ComboItem)cbColor.SelectedItem).Id;
                            int idTalla = ((ComboItem)cbTalla.SelectedItem).Id;
                            int idTela = ((ComboItem)cbTela.SelectedItem).Id;
                            int idNombreProducto = ((ComboItem)cbNombre.SelectedItem).Id;

                            //Obtiene la fecha y hora actual
                            DateTime now = DateTime.Now;
                            string fechahora = now.ToString("yyyy-MM-dd HH:mm:ss");

                            // Agregar los parámetros al comando
                            comando.Parameters.AddWithValue("@codigo", CodBarras);
                            comando.Parameters.AddWithValue("@precio", txtPrecio.Text);
                            comando.Parameters.AddWithValue("@idNombreProducto", idNombreProducto);
                            comando.Parameters.AddWithValue("@idColor", idColor);
                            comando.Parameters.AddWithValue("@idTalla", idTalla);
                            comando.Parameters.AddWithValue("@idTela", idTela);
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
                    MessageBox.Show("Se han creado exitosamente " + txtCantidad.Text + " codigos de barras unicos para el producto ingresado");
                    objConection.cerrarCN();
                }
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
                string query = "SELECT * FROM dbleonv2.tipoproducto;";
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
                string query2 = "SELECT * FROM dbleonv2.color;";
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

                //Vamos a traer todo de la tabla tela
                string query4 = "SELECT * FROM dbleonv2.tela;";
                MySqlCommand comando4 = new MySqlCommand(query4, objConection.establecerCN());
                MySqlDataReader myReader4 = comando4.ExecuteReader();
                while (myReader4.Read())
                {
                    int id = myReader4.GetInt32("idTela"); // Asumiendo que el campo se llama idTela
                    string nombre = myReader4.GetString("nombreTela");
                    ComboItem item = new ComboItem() { Id = id, Nombre = nombre };
                    cbTela.Items.Add(item);
                }
                objConection.cerrarCN();

                //Vamos a traer todo de la tabla nombreproducto
                string query5 = "SELECT * FROM dbleonv2.nombreproducto;";
                MySqlCommand comando5 = new MySqlCommand(query5, objConection.establecerCN());
                MySqlDataReader myReader5 = comando5.ExecuteReader();
                while (myReader5.Read())
                {
                    int id = myReader5.GetInt32("idNombreProducto"); // Asumiendo que el campo se llama idNombreProducto
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

        private void SeleccionarNombreProducto(object sender, SelectionChangedEventArgs e)
        {
            if (cbNombre.SelectedItem != null && cbNombre.SelectedItem.ToString() == "OTRO")//Obtengo lo que hay dentro del ComboBox
            {
                txtOtroNombre.Visibility = Visibility.Visible;
                lblArticuloNuevo.Visibility = Visibility.Visible;
            }
            else
            {
                txtOtroNombre.Visibility = Visibility.Collapsed;
                lblArticuloNuevo.Visibility = Visibility.Collapsed;
            }
        }

        //Crea los codigos de barras segun el producto seleccionado en el objeto cbTipoProducto
        public string CrearCodigoBarras()
        {
            int idTipoProducto = ((ComboItem)cbTipoProducto.SelectedItem).Id;
            bool codigoExiste = true;
            string codigo = "";

            while (codigoExiste)
            {
                long numeroAleatorio = GenerarNumeroAleatorio();
                string prefijo = "";

                switch (idTipoProducto)
                {
                    case 1:
                        prefijo = "CA";
                        break;
                    case 2:
                        prefijo = "PA";
                        break;
                    case 3:
                        prefijo = "SA";
                        break;
                    case 5:
                        prefijo = "CH";
                        break;
                    default:

                        break;
                }
                codigo = prefijo + numeroAleatorio.ToString();
                codigoExiste = ExisteCodigoBarras(codigo);
            }

            return codigo;
        }

        //Verificar si el codigo de barras ya existen o no en la BD
        bool ExisteCodigoBarras(string codigoBarras)
        {
            objConection.cerrarCN();

            string query = "SELECT COUNT(*) FROM dbleonv2.producto WHERE idproducto = @codigo";

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
                    string query1 = "SELECT tt.idtalla, t.nombreTalla FROM dbleonv2.tipotall AS tt " +
                        "JOIN dbleonv2.talla AS t ON tt.talla_idtalla = t.idtalla WHERE tt.tipoProducto_idtipoProducto = '1'";
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
                    string query2 = "SELECT tt.idtalla, t.nombreTalla FROM dbleonv2.tipotall AS tt " +
                        "JOIN dbleonv2.talla AS t ON tt.talla_idtalla = t.idtalla WHERE tt.tipoProducto_idtipoProducto = '2'";
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
                    string query3 = "SELECT tt.idtalla, t.nombreTalla FROM dbleonv2.tipotall AS tt " +
                         "JOIN dbleonv2.talla AS t ON tt.talla_idtalla = t.idtalla WHERE tt.tipoProducto_idtipoProducto = '3'";
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
                    string query4 = "SELECT tt.idtalla, t.nombreTalla FROM dbleonv2.tipotall AS tt " +
                        "JOIN dbleonv2.talla AS t ON tt.talla_idtalla = t.idtalla WHERE tt.tipoProducto_idtipoProducto = '5'";
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
    }
}