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
    /// Lógica de interacción para mainInventario.xaml
    /// </summary>
    public partial class mainInventario : Window
    {
        public mainInventario()
        {
            InitializeComponent();
            cargarUser();
        }

        void cargarUser()
        {
            leerPass user = new leerPass();

            string usuario = user.getUser();
            lbUser.Content = usuario;

        }
        private void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
