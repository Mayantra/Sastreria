using MySql.Data.MySqlClient;
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
using ZXing;

namespace LoginSasteria
{
    /// <summary>
    /// Lógica de interacción para DevolucionRegalo.xaml
    /// </summary>
    public partial class DevolucionRegalo : Window
    {
        ConexionDB objConection = new ConexionDB();
        public DevolucionRegalo()
        {
            InitializeComponent();
        }
        private void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void obtenerFactura(object sender, RoutedEventArgs e)
        {
            string codigo = txFactura.Text;
            if (codigo == null | codigo == "")
            {
                MessageBox.Show("Ingrese un codigo de producto");
            }
            else
            {
                if (verificarFactura(codigo) == true)
                {
                    //MessageBox.Show("Acceso consedido");
                    CargarFactura(codigo);
                    
                }
                else
                {
                    MessageBox.Show("No es posible cumplir con el proceso de Regalo" +
                        "\nEl tiempo de devolución a Finalizado");
                }
            }
            
        }
        public Boolean verificarFactura(string codigo)
        {
            Boolean autorizar = false;
            DateTime fechaHoy = DateTime.Now;
            DateTime fechaFactura= DateTime.Now;
            objConection.cerrarCN();
            string query = "SELECT FechaHora FROM " + objConection.namedb() + ".detallesventa where idDetallesVenta='"+codigo+"';";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    fechaFactura = reader.GetDateTime(0);
                }
                reader.Close();
                objConection.cerrarobjConection();
            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
            TimeSpan diferencia = fechaHoy - fechaFactura;
            int diasDiferencia = Math.Abs((int)diferencia.TotalDays);
            if (diasDiferencia <= 5)
            {
                // La diferencia entre las fechas es menor o igual a 5 días
                autorizar = true;
            }
            else
            {
                // La diferencia entre las fechas es mayor a 5 días
                autorizar = false;
            }

            return autorizar;
        }
        public void CargarFactura(string codigoFactura)
        {
            objConection.cerrarCN();
            string query = "SELECT producto_idproducto as 'Producto'," +
                "\r\ntipoproducto.nombreTipo as 'Tipo'," +
                "\r\ntalla.nombreTalla As 'Talla',\r\nprecio as 'Precio'," +
                "\r\nFechaHora as 'Fecha',\r\nempleado.Nombre as 'Nombre Cajero'" +
                "\r\n \r\nFROM " + objConection.namedb() + ".registroventa\r\ninner join " + objConection.namedb() + ".detallesventa" +
                "\r\non DetallesVenta_idDetallesVenta=idDetallesVenta\r\ninner join " + objConection.namedb() + ".producto" +
                "\r\non producto_idproducto = idproducto\r\nINNER JOIN " + objConection.namedb() + ".tipotall" +
                "\r\nON producto.talla_idtalla = tipotall.idtalla\r\nINNER JOIN " + objConection.namedb() + ".tipoproducto" +
                "\r\nON tipotall.tipoProducto_idtipoProducto = tipoproducto.idtipoProducto" +
                "\r\nINNER JOIN  " + objConection.namedb() + ".talla\r\nON tipotall.talla_idtalla = talla.idtalla" +
                "\r\ninner join " + objConection.namedb() + ".empleado\r\non Empleado_idEmpleado = idEmpleado" +
                "\r\n\r\nwhere idDetallesVenta ='"+codigoFactura+"';";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataAdapter data = new MySqlDataAdapter(comando);
                DataTable tabla = new DataTable();

                data.Fill(tabla);
                if (tabla.Rows.Count < 1)//si el codigo es incorrecto
                {
                    MessageBox.Show("Ingrese un código de producto correcto");

                }
                else
                {

                    DataConsulta.DataContext = tabla;
                    objConection.cerrarCN();
                    CargarTotalFacturas(codigoFactura);

                }
                objConection.cerrarCN();
            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
        }

        public void CargarTotalFacturas(string codigoFactura)
        {
            
            objConection.cerrarCN();
            string query = "SELECT  sum(precio) as Total" +
                "\r\n \r\nFROM " + objConection.namedb() + ".registroventa\r\ninner join " + objConection.namedb() + ".detallesventa" +
                "\r\non DetallesVenta_idDetallesVenta=idDetallesVenta\r\ninner join " + objConection.namedb() + ".producto" +
                "\r\non producto_idproducto = idproducto\r\nINNER JOIN " + objConection.namedb() + ".tipotall" +
                "\r\nON producto.talla_idtalla = tipotall.idtalla\r\nINNER JOIN " + objConection.namedb() + ".tipoproducto" +
                "\r\nON tipotall.tipoProducto_idtipoProducto = tipoproducto.idtipoProducto" +
                "\r\nINNER JOIN  " + objConection.namedb() + ".talla\r\nON tipotall.talla_idtalla = talla.idtalla" +
                "\r\ninner join " + objConection.namedb() + ".empleado\r\non Empleado_idEmpleado = idEmpleado" +
                "\r\n\r\nwhere idDetallesVenta ='" + codigoFactura + "';";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {                    
                    txTotalFac.Text = "Q " + reader.GetDouble(0).ToString();
                }
                reader.Close();
                objConection.cerrarCN();
            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
        }

        private void btnInicio_Click(object sender, RoutedEventArgs e)
        {
            mainInventario abrir = new mainInventario();
            abrir.Show();
            this.Close();
        }

        private void btnVenta_Click(object sender, RoutedEventArgs e)
        {
            ventaInventario abrir = new ventaInventario();
            abrir.Show();
            this.Close();
        }

        private void btnConsulta_Click(object sender, RoutedEventArgs e)
        {
            consultaInventario abrir = new consultaInventario();
            abrir.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InventarioInventario abrir = new InventarioInventario();
            abrir.Show();
            this.Close();
        }

        private void btnBarras_Click(object sender, RoutedEventArgs e)
        {
            crearBarrasMenu abrir = new crearBarrasMenu();
            abrir.Show();
            this.Close();
        }
    }
}
