using MySql.Data.MySqlClient;
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
using System.Windows.Shapes;

namespace LoginSasteria
{
    /// <summary>
    /// Lógica de interacción para rebibirEncargoInv.xaml
    /// </summary>
    public partial class rebibirEncargoInv : Window
    {
        ConexionDB cn = new ConexionDB();
        public rebibirEncargoInv()
        {
            InitializeComponent();
            ObtenerEmpleado();
            getInfo("SELECT nombre FROM " + cn.namedb() + ".almacen;", "Almacen");
            getInfo("SELECT Nombre FROM " + cn.namedb() + ".Proveedor;", "Proveedor");
        }
        private void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnCancelar_Click_1(object sender, RoutedEventArgs e)
        {
            mainInventario abrir = new mainInventario();
            abrir.Show();
            this.Close();
        }
        private void LeerCodigoBarras(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //--------------
                actualizarEncargo(txProducto.Text);
                txProducto.Clear();
            }
        }

        private void actualizarEncargo(string code)
        {
            if (txEmpleado.Text != "")
            {
                if (cbAlamcen.SelectedItem != null)
                {
                    if (cbProveedor.SelectedItem != null)
                    {
                        int verEstadoCodigo = verificarEstado("SELECT estadoEncargo FROM " + cn.namedb() + ".EncargoProducto where producto_idproducto='" + code + "';");
                        if (verEstadoCodigo == 0)
                        {
                            DateTime fechaActual = DateTime.Now;
                            string fecha = fechaActual.ToString("dddd/MMMM/yyyy HH:mm");
                            string nombreUsuario = txEmpleado.Text;
                            string nombreProveedor = cbProveedor.Text;
                            string nombreAlamacen = cbAlamcen.Text;
                            string estadoCadena = "El producto fue recibido por " + nombreUsuario + " y entregado por: " +
                                nombreProveedor + " el día " + fecha + " en la sucursal " + nombreAlamacen;

                            cambiarEstados("UPDATE `" + cn.namedb() + "`.`EncargoProducto` " +
                                "SET `estadoEncargo` = '1', `estadoDetalles` = '" + estadoCadena + "' " +
                                "WHERE (`producto_idproducto` = '" + code + "');");
                            agregarInventario(code);

                        }
                        else if (verEstadoCodigo == 2)
                        {
                            MessageBox.Show("Ingrese un codigo de producto correcto");
                        }
                        else
                        {
                            MessageBox.Show("El producto ya esta en la tienda");
                        }

                    }
                    else
                    {
                        MessageBox.Show("Debes de Seleccionar el Proveedor del Producto donde estará el producto");
                    }
                }
                else
                {
                    MessageBox.Show("Debes de Seleccionar el Almacen donde estará el producto");
                }
            }
            else
            {
                MessageBox.Show("Existe un error en el Usuario, no se puede recibir el producto");
            }

        }
        private void agregarInventario(string code)
        {
            string idEncargo = getIDEncargo(code);
            if (verificarProductos(idEncargo) == true)
            {
                int idAlmacen = getIDs("SELECT idalmacen FROM " + cn.namedb() + ".almacen where nombre='" + cbAlamcen.Text + "';");
                int idProveedor = getIDs("SELECT idProveedor FROM " + cn.namedb() + ".Proveedor where Nombre ='" + cbProveedor.Text + "';");
                int idUsuario = getIDs("SELECT idEmpleado FROM " + cn.namedb() + ".Empleado where Nombre = '" + txEmpleado.Text + "';");
                DateTime fechaActual = DateTime.Now;
                string fecha = fechaActual.ToString("yyyy-MM-dd HH:mm:ss");


                string query = "INSERT INTO `" + cn.namedb() + "`.`inventario` (`fechaIngreso`, `almacen_idalmacen`, `Empleado_idEmpleado`, `Proveedor_idProveedor`, `producto_idproducto`) " +
                    "VALUES ('" + fecha + "', '" + idAlmacen + "', '" + idUsuario + "', '" + idProveedor + "', '" + idEncargo + "');";
                try
                {

                    cn.cerrarCN();
                    MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                    MySqlDataReader reader = comando.ExecuteReader();

                    MessageBox.Show("EL ENCARGO HA SIDO COMPLETADO, ESTÁ LISTO PARA SU ENTREGA");
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    cn.cerrarCN();
                }

            }

        }
        private int getIDs(string query)
        {
            int dato = 0;
            try
            {
                cn.cerrarCN();
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    dato = reader.GetInt32(0);
                }
                return dato;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
                return dato;
            }
            finally
            {
                cn.cerrarCN();
            }

        }
        private string getIDEncargo(string code)
        {
            string IdEncargo = "";
            string query = "SELECT Encargo_idEncargo FROM hismanreina_PruebasDBLeonV2.EncargoProducto where producto_idproducto='" + code + "';";
            try
            {
                cn.cerrarCN();
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    IdEncargo = reader.GetString(0);
                }
                return IdEncargo;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
                return IdEncargo;
            }
            finally
            {
                cn.cerrarCN();
            }

        }
        private Boolean verificarProductos(string code)
        {
            string query = "SELECT estadoEncargo FROM " + cn.namedb() + ".EncargoProducto where Encargo_idEncargo='" + code + "';";
            int id = 0;
            Boolean dato = true;
            List<int> estados = new List<int>();
            try
            {
                cn.cerrarCN();
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    id = reader.GetInt32(0);
                    if (id == 0)
                    {
                        dato = false;
                    }
                }
                return dato;


            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
                return dato;
            }
            finally
            {
                cn.cerrarCN();
            }
        }
        private int verificarEstado(string query)
        {
            try
            {
                cn.cerrarCN();
                int dato = 2;
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    dato = reader.GetInt32(0);
                }
                if (dato == 1)
                {
                    return 1;
                }
                else if (!reader.HasRows)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }


            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
                return 1;
            }
            finally
            {
                cn.cerrarCN();
            }
        }

        private void cambiarEstados(string query)
        {
            //Caambiar el estado de 0 a 1 del producto y agregar sus detalles
            try
            {
                cn.cerrarCN();
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader reader = comando.ExecuteReader();
                MessageBox.Show("PRODUCTO AGREGADO");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                cn.cerrarCN();
            }
        }
        private void ObtenerEmpleado()
        {
            string nombreUsuario = "";
            leerPass usuarios = new leerPass();
            int id = usuarios.getIDuser();
            try
            {
                if (id > 0)
                {
                    string query = "SELECT Nombre FROM " + cn.namedb() + ".Empleado where idEmpleado = '" + id + "';";
                    cn.cerrarCN();
                    MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                    MySqlDataReader reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        nombreUsuario = reader.GetString(0);
                    }
                }
                txEmpleado.Text = nombreUsuario;

            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                cn.cerrarCN();
            }



        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param> se ingresa el query correspondiente para el combo box
        /// <param name="tipoCb"></param> Se define que tipo de combo es 
        void getInfo(string query, string tipoCb)
        {
            try
            {
                cn.cerrarCN();
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader myread;

                myread = comando.ExecuteReader();

                while (myread.Read())
                {

                    string nombres = myread.GetString(0);
                    if (tipoCb == "Almacen")
                    {
                        cbAlamcen.Items.Add(nombres);
                    }
                    else
                    {
                        cbProveedor.Items.Add(nombres);
                    }

                }

            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
            finally { cn.cerrarCN(); }
        }

        private void cerrarSesion(object sender, RoutedEventArgs e)
        {
            MainWindow abrir = new MainWindow();
            abrir.Show();
            this.Close();
        }
    }
}
