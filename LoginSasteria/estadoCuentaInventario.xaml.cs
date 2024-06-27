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
    /// Lógica de interacción para estadoCuentaInventario.xaml
    /// </summary>
    public partial class estadoCuentaInventario : Window
    {
        public estadoCuentaInventario()
        {
            InitializeComponent();
        }

        private void abrirEstadoFechas(object sender, RoutedEventArgs e)
        {
            EstadoPorFechas abrir = new EstadoPorFechas();
            abrir.Show();
            this.Close();
        }

        public void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void abrirEstadoEmpleados(object sender, RoutedEventArgs e)
        {
            EstadoPorClientes abrir = new EstadoPorClientes();
            abrir.Show();
            this.Close();
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
    }
}
