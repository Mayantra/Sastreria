using MySql.Data.MySqlClient;
using Mysqlx.Cursor;
using MySqlX.XDevAPI.Relational;
using SkiaSharp;
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
using System.IO;
using iText.Layout;
using iText.Kernel.Pdf;
using iText.Kernel.Geom;
using iText.IO.Image;
using iText.Layout.Element;
using BarcodeStandard;
using System.CodeDom.Compiler;
using ZXing;
using ZXing.Common;

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
            objConection.cerrarCN();
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Now;
            string fecha = now.ToString("dd-MM-yyyy");

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

            //Creamos el documento PDF
            PdfWriter pdfEscribir = new PdfWriter(@"C:\Codigos\codigos"+ fecha +".pdf");
            PdfDocument pdf = new PdfDocument(pdfEscribir);
            Document documento = new Document(pdf, PageSize.LETTER);
            documento.SetMargins(2, 2, 2, 2);

            //Iniciamos a crear el codigo de barras
            var codigo = new Barcode();
            codigo.IncludeLabel = true;
            codigo.Alignment = AlignmentPositions.Center;

            string query = "SELECT idproducto FROM dbleonv2.producto WHERE fechaCodigo BETWEEN '"+ fechaHoraInicio + "' AND '"+ fechaHoraFin + "'";
            MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
            MySqlDataReader leer = comando.ExecuteReader();

            while (leer.Read())
            {
                string idproducto = leer["idproducto"].ToString();
                string nombreImg = @"C:\Codigos\img" + idproducto + ".jpg";

                codigo.Encode(BarcodeStandard.Type.Code128, idproducto, SKColors.Black, SKColors.White, 200, 100);
                codigo.SaveImage(nombreImg, SaveTypes.Jpg);

                iText.Layout.Element.Image imagen = new iText.Layout.Element.Image(ImageDataFactory.Create(nombreImg));
                iText.Layout.Element.Paragraph parrafo = new iText.Layout.Element.Paragraph().Add(imagen);
                documento.Add(parrafo);
            }
            documento.Close();
            objConection.cerrarCN();
        }

        private void btnCancelar_Click_1(object sender, RoutedEventArgs e)
        {
            crearBarrasMenu abrirMenu = new crearBarrasMenu();
            abrirMenu.Show();
            this.Close();
        }
    }
}
