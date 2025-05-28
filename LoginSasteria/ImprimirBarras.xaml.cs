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
using iTextSharp.text.pdf.parser;
using Xceed.Wpf.AvalonDock.Themes;
using System.Globalization;


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

        private Table CrearNuevaTabla()
        {
            return new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1 })).UseAllAvailableWidth();
        }

        private void CompletarUltimaFila(Table tabla, int columnas, int contador)
        {
            int celdasEnFila = contador % columnas;
            if (celdasEnFila != 0)
            {
                int celdasFaltantes = columnas - celdasEnFila;
                for (int i = 0; i < celdasFaltantes; i++)
                {
                    tabla.AddCell(new Cell().Add(new Paragraph("")));
                }
            }
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            // Validación de campos
            if (dpPrimeraFecha.SelectedDate == null || dpSegundaFecha.SelectedDate == null ||
                cbHoraInicio.SelectedItem == null || cbMinutosInicio.SelectedItem == null ||
                cbHoraFin.SelectedItem == null || cbMinutosFin.SelectedItem == null)
            {
                MessageBox.Show("Complete todos los campos antes de buscar.", "Campos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Obtener los valores seleccionados de los ComboBox
                int horaInicio = int.Parse(((ComboBoxItem)cbHoraInicio.SelectedItem).Content.ToString());
                int minutoInicio = int.Parse(((ComboBoxItem)cbMinutosInicio.SelectedItem).Content.ToString());
                int horaFin = int.Parse(((ComboBoxItem)cbHoraFin.SelectedItem).Content.ToString());
                int minutoFin = int.Parse(((ComboBoxItem)cbMinutosFin.SelectedItem).Content.ToString());

                // Construir las fechas y horas completas
                DateTime fechaInicio = dpPrimeraFecha.SelectedDate.Value.Date
                    .AddHours(horaInicio)
                    .AddMinutes(minutoInicio);

                DateTime fechaFin = dpSegundaFecha.SelectedDate.Value.Date
                    .AddHours(horaFin)
                    .AddMinutes(minutoFin);

                // Consulta SQL
                string query = $@"
                        SELECT p.idproducto AS codigo, p.precio, np.Nombre AS Producto, 
                               c.nombre AS Color, t.nombreTalla AS Talla, p.detalles, 
                               p.fechaCodigo AS Fecha_Creacion
                        FROM {objConection.namedb()}.producto p
                        LEFT JOIN {objConection.namedb()}.nombreProducto np ON p.nombreProducto_idnombreProducto = np.idnombreProducto
                        LEFT JOIN {objConection.namedb()}.color c ON p.color_idcolor = c.idcolor
                        LEFT JOIN {objConection.namedb()}.talla t ON p.talla_idtalla = t.idtalla
                        WHERE p.fechaCodigo BETWEEN '2025-05-27 00:00:00' AND '2025-05-27 23:59:59'
                        AND p.idproducto NOT LIKE 'ENC%'";

                // Ejecutar consulta
                using (var conn = objConection.nuevaConexion())
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    //MessageBox.Show($"Total de registros encontrados: {dataTable.Rows.Count}", "Resultado", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Mostrar los datos
                    dgFechas.ItemsSource = dataTable.DefaultView;
                    dgFechas.Items.SortDescriptions.Clear();
                    dgFechas.Items.SortDescriptions.Add(new SortDescription("Fecha_Creacion", ListSortDirection.Ascending));
                    dgFechas.Items.Refresh();
                }

                btnImprimir.IsEnabled = true;
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Error al interpretar los datos de hora o minutos. Verifique los valores seleccionados.", "Error de formato", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al buscar los productos:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

                int columnas = 4;
                int filasPorPagina = 7;
                int celdasPorPagina = columnas * filasPorPagina;
                int contador = 0;

                Table tabla = CrearNuevaTabla();

                // Lógica de la creación de la tabla para la colocación de los códigos
                foreach (DataRowView row in dgFechas.Items)
                {
                    if (row.Row.RowState != DataRowState.Detached)
                    {
                        string codigo = row["codigo"].ToString();
                        string producto = row["Producto"].ToString();
                        string precio = row["precio"].ToString();

                        byte[] barcodeBytes = GenerateBarcode(codigo);
                        ImageData imageData = ImageDataFactory.Create(barcodeBytes);
                        Image barcodeImage = new Image(imageData);
                        barcodeImage.SetWidth(4 * 28.35f); // 4 cm
                        barcodeImage.SetHeight(2 * 28.35f); // 2 cm

                        Cell cell = new Cell();
                        cell.Add(new Paragraph("Producto: " + producto).SetFontSize(10));
                        cell.Add(new Paragraph("Precio: Q" + precio).SetFontSize(10));
                        cell.Add(barcodeImage);

                        tabla.AddCell(cell);
                        contador++;

                        if (contador % celdasPorPagina == 0)
                        {
                            CompletarUltimaFila(tabla, columnas, contador);
                            document.Add(tabla);
                            document.Add(new iText.Layout.Element.AreaBreak());
                            tabla = CrearNuevaTabla();
                        }
                    }
                }

                if (contador % celdasPorPagina != 0)
                {
                    CompletarUltimaFila(tabla, columnas, contador);
                    document.Add(tabla);
                }

                document.Close();
                memoryStream.Seek(0, SeekOrigin.Begin);

                string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Codigos" + DateTime.Now.ToString("dd-MM-yyyy") + ".pdf");

                objConection.cerrarCN();
                try
                {
                    using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                    {
                        memoryStream.CopyTo(fileStream);
                    }

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(tempFilePath) { UseShellExecute = true });
                }
                catch (IOException)
                {
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
