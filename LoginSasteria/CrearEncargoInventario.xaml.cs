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

        private void LeerCodigoBarras(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AgregarCodigo(null, null);
            }
        }
    }
}
