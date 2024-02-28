using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
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
using System.Windows.Threading;


namespace LoginSasteria
{
    /// <summary>
    /// Lógica de interacción para ventaInventario.xaml
    /// </summary>
    public partial class ventaInventario : Window
    {
        ConexionDB objConection = new ConexionDB();
        DataTable tabla = new DataTable();
        public ventaInventario()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            
        }
        void timer_Tick(object sender, EventArgs e)
        {
            DateTime fechaActual = DateTime.Now;
            string hora = fechaActual.ToString("hh");
            string min = fechaActual.ToString("mm");
            string hora24 = fechaActual.ToString("HH");
            if (int.Parse(hora24) >= 12)
            {
                txblockHora.Text = hora + "\n" + min + " pm";
            }
            else
            {
                txblockHora.Text = hora + "\n" + min + " am";
            }
        }

        private void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void abrirInicio(object sender, RoutedEventArgs e)
        {
            mainInventario abrir = new mainInventario();
            abrir.Show();
            this.Close();
        }

        

        private void AgregarCodigo(object sender, RoutedEventArgs e)
        {
            string code = txCodigo.Text;
            if (code == null | code == "")
            {
                MessageBox.Show("Ingrese un codigo de producto");
            }
            else
            {
                string query = "SELECT idproducto AS 'Código', \r\nnombreproducto.Nombre AS Producto,\r\ncolor.nombre As Color,\r\nproducto.precio AS Precio, \r\ntipoproducto.nombreTipo AS 'Tipo Producto', \r\ntalla.nombreTalla As Talla\r\nFROM dbleonv2.inventario \r\nINNER JOIN dbleonv2.producto \r\nON inventario.producto_idproducto = producto.idproducto\r\nINNER JOIN dbleonv2.nombreproducto \r\nON producto.nombreProducto_idnombreProducto = nombreproducto.idnombreProducto \r\nINNER JOIN dbleonv2.color\r\nON producto.color_idcolor = color.idcolor\r\nINNER JOIN dbleonv2.tipotall\r\nON producto.talla_idtalla = tipotall.idtalla\r\nINNER JOIN dbleonv2.tipoproducto\r\nON tipotall.tipoProducto_idtipoProducto = tipoproducto.idtipoProducto\r\nINNER JOIN  dbleonv2.talla\r\nON tipotall.talla_idtalla = talla.idtalla\r\n" +
                "where idproducto='" + code + "'\r\n;";
                try
                {
                    MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                    MySqlDataAdapter data = new MySqlDataAdapter(comando);
                    



                    data.Fill(tabla);
                    if (DataDatos.Items.Count == 0)
                    {
                        
                        DataDatos.DataContext = tabla;
                    }
                    else
                    {
                        
                        
                         
                        List<string> fila = new List<string>();
                        fila.Clear();
                        List<string> nombrecolumna = new List<string>();
                        if (tabla.Columns.Count > 0)
                        {
                            foreach (DataColumn columna in tabla.Columns)
                            {
                                nombrecolumna.Add(columna.ColumnName);
                            }
                        }

                        foreach (string dato in nombrecolumna)
                        {
                            Console.WriteLine(dato);
                        }
                        objConection.cerrarCN();
                        MySqlCommand comando2 = new MySqlCommand(query, objConection.establecerCN());

                        MySqlDataReader reader = comando2.ExecuteReader();

                        while (reader.Read())
                        {
                            int i = 0;
                            foreach (string dato in nombrecolumna)
                            {                               
                                fila.Add(reader.GetValue(i).ToString());
                                i++;
                            }
                        }
                       
 
                        foreach (object dato in fila)
                        {
                            Console.WriteLine(dato.ToString());
                        }
                        DataRow firstRow;
                        firstRow = tabla.NewRow();

                        int j=0;
                        foreach (string dato in nombrecolumna)
                        {
                            firstRow[dato] = fila[j];
                            j++;
                        }
                        foreach(DataColumn dc in tabla.Columns)
                        {
                            Console.WriteLine(dc.ColumnName);
                        }
                        DataDatos.DataContext=tabla;

                    }
                    objConection.cerrarCN();
                }
                catch (MySqlException x)
                {
                    MessageBox.Show("Error: " + x);
                }
            }
        }
    }
}

