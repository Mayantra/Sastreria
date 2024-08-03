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
using iText.Layout.Renderer;
using System.Drawing;

namespace LoginSasteria
{
    internal class ImprimirEstados
    {
        public void ImprimirEstadoCuentas(DataTable DataProductos, string Total, string Detalles)
        {
            
            using (MemoryStream ms = new MemoryStream())
            {
                // Crear un documento PDF
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);




                iText.Layout.Element.Table table = new iText.Layout.Element.Table(3).UseAllAvailableWidth();
                table.SetWidth(UnitValue.CreatePercentValue(100));
                table.SetBorder(iText.Layout.Borders.Border.NO_BORDER);

                Cell imagenCell = new Cell();
                Cell DetalllesCell = new Cell();
                //imagen
                iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create("../../LogoBlack.png"));
                img.ScaleToFit(225f, 225f); // Ajustar el tamaño de la imagen si es necesario
                // Agregar la imagen a la celda izquierda
                imagenCell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                imagenCell.Add(img);

                table.AddCell(imagenCell);
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

                iText.Layout.Element.Paragraph TotalP = new iText.Layout.Element.Paragraph(Total);
                TotalP.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                Cell celdaTotal = new Cell();
                celdaTotal.SetPaddingLeft(300);
                celdaTotal.SetPaddingTop(20);
                
                celdaTotal.Add(TotalP);
                celdaTotal.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                celdaTotal.SetFontSize(20);

                iText.Layout.Element.Paragraph pDetalles = new iText.Layout.Element.Paragraph(Detalles);
                DetalllesCell.Add(pDetalles);
                DetalllesCell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                DetalllesCell.SetFontSize(12);
                DetalllesCell.SetMargin(25);

                iText.Layout.Element.Table tableFirmas = new iText.Layout.Element.Table(3).UseAllAvailableWidth();
                Cell cellEmisor = new Cell();
                Cell Separador = new Cell();
                Cell cellReceptor = new Cell();
                iText.Layout.Element.Paragraph pEmisor = new iText.Layout.Element.Paragraph("Firma del Emisor").SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                cellEmisor.Add(pEmisor);
                cellEmisor.SetBorderTop(new SolidBorder(ColorConstants.BLACK, 1));

                iText.Layout.Element.Paragraph pSeparador = new iText.Layout.Element.Paragraph("     \t\t      ").SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                Separador.Add(pSeparador);


                iText.Layout.Element.Paragraph pReceptor = new iText.Layout.Element.Paragraph("Firma del Receptor").SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);

                cellReceptor.Add(pReceptor);
                cellReceptor.SetBorderTop(new SolidBorder(ColorConstants.BLACK, 1));

                // Agregar las celdas a la tabla
                tableFirmas.AddCell(cellEmisor);
                tableFirmas.AddCell(Separador);
                tableFirmas.AddCell(cellReceptor);

                tableFirmas.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                tableFirmas.SetMarginTop(100);

                Separador.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                cellEmisor.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                cellEmisor.SetMargin(25);
                cellReceptor.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                cellReceptor.SetMargin(25);
                // Agregar las tabla al documento
                document.Add(table);
                document.Add(productos);
                document.Add(celdaTotal);
                document.Add(pDetalles);
                document.Add(tableFirmas);




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
    }
}
