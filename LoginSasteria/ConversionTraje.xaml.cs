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
using MySql.Data.MySqlClient;

namespace LoginSasteria
{
    /// <summary>
    /// Lógica de interacción para ConversionTraje.xaml
    /// </summary>
    public partial class ConversionTraje : Window
    {
        ConexionDB objConection = new ConexionDB();

        public ConversionTraje()
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

        private void Limpiar()
        {
            txtArtiCambiar.Clear();
            txtArtiCambio.Clear();
            txtArtiCambio.IsEnabled = false;
        }

        private void btnCancelar_Click_1(object sender, RoutedEventArgs e)
        {
            Limpiar();
            consultaInventario abrirMenu = new consultaInventario();
            abrirMenu.Show();
            this.Close();
        }

        private void ActivartxtArtiCambio(object sender, KeyEventArgs e)
        {
            txtArtiCambio.IsEnabled = true;
            //txtArtiCambio.Focus();
        }

        private void btnIntercambiar_Click(object sender, RoutedEventArgs e)
        {
            string nuevoCodigo = txtArtiCambio.Text;
            string codigoOriginal = txtArtiCambiar.Text;

            //Se ejecuta la accion de realizar el cambio de articulo en el traje

            // Verificar si los códigos son iguales
            if (nuevoCodigo == codigoOriginal)
            {
                MessageBox.Show("El código del artículo a cambiar y el nuevo código no pueden ser iguales o estar vacios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Limpiar();
                return;
            }

            //Verificar si el nuevo producto existe en la tabla producto
            string verificarExistenciaProductoQuery = "SELECT COUNT(*) FROM " + objConection.namedb() + ".producto WHERE idproducto = @nuevoCodigo";
            int productoExiste;

            using (MySqlCommand conexion5 = new MySqlCommand(verificarExistenciaProductoQuery, objConection.establecerCN()))
            {
                try
                {
                    conexion5.Parameters.AddWithValue("@nuevoCodigo", nuevoCodigo);

                    // Depuración: Verificar el valor de nuevoCodigo
                    MessageBox.Show($"Verificando existencia del producto con código: {nuevoCodigo}");

                    productoExiste = Convert.ToInt32(conexion5.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al verificar la existencia del producto: {ex.Message}");
                    return;
                }
            }

            if (productoExiste == 0)
            {
                MessageBox.Show("El nuevo producto no existe en la base de datos.");
                Limpiar();
                return;
            }


            // Paso 2: Obtener Encargo_idEncargo del artículo a cambiar
            string obtenerEncargoProductoIdQuery = "SELECT idEncargoProducto FROM " + objConection.namedb() + ".EncargoProducto WHERE producto_idproducto = @codigoArticuloCambiar";
            int idEncargoProductoOriginal = -1;

            using (MySqlCommand conexion = new MySqlCommand(obtenerEncargoProductoIdQuery, objConection.establecerCN()))
            {
                conexion.Parameters.AddWithValue("@codigoArticuloCambiar", txtArtiCambiar.Text);
                var resultado = conexion.ExecuteScalar();
                if (resultado != null)
                {
                    idEncargoProductoOriginal = Convert.ToInt32(resultado);
                }
            }

            if (idEncargoProductoOriginal == -1)
            {
                MessageBox.Show("El artículo a cambiar no se encontró.");
                Limpiar();
                return;
            }

            // Paso 3: Verificar si el código del nuevo artículo ya está en la tabla EncargoProducto
            string verificarArticuloCambioQuery = "SELECT idEncargoProducto FROM " + objConection.namedb() + ".EncargoProducto WHERE producto_idproducto = @codigoArticuloCambio";
            int idEncargoProductoCambio = -1;

            using (MySqlCommand conexion2 = new MySqlCommand(verificarArticuloCambioQuery, objConection.establecerCN()))
            {
                conexion2.Parameters.AddWithValue("@codigoArticuloCambio", txtArtiCambio.Text);
                var resultadoCambio = conexion2.ExecuteScalar();
                if (resultadoCambio != null)
                {
                    idEncargoProductoCambio = Convert.ToInt32(resultadoCambio);
                }
            }

            if (idEncargoProductoCambio != -1 && idEncargoProductoCambio != idEncargoProductoOriginal)
            {
                // Confirmar el intercambio
                MessageBoxResult confirmResult = MessageBox.Show("El nuevo artículo ya está asignado a otro encargo. ¿Desea intercambiar los artículos?", "Confirmar Intercambio", MessageBoxButton.OKCancel);
                if (confirmResult == MessageBoxResult.OK)
                {
                    // Intercambiar los códigos de los artículos
                    string actualizarArticuloQuery = "UPDATE " + objConection.namedb() + ".EncargoProducto SET producto_idproducto = @nuevoCodigo WHERE idEncargoProducto = @encargoId;" +
                        "UPDATE " + objConection.namedb() + ".EncargoProducto SET producto_idproducto = @codigoOriginal WHERE idEncargoProducto = @encargoIdCambio";

                    using (MySqlCommand conexion3 = new MySqlCommand(actualizarArticuloQuery, objConection.establecerCN()))
                    {
                        conexion3.Parameters.AddWithValue("@nuevoCodigo", txtArtiCambio.Text);
                        conexion3.Parameters.AddWithValue("@codigoOriginal", txtArtiCambiar.Text);
                        conexion3.Parameters.AddWithValue("@encargoId", idEncargoProductoOriginal);
                        conexion3.Parameters.AddWithValue("@encargoIdCambio", idEncargoProductoCambio);
                        conexion3.ExecuteNonQuery();
                    }

                    MessageBox.Show("Intercambio realizado con éxito.");
                    Limpiar();
                }
            }
            else
            {
                // Confirmar el cambio
                MessageBoxResult confirmResult = MessageBox.Show("¿Está seguro de que desea cambiar el artículo?", "Confirmar Cambio", MessageBoxButton.OKCancel);
                if (confirmResult == MessageBoxResult.OK)
                {
                    // Actualizar producto_idproducto
                    string actualizarProductoQuery = "UPDATE " + objConection.namedb() + ".EncargoProducto SET producto_idproducto = @nuevoCodigo WHERE idEncargoProducto = @encargoId";

                    using (MySqlCommand conexion4 = new MySqlCommand(actualizarProductoQuery, objConection.establecerCN()))
                    {
                        conexion4.Parameters.AddWithValue("@nuevoCodigo", txtArtiCambio.Text);
                        conexion4.Parameters.AddWithValue("@encargoId", idEncargoProductoOriginal);
                        conexion4.ExecuteNonQuery();
                    }

                    MessageBox.Show("Cambio realizado con éxito.");
                    Limpiar();
                }
            }
        }
    }
}
