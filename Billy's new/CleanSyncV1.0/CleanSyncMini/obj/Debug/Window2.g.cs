﻿#pragma checksum "..\..\Window2.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "028A3A9D0CB854DC97236545671F3932"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3603
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
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


namespace CleanSyncMini {
    
    
    /// <summary>
    /// Analyser
    /// </summary>
    public partial class Analyser : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\Window2.xaml"
        internal System.Windows.Controls.Button StartSync;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\Window2.xaml"
        internal System.Windows.Controls.Button Cancel;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\Window2.xaml"
        internal System.Windows.Controls.TreeView CompareResult;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\Window2.xaml"
        internal System.Windows.Controls.TextBox ResultDisplay;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/CleanSnycMini;component/window2.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Window2.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.StartSync = ((System.Windows.Controls.Button)(target));
            
            #line 10 "..\..\Window2.xaml"
            this.StartSync.Click += new System.Windows.RoutedEventHandler(this.StartSync_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Cancel = ((System.Windows.Controls.Button)(target));
            
            #line 11 "..\..\Window2.xaml"
            this.Cancel.Click += new System.Windows.RoutedEventHandler(this.Cancel_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.CompareResult = ((System.Windows.Controls.TreeView)(target));
            return;
            case 4:
            this.ResultDisplay = ((System.Windows.Controls.TextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
