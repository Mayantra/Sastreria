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
        ConexionDB cn = new ConexionDB();
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
            CargarFactura(codigo);
            CargarTotalFacturas(codigo);
        }
        public void CargarFactura(string codigoFactura)
        {
            cn.cerrarCN();
            string query = "SELECT producto_idproducto as 'Producto'," +
                "\r\ntipoproducto.nombreTipo as 'Tipo'," +
                "\r\ntalla.nombreTalla As 'Talla',\r\nprecio as 'Precio'," +
                "\r\nFechaHora as 'Fecha',\r\nempleado.Nombre as 'Nombre Cajero'" +
                "\r\n \r\nFROM dbleonv2.registroventa\r\ninner join dbleonv2.detallesventa" +
                "\r\non DetallesVenta_idDetallesVenta=idDetallesVenta\r\ninner join dbleonv2.producto" +
                "\r\non producto_idproducto = idproducto\r\nINNER JOIN dbleonv2.tipotall" +
                "\r\nON producto.talla_idtalla = tipotall.idtalla\r\nINNER JOIN dbleonv2.tipoproducto" +
                "\r\nON tipotall.tipoProducto_idtipoProducto = tipoproducto.idtipoProducto" +
                "\r\nINNER JOIN  dbleonv2.talla\r\nON tipotall.talla_idtalla = talla.idtalla" +
                "\r\ninner join dbleonv2.empleado\r\non Empleado_idEmpleado = idEmpleado" +
                "\r\n\r\nwhere idDetallesVenta ='"+codigoFactura+"';";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataAdapter data = new MySqlDataAdapter(comando);
                DataTable tabla = new DataTable();

                data.Fill(tabla);

                DataConsulta.DataContext = tabla;
                cn.cerrarCN();
            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
        }

        public void CargarTotalFacturas(string codigoFactura)
        {
            cn.cerrarCN();
            string query = "SELECT  sum(precio) as Total" +
                "\r\n \r\nFROM dbleonv2.registroventa\r\ninner join dbleonv2.detallesventa" +
                "\r\non DetallesVenta_idDetallesVenta=idDetallesVenta\r\ninner join dbleonv2.producto" +
                "\r\non producto_idproducto = idproducto\r\nINNER JOIN dbleonv2.tipotall" +
                "\r\nON producto.talla_idtalla = tipotall.idtalla\r\nINNER JOIN dbleonv2.tipoproducto" +
                "\r\nON tipotall.tipoProducto_idtipoProducto = tipoproducto.idtipoProducto" +
                "\r\nINNER JOIN  dbleonv2.talla\r\nON tipotall.talla_idtalla = talla.idtalla" +
                "\r\ninner join dbleonv2.empleado\r\non Empleado_idEmpleado = idEmpleado" +
                "\r\n\r\nwhere idDetallesVenta ='" + codigoFactura + "';";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    txTotalFac.Text = "Q " + reader.GetDouble(0).ToString();
                }
                reader.Close();
                cn.cerrarCN();
            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
        }
    }
}
