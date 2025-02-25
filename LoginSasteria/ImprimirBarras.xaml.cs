using MySql.Data.MySqlClient;
using Mysqlx.Cursor;
using MySqlX.XDevAPI.Relational;
using System.Data;
using ZXing;
using ZXing.Common;
using BarcodeStandard;
using iText.Layout;
using iText.Kernel.Pdf;
using iText.Kernel.Geom;
using iText.IO.Image;
using iText.Layout.Element;
using iText.Commons.Datastructures;
using System.Drawing.Imaging;
using System.Drawing;
using Image = iText.Layout.Element.Image;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Paragraph = iText.Layout.Element.Paragraph;
using iText.Layout.Properties;
using Table = iText.Layout.Element.Table;

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
using System.IO;
using System.Reflection.Emit;
using System.ComponentModel;


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
            btnImprimir.IsEnabled = false;
            //setiamos las fechas en la actual
            dpPrimeraFecha.SelectedDate = DateTime.Now;
            dpSegundaFecha.SelectedDate = DateTime.Now;
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
            // Verifica que todos los controles estén llenos
            if (dpPrimeraFecha.SelectedDate == null ||
                dpSegundaFecha.SelectedDate == null ||
                string.IsNullOrEmpty(cbHoraInicio.Text) ||
                string.IsNullOrEmpty(cbMinutosInicio.Text) ||
                string.IsNullOrEmpty(cbHoraFin.Text) ||
                string.IsNullOrEmpty(cbMinutosFin.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos antes de realizar la búsqueda.", "Campos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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

            /*string query = "SELECT p.idproducto AS codigo, p.precio, np.Nombre AS Producto, c.nombre AS Color, t.nombreTalla AS Talla, " +
                "p.detalles, p.fechaCodigo AS `Fecha_Creacion` FROM " + objConection.namedb() + ".producto AS p JOIN " + objConection.namedb() + ".nombreProducto AS np " +
                "ON p.nombreProducto_idnombreProducto = np.idnombreProducto JOIN " + objConection.namedb() + ".color AS c ON p.color_idcolor = c.idcolor JOIN " + objConection.namedb() + ".talla AS t " +
                "ON p.talla_idtalla = t.idtalla WHERE p.fechaCodigo BETWEEN @FechaInicio AND @FechaFin";*/

            string query = "SELECT p.idproducto AS codigo, p.precio, np.Nombre AS Producto, c.nombre AS Color, t.nombreTalla AS Talla, " +
                "p.detalles, p.fechaCodigo AS `Fecha_Creacion` FROM " + objConection.namedb() + ".producto AS p " +
                "JOIN " + objConection.namedb() + ".nombreProducto AS np ON p.nombreProducto_idnombreProducto = np.idnombreProducto " +
                "JOIN " + objConection.namedb() + ".color AS c ON p.color_idcolor = c.idcolor " +
                "JOIN " + objConection.namedb() + ".talla AS t ON p.talla_idtalla = t.idtalla " +
                "WHERE p.fechaCodigo BETWEEN @FechaInicio AND @FechaFin " +
                "AND p.idproducto NOT LIKE 'ENC%'";

            objConection.cerrarCN();
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

                // Ordena el DataGrid por la columna "Fecha Creación"
                dgFechas.Items.SortDescriptions.Clear();
                dgFechas.Items.SortDescriptions.Add(new SortDescription("Fecha_Creacion", ListSortDirection.Ascending));

                // Refresca la vista
                dgFechas.Items.Refresh();
            }
            objConection.cerrarCN();
            btnImprimir.IsEnabled = true;
        }

        //Permite generar el codigo de barras
        public byte[] GenerateBarcode(string data)
        {
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = 50, // Ajusta según sea necesario
                    Width = 100, // Ajusta según sea necesario
                    PureBarcode = true
                }
            };
            objConection.cerrarCN();
            var pixelData = writer.Write(data);
            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height + 20))
            using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
            using (var font = new Font(System.Drawing.FontFamily.GenericMonospace, 10))
            using (var brush = new SolidBrush(System.Drawing.Color.Black))
            using (var format = new StringFormat() { Alignment = System.Drawing.StringAlignment.Center })
            using (var stream = new MemoryStream())
            {
                graphics.FillRectangle(System.Drawing.Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
                var barcodeBitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppArgb);
                var bitmapData = barcodeBitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                barcodeBitmap.UnlockBits(bitmapData);

                graphics.DrawImage(barcodeBitmap, 1, 1);

                graphics.DrawString(data, font, brush, bitmap.Width / 2, pixelData.Height, format);

                bitmap.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            objConection.cerrarCN();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Crear un objeto PdfWriter que escribe en el MemoryStream
                PdfWriter writer = new PdfWriter(memoryStream);
                writer.SetCloseStream(false);

                // Crear un objeto PdfDocument
                PdfDocument pdf = new PdfDocument(writer);

                // Crear un objeto Document
                Document document = new Document(pdf);

                // Crear una tabla con 4 columnas para organizar los códigos de barras
                Table table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1 })).UseAllAvailableWidth();

                // Lógica de la creación de la tabla para la colocación de los códigos
                foreach (DataRowView row in dgFechas.Items)
                {
                    if (row.Row.RowState != DataRowState.Detached)
                    {
                        // Obtener los valores del "codigo", "Producto" y "precio" de la fila actual
                        string codigo = row["codigo"].ToString();
                        string producto = row["Producto"].ToString();
                        string precio = row["precio"].ToString();

                        // Generar la imagen del código de barras con el texto del código
                        byte[] barcodeBytes = GenerateBarcode(codigo);
                        ImageData imageData = ImageDataFactory.Create(barcodeBytes);
                        Image barcodeImage = new Image(imageData);

                        // Ajustar el tamaño de la imagen a 4 cm x 2 cm
                        barcodeImage.SetWidth(4 * 28.35f); // 4 cm a puntos
                        barcodeImage.SetHeight(2 * 28.35f); // 2 cm a puntos

                        // Crear una celda que contendrá el nombre del producto, el precio y el código de barras
                        Cell cell = new Cell();

                        // Agregar el nombre del producto y el precio en la parte superior
                        cell.Add(new Paragraph("Producto: " + producto).SetFontSize(10));
                        cell.Add(new Paragraph("Precio: Q" + precio).SetFontSize(10));

                        // Agregar la imagen del código de barras debajo
                        cell.Add(barcodeImage);

                        // Agregar la celda completa a la tabla
                        table.AddCell(cell);
                    }
                }

                // Asegurarse de que la última fila se llene completamente
                int itemsCount = dgFechas.Items.Count;
                int cellsToAdd = 4 - (itemsCount % 4);
                if (cellsToAdd < 4)
                {
                    for (int i = 0; i < cellsToAdd; i++)
                    {
                        table.AddCell(new Cell().Add(new Paragraph("")));
                    }
                }

                // Agregar la tabla al documento
                document.Add(table);

                // Cerrar el documento
                document.Close();

                // Es necesario rebobinar el MemoryStream antes de leerlo
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Crear una ruta temporal para el archivo PDF
                string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Codigos" + DateTime.Now.ToString("dd-MM-yyyy") + ".pdf");

                objConection.cerrarCN();
                try
                {
                    // Escribir el MemoryStream a un archivo temporal
                    using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                    {
                        memoryStream.CopyTo(fileStream);
                    }

                    // Abrir el archivo PDF temporal con el visor predeterminado
                    System.Diagnostics.Process.Start(tempFilePath);

                    // Si no abre el PDF con la línea anterior, usar esta línea:
                    //System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(tempFilePath) { UseShellExecute = true });
                }
                catch (IOException)
                {
                    // Mostrar un mensaje indicando que el archivo está siendo utilizado
                    MessageBox.Show("El archivo PDF está siendo utilizado. Por favor, ciérrelo e intente de nuevo.", "Error al generar PDF", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            objConection.cerrarCN();
            btnImprimir.IsEnabled = false;
        }

        private void btnCancelar_Click_1(object sender, RoutedEventArgs e)
        {
            objConection.cerrarCN();
            crearBarrasMenu abrirMenu = new crearBarrasMenu();
            abrirMenu.Show();
            this.Close();
        }
    }
}
