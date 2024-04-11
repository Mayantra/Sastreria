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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LoginSasteria
{
    /// <summary>
    /// Lógica de interacción para EstadoPorClientes.xaml
    /// </summary>
    public partial class EstadoPorClientes : Window
    {
        ConexionDB cn = new ConexionDB();
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
                    if (nombreEmpleado!=null)
                    {
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
            string query = "SELECT idRegistroVenta As 'Registro'," +
                "\r\nproducto_idproducto As 'Código Producto'," +
                "\r\nFechaHora As 'Fecha',\r\nNombre As 'Nombre'," +
                "\r\nprecio AS 'Monto'\r\nFROM dbleonv2.registroventa " +
                "\r\ninner join dbleonv2.detallesventa " +
                "\r\nON DetallesVenta_idDetallesVenta = idDetallesVenta" +
                "\r\ninner join dbleonv2.producto\r\nON producto_idproducto = idproducto" +
                "\r\ninner join dbleonv2.empleado\r\non Empleado_idEmpleado = idEmpleado" +
                "\r\nwhere FechaHora BETWEEN '"+fecha1+"' and '"+fecha2+" 23:59:59'" +
                "\r\nand Nombre ='"+nombre+"';";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataAdapter data = new MySqlDataAdapter(comando);
                DataTable tabla = new DataTable();

                data.Fill(tabla);

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
            string query = "SELECT sum(precio) AS 'Monto' FROM dbleonv2.registroventa " +
                "\r\ninner join dbleonv2.detallesventa " +
                "\r\nON DetallesVenta_idDetallesVenta = idDetallesVenta" +
                "\r\ninner join dbleonv2.producto\r\nON producto_idproducto = idproducto" +
                "\r\ninner join dbleonv2.empleado\r\non Empleado_idEmpleado = idEmpleado" +
                "\r\nwhere FechaHora BETWEEN '" + fecha1 + "' and '" + fecha2 + " 23:59:59'" +
                "\r\nand Nombre ='" + nombre + "';";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
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
            string query = "SELECT * FROM dbleonv2.empleado;";
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
    }
}
