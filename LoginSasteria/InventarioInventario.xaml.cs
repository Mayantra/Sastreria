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
    /// Lógica de interacción para InventarioInventario.xaml
    /// </summary>
    public partial class InventarioInventario : Window
    {
        public InventarioInventario()
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VerificarUsuario abrirVerificarUsuario = new VerificarUsuario();
            abrirVerificarUsuario.Show();
            this.Close();
        }

        private void btnEditarProducto_Click_1(object sender, RoutedEventArgs e)
        {
            EditarProdInventario abrirEditarProducto = new EditarProdInventario();
            abrirEditarProducto.Show();
            this.Close();
        }

        private void btnVerInventario_Click_1(object sender, RoutedEventArgs e)
        {
            VerInventario abrirVerInventario = new VerInventario();
            abrirVerInventario.Show();
            this.Close();
        }

        private void btnBarras_Click_1(object sender, RoutedEventArgs e)
        {
            crearBarrasMenu abrirMenuCrearBarras = new crearBarrasMenu();
            abrirMenuCrearBarras.Show();
            this.Close();
        }

        private void btnInicio_Click_1(object sender, RoutedEventArgs e)
        {
            mainInventario abrirmainInventario = new mainInventario();
            abrirmainInventario.Show();
            this.Close();
        }
    }
}
