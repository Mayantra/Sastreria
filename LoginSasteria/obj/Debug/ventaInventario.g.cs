﻿#pragma checksum "..\..\ventaInventario.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "08AAFD4A2C5C338F3A4D5193D853F7C570772A6F5A626D3A40296BC6CD374565"
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
        
        
        #line 39 "..\..\ventaInventario.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnExit;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\ventaInventario.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnMin;
        
        #line default
        #line hidden
        
        
        #line 122 "..\..\ventaInventario.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lbH;
        
        #line default
        #line hidden
        
        
        #line 128 "..\..\ventaInventario.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txblockHora;
        
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
            
            #line 39 "..\..\ventaInventario.xaml"
            this.btnExit.Click += new System.Windows.RoutedEventHandler(this.btnSalir);
            
            #line default
            #line hidden
            return;
            case 2:
            this.btnMin = ((System.Windows.Controls.Button)(target));
            
            #line 46 "..\..\ventaInventario.xaml"
            this.btnMin.Click += new System.Windows.RoutedEventHandler(this.Minimizar);
            
            #line default
            #line hidden
            return;
            case 3:
            this.lbH = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.txblockHora = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

