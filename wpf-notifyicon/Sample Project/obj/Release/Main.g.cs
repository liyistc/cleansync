﻿#pragma checksum "..\..\Main.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "0B5AD6391BA1C406008A4FB6AB549F06"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
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


namespace Samples {
    
    
    /// <summary>
    /// Main
    /// </summary>
    public partial class Main : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 42 "..\..\Main.xaml"
        internal System.Windows.Controls.Button btnDeclaration;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\Main.xaml"
        internal System.Windows.Controls.Button btnInlineToolTip;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\Main.xaml"
        internal System.Windows.Controls.Button btnPopups;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\Main.xaml"
        internal System.Windows.Controls.Button btnContextMenus;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\Main.xaml"
        internal System.Windows.Controls.Button btnBalloons;
        
        #line default
        #line hidden
        
        
        #line 103 "..\..\Main.xaml"
        internal System.Windows.Controls.Button btnCommands;
        
        #line default
        #line hidden
        
        
        #line 112 "..\..\Main.xaml"
        internal System.Windows.Controls.Button btnToolTipControl;
        
        #line default
        #line hidden
        
        
        #line 133 "..\..\Main.xaml"
        internal System.Windows.Controls.Button btnMainSample;
        
        #line default
        #line hidden
        
        
        #line 197 "..\..\Main.xaml"
        internal System.Windows.Controls.Button btnEvents;
        
        #line default
        #line hidden
        
        
        #line 206 "..\..\Main.xaml"
        internal System.Windows.Controls.Button btnDataBinding;
        
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
            System.Uri resourceLocater = new System.Uri("/Sample Project;component/main.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Main.xaml"
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
            
            #line 12 "..\..\Main.xaml"
            ((System.Windows.Controls.Grid)(target)).AddHandler(System.Windows.Documents.Hyperlink.RequestNavigateEvent, new System.Windows.Navigation.RequestNavigateEventHandler(this.OnNavigationRequest));
            
            #line default
            #line hidden
            return;
            case 2:
            this.btnDeclaration = ((System.Windows.Controls.Button)(target));
            
            #line 43 "..\..\Main.xaml"
            this.btnDeclaration.Click += new System.Windows.RoutedEventHandler(this.btnDeclaration_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnInlineToolTip = ((System.Windows.Controls.Button)(target));
            
            #line 68 "..\..\Main.xaml"
            this.btnInlineToolTip.Click += new System.Windows.RoutedEventHandler(this.btnInlineToolTip_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnPopups = ((System.Windows.Controls.Button)(target));
            
            #line 77 "..\..\Main.xaml"
            this.btnPopups.Click += new System.Windows.RoutedEventHandler(this.btnPopups_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btnContextMenus = ((System.Windows.Controls.Button)(target));
            
            #line 86 "..\..\Main.xaml"
            this.btnContextMenus.Click += new System.Windows.RoutedEventHandler(this.btnContextMenus_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.btnBalloons = ((System.Windows.Controls.Button)(target));
            
            #line 95 "..\..\Main.xaml"
            this.btnBalloons.Click += new System.Windows.RoutedEventHandler(this.btnBalloons_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.btnCommands = ((System.Windows.Controls.Button)(target));
            
            #line 104 "..\..\Main.xaml"
            this.btnCommands.Click += new System.Windows.RoutedEventHandler(this.btnCommands_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.btnToolTipControl = ((System.Windows.Controls.Button)(target));
            
            #line 113 "..\..\Main.xaml"
            this.btnToolTipControl.Click += new System.Windows.RoutedEventHandler(this.btnToolTipControl_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.btnMainSample = ((System.Windows.Controls.Button)(target));
            
            #line 134 "..\..\Main.xaml"
            this.btnMainSample.Click += new System.Windows.RoutedEventHandler(this.btnMainSample_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.btnEvents = ((System.Windows.Controls.Button)(target));
            
            #line 198 "..\..\Main.xaml"
            this.btnEvents.Click += new System.Windows.RoutedEventHandler(this.btnEvents_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.btnDataBinding = ((System.Windows.Controls.Button)(target));
            
            #line 207 "..\..\Main.xaml"
            this.btnDataBinding.Click += new System.Windows.RoutedEventHandler(this.btnDataBinding_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
