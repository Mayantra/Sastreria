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

namespace LoginSasteria
{
    /// <summary>
    /// Lógica de interacción para ProcesoVenta.xaml
    /// </summary>
    public partial class ProcesoVenta : Window
    {
        ConexionDB cn = new ConexionDB();
        DataTable tabla =new DataTable();
        public static List<string> listacodigos;
        public static int IDCliente;
        public static int IDVendedor;
        public static int Regalo;
        public static Boolean existeCliente;
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
            IDCliente = data.getIDCliente();
            Regalo = data.getRegalo();
            existeCliente = data.getExistenciaCliente();
        }
        public void printTable()
        {
            DataDatos.DataContext = tabla;
        }

        public void getDatosCliente()
        {
            if (IDCliente>0)
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
                else
                {
                    setDatoscliente();
                    /*Obtener los datos del cliente, la función llama a todo lo necesario para crear el cliente*/
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
            if (txNombres.Text == null |txNombres.Text=="" | txApellidos.Text==null | txApellidos.Text=="")
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
                if(nit == ""|nit.Length<=0)
                {
                    nit = null;
                }
                crearcliente(nombreclient, apellidosCliente, telefonoclient, nit);//funcion para crear al cliente
                txNombres.Clear();
                txApellidos.Clear();
                txTelefono.Clear();
                txNit.Clear();

            }
        }
        void crearcliente(string nombres, string apellidos, Int32 telefono, string nit)
        {
            //creamos al cliente con el id maximo que encuentre
            int maxid = getMaxIDclient() + 1;
            try{
                cn.cerrarCN();
                string query = "INSERT INTO `dbleonv2`.`cliente` (`idCliente`, `Nombres`, `Apellidos`, `telefono`, `puntos`, `NIT`) " +
                    "VALUES ('"+maxid+"', '"+nombres+"', '"+ apellidos +"', '"+telefono+"', '0', '"+nit+"');";

                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();
                dr.Close();

            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
            


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
            int idDetalles=maxIdDetalles()+1;
            DateTime now = DateTime.Now;
            string fechahora = now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                cn.cerrarCN();
                string query = "INSERT INTO `dbleonv2`.`detallesventa` (`idDetallesVenta`, `FechaHora`, `Cliente_idCliente`, `Empleado_idEmpleado`, `regalo`) " +
                    "VALUES ('"+idDetalles+"', '"+fechahora+"', '"+idClient+"', '"+idEmpleado+"', '"+regalo+"');";

                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();
                dr.Close();

            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        void completarVenta(int idVenta, int Detalles, List<string> productos)
        {
            //Completar la venta
        }

        private void crearVenta(object sender, RoutedEventArgs e)
        {
            Logeo abrir = new Logeo();
            abrir.Show();
        }

        public void setVendedor(int id)
        {
            IDVendedor = id;
        }
    }
}
