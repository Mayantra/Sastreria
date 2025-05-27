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
    /// Lógica de interacción para EliminarProdInventario.xaml
    /// </summary>
    public partial class EliminarProdInventario : Window
    {
        ConexionDB objConection = new ConexionDB();
        public EliminarProdInventario()
        {
            InitializeComponent();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCancelar_Click_1(object sender, RoutedEventArgs e)
        {
            InventarioInventario abrirMenuInventario = new InventarioInventario();
            abrirMenuInventario.Show();
            this.Close();
        }

        private void EliminarProducto(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string codigo = txtLeerBarras.Text;
                string queryVerificar = "SELECT COUNT(*) FROM " + objConection.namedb() + ".inventario WHERE producto_idproducto = @codigo";

                using (MySqlConnection conexion = objConection.nuevaConexion())
                {
                    if (conexion == null)
                        return;

                    try
                    {
                        using (MySqlCommand comandoVerificar = new MySqlCommand(queryVerificar, conexion))
                        {
                            comandoVerificar.Parameters.AddWithValue("@codigo", codigo);
                            int count = Convert.ToInt32(comandoVerificar.ExecuteScalar());

                            if (count == 0)
                            {
                                MessageBox.Show("El código no existe en el inventario, por favor ingrese un código válido.");
                                txtLeerBarras.Clear();
                                txtLeerBarras.Focus();
                                return;
                            }
                        }

                        // Confirmación de eliminación
                        MessageBoxResult result = MessageBox.Show($"¿Está seguro de que desea eliminar este producto con código de barras No. '{codigo}' del inventario?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Yes)
                        {
                            string queryEliminar = "DELETE FROM " + objConection.namedb() + ".inventario WHERE producto_idproducto = @codigo";

                            using (MySqlCommand comandoEliminar = new MySqlCommand(queryEliminar, conexion))
                            {
                                comandoEliminar.Parameters.AddWithValue("@codigo", codigo);

                                try
                                {
                                    comandoEliminar.ExecuteNonQuery();
                                    MessageBox.Show("Producto eliminado correctamente del inventario.");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Error al eliminar el producto: " + ex.Message);
                                }
                            }

                            txtLeerBarras.Clear();
                        }
                        else
                        {
                            txtLeerBarras.Clear();
                            txtLeerBarras.Focus();
                        }
                    }
                    finally
                    {
                        objConection.cerrarConexion(conexion);
                    }
                }
            }
        }
    }
}
