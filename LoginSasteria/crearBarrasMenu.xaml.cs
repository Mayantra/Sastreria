﻿using System;
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
    /// Lógica de interacción para crearBarrasMenu.xaml
    /// </summary>
    public partial class crearBarrasMenu : Window
    {
        public crearBarrasMenu()
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

        private void btnCrearBarras_Click(object sender, RoutedEventArgs e)
        {
            CrearBarras abrirCrear = new CrearBarras();
            abrirCrear.Show();
            this.Close();
        }

        private void btnImprimirBarras_Click(object sender, RoutedEventArgs e)
        {
            ImprimirBarras abrirImprimir = new ImprimirBarras();
            abrirImprimir.Show();
            this.Close();
        }
    }
}
