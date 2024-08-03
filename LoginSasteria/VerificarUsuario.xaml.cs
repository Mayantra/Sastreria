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
    /// Lógica de interacción para VerificarUsuario.xaml
    /// </summary>
    public partial class VerificarUsuario : Window
    {
        ConexionDB objConection = new ConexionDB();
        InventarioInventario MenuInventario = new InventarioInventario();
        leerPass read = new leerPass();

        public VerificarUsuario()
        {
            InitializeComponent();
        }

        private void CanelarLog(object sender, RoutedEventArgs e)
        {
            objConection.cerrarCN();
            this.Close();
            MenuInventario.Show();
        }

        private void IniciarSesion(object sender, RoutedEventArgs e)
        {
            objConection.cerrarCN();
            leerPass read = new leerPass();
            for (int i = 0; i <= PassBox.Password.Length; i++)
            {
                if (i == 5)
                {
                    string user = txUser.Text;
                    string pass = PassBox.Password;
                    if (read.setPass(user, pass) != 0)
                    {
                        AgregarProdIventario abrirAgregarProducto = new AgregarProdIventario();
                        objConection.cerrarCN();
                        abrirAgregarProducto.Show();
                        this.Close();

                    }

                    txUser.Text = "";
                    PassBox.Password = "";
                }

            }
            objConection.cerrarCN();
        }
    }
}
