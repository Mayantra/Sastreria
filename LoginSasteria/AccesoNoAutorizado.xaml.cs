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
    /// Lógica de interacción para AccesoNoAutorizado.xaml
    /// </summary>
    public partial class AccesoNoAutorizado : Window
    {
        public AccesoNoAutorizado()
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
    }
}
