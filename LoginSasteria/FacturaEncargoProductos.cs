using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Layout.Borders;
using ZXing;

namespace LoginSasteria
{
    internal class FacturaEncargoProductos
    {
        public void CrearFactura(string codigo, string DataEmpleado, string DataCliente, string DataDetalles, string Total, DataTable DataProductos, List<string> codigosBarras, string Abono, string DetallesPedido)
        {
            DateTime fechaHoy = DateTime.Now;
            BitmapSource barcodeBitmap = GenerateBarcode(codigo);

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    PdfWriter writer = new PdfWriter(ms);
                    PdfDocument pdf = new PdfDocument(writer);
                    Document document = new Document(pdf);
                    pdf.SetDefaultPageSize(iText.Kernel.Geom.PageSize.LETTER);
                    document.SetMargins(15, 15, 15, 15);

                    // Código original de generación del PDF (sin cambios)
                    //-------------------------------------------------------------------------------------------
                    //Encabezado
                    iText.Layout.Element.Table table = new iText.Layout.Element.Table(2).UseAllAvailableWidth();
                    table.SetWidth(UnitValue.CreatePercentValue(100));
                    table.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                    //table.SetBackgroundColor(customColor);//agregamos color con este
                    // Crear celdas para los elementos
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
                    //imagen
                    iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create("../../LogoBlack.png"));
                    img.ScaleToFit(100f, 150f); // Ajustar el tamaño de la imagen si es necesario
                                                // Agregar la imagen a la celda izquierda
                    imagenCell.Add(img);
                    imagenCell.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);


                    //crear el codigo de barras
                    var barcodeBytes = ConvertToByteArray(barcodeBitmap);

                    // Convertir el array de bytes en una imagen para iTextSharp
                    iText.Layout.Element.Image barcodeImage = new iText.Layout.Element.Image(ImageDataFactory.Create(barcodeBytes));

                    // Agregar la imagen del código de barras a la celda derecha
                    codigos.Add(barcodeImage);

                    //arreglar detalles visuales


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
                    // Agregar celdas a la tabla
                    Cell celldaTableDetalles = new Cell();
                    celldaTableDetalles.Add(tableDetalles);
                    celldaTableDetalles.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                    celldaTableDetalles.SetMaxWidth(75);
                    celldaTableDetalles.SetMinWidth(75);
                    table.AddCell(celldaTableDetalles);


                    //-----------------------------------------------------------------------------------------------

                    //-----------------------------------------------------------------------------------------------------
                    //Cliente y Empleado
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
                    productos.SetMaxWidth(400);
                    productos.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.RIGHT);

                    //------------------------------------------------------------------------------------
                    document.SetFontSize(9);
                    //------------------------------------------------------------------------------------
                    iText.Layout.Element.Paragraph PDetallesEncargo = new iText.Layout.Element.Paragraph(DetallesPedido);
                    //------------------------------------------------------------------------------------
                    //Agregar Total
                    iText.Layout.Element.Paragraph TotalP = new iText.Layout.Element.Paragraph(Total);
                    iText.Layout.Element.Paragraph AbonoP = new iText.Layout.Element.Paragraph(Abono);

                    TotalP.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    AbonoP.SetFontSize(12);
                    AbonoP.SetBorderBottom(new DottedBorder(ColorConstants.BLACK, 1));
                    Cell celdaTotal = new Cell();
                    celdaTotal.SetPaddingLeft(300);
                    celdaTotal.Add(AbonoP);
                    celdaTotal.Add(TotalP);
                    celdaTotal.SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
                    TotalP.SetPaddingRight(15);
                    celdaTotal.SetFontSize(12);

                    iText.Layout.Element.Table tablaDuplicados = new iText.Layout.Element.Table(2).UseAllAvailableWidth();
                    for (int i = 0; i < 4; i++)
                    {
                        Cell cellDuplicados = new Cell();
                        cellDuplicados.Add(codigos.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER));

                        tablaDuplicados.AddCell(cellDuplicados);
                    }
                    tablaDuplicados.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);


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

                    Cell celdaProductos = new Cell();
                    celdaProductos.Add(productos);
                    celdaProductos.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                    table.AddCell(celdaProductos);

                    // Agregar las tabla al documento
                    document.Add(noContableCell);
                    document.Add(EmpleadoCliente);
                    document.Add(table);
                    document.Add(PDetallesEncargo);
                    document.Add(celdaTotal);
                    document.Add(tablaDuplicados);
                    document.Add(PaNocodigos);
                    document.Add(BarrasCodigos);

                    document.Close();
                    AbrirPDF(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar la factura: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AbrirPDF(byte[] pdfData)
        {
            try
            {
                string tempFilePath = Path.Combine(Path.GetTempPath(), $"Factura_{Guid.NewGuid()}.pdf");
                File.WriteAllBytes(tempFilePath, pdfData);
                Process.Start(new ProcessStartInfo(tempFilePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir el PDF: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
