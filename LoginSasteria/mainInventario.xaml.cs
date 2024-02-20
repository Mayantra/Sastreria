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
    /// Lógica de interacción para mainInventario.xaml
    /// </summary>
    public partial class mainInventario : Window
    {
        public mainInventario()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
            cargarUser();
            getFecha();

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


        void cargarUser()
        {
            leerPass user = new leerPass();

            string usuario = user.getUser();
            txblockname.Text = usuario;

        }
        string getFecha()
        {
            DateTime fechaActual = DateTime.Now;
            string mes = fechaActual.ToString("MMMM");
            string dia = fechaActual.ToString("dd");
            txblockfecha.Text = dia +"\n"+ mes;
            

            return dia;
        }
        private void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void abrirCons(object sender, RoutedEventArgs e)
        {
            consultaInventario abrir = new consultaInventario();
            abrir.Show();
            this.Close();
        }
    }
}
