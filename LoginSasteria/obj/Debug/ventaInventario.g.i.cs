﻿#pragma checksum "..\..\ventaInventario.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "444D4DD56D82A86150C57909A1823B6903186BEADB8C63524CD96ED573526D7E"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using LoginSasteria;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace LoginSasteria {
    
    
    /// <summary>
    /// ventaInventario
    /// </summary>
    public partial class ventaInventario : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 61 "..\..\ventaInventario.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnExit;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\ventaInventario.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnMin;
        
        #line default
        #line hidden
        
        
        #line 80 "..\..\ventaInventario.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnInicio;
        
        #line default
        #line hidden
        
        
        #line 144 "..\..\ventaInventario.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lbH;
        
        #line default
        #line hidden
        
        
        #line 150 "..\..\ventaInventario.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txblockHora;
        
        #line default
        #line hidden
        
        
        #line 155 "..\..\ventaInventario.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid DataDatos;
        
        #line default
        #line hidden
        
        
        #line 204 "..\..\ventaInventario.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btAgregar;
        
        #line default
        #line hidden
        
        
        #line 257 "..\..\ventaInventario.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txCodigo;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/LoginSasteria;component/ventainventario.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ventaInventario.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.btnExit = ((System.Windows.Controls.Button)(target));
            
            #line 61 "..\..\ventaInventario.xaml"
            this.btnExit.Click += new System.Windows.RoutedEventHandler(this.btnSalir);
            
            #line default
            #line hidden
            return;
            case 2:
            this.btnMin = ((System.Windows.Controls.Button)(target));
            
            #line 68 "..\..\ventaInventario.xaml"
            this.btnMin.Click += new System.Windows.RoutedEventHandler(this.Minimizar);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnInicio = ((System.Windows.Controls.Button)(target));
            
            #line 80 "..\..\ventaInventario.xaml"
            this.btnInicio.Click += new System.Windows.RoutedEventHandler(this.abrirInicio);
            
            #line default
            #line hidden
            return;
            case 4:
            this.lbH = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.txblockHora = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.DataDatos = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 7:
            this.btAgregar = ((System.Windows.Controls.Button)(target));
            
            #line 204 "..\..\ventaInventario.xaml"
            this.btAgregar.Click += new System.Windows.RoutedEventHandler(this.AgregarCodigo);
            
            #line default
            #line hidden
            return;
            case 8:
            this.txCodigo = ((System.Windows.Controls.TextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

