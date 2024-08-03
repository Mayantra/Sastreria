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
            objConection.cerrarCN();
            InitializeComponent();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnSalir(object sender, RoutedEventArgs e)
        {
            objConection.cerrarCN();
            this.Close();
        }

        private void btnCancelar_Click_1(object sender, RoutedEventArgs e)
        {
            objConection.cerrarCN();
            InventarioInventario abrirMenuInventario = new InventarioInventario();
            abrirMenuInventario.Show();
            this.Close();
        }

        private void EliminarProducto(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //Se verifica que el codigo exista en el inventario
                string Query = "SELECT COUNT(*) FROM "+objConection.namedb()+".inventario WHERE producto_idproducto = @codigo";
                objConection.cerrarCN();
                using (MySqlCommand comando2 = new MySqlCommand(Query, objConection.establecerCN()))
                {
                    comando2.Parameters.AddWithValue("@codigo", txtLeerBarras.Text);
                    int count = Convert.ToInt32(comando2.ExecuteScalar());
                    objConection.cerrarCN();

                    // Si el conteo es 0, el código no existe
                    if (count == 0)
                    {
                        MessageBox.Show("El código no existe en el inventario, por favor ingrese un código valido");
                        txtLeerBarras.Clear();
                        txtLeerBarras.Focus();

                    }
                    else
                    {
                        // Pregunta al usuario si está seguro de eliminar el registro
                        MessageBoxResult result = MessageBox.Show("¿Está seguro de que desea eliminar este producto con código de barras No.'"+ txtLeerBarras.Text +"' del inventario?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Yes)
                        {
                            string query = "DELETE FROM "+objConection.namedb()+".inventario WHERE producto_idproducto = @codigo";
                            objConection.cerrarCN();
                            using (MySqlCommand comando = new MySqlCommand(query, objConection.establecerCN()))
                            {
                                comando.Parameters.AddWithValue("@codigo", txtLeerBarras.Text);

                                try
                                {
                                    comando.ExecuteNonQuery();
                                    MessageBox.Show("Producto eliminado correctamente del inventario");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Error al eliminar el producto: " + ex.Message);
                                }
                                finally
                                {
                                    objConection.cerrarCN();
                                    txtLeerBarras.Clear();
                                }
                            }
                            objConection.cerrarCN();
                        }
                        else
                        {
                            // Si el usuario elige 'No', simplemente limpia y enfoca el txtLeerBarras
                            txtLeerBarras.Clear();
                            txtLeerBarras.Focus();
                        }
                    }
                }
                objConection.cerrarCN();
            }
        }
    }
}
