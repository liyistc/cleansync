﻿#pragma checksum "..\..\..\..\Tutorials\05 - Balloons\BalloonSampleWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "454EF52291F551A29B173D3DE156D6D5"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Hardcodet.Wpf.TaskbarNotification;
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


namespace Samples.Tutorials.Balloons {
    
    
    /// <summary>
    /// BalloonSampleWindow
    /// </summary>
    public partial class BalloonSampleWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 12 "..\..\..\..\Tutorials\05 - Balloons\BalloonSampleWindow.xaml"
        internal Hardcodet.Wpf.TaskbarNotification.TaskbarIcon MyNotifyIcon;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\..\Tutorials\05 - Balloons\BalloonSampleWindow.xaml"
        internal System.Windows.Controls.Button btnShowStandardBalloon;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\..\Tutorials\05 - Balloons\BalloonSampleWindow.xaml"
        internal System.Windows.Controls.Button btnShowCustomBalloon;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\..\Tutorials\05 - Balloons\BalloonSampleWindow.xaml"
        internal System.Windows.Controls.Button btnHideStandardBalloon;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\..\Tutorials\05 - Balloons\BalloonSampleWindow.xaml"
        internal System.Windows.Controls.Button btnCloseCustomBalloon;
        
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
            System.Uri resourceLocater = new System.Uri("/Sample Project;component/tutorials/05%20-%20balloons/balloonsamplewindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Tutorials\05 - Balloons\BalloonSampleWindow.xaml"
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
            this.MyNotifyIcon = ((Hardcodet.Wpf.TaskbarNotification.TaskbarIcon)(target));
            return;
            case 2:
            this.btnShowStandardBalloon = ((System.Windows.Controls.Button)(target));
            
            #line 17 "..\..\..\..\Tutorials\05 - Balloons\BalloonSampleWindow.xaml"
            this.btnShowStandardBalloon.Click += new System.Windows.RoutedEventHandler(this.btnShowStandardBalloon_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnShowCustomBalloon = ((System.Windows.Controls.Button)(target));
            
            #line 23 "..\..\..\..\Tutorials\05 - Balloons\BalloonSampleWindow.xaml"
            this.btnShowCustomBalloon.Click += new System.Windows.RoutedEventHandler(this.btnShowCustomBalloon_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnHideStandardBalloon = ((System.Windows.Controls.Button)(target));
            
            #line 38 "..\..\..\..\Tutorials\05 - Balloons\BalloonSampleWindow.xaml"
            this.btnHideStandardBalloon.Click += new System.Windows.RoutedEventHandler(this.btnHideStandardBalloon_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btnCloseCustomBalloon = ((System.Windows.Controls.Button)(target));
            
            #line 44 "..\..\..\..\Tutorials\05 - Balloons\BalloonSampleWindow.xaml"
            this.btnCloseCustomBalloon.Click += new System.Windows.RoutedEventHandler(this.btnCloseCustomBalloon_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}