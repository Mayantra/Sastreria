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
using System.Windows.Navigation;
using System.Windows.Shapes;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.IO;
using System.Diagnostics;
using iText.IO.Image;
using iText.Layout.Properties;
using iText.Layout.Borders;
using ZXing;
using iText.Commons.Datastructures;
using iText.Kernel.Colors;
using System.Data;
using MySql.Data.MySqlClient;

namespace LoginSasteria
{
    internal class GenerarFactura
    {
        string TextoDetalles = "5ta calle 4-17 zona 1 \nSan Pedro  Sacatepéquez San Marcos\n Tel: 7760-3249";
        string textoEmpleado = "Atendido por:\nJorge Medrano";
        string textoCliente = "Cliente:Josue Fuentes\nNIT:264987K\nPuntos:5";
        ConexionDB cn = new ConexionDB();
        public void GenerarDatosFactura(string codigo, int idEmpleado, Int32 Cliente, DataTable datos, double total, int Regalo)
        {
            DataEmpleados(idEmpleado);
            DataCliente(Cliente);
            string Ttotal = "Q " + total.ToString();
            CrearFactura(codigo, textoEmpleado, textoCliente, TextoDetalles, Ttotal, datos, Regalo);
        }
        public void DataEmpleados(int idEmpleado)
        {
            string nombre = "";
            try
            {

                string query = "SELECT Nombre FROM " + cn.namedb() + ".Empleado where idEmpleado = '" + idEmpleado + "';";

                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();

                while (dr.Read())
                {
                    nombre = dr.GetString(0).ToString();
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
            textoEmpleado = "Atendido por:\n" + nombre;
            cn.cerrarCN();
        }
        public void DataCliente(int telefono)
        {
            string nombre = "";
            string apellido = "";
            int puntos = 0;
            string nit = "";
            try
            {

                string query = "SELECT Nombres, Apellidos, puntos, NIT " +
                    "FROM " + cn.namedb() + ".Cliente where telefono='" + telefono + "';";

                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();

                while (dr.Read())
                {

                    nombre = dr.GetString(0).ToString();
                    apellido = dr.GetString(1).ToString();
                    puntos = dr.GetInt16(2);
                    nit = dr.GetString(3).ToString();

                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
            textoCliente = "Cliente:" + nombre + " " + apellido + "\nNIT: " + nit + "\nPuntos: " + puntos;
            cn.cerrarCN();
        }

        public void CrearFactura(string codigo, String DataEmpleado, String DataCliente, String DataDetalles, String Total, DataTable DataProductos, int Regalo)
        {
            DateTime fechaHoy = DateTime.Now;
            fechaHoy.ToString("g");
            BitmapSource barcodeBitmap = GenerateBarcode(codigo);
            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);
                pdf.SetDefaultPageSize(iText.Kernel.Geom.PageSize.LETTER);
                document.SetMargins(15, 15, 15, 15);

                iText.Layout.Element.Table table = new iText.Layout.Element.Table(2).UseAllAvailableWidth();
                table.SetWidth(UnitValue.CreatePercentValue(100));
                table.SetBorder(iText.Layout.Borders.Border.NO_BORDER);

                Cell noContableCell = new Cell();
                Cell detallesCell = new Cell();
                Cell imagenCell = new Cell();
                Cell codigos = new Cell();
                Cell cellEmpleado = new Cell();
                Cell cellCliente = new Cell();
                detallesCell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                imagenCell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                codigos.SetBorder(iText.Layout.Borders.Border.NO_BORDER);

                iText.Layout.Element.Paragraph noContable = new iText.Layout.Element.Paragraph("ESTE DOCUMENTO NO CONSTITUYE A UN COMPROBANTE CONTABLE OFICIAL");
                noContableCell.Add(noContable);
                noContableCell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                noContableCell.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);

                iText.Layout.Element.Paragraph detalles = new iText.Layout.Element.Paragraph(DataDetalles);
                detallesCell.Add(detalles);
                iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create("../../LogoBlack.png"));
                img.ScaleToFit(100f, 150f);
                imagenCell.Add(img);
                imagenCell.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);

                var barcodeBytes = ConvertToByteArray(barcodeBitmap);
                iText.Layout.Element.Image barcodeImage = new iText.Layout.Element.Image(ImageDataFactory.Create(barcodeBytes));
                codigos.Add(barcodeImage);

                detallesCell.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);
                codigos.SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE);

                iText.Layout.Element.Paragraph paCodigo = new iText.Layout.Element.Paragraph(codigo);
                paCodigo.SetPaddingTop(-15);
                codigos.SetPaddingTop(10);
                codigos.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                codigos.Add(paCodigo);

                iText.Layout.Element.Table tableDetalles = new iText.Layout.Element.Table(1).UseAllAvailableWidth();
                tableDetalles.AddCell(cellEmpleado);
                tableDetalles.AddCell(codigos);
                Cell celldaTableDetalles = new Cell();
                celldaTableDetalles.Add(tableDetalles);
                celldaTableDetalles.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                celldaTableDetalles.SetMaxWidth(75);
                celldaTableDetalles.SetMinWidth(75);
                table.AddCell(celldaTableDetalles);

                iText.Layout.Element.Table EmpleadoCliente = new iText.Layout.Element.Table(3).UseAllAvailableWidth();
                EmpleadoCliente.SetWidth(UnitValue.CreatePercentValue(100));

                cellEmpleado.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                cellCliente.SetBorder(iText.Layout.Borders.Border.NO_BORDER);

                iText.Layout.Element.Paragraph pEmpleado = new iText.Layout.Element.Paragraph(DataEmpleado + "\n" + fechaHoy);
                cellEmpleado.Add(pEmpleado).SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.LEFT);

                iText.Layout.Element.Paragraph pCliente = new iText.Layout.Element.Paragraph(DataCliente);
                cellCliente.Add(pCliente).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);

                detallesCell.SetMaxWidth(150);
                EmpleadoCliente.AddCell(detallesCell);
                EmpleadoCliente.AddCell(imagenCell);
                EmpleadoCliente.AddCell(cellCliente);

                // Crear tabla de productos
                iText.Layout.Element.Table productos = new iText.Layout.Element.Table(DataProductos.Columns.Count).UseAllAvailableWidth();

                // Agregar encabezados (ocultando precios si es regalo)
                foreach (DataColumn column in DataProductos.Columns)
                {
                    if (Regalo == 1 && (column.ColumnName.ToLower().Contains("precio") || column.ColumnName.ToLower().Contains("subtotal") || column.ColumnName.ToLower().Contains("total")))
                        continue;

                    Cell headerCell = new Cell().Add(new iText.Layout.Element.Paragraph(column.ColumnName));
                    headerCell.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    productos.AddHeaderCell(headerCell.SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                }

                // Agregar datos
                foreach (DataRow row in DataProductos.Rows)
                {
                    for (int i = 0; i < DataProductos.Columns.Count; i++)
                    {
                        string columnName = DataProductos.Columns[i].ColumnName.ToLower();

                        if (Regalo == 1 && (columnName.Contains("precio") || columnName.Contains("subtotal") || columnName.Contains("total")))
                            continue;

                        object item = row[i];
                        Cell cell = new Cell().Add(new iText.Layout.Element.Paragraph(item.ToString()));
                        productos.AddCell(cell.SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                    }
                }

                productos.SetMaxWidth(400);
                productos.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.RIGHT);

                document.SetFontSize(9);

                Cell celdaProductos = new Cell();
                celdaProductos.Add(productos);
                celdaProductos.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                table.AddCell(celdaProductos);

                document.Add(noContableCell);
                document.Add(EmpleadoCliente);
                document.Add(table);

                // Solo mostrar total si no es regalo
                if (Regalo == 0)
                {
                    iText.Layout.Element.Paragraph TotalP = new iText.Layout.Element.Paragraph(Total);
                    TotalP.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    Cell celdaTotal = new Cell();
                    celdaTotal.SetPaddingLeft(300);
                    celdaTotal.Add(TotalP);
                    celdaTotal.SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
                    TotalP.SetPaddingRight(15);
                    celdaTotal.SetFontSize(12);
                    document.Add(celdaTotal);
                }

                document.Close();
                AbrirPDF(ms.ToArray());
            }
        }
        public void AbrirPDF(byte[] pdfData)
        {
            try
            {
                // Guardar el PDF en un archivo temporal
                string tempFilePath = System.IO.Path.GetTempFileName().Replace(".tmp", ".pdf"); // Aquí utilizamos System.IO.Path
                File.WriteAllBytes(tempFilePath, pdfData);

                // Abrir el PDF en la aplicación predeterminada
                Process.Start(tempFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir el PDF: " + ex.Message);
            }
        }
        private BitmapSource GenerateBarcode(string content)
        {
            BarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions { Width = 190, Height = 40 }
            };

            return writer.WriteAsWriteableBitmap(content);
        }

        // Método para convertir un BitmapSource a un array de bytes
        private byte[] ConvertToByteArray(BitmapSource bitmapSource)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(stream);
                return stream.ToArray();
            }
        }
    }
}
