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
    /// Lógica de interacción para CrearEncargoInventario.xaml
    /// </summary>
    public partial class CrearEncargoInventario : Window
    {
        public CrearEncargoInventario()
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

        private void LeerCodigoBarras(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                
            }
        }

        private void AbrirCrearEncargo(object sender, RoutedEventArgs e)
        {
            CrearEncargoFormulario abrir = new CrearEncargoFormulario();
            abrir.Show();
            this.Close();
        }
    }
}
