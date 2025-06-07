using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Numerics;
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
        List<string> ListaCodigosFactura = new List<string>();
        List<string> codigosDevolver = new List<string>();
        string AuxcodigoFactura = "";
        double totalFactura = 0;
        double totalNuevo = 0;
        string idfactura = "";
        static int idClienteid = 0;
        static int idClientes = 0;
        string user = "";
        string pass = "";
        int iduser = 0;
        DataTable tablaNueva = new DataTable();

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

        private void getFactura()
        {
            string codigo = txFactura.Text;
            if (codigo == null | codigo == "")
            {
                MessageBox.Show("Ingrese un Código");
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
            DateTime fechaFactura = DateTime.Now;
            objConection.cerrarCN();
            string query = "SELECT FechaHora FROM " + objConection.namedb() + ".DetallesVenta where idDetallesVenta='" + codigo + "';";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    fechaFactura = reader.GetDateTime(0);
                }
                reader.Close();
                objConection.cerrarCN();
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
                "\r\ntipoProducto.nombreTipo as 'Tipo'," +
                "\r\ntalla.nombreTalla As 'Talla',\r\nprecio as 'Precio'," +
                "\r\nFechaHora as 'Fecha',\r\nEmpleado.Nombre as 'Nombre Cajero'" +
                "\r\n \r\nFROM " + objConection.namedb() + ".RegistroVenta\r\ninner join " + objConection.namedb() + ".DetallesVenta" +
                "\r\non DetallesVenta_idDetallesVenta=idDetallesVenta\r\ninner join " + objConection.namedb() + ".producto" +
                "\r\non producto_idproducto = idproducto\r\nINNER JOIN " + objConection.namedb() + ".tipoTall" +
                "\r\nON producto.talla_idtalla = tipoTall.idtalla\r\nINNER JOIN " + objConection.namedb() + ".tipoProducto" +
                "\r\nON tipoTall.tipoProducto_idtipoProducto = tipoProducto.idtipoProducto" +
                "\r\nINNER JOIN  " + objConection.namedb() + ".talla\r\nON tipoTall.talla_idtalla = talla.idtalla" +
                "\r\ninner join " + objConection.namedb() + ".Empleado\r\non Empleado_idEmpleado = idEmpleado" +
                "\r\n\r\nwhere idDetallesVenta ='" + codigoFactura + "';";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataAdapter data = new MySqlDataAdapter(comando);
                DataTable tabla = new DataTable();

                data.Fill(tabla);
                if (tabla.Rows.Count < 1)//si el codigo es incorrecto
                {
                    MessageBox.Show("Ingrese el código de factura correcto");

                }
                else
                {

                    DataConsulta.DataContext = tabla;
                    objConection.cerrarCN();
                    tablaNueva = tabla;
                    AuxcodigoFactura = codigoFactura;
                    getListaCodigos(codigoFactura);
                    CargarTotalFacturas(codigoFactura);

                }
                objConection.cerrarCN();
            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
        }
        public void getListaCodigos(string codigo)
        {
            string query = "SELECT producto_idproducto FROM " + objConection.namedb() + ".RegistroVenta " +
                "where DetallesVenta_idDetallesVenta='" + codigo + "' order by producto_idproducto asc;";
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string codes = reader[0].ToString();
                    ListaCodigosFactura.Add(codes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            foreach (string cod in ListaCodigosFactura)
            {
                Console.WriteLine(cod);
            }
        }

        public void CargarTotalFacturas(string codigoFactura)
        {

            objConection.cerrarCN();
            string query = "SELECT  sum(precio) as Total" +
                "\r\n \r\nFROM " + objConection.namedb() + ".RegistroVenta\r\ninner join " + objConection.namedb() + ".DetallesVenta" +
                "\r\non DetallesVenta_idDetallesVenta=idDetallesVenta\r\ninner join " + objConection.namedb() + ".producto" +
                "\r\non producto_idproducto = idproducto\r\nINNER JOIN " + objConection.namedb() + ".tipoTall" +
                "\r\nON producto.talla_idtalla = tipoTall.idtalla\r\nINNER JOIN " + objConection.namedb() + ".tipoProducto" +
                "\r\nON tipoTall.tipoProducto_idtipoProducto = tipoProducto.idtipoProducto" +
                "\r\nINNER JOIN  " + objConection.namedb() + ".talla\r\nON tipoTall.talla_idtalla = talla.idtalla" +
                "\r\ninner join " + objConection.namedb() + ".Empleado\r\non Empleado_idEmpleado = idEmpleado" +
                "\r\n\r\nwhere idDetallesVenta ='" + codigoFactura + "';";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    totalFactura = reader.GetDouble(0);
                    txTotalFac.Text = "Q " + totalFactura;
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

        private void btnCancelar_Click_1(object sender, RoutedEventArgs e)
        {
            consultaInventario abrir = new consultaInventario();
            abrir.Show();
            this.Close();
        }

        private void LeerCodigoBarras(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txTotalNuevo.Text = "Q 00.00";
                ListaCodigosFactura.Clear();
                codigosDevolver.Clear();

                getFactura();
                idfactura = txFactura.Text;
                idClienteid = getIdCliente(idfactura, "SELECT Cliente_idCliente FROM " + objConection.namedb() + ".DetallesVenta " +
                    "\r\nwhere idDetallesVenta ='" + idfactura + "';");
                idClientes = getIdCliente(idfactura, "SELECT Cliente.telefono FROM " + objConection.namedb() + ".DetallesVenta " +
                    "\r\ninner join " + objConection.namedb() + ".Cliente on DetallesVenta.Cliente_idCliente = Cliente.idCliente" +
                    "\r\nwhere idDetallesVenta ='" + idfactura + "';");

                txFactura.Clear();

            }
        }

        private void QuitarProductoFactura(object sender, RoutedEventArgs e)
        {
            int indice = -1;
            indice = DataConsulta.SelectedIndex;

            if (indice > -1)
            {
                tablaNueva.Rows.RemoveAt(indice);
                DataConsulta.DataContext = tablaNueva;
                codigosDevolver.Add(ListaCodigosFactura[indice]);
                foreach (var item in codigosDevolver)
                {
                    Console.WriteLine("///" + item);
                }

                ListaCodigosFactura.RemoveAt(indice);
                foreach (string cod in ListaCodigosFactura)
                {
                    Console.WriteLine("--" + cod);
                }
                sumaDeProductos(ListaCodigosFactura);
                txProducto.IsEnabled = true;
            }


        }
        private void sumaDeProductos(List<string> productosToSum)
        {
            double total = 0.00;
            try
            {
                for (int i = 0; i < productosToSum.Count; i++)
                {
                    objConection.cerrarCN();
                    string query = "SELECT precio FROM " + objConection.namedb() + ".producto where idproducto='" + productosToSum[i] + "';";

                    MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                    MySqlDataReader dr = comando.ExecuteReader();
                    while (dr.Read())
                    {
                        total += dr.GetDouble(0);
                    }
                    dr.Close();

                }
                objConection.cerrarCN();

            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
            txTotalNuevo.Text = "Q " + total.ToString();
            totalNuevo = total;
            Console.WriteLine(totalNuevo);
        }
        private int getIdCliente(string datoFactura, string query)
        {
            int ids = 0;
            try
            {

                MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();

                while (dr.Read())
                {
                    ids = dr.GetInt32(0); // Asigna el valor real si no es NULL                   
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
            objConection.cerrarCN();

            return ids;
        }

        private void getDevolucion(object sender, RoutedEventArgs e)
        {
            if (totalNuevo >= totalFactura && totalFactura > 0)
            {
                //proceso de decolucion
                GridLogueo.Visibility = Visibility.Visible;

            }
            else
            {
                MessageBox.Show("El valor Total debe de ser MAYOR O IGUAL al total de la factura para el cambio");
            }
        }
        private void iniciarProceso()
        {
            borrarVenta(idfactura);
            detallesVenta(idClienteid, iduser, 0);
            DevolucionRegalo abrir = new DevolucionRegalo();
            abrir.Show();
            this.Close();
        }
        private void CancelarLogueo(object sender, RoutedEventArgs e)
        {
            GridLogueo.Visibility = Visibility.Collapsed;
            DevolucionRegalo abrir = new DevolucionRegalo();
            abrir.Show();
            this.Close();

        }
        private void lecturapass(object sender, RoutedEventArgs e)
        {
            getAcceso();
        }
        private void getAcceso()
        {
            leerPass read = new leerPass();
            for (int i = 0; i <= PassBox.Password.Length; i++)
            {
                if (i == 5)
                {
                    user = txUser.Text;
                    pass = PassBox.Password;
                    if (read.getAcceso(user, pass) != false)
                    {


                        iduser = read.getIdLogUser(user);
                        GridLogueo.Visibility = Visibility.Collapsed;
                        iniciarProceso();
                        txUser.Text = "";
                        PassBox.Password = "";

                    }
                    txUser.Text = "";
                    PassBox.Password = "";
                }
            }

        }

        private void AgregarProducto(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                InsertarProductoData(txProducto.Text);

                txProducto.Clear();
                sumaDeProductos(ListaCodigosFactura);
                foreach (var item in ListaCodigosFactura)
                {
                    Console.WriteLine(item);
                }
            }
            e.Handled = true;
            
        }


        private void InsertarProductoData(string code)
        {
            if (code == null | code == "")
            {
                MessageBox.Show("Ingrese un codigo de producto");
            }
            else
            {
                
                Boolean existe = false;
                for (int z = 0; z < ListaCodigosFactura.Count; z++)
                {
                    if (ListaCodigosFactura[z].Contains(code))
                    {
                        existe = true;
                    }
                }
                if (existe == false)
                {
                    DateTime now = DateTime.Now;
                    string fechahora = now.ToString("yyyy-MM-dd HH:mm:ss");
                    string query = "SELECT producto_idproducto as Producto," +
                        "\r\ntipoProducto.nombreTipo AS Tipo," +
                        "\r\ntalla.nombreTalla As Talla," +
                        "\r\nproducto.precio As Precio," +
                        "\r\n'" + fechahora + "' As Fecha," +
                        "\r\n'Devolución' As 'Nombre Cajero'" +
                        "\r\nFROM " + objConection.namedb() + ".inventario" +
                        "\r\ninner join "+objConection.namedb()+".producto on inventario.producto_idproducto = producto.idproducto" +
                        "\r\ninner join " + objConection.namedb() + ".tipoTall on producto.talla_idtalla = tipoTall.idtalla" +
                        "\r\ninner join " + objConection.namedb() + ".tipoProducto " +
                        "\r\non tipoTall.tipoProducto_idtipoProducto = tipoProducto.idtipoProducto" +
                        "\r\ninner join " + objConection.namedb() + ".talla on tipoTall.talla_idtalla = talla.idtalla" +
                        "\r\nwhere idproducto = '" + code + "';";
                    MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                    try
                    {
                        
                        MySqlDataAdapter data = new MySqlDataAdapter(comando);

                        //la fumanda mas grande del codigo creo XD

                        data.Fill(tablaNueva);
                        if (tablaNueva.Rows.Count < 1)//si el codigo es incorrecto
                        {
                            MessageBox.Show("Ingrese un código de producto correcto");

                        }
                        else
                        {
                            if (DataConsulta.Items.Count == 0)//verificar si el datagrid tiene algo
                            {

                                DataConsulta.DataContext = tablaNueva;
                                addlist(code);
                            }
                            else
                            {

                                //obtener datos de filas y columnas de datagrid y datatable

                                List<string> fila = new List<string>();
                                fila.Clear();
                                List<string> nombrecolumna = new List<string>();
                                if (tablaNueva.Columns.Count > 0)
                                {
                                    foreach (DataColumn columna in tablaNueva.Columns)
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
                                    firstRow = tablaNueva.NewRow();//crear una nueva fila

                                    int j = 0;
                                    foreach (string dato in nombrecolumna)//Asignar las celdas en cada columna
                                    {
                                        firstRow[dato] = fila[j];
                                        j++;
                                    }

                                    DataConsulta.DataContext = tablaNueva;
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
        }

        public void addlist(string codigosList)
        {
            ListaCodigosFactura.Add(codigosList);
        }

        private int getMax(string query)
        {
            int ids = 0;

            try
            {


                MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();

                while (dr.Read())
                {
                    if (dr.IsDBNull(0)) // Verifica si el valor en la columna 0 es NULL
                    {
                        ids = 1; // Asigna 1 si es NULL
                    }
                    else
                    {
                        ids = dr.GetInt32(0); // Asigna el valor real si no es NULL
                    }
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
            objConection.cerrarCN();

            return ids;
        }
        private void borrarVenta(string codVenta)
        {
            string queryRegistro = "DELETE FROM `" + objConection.namedb() + "`.`RegistroVenta` " +
                "WHERE (`DetallesVenta_idDetallesVenta` = '" + codVenta + "');";
            string queryDetalles = "DELETE FROM `" + objConection.namedb() + "`.`DetallesVenta` " +
                "WHERE (`idDetallesVenta` = '" + codVenta + "');";
            try
            {
                objConection.cerrarCN();
                MySqlCommand comando = new MySqlCommand(queryRegistro, objConection.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();
                dr.Close();
                objConection.cerrarCN();
                MySqlCommand comando2 = new MySqlCommand(queryDetalles, objConection.establecerCN());
                MySqlDataReader dr2 = comando2.ExecuteReader();
                dr2.Close();
                objConection.cerrarCN();

            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        void detallesVenta(int idClient, int idEmpleado, int regalo)//agregar datos de los detalles de la venta
        {
            int idDetalles = getMax("SELECT max(idDetallesVenta) FROM " + objConection.namedb() + ".DetallesVenta;") + 1;
            DateTime now = DateTime.Now;
            string fechahora = now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                objConection.cerrarCN();
                string query = "INSERT INTO `" + objConection.namedb() + "`.`DetallesVenta` (`idDetallesVenta`, `FechaHora`, `Cliente_idCliente`, `Empleado_idEmpleado`, `regalo`) " +
                    "VALUES ('" + idDetalles + "', '" + fechahora + "', '" + idClient + "', '" + idEmpleado + "', '" + regalo + "');";

                MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();
                dr.Close();

            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
            objConection.cerrarCN();
            completarVenta(idDetalles, ListaCodigosFactura);

        }
        void completarVenta(int Detalles, List<string> productos)
        {
            //Completar la venta
            int idVenta = 0;
            for (int i = 0; i < productos.Count; i++)
            {
                objConection.cerrarCN();
                idVenta = getMax("SELECT max(idRegistroVenta) FROM " + objConection.namedb() + ".RegistroVenta;") + 1;

                try
                {
                    objConection.cerrarCN();
                    string query = "INSERT INTO `" + objConection.namedb() + "`.`RegistroVenta` (`idRegistroVenta`, `DetallesVenta_idDetallesVenta`, `producto_idproducto`) " +
                        "VALUES ('" + idVenta + "', '" + Detalles + "', '" + productos[i] + "');";

                    MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                    MySqlDataReader dr = comando.ExecuteReader();
                    dr.Close();

                }
                catch (MySqlException e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
            objConection.cerrarCN();

            DeletFromInventario(ListaCodigosFactura);
            devolverProducto(codigosDevolver);
            //creacion de funcion suma puntos
            MessageBox.Show("Devolución Realizada");
            GenerarFactura factura = new GenerarFactura();
            //GENERAR la FACTURA -------------------------------------------------------------------------*************
            factura.GenerarDatosFactura(Detalles.ToString(), iduser, idClientes, tablaNueva, totalNuevo, 1);
            //------------------------------------------------------------------------------------------------------


        }
        private void devolverProducto(List<string> productosDevolver)
        {
            DateTime now = DateTime.Now;
            string fechahora = now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                for (int i = 0; i < productosDevolver.Count; i++)
                {
                    objConection.cerrarCN();
                    if (verificarInv(productosDevolver[i]) == false)
                    {
                        try
                        {
                            objConection.cerrarCN();
                            string query = "INSERT INTO `" + objConection.namedb() + "`.`inventario` " +
                                "(`fechaIngreso`, `almacen_idalmacen`, `Empleado_idEmpleado`, `Proveedor_idProveedor`, `producto_idproducto`) " +
                                "VALUES ('" + fechahora + "', '1', '" + iduser + "', '1', '" + productosDevolver[i] + "');";

                            MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                            MySqlDataReader dr = comando.ExecuteReader();
                            dr.Close();
                            objConection.cerrarCN();

                        }
                        catch (MySqlException e)
                        {
                            MessageBox.Show("Estos productos ya han sido vendidos" + e.ToString());
                        }
                    }


                }
                objConection.cerrarCN();
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public Boolean verificarInv(string productoInv)
        {
            Boolean estado = false;
            string query = "SELECT producto_idproducto FROM " + objConection.namedb() + ".inventario where producto_idproducto='" + productoInv + "';";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                MySqlDataReader myread;

                myread = comando.ExecuteReader();
                string resultado = "";
                while (myread.Read())
                {

                    resultado = myread.GetValue(0).ToString();

                }
                objConection.cerrarCN();
                if (resultado == null || resultado == "")
                {
                    estado = false;
                    return estado;

                }
                else
                {
                    estado = true;
                    return estado;
                }


            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
                return estado;
            }
        }
        public void DeletFromInventario(List<string> productos)//eliminar los productos del inventario
        {
            try
            {
                for (int i = 0; i < productos.Count; i++)
                {
                    objConection.cerrarCN();
                    if (verificarInv(productos[i]) == true)
                    {
                        try
                        {
                            objConection.cerrarCN();
                            string query = "DELETE FROM `" + objConection.namedb() + "`.`inventario` " +
                                "WHERE (`producto_idproducto` = '" + productos[i] + "');";

                            MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN());
                            MySqlDataReader dr = comando.ExecuteReader();
                            dr.Close();
                            objConection.cerrarCN();

                        }
                        catch (MySqlException e)
                        {
                            MessageBox.Show("Estos productos ya han sido vendidos" + e.ToString());
                        }
                    }


                }
                objConection.cerrarCN();
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
