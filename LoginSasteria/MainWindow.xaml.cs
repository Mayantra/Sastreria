using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text.RegularExpressions;

namespace LoginSasteria
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ConexionDB objConection = new ConexionDB();
        
        public MainWindow()
        {
            InitializeComponent();
            /*ConexionDB objConection = new ConexionDB();
            objConection.establecerCN();*/

            primeraCon();
            tipoCon();
            getFecha();
            

        }
        void primeraCon()
        {
            string query = "SELECT idproducto AS 'Código', \r\nalmacen.nombre AS Sucursal, \r\nnombreproducto.Nombre AS Producto,\r\ncolor.nombre As Color,\r\nproducto.precio AS Precio, \r\ntipoproducto.nombreTipo AS 'Tipo Producto', \r\ntalla.nombreTalla As Talla\r\nFROM dbleonv2.inventario \r\nINNER JOIN dbleonv2.producto \r\nON inventario.producto_idproducto = producto.idproducto\r\nINNER JOIN dbleonv2.nombreproducto \r\nON producto.nombreProducto_idnombreProducto = nombreproducto.idnombreProducto \r\nINNER JOIN dbleonv2.almacen\r\nON inventario.almacen_idalmacen = almacen.idalmacen\r\nINNER JOIN dbleonv2.color\r\nON producto.color_idcolor = color.idcolor\r\nINNER JOIN dbleonv2.tipotall\r\nON producto.talla_idtalla = tipotall.idtalla\r\nINNER JOIN dbleonv2.tipoproducto\r\nON tipotall.tipoProducto_idtipoProducto = tipoproducto.idtipoProducto\r\nINNER JOIN  dbleonv2.talla\r\nON tipotall.talla_idtalla = talla.idtalla;";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataAdapter data = new MySqlDataAdapter(comando);
                DataTable tabla = new DataTable();

                data.Fill(tabla);

                DataConsulta.DataContext = tabla;
                objConection.cerrarCN();
            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
        }

        void consultaFiltro(string Query)
        {
            string query = Query;
            try
            {
                MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataAdapter data = new MySqlDataAdapter(comando);
                DataTable tabla = new DataTable();

                data.Fill(tabla);

                DataConsulta.DataContext = tabla;
                objConection.cerrarCN();
            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }

        }
        void tipoCon()
        {
            string query = "SELECT * FROM dbleonv2.tipoproducto;";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataReader myread;

                myread = comando.ExecuteReader();

                while (myread.Read())
                {
                    
                    string nombres = myread.GetString("nombreTipo");
                    cbTipo.Items.Add(nombres);

                }
                objConection.cerrarCN();
                query = "SELECT * FROM dbleonv2.color;";
                MySqlCommand comando2 = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataReader myread2;

                myread2 = comando2.ExecuteReader();
                while (myread2.Read())
                {
                    string nombres = myread2.GetString("nombre");
                    cbColor.Items.Add(nombres);
                }
                objConection.cerrarCN();

                query = "SELECT * FROM dbleonv2.talla;";
                MySqlCommand comando3 = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataReader myread3;

                myread3 = comando3.ExecuteReader();
                while (myread3.Read())
                {
                    string nombres = myread3.GetString("nombreTalla");
                    cbTalla.Items.Add(nombres);
                }
                objConection.cerrarCN();
            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
        }

        string getFecha()
        {
            DateTime fechaActual = DateTime.Now;

            string mes = fechaActual.ToString("MMMM");
            string dia = fechaActual.ToString("dd");
            
            
            lbFecha.Content = dia+"\n"+mes;

            return dia;
        }

        private void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ConsultaGeneral(object sender, RoutedEventArgs e)
        {
            cbTipo.SelectedValue = null;
            cbColor.SelectedValue = null;
            cbTalla.SelectedValue = null;

            primeraCon();
        }

        private void Obtener(object sender, RoutedEventArgs e)
        {
            string query = "";
            //MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
            object idTipo = cbTipo.SelectedValue;
            object talla = cbTalla.SelectedValue;
            object color = cbColor.SelectedValue;
            
            if(idTipo != null)
            {
                idTipo.ToString();

                if (talla == null)//pero si la talla esta vacía
                {
                    if (color == null)// pero si el color esta vacio
                    {
                        //generar query solamente de los tipos
                        query = "SELECT idproducto AS 'Código', \r\nalmacen.nombre AS Sucursal, \r\nnombreproducto.Nombre AS Producto,\r\ncolor.nombre As Color,\r\nproducto.precio AS Precio, \r\ntipoproducto.nombreTipo AS 'Tipo Producto', \r\ntalla.nombreTalla As Talla\r\nFROM dbleonv2.inventario \r\nINNER JOIN dbleonv2.producto \r\nON inventario.producto_idproducto = producto.idproducto\r\nINNER JOIN dbleonv2.nombreproducto \r\nON producto.nombreProducto_idnombreProducto = nombreproducto.idnombreProducto \r\nINNER JOIN dbleonv2.almacen\r\nON inventario.almacen_idalmacen = almacen.idalmacen\r\nINNER JOIN dbleonv2.color\r\nON producto.color_idcolor = color.idcolor\r\nINNER JOIN dbleonv2.tipotall\r\nON producto.talla_idtalla = tipotall.idtalla\r\nINNER JOIN dbleonv2.tipoproducto\r\nON tipotall.tipoProducto_idtipoProducto = tipoproducto.idtipoProducto\r\nINNER JOIN  dbleonv2.talla\r\nON tipotall.talla_idtalla = talla.idtalla\r\n" +
                            "WHERE tipoproducto.nombreTipo ='"+ idTipo +"';";
                        consultaFiltro(query);
                    }
                    else//si hay un color sellecionado
                    {
                        color.ToString();
                        query = "SELECT idproducto AS 'Código', \r\nalmacen.nombre AS Sucursal, \r\nnombreproducto.Nombre AS Producto,\r\ncolor.nombre As Color,\r\nproducto.precio AS Precio, \r\ntipoproducto.nombreTipo AS 'Tipo Producto', \r\ntalla.nombreTalla As Talla\r\nFROM dbleonv2.inventario \r\nINNER JOIN dbleonv2.producto \r\nON inventario.producto_idproducto = producto.idproducto\r\nINNER JOIN dbleonv2.nombreproducto \r\nON producto.nombreProducto_idnombreProducto = nombreproducto.idnombreProducto \r\nINNER JOIN dbleonv2.almacen\r\nON inventario.almacen_idalmacen = almacen.idalmacen\r\nINNER JOIN dbleonv2.color\r\nON producto.color_idcolor = color.idcolor\r\nINNER JOIN dbleonv2.tipotall\r\nON producto.talla_idtalla = tipotall.idtalla\r\nINNER JOIN dbleonv2.tipoproducto\r\nON tipotall.tipoProducto_idtipoProducto = tipoproducto.idtipoProducto\r\nINNER JOIN  dbleonv2.talla\r\nON tipotall.talla_idtalla = talla.idtalla\r\n" +
                            "WHERE tipoproducto.nombreTipo ='" + idTipo +"'"+
                            "\r\nAND color.nombre = '"+color+ "';";
                        consultaFiltro(query);

                    }

                }
                else//pero si hay algo en la talla
                {
                    if(color == null)//pero no hay nada en color
                    {
                        talla.ToString();
                        query = "SELECT idproducto AS 'Código', \r\nalmacen.nombre AS Sucursal, \r\nnombreproducto.Nombre AS Producto,\r\ncolor.nombre As Color,\r\nproducto.precio AS Precio, \r\ntipoproducto.nombreTipo AS 'Tipo Producto', \r\ntalla.nombreTalla As Talla\r\nFROM dbleonv2.inventario \r\nINNER JOIN dbleonv2.producto \r\nON inventario.producto_idproducto = producto.idproducto\r\nINNER JOIN dbleonv2.nombreproducto \r\nON producto.nombreProducto_idnombreProducto = nombreproducto.idnombreProducto \r\nINNER JOIN dbleonv2.almacen\r\nON inventario.almacen_idalmacen = almacen.idalmacen\r\nINNER JOIN dbleonv2.color\r\nON producto.color_idcolor = color.idcolor\r\nINNER JOIN dbleonv2.tipotall\r\nON producto.talla_idtalla = tipotall.idtalla\r\nINNER JOIN dbleonv2.tipoproducto\r\nON tipotall.tipoProducto_idtipoProducto = tipoproducto.idtipoProducto\r\nINNER JOIN  dbleonv2.talla\r\nON tipotall.talla_idtalla = talla.idtalla\r\n" +
                            "WHERE tipoproducto.nombreTipo ='" + idTipo + "'" +
                            "\r\nAND talla.nombreTalla = '" + talla + "';";
                        consultaFiltro(query);

                    }
                    else //pero si hay algo en el color SCRIPT COMPLETO
                    {
                        talla.ToString();
                        color.ToString();
                        query = "SELECT idproducto AS 'Código', \r\nalmacen.nombre AS Sucursal, \r\nnombreproducto.Nombre AS Producto,\r\ncolor.nombre As Color,\r\nproducto.precio AS Precio, \r\ntipoproducto.nombreTipo AS 'Tipo Producto', \r\ntalla.nombreTalla As Talla\r\nFROM dbleonv2.inventario \r\nINNER JOIN dbleonv2.producto \r\nON inventario.producto_idproducto = producto.idproducto\r\nINNER JOIN dbleonv2.nombreproducto \r\nON producto.nombreProducto_idnombreProducto = nombreproducto.idnombreProducto \r\nINNER JOIN dbleonv2.almacen\r\nON inventario.almacen_idalmacen = almacen.idalmacen\r\nINNER JOIN dbleonv2.color\r\nON producto.color_idcolor = color.idcolor\r\nINNER JOIN dbleonv2.tipotall\r\nON producto.talla_idtalla = tipotall.idtalla\r\nINNER JOIN dbleonv2.tipoproducto\r\nON tipotall.tipoProducto_idtipoProducto = tipoproducto.idtipoProducto\r\nINNER JOIN  dbleonv2.talla\r\nON tipotall.talla_idtalla = talla.idtalla\r\n" +
                            "WHERE tipoproducto.nombreTipo ='" + idTipo + "'" +
                            "\r\nAND talla.nombreTalla = '" + talla + "'" +
                            "\r\nAND color.nombre = '" + color + "';";
                        consultaFiltro(query);

                    }

                }

            }
            else // ahora si no esta el tipo activo
            {
                if (talla == null)//si la talla no esta seleccionada
                {
                    if (color == null)//tampoco el color esta seleccionado
                    {
                        MessageBox.Show("Selccione algún filtro para la busqueda");
                    }
                    else // solo color Seleccionado
                    {
                        color.ToString();
                        query = "SELECT idproducto AS 'Código', \r\nalmacen.nombre AS Sucursal, \r\nnombreproducto.Nombre AS Producto,\r\ncolor.nombre As Color,\r\nproducto.precio AS Precio, \r\ntipoproducto.nombreTipo AS 'Tipo Producto', \r\ntalla.nombreTalla As Talla\r\nFROM dbleonv2.inventario \r\nINNER JOIN dbleonv2.producto \r\nON inventario.producto_idproducto = producto.idproducto\r\nINNER JOIN dbleonv2.nombreproducto \r\nON producto.nombreProducto_idnombreProducto = nombreproducto.idnombreProducto \r\nINNER JOIN dbleonv2.almacen\r\nON inventario.almacen_idalmacen = almacen.idalmacen\r\nINNER JOIN dbleonv2.color\r\nON producto.color_idcolor = color.idcolor\r\nINNER JOIN dbleonv2.tipotall\r\nON producto.talla_idtalla = tipotall.idtalla\r\nINNER JOIN dbleonv2.tipoproducto\r\nON tipotall.tipoProducto_idtipoProducto = tipoproducto.idtipoProducto\r\nINNER JOIN  dbleonv2.talla\r\nON tipotall.talla_idtalla = talla.idtalla\r\n" +
                            "WHERE color.nombre = '" + color + "';";
                        consultaFiltro(query);
                    }

                }
                else //talla seleccionada
                {
                    if(color == null) //color no seleccionado osea solo la talla seleccionada
                    {
                        talla.ToString();
                        query = "SELECT idproducto AS 'Código', \r\nalmacen.nombre AS Sucursal, \r\nnombreproducto.Nombre AS Producto,\r\ncolor.nombre As Color,\r\nproducto.precio AS Precio, \r\ntipoproducto.nombreTipo AS 'Tipo Producto', \r\ntalla.nombreTalla As Talla\r\nFROM dbleonv2.inventario \r\nINNER JOIN dbleonv2.producto \r\nON inventario.producto_idproducto = producto.idproducto\r\nINNER JOIN dbleonv2.nombreproducto \r\nON producto.nombreProducto_idnombreProducto = nombreproducto.idnombreProducto \r\nINNER JOIN dbleonv2.almacen\r\nON inventario.almacen_idalmacen = almacen.idalmacen\r\nINNER JOIN dbleonv2.color\r\nON producto.color_idcolor = color.idcolor\r\nINNER JOIN dbleonv2.tipotall\r\nON producto.talla_idtalla = tipotall.idtalla\r\nINNER JOIN dbleonv2.tipoproducto\r\nON tipotall.tipoProducto_idtipoProducto = tipoproducto.idtipoProducto\r\nINNER JOIN  dbleonv2.talla\r\nON tipotall.talla_idtalla = talla.idtalla\r\n" +
                            "WHERE talla.nombreTalla = '" + talla + "';";
                        consultaFiltro(query);
                    }
                    else //color y talla seleccionados
                    {
                        talla.ToString();
                        color.ToString();
                        query = "SELECT idproducto AS 'Código', \r\nalmacen.nombre AS Sucursal, \r\nnombreproducto.Nombre AS Producto,\r\ncolor.nombre As Color,\r\nproducto.precio AS Precio, \r\ntipoproducto.nombreTipo AS 'Tipo Producto', \r\ntalla.nombreTalla As Talla\r\nFROM dbleonv2.inventario \r\nINNER JOIN dbleonv2.producto \r\nON inventario.producto_idproducto = producto.idproducto\r\nINNER JOIN dbleonv2.nombreproducto \r\nON producto.nombreProducto_idnombreProducto = nombreproducto.idnombreProducto \r\nINNER JOIN dbleonv2.almacen\r\nON inventario.almacen_idalmacen = almacen.idalmacen\r\nINNER JOIN dbleonv2.color\r\nON producto.color_idcolor = color.idcolor\r\nINNER JOIN dbleonv2.tipotall\r\nON producto.talla_idtalla = tipotall.idtalla\r\nINNER JOIN dbleonv2.tipoproducto\r\nON tipotall.tipoProducto_idtipoProducto = tipoproducto.idtipoProducto\r\nINNER JOIN  dbleonv2.talla\r\nON tipotall.talla_idtalla = talla.idtalla\r\n" +
                            "WHERE color.nombre = '" + color + "'" +
                            "\r\nAND talla.nombreTalla = '" + talla + "';";
                        consultaFiltro(query);

                    }

                }

            }
            

            
            Console.WriteLine(idTipo);
        }

        

        private void cambioPass(object sender, RoutedEventArgs e)
        {
            leerPass read = new leerPass();
            for (int i = 0; i <= PassBox.Password.Length; i++)
            {
                if (i == 5)
                {
                    string user = txUser.Text;
                    string pass = PassBox.Password;
                    if(read.setPass(user, pass) != 0)
                    {
                        ventanaInicio abrirVentana = new ventanaInicio();
                        
                        abrirVentana.Show();
                        this.Close();

                    }
                    
                    txUser.Text = "";
                    PassBox.Password = "";          
                }

            }
        }

        private void LeerSinNumeros(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !OnlyLetras(e.Text);
        }
        private static readonly Regex _regex = new Regex(@"[^a-zA-Z]+$");
        private static bool OnlyLetras(string text)
        {
            return !_regex.IsMatch(text);
        }
    }
}
