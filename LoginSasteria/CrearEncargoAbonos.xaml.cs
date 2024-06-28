using iTextSharp.text.xml;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZXing;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace LoginSasteria
{
    /// <summary>
    /// Lógica de interacción para CrearEncargoAbonos.xaml
    /// </summary>
    public partial class CrearEncargoAbonos : Window
    {
        ConexionDB cn = new ConexionDB();
        DataTable tabla = new DataTable();
        string Detalles="";
        string FacturaCode = "";
        int idEmpleado = 0;
        int idcliente = 0;
        double AbonoEncargo =0.00;
        double TotalEncargo = 0.00;
        string fechaEncargo = "";
        List<string> ListaProdcutos = new List<string>();

        public CrearEncargoAbonos()
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
                FacturaCode = txCodigoFactura.Text;
                ListaProdcutos.Clear();
                GenearGrid(FacturaCode);
                idEmpleado = getIds("SELECT Empleado_idEmpleado FROM "+cn.namedb()+".Encargo where idEncargo='"+FacturaCode+"';");
                idcliente = getIds("SELECT Cliente_idCliente FROM " + cn.namedb() + ".Encargo where idEncargo='" + FacturaCode + "';");
                getListaProductos(FacturaCode);
                txCodigoFactura.Clear();
            }
        }
        private void getListaProductos(string code)
        {
            string query = "SELECT producto_idproducto As Productos FROM "+cn.namedb()+".EncargoProducto where Encargo_idEncargo = '"+code+"';";
            try
            {
                cn.cerrarCN();
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();

                while (dr.Read())
                {
                    // Obtener el nombre del producto
                    string producto = dr["Productos"].ToString();

                    // Agregar el producto a la lista
                    ListaProdcutos.Add(producto);
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
            cn.cerrarCN();

        }
        
        private int getIds(string query)
        {
            int dato=0;
            try
            {
                cn.cerrarCN();
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();

                while (dr.Read())
                {
                    dato = dr.GetInt32(0);
                }
                dr.Close();
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
            cn.cerrarCN();

            return dato;
        }

        private void GenearGrid(string code)
        {
            tabla.Clear();
            if (code == null | code == "")
            {
                MessageBox.Show("Ingrese un codigo de producto");
            }
            else
            {
                string query = "SELECT producto_idproducto As Productos FROM "+cn.namedb()+".EncargoProducto where Encargo_idEncargo = '"+code+"';";
                try
                {
                    cn.cerrarCN();
                    MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                    MySqlDataAdapter data = new MySqlDataAdapter(comando);

                   

                    data.Fill(tabla);
                    if (tabla.Rows.Count < 1)//si el codigo es incorrecto
                    {
                        MessageBox.Show("Ingrese un código de producto correcto");

                    }
                    else
                    {   
                        DataDatos.DataContext = tabla;      
                    }
                    cn.cerrarCN();
                }
                catch (MySqlException x)
                {
                    MessageBox.Show("Error: " + x);
                }
            }
            getDetalles(code);

        }
        private void getDetalles(string code)
        {
            
            string query = "SELECT Detalles FROM "+cn.namedb()+".Encargo where idEncargo='"+code+"';";
            try
            {
                cn.cerrarCN();
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());                
                MySqlDataReader dr = comando.ExecuteReader();

                while (dr.Read())
                {
                    Detalles = dr.GetValue(0).ToString(); 
                }
                dr.Close();
                cn.cerrarCN();
                
                Paragraph parrafo = new Paragraph(new Run(Detalles));

                rtxDetalles.Document.Blocks.Clear(); // Limpiar cualquier contenido existente
                rtxDetalles.Document.Blocks.Add(parrafo);
            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
            getAbonoTotalFecha(code);

        }
        private void getAbonoTotalFecha(string code)
        {
            string query = "SELECT abono, precio, fechaCodigo FROM "+cn.namedb()+".producto " +
                "where idproducto = '"+code+"';";
            try
            {
                cn.cerrarCN();
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();

                while (dr.Read())
                {
                    AbonoEncargo = dr.GetDouble(0);
                    TotalEncargo = dr.GetDouble(1);
                    fechaEncargo = dr.GetValue(2).ToString();
                }
                dr.Close();
                cn.cerrarCN();
                txAbono.Text = AbonoEncargo.ToString();
                txTotal.Text = TotalEncargo.ToString();
                lbFechas.Content = "Fecha: "+fechaEncargo;

            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }

        }

        private void ActualizarEncargo(object sender, RoutedEventArgs e)
        {
            double abonoactualizado = double.Parse(txAbono.Text);
            if (abonoactualizado > AbonoEncargo & abonoactualizado <= TotalEncargo)
            {
                GenerarActualizacion(FacturaCode, abonoactualizado);
            }
            else
            {
                MessageBox.Show("El abono debe de ser mayor al anterior y menor al total");
            }
        }
        private void GenerarActualizacion(string code, double abonoNuevo)
        {
            DateTime now = DateTime.Now;
            string fechahora = now.ToString("yyyy-MM-dd HH:mm:ss");
            string DetallesNuevos = (Detalles + " Se hizo un abono de Q "+ abonoNuevo+ " Sobre el total Q " + TotalEncargo+
                " El día "+ fechahora);

            string queryEncargo = "UPDATE `"+cn.namedb()+"`.`Encargo` SET `Detalles` = '"+DetallesNuevos+"' " +
                "WHERE (`idEncargo` = '"+code+"');";
            string queryProducto = "UPDATE `"+cn.namedb()+"`.`producto` SET `abono` = '"+abonoNuevo+"', `detalles` = '"+DetallesNuevos+"' " +
                "WHERE (`idproducto` = '"+code+"');";
            try
            {
                cn.cerrarCN();
                MySqlCommand comando = new MySqlCommand(queryEncargo, cn.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();
                cn.cerrarCN();
                dr.Close();

                MySqlCommand comando2 = new MySqlCommand(queryProducto, cn.establecerCN());
                MySqlDataReader dr2 = comando2.ExecuteReader();
                cn.cerrarCN();
                dr2.Close();

                FacturaEncargoProductos factura = new FacturaEncargoProductos();
                string infoEmpleado = "Atendido por:\n" + getDataConsultas("SELECT Nombre " +
                    "FROM " + cn.namedb() + ".Empleado where idEmpleado='" + idEmpleado + "';");
                string infoCliente = DataCliente(idcliente);
                string TextoDetalles = "Av 12 Zona 1 \n San Pedro Sac.\n Tel: 55887766";
                factura.CrearFactura(code, infoEmpleado, infoCliente, TextoDetalles,
                    "Total: " + txTotal.Text, tabla, ListaProdcutos, "Abono: " + abonoNuevo, DetallesNuevos);

                if(TotalEncargo == abonoNuevo)
                {
                    ventaInventario abrir = new ventaInventario();
                    abrir.Show();
                    this.Close();
                }
                else
                {
                    mainInventario abrir = new mainInventario();
                    abrir.Show();
                    this.Close();
                }


            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
            cn.cerrarCN();

        }
        public string getDataConsultas(string query)
        {
            string Datos = "";
            try
            {
                cn.cerrarCN();
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();

                while (dr.Read())
                {
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        Datos += dr.GetValue(i).ToString() + " ";
                    }
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
            cn.cerrarCN();
            return Datos;
        }
        public string DataCliente(int id)
        {
            string nombre = "";
            string apellido = "";
            int puntos = 0;
            string nit = "";
            try
            {

                string query = "SELECT Nombres, Apellidos, puntos, NIT " +
                    "FROM " + cn.namedb() + ".Cliente where idCliente='" + id + "';";
                cn.cerrarCN();
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();

                while (dr.Read())
                {

                    nombre = dr.GetString(0).ToString();
                    apellido = dr.GetString(1).ToString();
                    puntos = dr.GetInt16(2);
                    nit = dr.GetString(3).ToString();

                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
            cn.cerrarCN();
            return "Cliente:" + nombre + " " + apellido + "\nNIT: " + nit + "\nPuntos: " + puntos;

        }
    }
}

