using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Messaging;
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
        public static List<string> listacodigos = new List<string>();
        public static Boolean existeCliente = false;
        public static int IDCliente=0;
        public static int regalo;


        public ventaInventario()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();


        }
        public void cleanValues()
        {
            listacodigos.Clear();
            tabla.Clear();
            existeCliente = false;
            IDCliente = 0;

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
            System.Windows.Application.Current.Shutdown();
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

        public void addlist(string codigosList)
        {
            listacodigos.Add(codigosList);
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
                Boolean existe = false;
                for (int z =0; z < listacodigos.Count; z++)
                {
                    if (listacodigos[z].Contains(code))
                    {
                        existe = true;
                    }
                }
                if (existe == false)
                {
                    string query = "SELECT idproducto AS 'Código', \r\nnombreproducto.Nombre AS Producto,\r\ncolor.nombre As Color,\r\nproducto.precio AS Precio, \r\ntipoproducto.nombreTipo AS 'Tipo Producto', \r\ntalla.nombreTalla As Talla\r\nFROM dbleonv2.inventario \r\nINNER JOIN dbleonv2.producto \r\nON inventario.producto_idproducto = producto.idproducto\r\nINNER JOIN dbleonv2.nombreproducto \r\nON producto.nombreProducto_idnombreProducto = nombreproducto.idnombreProducto \r\nINNER JOIN dbleonv2.color\r\nON producto.color_idcolor = color.idcolor\r\nINNER JOIN dbleonv2.tipotall\r\nON producto.talla_idtalla = tipotall.idtalla\r\nINNER JOIN dbleonv2.tipoproducto\r\nON tipotall.tipoProducto_idtipoProducto = tipoproducto.idtipoProducto\r\nINNER JOIN  dbleonv2.talla\r\nON tipotall.talla_idtalla = talla.idtalla\r\n" +
                "where idproducto='" + code + "'\r\n;";
                    try
                    {
                        MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                        MySqlDataAdapter data = new MySqlDataAdapter(comando);

                        //la fumanda mas grande del codigo creo XD

                        data.Fill(tabla);
                        if (tabla.Rows.Count < 1)//si el codigo es incorrecto
                        {
                            MessageBox.Show("Ingrese un código de producto correcto");

                        }
                        else
                        {
                            if (DataDatos.Items.Count == 0)//verificar si el datagrid tiene algo
                            {

                                DataDatos.DataContext = tabla;
                                addlist(code);
                            }
                            else
                            {

                                //obtener datos de filas y columnas de datagrid y datatable

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


                                objConection.cerrarCN();
                                MySqlCommand comando2 = new MySqlCommand(query, objConection.establecerCN());

                                MySqlDataReader reader = comando2.ExecuteReader();

                                if (reader.HasRows)//leer si query es correcto
                                {

                                    while (reader.Read())
                                    {
                                        int i = 0;
                                        foreach (string dato in nombrecolumna)
                                        {
                                            fila.Add(reader.GetValue(i).ToString());
                                            i++;
                                        }
                                    }


                                    DataRow firstRow;
                                    firstRow = tabla.NewRow();//crear una nueva fila

                                    int j = 0;
                                    foreach (string dato in nombrecolumna)//Asignar las celdas en cada columna
                                    {
                                        firstRow[dato] = fila[j];
                                        j++;
                                    }

                                    DataDatos.DataContext = tabla;
                                    addlist(code);
                                }
                                else
                                {
                                    MessageBox.Show("Ingrese un código de producto correcto");
                                }
                            }

                        }
                        objConection.cerrarCN();
                    }
                    catch (MySqlException x)
                    {
                        MessageBox.Show("Error: " + x);
                    }
                }
                else
                {
                    MessageBox.Show("Este articulo ya se encuenta dentro de la venta");
                }
            }
            txCodigo.Clear();

        }

        private void EliminarFila(object sender, RoutedEventArgs e)
        {
            int indice = -1;
            indice = DataDatos.SelectedIndex;

            if (indice > -1)
            {
                tabla.Rows.RemoveAt(indice);
                DataDatos.DataContext = tabla;
                listacodigos.RemoveAt(indice);
            }

        }

        
        
        public void buscarCliente(string query)
        {
            MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
            MySqlDataReader reader = comando.ExecuteReader();

            string lectura = "";
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        lectura += reader.GetValue(i).ToString() + "\t\t";
                    }
                }
                txNombreCliente.Text = "Nombre del Cliente \n" + lectura;
                existeCliente = true;

                txNit.Clear();
                txTelefono.Clear();
                

            }
            else
            {
                MessageBox.Show("No se encontro un cliente con la información proporcionada");
                txNit.Clear();
                txTelefono.Clear();
            }
            reader.Close();
            objConection.cerrarCN();

        }

        void idcliente(string dato, Boolean busqueda)
        {
            string query="";
            if(busqueda == true)
            {
                query = "SELECT idCliente FROM dbleonv2.cliente where NIT = '"+dato+"';";
            }
            else {
                query = "SELECT idCliente FROM dbleonv2.cliente where telefono = '" + dato + "';";
            }
            MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
            MySqlDataReader reader = comando.ExecuteReader();
                        
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    IDCliente = reader.GetInt32(0);
                }                
            }
            Console.WriteLine(IDCliente);
        }
        private void buscarCliente(object sender, RoutedEventArgs e)
        {
            
            if (txNit.Text.Length > 0)
            {
                string NIT = txNit.Text;
                string query = "SELECT Nombres, Apellidos, puntos FROM dbleonv2.cliente where NIT ='" + NIT + "';";
                buscarCliente(query);
                if (existeCliente == true)
                {
                    idcliente(NIT, true);
                }
                

            }
            else if (txTelefono.Text.Length>0)
            {
                string cel = txTelefono.Text;
                string query = "SELECT Nombres, Apellidos, puntos FROM dbleonv2.cliente where telefono ='" + cel + "';";
                buscarCliente(query);
                if (existeCliente == true)
                {
                    idcliente(cel, false);
                }
            }
            else
            {
                MessageBox.Show("Ingrese un Número de Teléfono o un Número de NIT para bucar el cliente");
            }
            objConection.cerrarCN();

        }
        private void RealizarVenta(object sender, RoutedEventArgs e)
        {
            
            regalo = 0;
            if (DataDatos.Items.Count == 0)
            {
                MessageBox.Show("Debe de agregar productos para realizar una venta");
            }
            else
            {

                ClientesVentas agregar = new ClientesVentas();
                agregar.setDatos(tabla, listacodigos, IDCliente, regalo, existeCliente);
                agregar.abrir();

            }
            this.Close();

        }

        private void ProcesoRegaloVenta(object sender, RoutedEventArgs e)
        {
            
            regalo = 1;
            if (DataDatos.Items.Count == 0)
            {
                MessageBox.Show("Debe de agregar productos para realizar una venta");
            }
            else
            {

                ClientesVentas agregar = new ClientesVentas();
                agregar.setDatos(tabla, listacodigos, IDCliente, regalo, existeCliente);
                agregar.abrir();

            }
            
            this.Close();
            

        }

        private void LeerCodigoBarras(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AgregarCodigo(null,null);
            }
        }
    }
}

