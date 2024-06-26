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


namespace LoginSasteria
{
    internal class FacturaEncargoProductos
    {
        public void CrearFactura(string codigo, String DataEmpleado, String DataCliente, String DataDetalles, String Total, DataTable DataProductos, List<string> codigosBarras, string Abono, string DetallesPedido)
        {
            // Crear un MemoryStream para mantener los datos del PDF en la memoria
            DateTime fechaHoy = DateTime.Now;
            fechaHoy.ToString("g");
            BitmapSource barcodeBitmap = GenerateBarcode(codigo);
            using (MemoryStream ms = new MemoryStream())
            {
                // Crear un documento PDF
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                //DeviceRgb customColor = new DeviceRgb(139, 100, 75);//para agregar color


                //-------------------------------------------------------------------------------------------
                //Encabezado
                iText.Layout.Element.Table table = new iText.Layout.Element.Table(3).UseAllAvailableWidth();
                table.SetWidth(UnitValue.CreatePercentValue(100));
                table.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                //table.SetBackgroundColor(customColor);//agregamos color con este
                // Crear celdas para los elementos
                Cell detallesCell = new Cell();
                Cell imagenCell = new Cell();
                Cell codigos = new Cell();
                detallesCell.SetWidth(175);
                detallesCell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                imagenCell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);



                codigos.SetBorder(iText.Layout.Borders.Border.NO_BORDER);


                iText.Layout.Element.Paragraph detalles = new iText.Layout.Element.Paragraph(DataDetalles);
                detallesCell.Add(detalles);

                //imagen
                iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create("../../LogoBlack.png"));
                img.ScaleToFit(225f, 225f); // Ajustar el tamaño de la imagen si es necesario
                // Agregar la imagen a la celda izquierda
                imagenCell.Add(img);


                //crear el codigo de barras
                var barcodeBytes = ConvertToByteArray(barcodeBitmap);

                // Convertir el array de bytes en una imagen para iTextSharp
                iText.Layout.Element.Image barcodeImage = new iText.Layout.Element.Image(ImageDataFactory.Create(barcodeBytes));

                // Agregar la imagen del código de barras a la celda derecha
                codigos.Add(barcodeImage);
                codigos.SetBorder(new DottedBorder(ColorConstants.BLACK, 1));

                //arreglar detalles visuales

                detallesCell.SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE);
                detallesCell.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);
                codigos.SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE);
                codigos.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.RIGHT);

                iText.Layout.Element.Paragraph paCodigo = new iText.Layout.Element.Paragraph(codigo);
                paCodigo.SetPaddingTop(-15);
                codigos.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                codigos.Add(paCodigo);





                // Agregar celdas a la tabla
                table.AddCell(detallesCell);
                table.AddCell(imagenCell);
                table.AddCell(codigos);
                //-----------------------------------------------------------------------------------------------

                //-----------------------------------------------------------------------------------------------------
                //Cliente y Empleado
                iText.Layout.Element.Table EmpleadoCliente = new iText.Layout.Element.Table(2).UseAllAvailableWidth();
                EmpleadoCliente.SetWidth(UnitValue.CreatePercentValue(100));

                Cell cellEmpleado = new Cell();
                Cell cellCliente = new Cell();
                cellEmpleado.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                cellCliente.SetBorder(iText.Layout.Borders.Border.NO_BORDER);

                iText.Layout.Element.Paragraph pEmpleado = new iText.Layout.Element.Paragraph(DataEmpleado + "\n" + fechaHoy);
                cellEmpleado.Add(pEmpleado).SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.LEFT);

                iText.Layout.Element.Paragraph pCliente = new iText.Layout.Element.Paragraph(DataCliente);
                cellCliente.Add(pCliente).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);

                EmpleadoCliente.AddCell(cellEmpleado);
                EmpleadoCliente.AddCell(cellCliente);
                //-----------------------------------------------------------------------------------------------------

                //---------------------------------------------------------------------------------------------------
                //agregar dataTable
                iText.Layout.Element.Table productos = new iText.Layout.Element.Table(DataProductos.Columns.Count).UseAllAvailableWidth();

                // Agregar los encabezados de columna a la tabla
                foreach (DataColumn column in DataProductos.Columns)
                {
                    Cell headerCell = new Cell().Add(new iText.Layout.Element.Paragraph(column.ColumnName));
                    headerCell.SetBackgroundColor(ColorConstants.LIGHT_GRAY); // Opcional: establecer un color de fondo para los encabezados
                    productos.AddHeaderCell(headerCell.SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                }

                // Agregar los datos de la tabla
                foreach (DataRow row in DataProductos.Rows)
                {
                    foreach (object item in row.ItemArray)
                    {
                        Cell cell = new Cell().Add(new iText.Layout.Element.Paragraph(item.ToString()));
                        productos.AddCell(cell.SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                    }
                }

                EmpleadoCliente.SetMargin(10);
                //------------------------------------------------------------------------------------
                iText.Layout.Element.Paragraph PDetallesEncargo = new iText.Layout.Element.Paragraph(DetallesPedido);
                //------------------------------------------------------------------------------------
                //Agregar Total
                iText.Layout.Element.Paragraph TotalP = new iText.Layout.Element.Paragraph(Total);
                iText.Layout.Element.Paragraph AbonoP = new iText.Layout.Element.Paragraph(Abono);

                TotalP.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                AbonoP.SetFontSize(18);
                AbonoP.SetBorderBottom(new DottedBorder(ColorConstants.BLACK, 1));
                AbonoP.SetMarginBottom(5);
                Cell celdaTotal = new Cell();
                celdaTotal.SetPaddingLeft(300);
                celdaTotal.SetPaddingTop(20);
                celdaTotal.Add(AbonoP);
                celdaTotal.Add(TotalP);
                celdaTotal.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                celdaTotal.SetFontSize(20);


                //-----------------------------------------------------------------------------------------------
                //agregar codigos barras
                iText.Layout.Element.Table BarrasCodigos = new iText.Layout.Element.Table(2).UseAllAvailableWidth();

                iText.Layout.Element.Paragraph PaNocodigos = new iText.Layout.Element.Paragraph(
                    "SE HAN ENCARGADO " + codigosBarras.Count + " PRENDAS"
                    );
                PaNocodigos.SetFontSize(24);
                PaNocodigos.SetBorderBottom(new DottedBorder(ColorConstants.BLACK, 1));

                foreach (string barras in codigosBarras)
                {
                    Cell BarrasCelda = new Cell();
                    BitmapSource barcodeProductos = GenerateBarcode(barras);
                    var barraConvertidas = ConvertToByteArray(barcodeProductos);
                    iText.Layout.Element.Image barrasImagenes = new iText.Layout.Element.Image(ImageDataFactory.Create(barraConvertidas));
                    iText.Layout.Element.Paragraph dataBarras = new iText.Layout.Element.Paragraph(barras);
                    BarrasCelda.SetPadding(5);
                    barrasImagenes.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);
                    dataBarras.SetPaddingTop(-15);
                    BarrasCelda.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                    BarrasCelda.SetBorder(new DottedBorder(ColorConstants.BLACK, 2));
                    BarrasCelda.Add(barrasImagenes);
                    BarrasCelda.Add(dataBarras);
                    BarrasCodigos.AddCell(BarrasCelda);

                }
                BarrasCodigos.SetMarginTop(20);



                // Agregar las tabla al documento
                document.Add(table);
                document.Add(EmpleadoCliente);
                document.Add(productos);
                document.Add(PDetallesEncargo);
                document.Add(celdaTotal);
                document.Add(PaNocodigos);
                document.Add(BarrasCodigos);



                // Cerrar el documento
                document.Close();

                // Abrir el PDF en la aplicación predeterminada
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
                Options = new ZXing.Common.EncodingOptions { Width = 150, Height = 60 }
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
