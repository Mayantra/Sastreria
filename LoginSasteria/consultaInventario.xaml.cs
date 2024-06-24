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
using System.Windows.Threading;

namespace LoginSasteria
{
    /// <summary>
    /// Lógica de interacción para consultaInventario.xaml
    /// </summary>
    public partial class consultaInventario : Window
    {
        public consultaInventario()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            DateTime fechaActual = DateTime.Now;
            string hora = fechaActual.ToString("hh");
            string min = fechaActual.ToString("mm");
            string hora24 = fechaActual.ToString("HH");
            if (int.Parse(hora24) >= 12)
            {
                txblockHora.Text = hora + "\n" + min + " pm";
            }
            else
            {
                txblockHora.Text = hora + "\n" + min + " am";
            }
        }

        private void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void abrirInicio(object sender, RoutedEventArgs e)
        {
            mainInventario abrir = new mainInventario();
            abrir.Show();
            this.Close();
        }
        private void abrirCons(object sender, RoutedEventArgs e)
        {
            consultaInventario abrir = new consultaInventario();
            abrir.Show();
            this.Close();
        }

        private void abrirVenta(object sender, RoutedEventArgs e)
        {
            ventaInventario abrir = new ventaInventario();
            abrir.Show();
            this.Close();
        }

        private void abrirBarras(object sender, RoutedEventArgs e)
        {
            crearBarrasMenu abrir = new crearBarrasMenu();
            abrir.Show();
            this.Close();
        }

        private void abrirInventario(object sender, RoutedEventArgs e)
        {
            InventarioInventario abrir = new InventarioInventario();
            abrir.Show();
            this.Close();
        }

        private void abrirEstadoCuenta(object sender, RoutedEventArgs e)
        {
            estadoCuentaInventario abrir = new estadoCuentaInventario();
            abrir.Show();
            this.Close();
        }

        private void AbrirProcesoRegalo(object sender, RoutedEventArgs e)
        {
            DevolucionRegalo abrir = new DevolucionRegalo();
            abrir.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConversionTraje abrir = new ConversionTraje();
            abrir.Show();
            this.Close();
        }
    }
}
