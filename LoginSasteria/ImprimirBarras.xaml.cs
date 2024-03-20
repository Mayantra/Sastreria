using MySql.Data.MySqlClient;
using Mysqlx.Cursor;
using MySqlX.XDevAPI.Relational;
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
    /// Lógica de interacción para ImprimirBarras.xaml
    /// </summary>
    public partial class ImprimirBarras : Window
    {
        ConexionDB objConection = new ConexionDB();

        public ImprimirBarras()
        {
            InitializeComponent();
        }

        private void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            //Vamos a crear un DateTimePicker con los DatePicker y los ComboBox
            int horaPrimeraFecha = int.Parse(cbHoraInicio.Text);
            int minutosPrimeraFecha = int.Parse(cbMinutosInicio.Text);

            DateTime fechaHoraInicio = dpPrimeraFecha.SelectedDate.Value;
            fechaHoraInicio = fechaHoraInicio.AddHours(horaPrimeraFecha);
            fechaHoraInicio = fechaHoraInicio.AddMinutes(minutosPrimeraFecha);

            // Repite para la segunda fecha
            int horaSegundaFecha = int.Parse(cbHoraFin.Text);
            int minutosSegundaFecha = int.Parse(cbMinutosFin.Text);

            DateTime fechaHoraFin = dpSegundaFecha.SelectedDate.Value;
            fechaHoraFin = fechaHoraFin.AddHours(horaSegundaFecha);
            fechaHoraFin = fechaHoraFin.AddMinutes(minutosSegundaFecha);

            string query = "SELECT p.idproducto AS codigo, p.precio, np.Nombre AS Producto, c.nombre AS Color, t.nombreTalla AS Talla, " +
                "te.nombreTela AS Tela, p.detalles, p.fechaCodigo AS `Fecha_Creacion` FROM dbleonv2.producto AS p JOIN dbleonv2.nombreproducto AS np " +
                "ON p.nombreProducto_idnombreProducto = np.idnombreProducto JOIN dbleonv2.color AS c ON p.color_idcolor = c.idcolor JOIN dbleonv2.talla AS t " +
                "ON p.talla_idtalla = t.idtalla JOIN dbleonv2.tela AS te ON p.tela_idtela = te.idtela WHERE p.fechaCodigo BETWEEN @FechaInicio AND @FechaFin";

            using (MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN()))
            {
                comando.Parameters.AddWithValue("@FechaInicio", fechaHoraInicio);
                comando.Parameters.AddWithValue("@FechaFin", fechaHoraFin);

                // Ejecuta la consulta
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(comando);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                // Asigna el DataTable como la fuente de datos del DataGrid
                dgFechas.ItemsSource = dataTable.DefaultView;
            }
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancelar_Click_1(object sender, RoutedEventArgs e)
        {
            crearBarrasMenu abrirMenu = new crearBarrasMenu();
            abrirMenu.Show();
            this.Close();
        }
    }
}
