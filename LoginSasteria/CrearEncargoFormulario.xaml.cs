using iText.IO.Util;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
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

namespace LoginSasteria
{
    /// <summary>
    /// Lógica de interacción para CrearEncargoFormulario.xaml
    /// </summary>
    public partial class CrearEncargoFormulario : Window
    {
        ConexionDB cn = new ConexionDB();

        List <string> codigosProductos = new List <string> ();//Alamecenar todos los codigos
        Boolean EstadoCliente = false;
        string user="";
        string pass="";
        int iduser=0;
        int idCliente = 0;
        public CrearEncargoFormulario()
        {
            InitializeComponent();
            tipoCon();
        }
        private static readonly Random rng = new Random();
        private void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        public static long GenerarNumeroAleatorio()
        {
            byte[] buf = new byte[8];
            rng.NextBytes(buf); // Llenamos el buffer con bytes aleatorios
            long longRand = BitConverter.ToInt64(buf, 0);

            return Math.Abs(longRand % 999999999999999999) + 1;
        }
        void tipoCon()
        {
            string query = "SELECT * FROM "+cn.namedb()+".tipoProducto;";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader myread;

                myread = comando.ExecuteReader();

                while (myread.Read())
                {

                    string nombres = myread.GetString("nombreTipo");
                    CbTipo.Items.Add(nombres);

                }
                cn.cerrarCN();
                
            }
            catch (MySqlException x)
            {
                MessageBox.Show("Error: " + x);
            }
        }
        private int ObtenerCantidad()
        {
            // Accede a la propiedad Value del control IntegerUpDown
            int cantidad = NumberData.Value ?? 0; // Usa 0 como valor predeterminado si el valor es nulo
            return cantidad;
        }
        private string ObtenerValorComboBox()
        {
            // Accede al valor seleccionado del ComboBox
            string valorSeleccionadoCompleto = CbTipo.SelectedItem?.ToString(); // Obtén la cadena completa seleccionada
            if (valorSeleccionadoCompleto != null)
            {
                // Divide la cadena en palabras
                if (valorSeleccionadoCompleto.Length >= 2)
                {
                    string firstTwoLetters = valorSeleccionadoCompleto.Substring(0, 2);
                    return firstTwoLetters;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                MessageBox.Show("No se ha seleccionado ningún elemento en el ComboBox.");
                return "";
            }
        }

        private async void AgregarProducto(object sender, RoutedEventArgs e)
        {
            int  cantidad = ObtenerCantidad();
            string tipo = ObtenerValorComboBox();
            MessageBox.Show("Agregaste "+cantidad.ToString()+" "+ tipo);
            progressBar.Visibility = Visibility.Visible;
            txtCargando.Visibility = Visibility.Visible;
            progressBar.Value = 50;
            await Task.Delay(500);
            AgregarCodigosLista(tipo, cantidad);

            for (int i = 0; i < cantidad; i++)
            {
                Console.WriteLine(codigosProductos[i]);
            }
            AgregarDataGrid(codigosProductos);
            progressBar.Value = 100;
            await Task.Delay(500);
            progressBar.Visibility = Visibility.Collapsed;
            txtCargando.Visibility = Visibility.Collapsed;


        }
        
        /// <summary>
        /// Agregar los codigos a lista
        /// </summary>
        /// <param name="tipo">El tipo de pero solamenta las dos primeras letras para codigo</param>
        /// <param name="cantidad">La cantidad de productos</param>
        public void AgregarCodigosLista(string tipo, int cantidad)
        {
            
            string CodigoProducto="";//variable que guarda los codigos temporalmente
            for (int i = 0; i < cantidad; i++)
            {              
                progressBar.Value = Math.Round((i / (double)cantidad * 100), 2);
                do
                {
                    long numeroAleatorio = GenerarNumeroAleatorio();
                    CodigoProducto = tipo + numeroAleatorio;
                }
                while (VerificarExistenciaEncargo(CodigoProducto) == false);

                codigosProductos.Add(CodigoProducto);
            }
            

        }
        public void AgregarDataGrid(List<string> ListaCodigos)
        {
            DataTable dataTable = new DataTable();

            // Agrega una columna al DataTable
            dataTable.Columns.Add("Codigos de Productos", typeof(string));

            // Agrega filas al DataTable basado en los elementos de la lista
            foreach (var item in ListaCodigos)
            {
                DataRow row = dataTable.NewRow();
                row["Codigos de Productos"] = item;
                dataTable.Rows.Add(row);
            }

            DataDatos.DataContext = dataTable;
        }
        

        private void CancelarFormulario(object sender, RoutedEventArgs e)
        {
            mainInventario abrir = new mainInventario();
            abrir.Show();
            this.Close();
        }
        //obtner texto del richtextbox
        private string GetTextFromRichTextBox(RichTextBox richTextBox)
        {
            TextRange textRange = new TextRange(
                richTextBox.Document.ContentStart,
                richTextBox.Document.ContentEnd
            );
            return textRange.Text;
        }
        //Generar Encargo
        public async void GenerarEncargo()
        {
            progressBar.Visibility = Visibility.Visible;
            txtCargando.Visibility = Visibility.Visible;
            progressBar.Value = 10;
            string CodigoEncargo = "";
            await Task.Delay(500);
            progressBar.Value = 40;
            do
            {
                CodigoEncargo = "EN" + GenerarNumeroAleatorio();

            } while (VerificarExistenciaEncargo(CodigoEncargo) == false);

            int idNombreProducto = getIDsTables("SELECT idnombreProducto FROM "+cn.namedb()+".nombreProducto " +
                "where Nombre ='Sin Especificar';");
            //-------------------------------------
            int idColor = getIDsTables("SELECT idcolor FROM " + cn.namedb() + ".color where nombre='Sin Especificar';");
            //--------------------------------------
            int idtipoTalla = getIDsTables("SELECT idtalla FROM " + cn.namedb() + ".tipoTall" +
                "\nINNER JOIN " + cn.namedb() + ".tipoProducto " +
                "\nON tipoProducto_idtipoProducto = idtipoProducto" +
                "\nwhere nombreTipo ='Encargo';");

            DateTime now = DateTime.Now;
            string fechahora = now.ToString("yyyy-MM-dd HH:mm:ss");

            await Task.Delay(500);
            progressBar.Value = 60;
            try
            {
                cn.cerrarCN();
                string query = "INSERT INTO `"+cn.namedb()+"`.`producto` " +
                    "(`idproducto`, `abono`, `precio`, `nombreProducto_idnombreProducto`, `color_idcolor`, `talla_idtalla`, `detalles`, `fechaCodigo`) " +
                    "VALUES ('"+CodigoEncargo+"', '"+txabono.Text+"', '"+txtotal.Text+"', " +
                    "'"+idNombreProducto+"', '"+idColor+"', '"+idtipoTalla+"', '"+GetTextFromRichTextBox(rtxDeatalles)+"', '"+fechahora+"');";

                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();
                dr.Close();
                
                cn.cerrarCN();
                string query2 = "INSERT INTO "+ cn.namedb() + ".`Encargo` " +
                    "(`idEncargo`, `Detalles`, `Empleado_idEmpleado`, `Cliente_idCliente`) " +
                    "VALUES ('"+CodigoEncargo+"', '"+GetTextFromRichTextBox(rtxDeatalles)+"', '"+iduser+"', '"+idCliente+"');";

                MySqlCommand comando2 = new MySqlCommand(query2, cn.establecerCN());
                MySqlDataReader dr2 = comando2.ExecuteReader();
                dr2.Close();
                cn.cerrarCN();
                await Task.Delay(500);
                progressBar.Value = 80;
                

            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                cn.cerrarCN();
            }
            await Task.Delay(500);

            progressBar.Value = 100;

            progressBar.Visibility = Visibility.Collapsed; // Ocultar ProgressBar
            txtCargando.Visibility = Visibility.Collapsed;
            //MessageBox.Show(CodigoEncargo);
            
        }
        //Verificaar Existencia del ID Encargo
        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigo">Codigo para verificar si los codigo se encuentran</param>
        /// <returns></returns>
        public Boolean VerificarExistenciaEncargo(string codigo)
        {
            string query = "SELECT idproducto FROM "+cn.namedb()+".producto where idproducto='"+codigo+"';";
            try
            {
                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader myread;

                myread = comando.ExecuteReader();
                string resultado="";
                while (myread.Read())
                {

                    resultado = myread.GetValue(0).ToString();

                }
                cn.cerrarCN();
                if (resultado == null || resultado =="")
                {
                    return true;

                }
                else
                {
                    return false;
                }
                

            }
            catch (MySqlException x)
            {                
                MessageBox.Show("Error: " + x);
                return false;
            }
        }
        //Obtener IDs para generar Encargo en Productos

        public int getIDsTables(string query)
        {
            int id = 0;
            try
            {               

                MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
                MySqlDataReader dr = comando.ExecuteReader();

                while (dr.Read())
                {
                    id = dr.GetInt32(0);
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
            cn.cerrarCN();

            return id;
        }
        private void CrearEncargo(object sender, RoutedEventArgs e)
        {
            string letras = GetTextFromRichTextBox(rtxDeatalles);
            if (letras =="\r\n")
            {
                letras = "";
            }
            int lenLetras = letras.Length;
            if (lenLetras >0)
            {
                if(DataDatos.Items.Count > 0)
                {
                    if (txtotal.Text=="" & txabono.Text=="")
                    {
                        MessageBox.Show("DEBE DE AGREGAR UN ABONO Y UN TOTAL A LAS CASILLAS");
                    }
                    else
                    {
                        if(EstadoCliente == true)
                        {
                            GridLogueo.Visibility = Visibility.Visible;//que se vea el logue si todo se cumple
                        }
                        else
                        {
                            MessageBox.Show("DEBE DE AGREGAR UN CLIENTE AL ENCARGO");
                        }
                        
                    }
                    
                }
                else
                {
                    MessageBox.Show("DEBE DE AGREGAR PRODUCTOS AL ENCARGO");
                }
                
            }
            else
            {                
                MessageBox.Show("ES NECESARIO QUE AGREGUE LOS DETALLES DEL ENCARGO");
            }
            
            
        }

        private void LeerSoloNumeros(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !OnlyNumeros(e.Text);
        }
        private static readonly Regex _regex = new Regex(@"[^0-9]+");
        private static bool OnlyNumeros(string text)
        {
            return !_regex.IsMatch(text);
        }

        public Boolean SerachClientDB(BigInteger numtel)
        {
            string query = "SELECT idCliente, Nombres, Apellidos FROM " + cn.namedb()+".Cliente where telefono='" + numtel + "';";
            MySqlCommand comando = new MySqlCommand(query, cn.establecerCN());
            MySqlDataReader reader = comando.ExecuteReader();

            string lectura = "";
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        idCliente = reader.GetUInt16(0);
                        lectura += reader.GetValue(i).ToString()+" ";
                    }
                }
                txNombreCliente.Text = lectura;
                reader.Close();
                cn.cerrarCN();
                return EstadoCliente = true;
            }
            else
            {
                MessageBox.Show("No se encontro un cliente con la información proporcionada");
                reader.Close();
                cn.cerrarCN();
                return EstadoCliente = false;
            }
            
        }
        private void BuscarCliente(object sender, RoutedEventArgs e)
        {
            BigInteger numTel = BigInteger.Parse(txBTelClient.Text);
            SerachClientDB(numTel);
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
                        cargarDatos();
                        txUser.Text = "";
                        PassBox.Password = "";
                        
                    }  
                }
            }
            
        }

        private void CancelarLogueo(object sender, RoutedEventArgs e)
        {
            GridLogueo.Visibility = Visibility.Collapsed;
            CrearEncargoFormulario abrir = new CrearEncargoFormulario();
            abrir.Show();
            this.Close();

        }
        private void lecturapass(object sender, RoutedEventArgs e)
        {
            getAcceso();
        }
        
        
        private void cargarDatos()
        {   
            GenerarEncargo();

            GridLogueo.Visibility = Visibility.Collapsed;
        }
    }
}
