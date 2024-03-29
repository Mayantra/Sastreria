﻿using MySql.Data.MySqlClient;
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
using iText.Commons.Datastructures;
using System.Drawing.Imaging;
using System.Drawing;
using Image = iText.Layout.Element.Image;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Paragraph = iText.Layout.Element.Paragraph;
using iText.Layout.Properties;
using Table = iText.Layout.Element.Table;


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
            btnImprimir.IsEnabled = true;
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Now;
            string fecha = now.ToString("dd-MM-yyyy");

            // Ruta para el archivo PDF que se creará
            string dest = "C:\\Codigos\\Codigos"+fecha+".pdf";

            // Crear un objeto PdfWriter
            PdfWriter writer = new PdfWriter(dest);

            // Crear un objeto PdfDocument
            PdfDocument pdf = new PdfDocument(writer);

            // Crear un objeto Document
            Document document = new Document(pdf);

            // Crear una tabla con 2 columnas para organizar los códigos de barras
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1 })).UseAllAvailableWidth();


            foreach (DataRowView row in dgFechas.Items)
            {
                if (row.Row.RowState != DataRowState.Detached)
                {
                    // Obtener el valor del "codigo" de la fila actual
                    string codigo = row["codigo"].ToString();

                    // Generar la imagen del código de barras con el texto
                    byte[] barcodeBytes = GenerateBarcode(codigo);
                    ImageData imageData = ImageDataFactory.Create(barcodeBytes);
                    Image barcodeImage = new Image(imageData);

                    // Agregar la celda con la imagen del código de barras a la tabla
                    Cell cell = new Cell().Add(barcodeImage);
                    table.AddCell(cell);
                }
            }

            // Asegurarse de que la última fila se llene completamente
            if ((dgFechas.Items.Count % 2) != 0)
            {
                table.AddCell(new Cell().Add(new Paragraph(""))); // Agregar una celda vacía si el número de códigos es impar
            }

            // Agregar la tabla al documento
            document.Add(table);

            // Cerrar el documento
            document.Close();
            MessageBox.Show("Archivo pdf generado en: " + "C:\\Codigos\\Codigos"+fecha+".pdf");
            btnImprimir.IsEnabled = false;
        }

        //Permite generar el codigo de barras
        public  byte[] GenerateBarcode(string data)
        {
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = 50,
                    Width = 100,
                    PureBarcode = true
                }
            };

            var pixelData = writer.Write(data);
            // Crear un bitmap para el código de barras
            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height + 20)) // +20 pixeles para el texto
            using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
            using (var font = new Font(System.Drawing.FontFamily.GenericMonospace, 10))
            using (var brush = new SolidBrush(System.Drawing.Color.Black))
            using (var format = new StringFormat() { Alignment = System.Drawing.StringAlignment.Center })
            using (var stream = new MemoryStream())
            {
                // Dibujar el código de barras
                graphics.FillRectangle(System.Drawing.Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
                var barcodeBitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppArgb);
                var bitmapData = barcodeBitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                barcodeBitmap.UnlockBits(bitmapData);

                graphics.DrawImage(barcodeBitmap, 1, 1);

                // Dibujar el texto del código debajo del código de barras
                graphics.DrawString(data, font, brush, bitmap.Width / 2, pixelData.Height, format);

                // Guardar la imagen combinada en un stream y retornarlo
                bitmap.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }

        private void btnCancelar_Click_1(object sender, RoutedEventArgs e)
        {
            crearBarrasMenu abrirMenu = new crearBarrasMenu();
            abrirMenu.Show();
            this.Close();
        }
    }
}
