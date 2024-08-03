using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZXing.Rendering;

namespace LoginSasteria
{
    /// <summary>
    /// Lógica de interacción para EstadoPorClientes.xaml
    /// </summary>
    public partial class EstadoPorClientes : Window
    {
        ConexionDB cn = new ConexionDB();
        DataTable auxTable = new DataTable();
        string fechaGlobal1 = "";
        string fechaGlobal2 = "";
        string nombreGlobal = "";
        List<int> listaDetalles = new List<int>();
        List<int> ListaCierres = new List<int>();
        List<int> listaDetallesCierre = new List<int>();
        Boolean estadoListas = false;
        double totalGlobal = 0.00;
        public EstadoPorClientes()
        {
            InitializeComponent();
            getNombres();
        }
        public void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void AbrirCalendario(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            if (button != null)
            {
                var datePicker = FindVisualParent<DatePicker>(button);
                if (datePicker != null)
                {
                    var popup = datePicker.Template.FindName("PART_Popup", datePicker) as Popup;
                    if (popup != null)
                    {

                        popup.IsOpen = !popup.IsOpen;
                    }
                }
            }
        }
        private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
                return null;

            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindVisualParent<T>(parentObject);
        }
        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            var datePicker = calendar.TemplatedParent as DatePicker;

            if (calendar != null && datePicker != null)
            {
                var textBox = datePicker.Template.FindName("PART_TextBox", datePicker) as DatePickerTextBox;

                if (textBox != null)
                {
                    if (calendar.SelectedDate != null)
                    {
                        textBox.Text = calendar.SelectedDate.Value.ToString();
                        var popup = datePicker.Template.FindName("PART_Popup", datePicker) as Popup;
                        if (popup != null)
                        {
                            popup.IsOpen = false;
                        }
                    }
                }
            }
        }
        private void ObtenerFechas(object sender, RoutedEventArgs e)
        {
            var datePickerTextBox = Date1.Template.FindName("PART_TextBox", Date1) as DatePickerTextBox;
            string fecha1 = datePickerTextBox.Text.Trim(); // Obtén el texto y elimina espacios en blanco al inicio y al final

            var datePickerTextBox2 = Date2.Template.FindName("PART_TextBox", Date2) as DatePickerTextBox;
            string fecha2 = datePickerTextBox2.Text.Trim(); // Obtén el texto y elimina espacios en blanco al inicio y al final

            // Verifica si las fechas están vacías
            if (string.IsNullOrWhiteSpace(fecha1) || string.IsNullOrWhiteSpace(fecha2))
            {
                MessageBox.Show("Debe ingresar el rango de fechas");
            }
            else
            {

                // Las fechas no están vacías
                DateTime fecha1Formateada;
                DateTime fecha2Formateada;

                // Intenta convertir las cadenas a objetos DateTime
                if (DateTime.TryParse(fecha1, out fecha1Formateada) && DateTime.TryParse(fecha2, out fecha2Formateada))
                {
                    // Formatea las fechas al formato "yyyy-MM-dd"
                    string fecha1FormatoDeseado = fecha1Formateada.ToString("yyyy-MM-dd");
                    string fecha2FormatoDeseado = fecha2Formateada.ToString("yyyy-MM-dd");

                    // Ahora las variables 'fecha1FormatoDeseado' y 'fecha2FormatoDeseado' contienen las fechas formateadas
                    // Puedes usarlas según sea necesario
                    object nombreEmpleado = cbNombres.SelectedValue;
                    if (nombreEmpleado != null)
                    {
                        fechaGlobal1 = fecha1FormatoDeseado;
                        fechaGlobal2 = fecha2FormatoDeseado;
                        nombreGlobal = nombreEmpleado.ToString();
                        ConsultaRango(fecha1FormatoDeseado, fecha2FormatoDeseado, nombreEmpleado.ToString());
                        consultaTotalFechas(fecha1FormatoDeseado, fecha2FormatoDeseado, nombreEmpleado.ToString());
                    }
                    else
                    {
                        MessageBox.Show("Seleccione un empleado para realizar la consulta");
                    }

                }
                else
                {
                    MessageBox.Show("El formato de las fechas es incorrecto.");
                }

            }
        }
        public void ConsultaRango(string fecha1, string fecha2, string nombre)
        {
            string query = "SELECT idRegistroVenta as 'Registro'," +
                "\r\nproducto_idproducto As 'Código Producto'," +
                "\r\nFechaHora As 'Fecha'," +
                "\r\nNombre As 'Nombre'," +
                "\r\nprecio AS 'Monto' FROM " + cn.namedb() + ".RegistroVenta " +
                "\r\ninner join " + cn.namedb() + ".DetallesVenta ON DetallesVenta_idDetallesVenta = idDetallesVenta" +
                "\r\ninner join " + cn.namedb() + ".producto ON producto_idproducto = idproducto" +
                "\r\ninner join " + cn.namedb() + ".Empleado ON Empleado_idEmpleado = idEmpleado" +
                "\r\nwhere FechaHora BETWEEN '" + fecha1 + "' and '" + fecha2 + " 23:59:59' and Nombre ='" + nombre + "';";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataAdapter data = new MySqlDataAdapter(comando);
                DataTable tabla = new DataTable();

                data.Fill(tabla);
                auxTable = tabla;
                DataConsulta.DataContext = tabla;
                cn.cerrarCN();
            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }

        }
        public void consultaTotalFechas(string fecha1, string fecha2, string nombre)
        {
            string query = "SELECT sum(precio) as Monto FROM " + cn.namedb() + ".RegistroVenta " +
                "\r\ninner join " + cn.namedb() + ".DetallesVenta ON DetallesVenta_idDetallesVenta = idDetallesVenta" +
                "\r\ninner join " + cn.namedb() + ".producto ON producto_idproducto = idproducto" +
                "\r\ninner join " + cn.namedb() + ".Empleado ON Empleado_idEmpleado = idEmpleado" +
                "\r\nwhere FechaHora BETWEEN '" + fecha1 + "' and '" + fecha2 + " 23:59:59' and Nombre ='" + nombre + "';";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader reader = comando.ExecuteReader();
                
                while (reader.Read())
                {
                    if (reader.GetValue(0).ToString() == "")
                    {
                        totalGlobal = 0;
                    }
                    else
                    {
                        totalGlobal = reader.GetDouble(0);
                    }
                    txTotalFecha.Text = "Monto:  Q " + reader.GetValue(0).ToString();
                }
               


                reader.Close();


                cn.cerrarCN();
            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
        }

        public void getNombres()
        {
            cn.cerrarCN();
            string query = "SELECT * FROM " + cn.namedb() + ".Empleado;";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader myread;

                myread = comando.ExecuteReader();

                while (myread.Read())
                {

                    string nombres = myread.GetString("Nombre");
                    cbNombres.Items.Add(nombres);

                }
                cn.cerrarCN();
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void ImprimirCuentas(object sender, RoutedEventArgs e)
        {
            listaDetalles.Clear();
            ListaCierres.Clear();
            listaDetallesCierre.Clear();
            if (DataConsulta.Items.Count == 0)
            {
                MessageBox.Show("No hay nada que imprimir");
            }
            else
            {
                try
                {
                    string queryListasDetalles = "SELECT distinct DetallesVenta_idDetallesVenta " +
                "FROM " + cn.namedb() + ".RegistroVenta" +
                " inner join " + cn.namedb() + ".DetallesVenta ON DetallesVenta_idDetallesVenta = idDetallesVenta" +
                " inner join " + cn.namedb() + ".producto ON producto_idproducto = idproducto" +
                " inner join " + cn.namedb() + ".Empleado ON Empleado_idEmpleado = idEmpleado" +
                " where FechaHora BETWEEN '" + fechaGlobal1 + "' and '" + fechaGlobal2 + " 23:59:59' and Nombre ='" + nombreGlobal + "';";


                    string queryListaRegistros = "SELECT idcierreVenta FROM " + cn.namedb() + ".cierreVenta order by fechaCierre desc limit 10;";
                    if (getLista(queryListasDetalles, 1) == true)
                    {
                        if (getLista(queryListaRegistros, 2) == true)
                        {
                            if (estadoListas == true)//si no hay nada aun
                            {
                                //hacer el registro
                                crearCierre();
                            }
                            else
                            {
                                if (verificarCierres() == true)
                                {
                                    MessageBox.Show("Usted ya ha creado este cierre");
                                }
                                else
                                {
                                    //MessageBox.Show("Cierre nuevo");
                                    crearCierre();
                                }
                            }

                        }

                    }

                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }

        }
        private void crearCierre()
        {
            DateTime dateTime = DateTime.Now;
            string fecha = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            string DetallesRegistro = "Se Realizó un Cierre el " + fecha + " para: " + nombreGlobal +
                " Desde la fecha: " + fechaGlobal1 + " Hasta la fecha: " + fechaGlobal2 + " 23:59:59";

            addRegistro("INSERT INTO `" + cn.namedb() + "`.`cierreVenta` (`detalles`, `total`, `fechaCierre`) " +
                "VALUES ('" + DetallesRegistro + "', '" + totalGlobal + "', '" + fecha + "');");

            int idCierre = getID("SELECT max(idcierreVenta) FROM " + cn.namedb() + ".cierreVenta;");

            foreach (int i in listaDetalles)
            {
                addRegistro("INSERT INTO `" + cn.namedb() + "`.`DetallesVenta_has_cierreVenta` (`DetallesVenta_idDetallesVenta`, `cierreVenta_idcierreVenta`) " +
                    "VALUES ('" + i + "', '" + idCierre + "');");
            }
            ImprimirEstados imprimir = new ImprimirEstados();
            imprimir.ImprimirEstadoCuentas(auxTable, txTotalFecha.Text, DetallesRegistro);

            estadoCuentaInventario abrir  = new estadoCuentaInventario();
            abrir.Show();
            this.Close();

        }
        private int getID(string query)
        {
            int id = 0;
            try
            {
                cn.cerrarCN();
                MySqlCommand comanod = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader reader = comanod.ExecuteReader();
                while (reader.Read())
                {
                    id = reader.GetInt32(0);
                }
                return id;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
                return id;
            }
            finally
            {

            }
        }
        private void addRegistro(string query)
        {
            try
            {
                cn.cerrarCN();
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader reader = comando.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                cn.cerrarCN();
            }
        }


        private Boolean getLista(string query, int dato)
        {
            int aux = 0;
            try
            {
                cn.cerrarCN();
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    aux = reader.GetInt32(0);
                    if (dato == 1)
                    {
                        listaDetalles.Add(aux);
                    }
                    else if (!reader.HasRows)
                    {
                        estadoListas = true;

                    }
                    else
                    {
                        ListaCierres.Add(aux);
                    }

                }
                return true;

            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
            finally
            {
                cn.cerrarCN();
            }


        }

        private Boolean verificarCierres()
        {
            Boolean estadoEncontrado = false;
            int aux = 0;
            List<int> listaAux = new List<int>();
            foreach (int i in ListaCierres)
            {
                listaAux.Clear();
                string query = "SELECT DetallesVenta_idDetallesVenta " +
                    "FROM " + cn.namedb() + ".DetallesVenta_has_cierreVenta " +
                    "where cierreVenta_idcierreVenta ='" + i + "';";
                try
                {
                    cn.cerrarCN();
                    MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                    MySqlDataReader reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        aux = reader.GetInt32(0);
                        listaAux.Add(aux);
                    }
                    listaDetallesCierre = new List<int>(listaAux); // Asignar una nueva lista

                    if (listaDetallesCierre.SequenceEqual(listaDetalles)) // Comparar las listas por contenido
                    {
                        estadoEncontrado = true;
                        break; // Salir del bucle si se encuentra un estado coincidente
                    }


                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.ToString());
                    return false;
                }
                finally
                {
                    cn.cerrarCN();
                }
            }
            return estadoEncontrado;


        }

        private void btnCancelar_Click_1(object sender, RoutedEventArgs e)
        {
            estadoCuentaInventario abrir = new estadoCuentaInventario();
            abrir.Show();
            this.Close();
        }
    }
}
