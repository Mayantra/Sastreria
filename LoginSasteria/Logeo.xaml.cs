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
    /// Lógica de interacción para Logeo.xaml
    /// </summary>
    public partial class Logeo : Window
    {
        ProcesoVenta venta = new ProcesoVenta();
        leerPass read = new leerPass();
        public Logeo()
        {
            InitializeComponent();
        }
        
        void acceso()
        {
            for (int i = 0; i <= PassBox.Password.Length; i++)
            {
                if (i == 5)
                {
                    string user = txUser.Text;
                    string pass = PassBox.Password;
                    if (read.getAcceso(user, pass) != false)
                    {
                        int aux;
                        aux = read.getIdLogUser(user);
                        venta.setVendedor(aux);
                        venta.setLog(true);
                        this.Close();
                    }

                    txUser.Text = "";
                    PassBox.Password = "";
                }

            }
        }

        private void lecturapass(object sender, RoutedEventArgs e)
        {
            acceso();
        }

        private void CanelarLog(object sender, RoutedEventArgs e)
        {
            this.Close();
            venta.Show();
        }
    }
}
