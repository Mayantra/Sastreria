using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
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

namespace LoginSasteria
{
    /// <summary>
    /// Lógica de interacción para ProcesoVenta.xaml
    /// </summary>
    public partial class ProcesoVenta : Window
    {
        ConexionDB cn = new ConexionDB();
        DataTable tabla = new DataTable();
        public static List<string> listacodigos;
        public int IDCliente;
        public static int IDVendedor;
        public static int Regalo;
        public Boolean existeCliente;
        public static Boolean Log = false;

        ClientesVentas data = new ClientesVentas();
        public ProcesoVenta()
        {
            
            InitializeComponent();
            getData();
            printTable();
            getDatosCliente();
            getVendedor();
        }

        private void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        public void getData()
        {
            tabla = data.getTabla();

            listacodigos = data.getLista();            
            Regalo = data.getRegalo();
            IDCliente = data.getIDCliente();
            existeCliente = data.getExistenciaCliente();
            sumaTotal(listacodigos);
            
        }
        public void printTable()
        {
            DataDatos.DataContext = tabla;
        }
        public void setExistencia(Boolean existe)
        {
            existeCliente = existe;
        }
        public void sumaTotal(List<string> codigos)
        {
            double total = 0.00;
            try
            {
                
                
                for (int i = 0; i < codigos.Count; i++)
                {
                    cn.cerrarCN();
                    string query = "SELECT precio FROM dbleonv2.producto where idproducto='" + codigos[i] + "';";

                    MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                    MySqlDataReader dr = comando.ExecuteReader();
                    while (dr.Read())
                    {
                        total += dr.GetDouble(0);
                    }
                    dr.Close();

                }
                cn.cerrarCN();
                
            }
            catch(MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
            txTotal.Text = "Q "+total.ToString();
            
        }
        

        public void getDatosCliente()
        {
            
            if (existeCliente == true)
            {

                try
                {

                    string query = "SELECT * FROM dbleonv2.cliente where idCliente='" + IDCliente + "';";

                    MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                    MySqlDataReader dr = comando.ExecuteReader();

                    while (dr.Read())
                    {
                        txNombres.Text = dr.GetValue(1).ToString();
                        txApellidos.Text = dr.GetValue(2).ToString();
                        txTelefono.Text = dr.GetValue(3).ToString();
                        txNit.Text = dr.GetValue(5).ToString();

                    }
                }
                catch (MySqlException e)
                {
                    MessageBox.Show(e.ToString());
                }

                cn.cerrarCN();

            }
            else
            {
                MessageBox.Show("Recomendamos llenar los datos del cliente");
            }
        }

        void getVendedor()
        {
            leerPass id = new leerPass();
            IDVendedor = id.getIDuser();
        }
        public void setLog(Boolean acceso)
        {
            Log = acceso;
            venta();
        }

        void venta()
        {

            if (Log == true)
            {
                if (existeCliente == true)
                {
                    detallesVenta(IDCliente, IDVendedor, Regalo);
                }                
            }
            else
            {
                MessageBox.Show("algunas credenciales estan mal");
            }


        }

        int getMaxIDclient()
        {
            int maxid = 0;
            try
            {

                string query = "SELECT max(idCliente) FROM dbleonv2.cliente;";

                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();

                while (dr.Read())
                {
                    maxid = dr.GetInt32(0);
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
            cn.cerrarCN();

            return maxid;
        }

        void setDatoscliente()
        {
            if (txNombres.Text == null | txNombres.Text == "" | txApellidos.Text == null | txApellidos.Text == "")
            {
                MessageBox.Show("Debe de ingresar los nombres y apellidos del cliente");
            }
            else if (txTelefono.Text == "")
            {
                MessageBox.Show("Debe de ingresar un numero de celular");

            }
            else
            {
                string nombreclient = txNombres.Text;
                string apellidosCliente = txApellidos.Text;
                Int32 telefonoclient = Int32.Parse(txTelefono.Text);
                string nit = txNit.Text;
                if (nit == "" | nit.Length <= 0)
                {
                    nit = null;
                }

                crearcliente(nombreclient, apellidosCliente, telefonoclient, nit);//funcion para crear al cliente
                
            }
        }
        void crearcliente(string nombres, string apellidos, Int32 telefono, string nit)
        {
            //creamos al cliente con el id maximo que encuentre
            int maxid = getMaxIDclient() + 1;
            try
            {
                cn.cerrarCN();
                string query = "INSERT INTO `dbleonv2`.`cliente` (`idCliente`, `Nombres`, `Apellidos`, `telefono`, `puntos`, `NIT`) " +
                    "VALUES ('" + maxid + "', '" + nombres + "', '" + apellidos + "', '" + telefono + "', '0', '" + nit + "');";

                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();
                dr.Close();
                IDCliente = maxid;
                existeCliente = true;
                data.setExistencia(true);
                data.setIdcliente(maxid);
                crearVenta(null,null);
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
            cn.cerrarCN();
            
            


        }
        int maxIdDetalles()
        {
            int maxid = 0;
            try
            {

                string query = "SELECT max(idDetallesVenta) FROM dbleonv2.detallesventa;";

                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();

                while (dr.Read())
                {
                    maxid = dr.GetInt32(0);
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
            cn.cerrarCN();

            return maxid;
        }
        void detallesVenta(int idClient, int idEmpleado, int regalo)
        {
            int idDetalles = maxIdDetalles() + 1;
            DateTime now = DateTime.Now;
            string fechahora = now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                cn.cerrarCN();
                string query = "INSERT INTO `dbleonv2`.`detallesventa` (`idDetallesVenta`, `FechaHora`, `Cliente_idCliente`, `Empleado_idEmpleado`, `regalo`) " +
                    "VALUES ('" + idDetalles + "', '" + fechahora + "', '" + idClient + "', '" + idEmpleado + "', '" + regalo + "');";

                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();
                dr.Close();

            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
            cn.cerrarCN();
            completarVenta(idDetalles, listacodigos);

        }
        int maxIdVentas()
        {
            int maxid = 0;
            try
            {

                string query = "SELECT max(idRegistroVenta) FROM dbleonv2.registroventa;";

                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();

                while (dr.Read())
                {
                    maxid = dr.GetInt32(0);
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
            cn.cerrarCN();

            return maxid;
        }

        void completarVenta(int Detalles, List<string> productos)
        {
            //Completar la venta
            for (int i =0; i < productos.Count;i++)
            {
                cn.cerrarCN();
                int idVenta = maxIdVentas() + 1;
                
                try
                {
                    cn.cerrarCN();
                    string query = "INSERT INTO `dbleonv2`.`registroventa` (`idRegistroVenta`, `DetallesVenta_idDetallesVenta`, `producto_idproducto`) " +
                        "VALUES ('"+idVenta+"', '"+Detalles+"', '" + productos[i] +"');";

                    MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                    MySqlDataReader dr = comando.ExecuteReader();
                    dr.Close();

                }
                catch (MySqlException e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
            cn.cerrarCN();
            DeletFromInventario(listacodigos);
            MessageBox.Show("LOS PRODUCTOS HAN SIDO VENDIDOS");
            btnSalir(null,null);
        }
        public void DeletFromInventario(List<string> productos)
        {
            try
            {                
                for (int i = 0; i < productos.Count; i++)
                {
                    cn.cerrarCN();

                    try
                    {
                        cn.cerrarCN();
                        string query = "DELETE FROM `dbleonv2`.`inventario` " +
                            "WHERE (`producto_idproducto` = '" + productos[i] +"');";

                        MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                        MySqlDataReader dr = comando.ExecuteReader();
                        dr.Close();

                    }
                    catch (MySqlException e)
                    {
                        MessageBox.Show("Estos productos ya han sido vendidos"+e.ToString());
                    }
                }
                cn.cerrarCN();
            }
            catch (MySqlException e)
            {
                MessageBox.Show (e.ToString());
            }
        }

        private void crearVenta(object sender, RoutedEventArgs e)
        {
            if(existeCliente == false)
            {
                setDatoscliente();
            }
            else
            {
                Logeo abrir = new Logeo();
                cn.cerrarCN();
                abrir.Show();

            }

            
            
            
        }

        public void setVendedor(int id)
        {
            IDVendedor = id;
        }

        private void CancelarOp(object sender, RoutedEventArgs e)
        {
            ventaInventario abrir = new ventaInventario();
            abrir.Show();
            this.Close();
            
            
            
        }
    }
}
